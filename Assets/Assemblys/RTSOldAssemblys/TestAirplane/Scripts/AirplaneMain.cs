using DM.Core.Map;
using DM.Entity;
using DM.IFS;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace 测试飞机
{
    public class AirplaneMain:ScriptManager,IMesRec,IRTSHandler
    {
        RTSInformationMgr rtsInfoMgr;
        RTSInstructionMgr rtsInsMgr;
        RTSPropertyMgr rtsProMgr;
        void Awake()
        {
            AnimStatusEnums = new EnumDescription[] {
                new EnumDescription(0,"正常"),
                new EnumDescription(1,"故障"),
                new EnumDescription(2,"单发停机")
            };
            StatusEnums = new EnumDescription[] {
                new EnumDescription(0,"正常"),
                new EnumDescription(1,"起飞"),
                new EnumDescription(2,"飞行"),
                new EnumDescription(3,"悬停"),
                new EnumDescription(4,"降落"),
                new EnumDescription(5,"坠毁")
            };
            Properties = new DynamicProperty[] {
                new InputFloatUnitProperty("最大速度",200,"KM/H"),
                new InputIntUnitProperty("最大高度",1000,"M"),
                new DropDownSceneSelectProperty("选择机场"),
                new OpenFileDialogProperty("参数路径","gigig"),
                new InputIntUnitLimitProperty("限制区间Int",20,"M",10,50),
                new InputFloatUnitLimitProperty("限制区间Float",30.5f,"Kg"),
            };

            //脚本挂到组件上的任何位置都可以,如果有多个则使用GetComponentsInChildren方法获取到的第一个
            rtsInfoMgr=gameObject.AddComponent<RTSInformationMgr>();
            rtsInsMgr=gameObject.AddComponent<RTSInstructionMgr>();
            rtsProMgr=gameObject.AddComponent<RTSPropertyMgr>();

            Instruction ins = new Instruction(); //指令
            sender.RunSend(SendType.SubToMain,"指令执行对象id",(int)SystemInputType.RTSInstruction,ins.ToString());
        }

        public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
        {
            base.RunModeInitialized(isRoomCreator, info);
            List<BObjectModel> equipments = allBObjects.FindBObjectsWithTag("15", "0"); //RTS救援力量
            List<BObjectModel> targets = allBObjects.FindBObjectsWithTag("15", "1"); //RTS遇险目标
            List<BObjectModel> factors = allBObjects.FindBObjectsWithTag("15", "2"); //RTS干扰因素
            List<BObjectModel> globalEnv = allBObjects.FindBObjectsWithTag("15", "3"); //RTS全局环境
        }

        //指令同步和交互
        public void RecMessage(SendType type, GameObject senderObj, int eventType, string param)
        {
            if (type == SendType.MainToMain)
            {
                //如果主从处理逻辑完全一致，主机处理指令后将状态同步给从机
                if (eventType == (int)SystemInputType.RTSInstruction)
                {
                    Instruction ins = Instruction.ToInstruction(param);
                    Debug.LogError("RecMessage执行指令:" + ins.Name);
                    rtsInsMgr.InsHandler(ins);
                }
                //Method2:如果主从处理逻辑完全一致，主机可以直接将消息广播，全部客户端都处理
                //if (eventType == (int)SystemInputType.RTSInstruction)
                //    sender.RunSend(SendType.MainToAll,this.BObjectId,(int)SystemInputType.RTSInstruction, param);
            }
            else if (type == SendType.MainToSubs)
            {
                if (eventType == (int)SystemInputType.RTSInstruction)
                {
                    //从机同步指令执行后的状态
                }
            }
            //Method2
            //else if (type == SendType.MainToAll)
            //{
            //    if (eventType == (int)SystemInputType.RTSInstruction)
            //    {
            //        Instruction ins = Instruction.ToInstruction(param);
            //        rtsInsMgr.InsHandler(ins);
            //    }
            //}
        }

        public void InstructionHandler(Instruction ins)
        {
            Debug.LogError("InstructionHandler执行指令:" + ins.Name);
            rtsInsMgr.InsHandler(ins);
        }

        private int id = 0;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Debug.LogError("Add Result");
                //type:misname-battleid-score:other
                id = UnityEngine.Random.Range(100000,999999);
                string data =string.Format("Add:misName-{0}-0:null",id);
                sender.RunSend(SendType.MainSystem,null,(int)MainSystemType.UploadDataCurrent,data);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                Debug.LogError("Add Result");
                int score = UnityEngine.Random.Range(0,99);
                string data = string.Format("Update:misName-{0}-{1}:1-2-3-4-5-6-7-8-9",id,score);
                sender.RunSend(SendType.MainSystem, null, (int)MainSystemType.UploadDataCurrent, data);
            }
        }
    }
}
