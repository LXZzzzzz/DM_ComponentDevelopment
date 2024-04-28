using DM.IFS;
using UnityEngine;

namespace DefaultRole
{
    public class MouseInput : DMonoBehaviour
    {
        private void Update()
        {
            UIInput("MousePos",Input.mousePosition.x+","+Input.mousePosition.y);
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                UIInput("MouseClick","True");
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                UIInput("MouseClick","False");
        }
        public void UIInput(string key, string value)
        {
            sender.RunSend(SendType.SubToMain, main.BObjectId, (int)RoleEvent.MouseInput, key + "," + value);
        }
    }
}
