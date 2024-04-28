using System;
using DM.IFS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ToolsLibrary;

namespace UiManager.IconShowPart
{
    public class UIItem_IconShow : DMonoBehaviour
    {
        private IconItem_Btn _itemBtn;
        private IconItemBase currentIcon;

        private DMOperationItem target;

        private void Init()
        {
            _itemBtn = new IconItem_Btn();
            _itemBtn.OnInit(this);
        }
        public void OnInit(DMOperationItem data, UnityAction<int, int, string> cb)
        {
            Init();
            //根据传递进来的数据决定显示内容
            target = data;
            currentIcon?.OnClose();
            currentIcon = getItemIcon();
            currentIcon?.OnShow(data, cb);
        }

        private IconItemBase getItemIcon()
        {
            return _itemBtn;
        }

        void Update()
        {
            if (currentIcon == null || target == null) return;
            if (Camera.main == null) return;
            //跟随移动方面逻辑,判断与主摄像机距离决定显隐
            currentIcon?.SetIsShow(Vector3.Distance(Camera.main.transform.position, target.transform.position) < target.OperDistance &&
                                   Vector3.Angle(Camera.main.transform.forward, Vector3.Normalize(target.transform.position - Camera.main.transform.position)) < 60
                                   && currentIcon.itemData.Usable);
            currentIcon?.Update();

            Vector2 screenPoint = Camera.main.WorldToScreenPoint(target.transform.position);

            Vector2 pointUGUIPos = new Vector2();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(UIManager.Instance.CurrentCanvans.transform as RectTransform, screenPoint, null, out pointUGUIPos))
                transform.GetComponent<RectTransform>().anchoredPosition = pointUGUIPos;

        }
    }

    public abstract class IconItemBase
    {
        protected UIItem_IconShow main;
        public DMOperationItem itemData;
        protected UnityAction<int, int, string> clickEvent;

        public void OnInit(UIItem_IconShow main)
        {
            this.main = main;
            Init();
        }
        public void OnShow(DMOperationItem data, UnityAction<int, int, string> cb)
        {
            itemData = data;
            clickEvent = cb;
            //需要将下方所有触发组件全部隐藏掉

            Show();
        }

        public void OnClose()
        {

        }

        protected abstract void Init();
        protected abstract void Show();
        public abstract void SetIsShow(bool isShow);
        public abstract void Update();
    }

    public class IconItem_Btn : IconItemBase
    {
        private Button clickBtn;
        private int currentStatus;

        protected override void Init()
        {
            clickBtn = main.GetComponentInChildren<Button>();
        }

        protected override void Show()
        {
            main.sender.LogError($"操作项：Id{itemData.Id};状态{itemData.Status};sourceId{itemData.SourceId}");
            refreshShowInfo();
            //Id为操作项的Id；Status为操作项当前状态；SourceId为操作项所在物体的Id
            clickBtn.onClick.AddListener(() => clickEvent(itemData.Id, itemData.Status, itemData.SourceId));
        }

        private void refreshShowInfo()
        {
            if (currentStatus == itemData.Status) return;
            for (int i = 0; i < itemData.OperItems.Length; i++)
            {
                var item = itemData.OperItems[i];
                if (item.Status == itemData.Status)
                {
                    currentStatus = itemData.Status;
                    clickBtn.GetComponentInChildren<Text>().text = item.Text;
                    break;
                }
            }

            //todo: 关于需要物品的操作项，要先判断该操作项的Type如果不为0，就要显示需要前置物品的图标
            //todo: 判断角色当前选中的物品项的Type和TargetType等于操作项的Type时，再打开按钮交互，否则关闭

            //todo: 当itemData为物品项时，显示物品项的图标；物品项所在物体接收到之后，转发给主角，主角找到物体，记录在背包数据中 DMKnapsackItem

        }

        public override void SetIsShow(bool isShow)
        {
            clickBtn.gameObject.SetActive(isShow);
        }

        public override void Update()
        {
            refreshShowInfo();
        }
    }
}
