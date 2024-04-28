using DM.IFS;
using UnityEngine;

namespace DefaultRole
{
    public class TranData : DataBase
    {
        public Vector3 Pos
        {
            get { return pos; }
            set
            {
                if (pos != value)
                {
                    pos = value;
                    OnDataChanged("Null");
                }
            }
        }
        public Quaternion Rot
        {
            get { return rot; }
            set
            {
                if (rot != value)
                {
                    rot = value;
                    OnDataChanged("Null");
                }
            }
        }

        private Vector3 pos = Vector3.zero;
        private Quaternion rot = Quaternion.identity;

        public override string ToString()
        {
            return Pos.x.ToString("0.0000") + "_" + Pos.y.ToString("0.0000") + "_" + Pos.z.ToString("0.0000") + "_" +
                Rot.x.ToString("0.0000") + "_" + Rot.y.ToString("0.0000") + "_" + Rot.z.ToString("0.0000") + "_" + Rot.w.ToString("0.0000");
        }
        public override void ToData(string str)
        {
            string[] strs = str.Split('_');
            Pos = new Vector3(float.Parse(strs[0]), float.Parse(strs[1]), float.Parse(strs[2]));
            Rot = new Quaternion(float.Parse(strs[3]), float.Parse(strs[4]), float.Parse(strs[5]), float.Parse(strs[6]));
        }
    }
    public class RoleData : DataBase
    {
        public int Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnDataChanged("State");
                }
            }
        }
        public int AnimStatus
        {
            get { return animStatus; }
            set
            {
                if (animStatus != value)
                {
                    animStatus = value;
                    OnDataChanged("AnimStatus");
                }
            }
        }
        public int SkinStatus
        {
            get { return skinStatus; }
            set
            {
                if (skinStatus != value)
                {
                    skinStatus = value;
                    OnDataChanged("SkinStatus");
                }
            }
        }
        public float CamYRot
        {
            get { return camYRot; }
            set
            {
                if (camYRot != value)
                {
                    camYRot = value;
                    OnDataChanged("Null");
                }
            }
        }
        public float CamXRot
        {
            get { return camXRot; }
            set
            {
                if (camXRot != value)
                {
                    camXRot = value;
                    OnDataChanged("Null");
                }
            }
        }
        public RoleMode Mode
        {
            get { return mode; }
            set
            {
                if (mode != value)
                {
                    mode = value;
                }
            }
        }
        public string VehicleId
        {
            get { return vehicleId; }
            set
            {
                if (vehicleId != value)
                {
                    vehicleId = value;
                }
            }
        }
  
        private int status;
        private int animStatus= (int)AnimStatusEnum.Idle;
        private int skinStatus;
        private float camYRot;
        private float camXRot;
        private RoleMode mode=RoleMode.Normal;
        private string vehicleId;

        public override string ToString()
        {
            return Status + "_" + AnimStatus + "_" + SkinStatus + "_" + CamYRot.ToString("0.0000") + "_" + CamXRot.ToString("0.0000")+"_"+(int)Mode+"_"+VehicleId;
        }
        public override void ToData(string str)
        {
            string[] strs = str.Split('_');
            Status = int.Parse(strs[0]);
            AnimStatus = int.Parse(strs[1]);
            SkinStatus = int.Parse(strs[2]);
            CamYRot = float.Parse(strs[3]);
            CamXRot = float.Parse(strs[4]);
            Mode = (RoleMode)int.Parse(strs[5]);
            VehicleId = strs[6];
        }
    }
    public enum AnimStatusEnum
    {
        Idle=100,   
        MoveVertical=101,
        MoveHorizontal=103,
        SquatDown=104,  //蹲下
        Down=105,       //趴下

        Driver=120,     //驾驶姿势
        SitDown=121,    //坐下
        SitDown2=122,   //载具座位(伸腿)
        SitDown3 =123,  //载具座位(弯腰)

        Operator =130,  //绞车操作
        Operator2 = 131, //绞车操作(伸手) 
        Operator3 = 132, //绞车操作(抬手) 

        OperatorDown =140,  //吊桶操作

        Observer =150,  //窗户观察
        Observer2 =151, //窗户观察(前倾-低)

        Check =160,   //检查仪表(驾驶员)
        Check2 =161,  //检查仪表(其他) 

        Rescuer =170,   //索降位置(AC313) 
        Rescuer2 = 171, //索降位置(下降)
        Rescuer3 = 172,  //索降位置(上升)  
        Rescuer4 = 173,  //索降位置(单人)  
        Rescuer5 = 174,  //索降位置(救人) 
    }
}

