using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using UnityEngine;

namespace CarTest
{
    public class CarController : DMonoBehaviour, IDrive
    {
        private List<AxleInfo> axleInfos;
        private float maxMotorTorque;
        private float maxSteeringAngle;
        private float motor;
        private float steering;
        private float brake;


        public void Init()
        {
            sender.LogError("小车Init");
            axleInfos = new List<AxleInfo>();
            AxleInfo frontInfo = new AxleInfo()
            {
                leftWheel = transform.Find("wheels/LeftFront").GetComponent<WheelCollider>(),
                rightWheel = transform.Find("wheels/RightFront").GetComponent<WheelCollider>(),
                motor = false,
                steering = true
            };
            AxleInfo rearInfo = new AxleInfo()
            {
                leftWheel = transform.Find("wheels/LeftRear").GetComponent<WheelCollider>(),
                rightWheel = transform.Find("wheels/RightRear").GetComponent<WheelCollider>(),
                motor = true,
                steering = false
            };
            axleInfos.Add(frontInfo);
            axleInfos.Add(rearInfo);
            maxMotorTorque = 310;
            maxSteeringAngle = 30;
        }

        // 查找相应的可视车轮
        // 正确应用变换
        public void ApplyLocalPositionToVisuals(WheelCollider collider)
        {
            if (collider.transform.childCount == 0)
            {
                return;
            }

            Transform visualWheel = collider.transform.GetChild(0);

            Vector3 position;
            Quaternion rotation;
            collider.GetWorldPose(out position, out rotation);

            visualWheel.transform.position = position;
            visualWheel.transform.rotation = rotation;
        }

        public void FixedUpdate()
        {
            if (axleInfos == null) return;
            foreach (AxleInfo axleInfo in axleInfos)
            {
                if (axleInfo.steering)
                {
                    axleInfo.leftWheel.steerAngle = steering;
                    axleInfo.rightWheel.steerAngle = steering;
                }

                if (axleInfo.motor)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;
                }

                axleInfo.leftWheel.brakeTorque = brake;
                axleInfo.rightWheel.brakeTorque = brake;

                ApplyLocalPositionToVisuals(axleInfo.leftWheel);
                ApplyLocalPositionToVisuals(axleInfo.rightWheel);
            }
        }

        private Transform driver;
        private Transform lastParent;
        public void OnEnter(GameObject player, bool isMe)
        {
            driver = player.transform;
            lastParent = driver.parent;
            Debug.LogError("角色原先服务提" + lastParent);
            driver.parent = transform;
            driver.localPosition = Vector3.zero;
            driver.GetComponentInChildren<BoxCollider>().enabled = false;
            if (isMe)
            {
                Debug.LogError("把车辆改为可用状态");
                ThirdPersonCamera.Instance.Init(transform, new Vector3(0, 7, -10), 5, true);
                for (int i = 0; i < (main as CarMain).myData.Length; i++)
                {
                    (main as CarMain).myData[i].Usable = true;
                }
            }
            else
            {
                Debug.LogError("把车辆改为不可用");
                //当前上车的不是自己，就把操作项改为不可用
                for (int i = 0; i < (main as CarMain).myData.Length; i++)
                {
                    (main as CarMain).myData[i].Usable = true;
                }
            }

            //Vector3 offset = new Vector3((Properties[0] as InputFloatProperty).Value, (Properties[1] as InputFloatProperty).Value, (Properties[2] as InputFloatProperty).Value);
            //tpc.Init(transform, offset, (Properties[3] as InputFloatProperty).Value, true);
        }

        public void OnDrive(float ver, float hor, float stop)
        {
            motor = maxMotorTorque * ver;
            steering = maxSteeringAngle * hor;
            brake = 1000 * stop;
        }

        public void OnExit()
        {
            Debug.LogError("玩家退出了汽车驾驶舱");
            driver.parent = lastParent;
            driver.position += Vector3.right * 3;
            driver.GetComponentInChildren<BoxCollider>().enabled = true;
            for (int i = 0; i < (main as CarMain).myData.Length; i++)
            {
                (main as CarMain).myData[i].Usable = true;
            }
        }
    }

    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor; // 此车轮是否连接到电机？
        public bool steering; // 此车轮是否施加转向角？
    }
}
