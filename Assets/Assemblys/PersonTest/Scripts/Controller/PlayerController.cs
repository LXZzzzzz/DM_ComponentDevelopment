using System;
using DM.Entity;
using Lockstep.Math;
using ToolsLibrary;
using UnityEngine;

namespace TestBuild
{
    public class PlayerController : DMonoBehaviour
    {
        private PersonTestMain playerMainData;
        private IDrive currentVehicle; //当前载具

        private LFloat inputMoveX;
        private LFloat inputMoveY;
        private LFloat inputRotate;

        public void Init(PersonTestMain playerMainData)
        {
            this.playerMainData = playerMainData;
        }

        LFloat timeDelta;

        private void Start()
        {
            //要让客户端一秒执行制定次数
            timeDelta = new LFloat(1) / new LFloat(60);
            timer = Time.time;
            runNums = 0;
            Debug.LogError("时间间隔" + timeDelta);
        }

        float timer;
        int runNums;

        private void Update()
        {
            //被操控对象才会有输入监听
            if (playerMainData.isLead)
                SendOperationInstructions();
        }

        private void RunPlayer()
        {
            if (Time.time >= timer)
            {
                timer = Time.time + timeDelta;

                LVector3 ltranslation = new LVector3(inputMoveX, 0, inputMoveY) *
                                        (playerMainData.Properties[2] as InputFloatProperty).Value.ToLFloat() *
                                        timeDelta;
                transform.Translate(new Vector3(ltranslation.x.ToFloat(), ltranslation.y.ToFloat(),
                    ltranslation.z.ToFloat()));

                transform.Rotate(Vector3.up * inputRotate * (playerMainData.Properties[7] as InputFloatProperty).Value *
                                 Time.deltaTime);
            }
        }

        private void SendOperationInstructions()
        {
            LFloat inputX = InputManager.GetAxis("Horizontal");
            LFloat inputY = InputManager.GetAxis("Vertical");
            LFloat inputR = InputManager.GetAxis("Rotate");
            LFloat inputS = InputManager.GetAxis("Jump");

            InputInfoData dataParm = new InputInfoData()
                { inputX = inputX, inputY = inputY, inputR = inputR, inputS = inputS };

            string parm = Montage(dataParm);
            if (inputX != 0 || inputY != 0 || inputR != 0 || inputS != 0)
            {
                sender.LogError("操作者的输入数据："+parm);
                sender.RunSend(DM.IFS.SendType.SubToMain, playerMainData.BObjectId, 100, parm);
            }


            // MyDataInfo.netLogic.OnRecordOperation(new ToolsLibrary.FrameSync.PlayerControlData()
            // { objectId = playerMainData.BObjectId, parm = parm });
        }


        private void Control(float hor, float ver, float rot)
        {
            inputMoveX = (LFloat)hor;
            inputMoveY = (LFloat)ver;
            inputRotate = (LFloat)rot;
            runNums++;

            RunPlayer();
        }

        //进入和退出载具
        public void EnterVehicle(GameObject vehicle, bool isMe)
        {
            if (vehicle == null)
            {
                sender.LogError("没找到汽车dididi");
                currentVehicle?.OnExit();
                currentVehicle = null;
                //判断这个角色是不是我来控制，如果是就切相机
                if (isMe)
                {
                    Vector3 offset = new Vector3((main.Properties[3] as InputFloatProperty).Value,
                        (main.Properties[4] as InputFloatProperty).Value,
                        (main.Properties[5] as InputFloatProperty).Value);
                    ThirdPersonCamera.Instance.Init(transform, offset, (main.Properties[6] as InputFloatProperty).Value,
                        false);
                }
            }
            else
            {
                sender.LogError("找到了汽车");
                currentVehicle = vehicle.GetComponentInChildren<IDrive>();
                sender.LogError("获取到驾驶接口了吗" + currentVehicle);
                currentVehicle.OnEnter(gameObject, isMe);
            }
        }

        public void InputInfo(string inputData)
        {
            InputInfoData data = Split(inputData);
            //解析出来横向，纵向，空格，。。。
            //输入内容传入
            //if (!playerMainData.isLead) return;
            if (currentVehicle != null)
            {
                currentVehicle.OnDrive(data.inputY, data.inputX, data.inputS);
            }
            else
            {
                sender.LogError("指令数据：" + data);
                Control(data.inputX, data.inputY, data.inputR);
            }
        }

        #region 压缩

        public string Montage(InputInfoData data)
        {
            return String.Format($"{data.inputX}_{data.inputY}_{data.inputS}");
        }

        public InputInfoData Split(string dataStr)
        {
            var strs = dataStr.Split("_");
            InputInfoData iid = new InputInfoData()
            {
                inputX = float.Parse(strs[0]), inputY = float.Parse(strs[1]), inputS = float.Parse(strs[2])
            };
            return iid;
        }


        // public string CompressJson(string json)
        // {
        //     byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
        //     using (var memoryStream = new MemoryStream())
        //     {
        //         using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
        //         {
        //             gzipStream.Write(jsonBytes, 0, jsonBytes.Length);
        //             gzipStream.Flush();
        //             byte[] compressedBytes = memoryStream.ToArray();
        //             return Convert.ToBase64String(compressedBytes);
        //         }
        //     }
        // }
        //
        // public string DecompressJson(string compressedJson)
        // {
        //     byte[] compressedBytes = Convert.FromBase64String(compressedJson);
        //     using (var memoryStream = new MemoryStream(compressedBytes))
        //     {
        //         using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
        //         {
        //             using (var reader = new StreamReader(gzipStream))
        //             {
        //                 return reader.ReadToEnd();
        //             }
        //         }
        //     }
        // }

        #endregion
    }

    public struct InputInfoData
    {
        public float inputX;
        public float inputY;
        public float inputR;
        public float inputS;

        public override string ToString()
        {
            return string.Format($"{inputX},{inputY},{inputR},{inputS}");
        }
    }
}