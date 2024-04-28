using DM.IFS;
using UnityEngine;

namespace DefaultRole
{
    public class UIPanelInput:DMonoBehaviour
    {
        private DataManager dm;
        private void Start()
        {
            dm = ((ZhiHuiDuanMain)main).Data;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (dm.UI.PanelSelect == PanelType.Mission)
                    UIInput("PanelSelect", PanelType.None);
                else
                    UIInput("PanelSelect", PanelType.Mission);
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                if (dm.UI.PanelSelect == PanelType.Help)
                    UIInput("PanelSelect", PanelType.None);
                else
                    UIInput("PanelSelect",PanelType.Help);
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (dm.UI.PanelSelect == PanelType.Map)
                    UIInput("PanelSelect", PanelType.None);
                else
                    UIInput("PanelSelect",PanelType.Map);
            }
        }
        public void UIInput(string key, PanelType value)
        {
            sender.RunSend(SendType.SubToMain, main.BObjectId, (int)RoleEvent.UIPanelInput, key + "," + (int)value);
        }
    }
}
