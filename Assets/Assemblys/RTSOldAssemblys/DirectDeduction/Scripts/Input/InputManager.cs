using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DefaultRole
{
    public class InputManager:DMonoBehaviour
    {
        public bool IsOn;

        public KeyboardInput roleKeyboard;
        public UIToolInput uiTool;
        public UIPanelInput uiPanel;
        public MouseInput mouse;

        public void Awake()
        {
            roleKeyboard =gameObject.AddComponent<KeyboardInput>();
            uiTool = gameObject.AddComponent<UIToolInput>();
            uiPanel = gameObject.AddComponent<UIPanelInput>();
            mouse = gameObject.AddComponent<MouseInput>();
            roleKeyboard.enabled = false;
            uiTool.enabled = false;
            uiPanel.enabled = false;
            mouse.enabled = false;
        }
        public void SetActive(bool active)
        {
            IsOn = active;
            //roleKeyboard.enabled = active;
            uiTool.enabled = active;
            uiPanel.enabled = active;
            mouse.enabled = active;
        }
    }
}
