using System;

namespace DefaultRole
{
    public class DataManager : DMonoBehaviour
    {
        public Action<DataType,string> OnDataChanged;
        
        public RoleData Role;
        public TranData Tran;
        public KnapsackData Knap;
        public PickupData Pickup;
        public UIData UI;

        public bool isMain;
        private void Awake()
        {
            Role = new RoleData();
            Tran = new TranData();
            Knap = new KnapsackData();
            Pickup = new PickupData();
            UI = new UIData();
            OnDataChanged += SendSync;
            Role.OnDataChanged += (attr) => { OnDataChanged(DataType.Role,attr); };
            Tran.OnDataChanged += (attr) => { OnDataChanged(DataType.Tran,attr); };
            Knap.OnDataChanged += (attr) => { OnDataChanged(DataType.Knap,attr); };
            Pickup.OnDataChanged += (attr) => { OnDataChanged(DataType.Pickup,attr); };
            UI.OnDataChanged += (attr) => { OnDataChanged(DataType.UI,attr); };
        }

        public override string ToString()
        {
            return Role.ToString() + ";" + Tran.ToString()+ ";" + Knap.ToString()+";"+Pickup.ToString()+";"+UI.ToString();
        }
        public void ToData(string str)
        {
            string[] data = str.Split(';');
            Role.ToData(data[0]);
            Tran.ToData(data[1]);
            Knap.ToData(data[2]);
            Pickup.ToData(data[3]);
            UI.ToData(data[4]);
        }

        public void SendSync(DataType dt,string attr)
        {
            if (isMain)
                sender.RunSend(DM.IFS.SendType.MainToSubs, main.BObjectId, (int)RoleEvent.SyncData, ToString());
        }
    }

    public enum DataType
    {
        Role,
        Tran,
        Knap,
        Pickup,
        UI,
    }
}


