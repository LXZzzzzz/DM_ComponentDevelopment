using UnityEngine;

namespace DefaultRole
{
    public class PickupData:DataBase
    {
        public int SelectBarId
        {
            get { return selectBarId; }
            set
            {
                if (selectBarId != value)
                {
                    selectBarId = value;
                    OnDataChanged("SelectBar");
                }
            }
        }
        public string SelectToolId
        {
            get { return selectToolId; }
            set
            {
                if (selectToolId != value)
                {
                    selectToolId = value;
                    OnDataChanged("SelectTool");
                }
            }
        }
        public string SourceId;
        public int KnapId=-1;

        private int selectBarId = 1;
        private string selectToolId="";

        public void SetPickup(string sourceId,int knapId)
        {
            SourceId = sourceId;
            KnapId = knapId;
            OnDataChanged("PickupKnap");
        }

        public override string ToString()
        {
            return SelectBarId + "_" + SelectToolId+"_"+SourceId + "_"+KnapId;
            
        }
        public override void ToData(string str)
        {
            string[] strs = str.Split('_');
            SelectBarId = int.Parse(strs[0]);
            SelectToolId = strs[1];
            SetPickup(strs[2],int.Parse(strs[3]));
        }
    }
}
