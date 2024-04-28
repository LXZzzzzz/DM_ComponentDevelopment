using DM.IFS;
using UnityEngine;

namespace DefaultRole
{
    public class UIToolInput : DMonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                UIInput("SelectBar", 0);
            if (Input.GetKeyDown(KeyCode.F2))
                UIInput("SelectBar", 1);
            if (Input.GetKeyDown(KeyCode.F3))
                UIInput("SelectBar", 2);
            if (Input.GetKeyDown(KeyCode.F4))
                UIInput("SelectBar", 3);
            if (Input.GetKeyDown(KeyCode.F5))
                UIInput("SelectBar", 4);
            if (Input.GetKeyDown(KeyCode.Alpha1))
                UIInput("SelectTool", 0);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                UIInput("SelectTool", 1);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                UIInput("SelectTool", 2);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                UIInput("SelectTool", 3);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                UIInput("SelectTool", 4);
            if (Input.GetKeyDown(KeyCode.Alpha6))
                UIInput("SelectTool", 5);
            if (Input.GetKeyDown(KeyCode.Alpha7))
                UIInput("SelectTool", 6);
            if (Input.GetKeyDown(KeyCode.Alpha8))
                UIInput("SelectTool", 7);
            if (Input.GetKeyDown(KeyCode.Alpha9))
                UIInput("SelectTool", 8);
            if (Input.GetKeyDown(KeyCode.Alpha0))
                UIInput("SelectTool", 9);
            if (Input.GetMouseButtonDown(1))
                UIInput("RightClickCancel", 0);
        }
        public void UIInput(string key, int value)
        {
            sender.RunSend(SendType.SubToMain,main.BObjectId,(int)RoleEvent.UIToolInput,key+","+value);
        }
    }
}
