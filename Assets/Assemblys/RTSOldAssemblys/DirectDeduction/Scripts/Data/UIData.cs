using UnityEngine;

namespace DefaultRole
{
    public enum PanelType
    {
        None,
        Mission,
        Map,
        Help,
    }
    public class UIData:DataBase
    {
        public PanelType PanelSelect
        {
            get { return panelSelect; }
            set
            {
                if (panelSelect != value)
                {
                    panelSelect = value;
                    OnDataChanged("PanelSelect");
                }
            }
        }
        public Vector3 MousePos
        {
            get { return mousePos; }
            set
            {
                if (mousePos != value)
                {
                    mousePos = value;
                    OnDataChanged("MousePos");
                }     
            }
        }
        public bool MouseClick
        {
            get { return mouseClick; }
            set
            {
                if (mouseClick != value)
                {
                    mouseClick = value;
                    OnDataChanged("MouseClick");
                }
            }
        }
        public Vector3 MapPos
        {
            get { return mapPos; }
            set
            {
                if (mapPos != value)
                {
                    mapPos = value;
                    OnDataChanged("MapPos");
                }
            }
        }
        public string OtherUIPath
        {
            get { return otherUIPath; }
            set
            {
                if (otherUIPath != value)
                {
                    otherUIPath = value;
                    OnDataChanged("OtherUIPath");
                }
            }
        }
        public string ConfirmUIStr
        {
            get { return confirmUIStr; }
            set
            {
                if (confirmUIStr != value)
                {
                    confirmUIStr = value;
                    OnDataChanged("ConfirmUIStr");
                }
            }
        }

        private PanelType panelSelect;
        private Vector3 mousePos=Vector3.zero;
        private bool mouseClick;
        private Vector3 mapPos = Vector3.zero;
        private string otherUIPath;
        private string confirmUIStr;

        public override string ToString()
        {
            return ((int)PanelSelect).ToString()+"_"+MousePos.x+"_"+MousePos.y+"_"+MouseClick+"_"+
                   MapPos.x+"_"+MapPos.y+"_"+MapPos.z+"_"+OtherUIPath+"_"+ConfirmUIStr;
        }
        public override void ToData(string str)
        {
            string[] strs = str.Split('_');
            PanelSelect = (PanelType)int.Parse(strs[0]);
            MousePos = new Vector3(float.Parse(strs[1]),float.Parse(strs[2]),0);
            MouseClick = bool.Parse(strs[3]);
            MapPos = new Vector3(float.Parse(strs[4]),float.Parse(strs[5]),0);
            OtherUIPath = strs[7];
            ConfirmUIStr = strs[8];
        }
    }
}
