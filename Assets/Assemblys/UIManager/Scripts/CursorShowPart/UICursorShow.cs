using System;
using ToolsLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace UiManager.CursorShowPart
{
    //目前暂定 只有复盘模式显示自定义鼠标
    public class UICursorShow : BasePanel
    {
        private RectTransform curPic;
        public override void ShowMe(object userData)
        {
            base.ShowMe(userData);
            curPic = GetControl<Image>("curPic").GetComponent<RectTransform>();
            //这里注册鼠标事件
            InputManager.cursorControl += OnMouseMapping;
            
        }

        private void Update()
        {
            transform.SetAsLastSibling();
        }

        private void OnMouseMapping(Vector2 pos, bool isClick)
        {
            curPic.anchoredPosition = new Vector2(pos.x - Screen.width / 2, pos.y - Screen.height / 2);
        }
        public override void HideMe()
        {
            InputManager.cursorControl -= OnMouseMapping;
            base.HideMe();
        }
    }
}
