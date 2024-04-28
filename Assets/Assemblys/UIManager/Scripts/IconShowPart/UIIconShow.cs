using UnityEngine;
using System.Collections.Generic;
using DM.IFS;
using System;
using ToolsLibrary;

namespace UiManager.IconShowPart
{
    public class UIIconShow : BasePanel
    {

        private UIItem_IconShow iconItem;

        public override void ShowMe(object userDate)
        {
            base.ShowMe(userDate);
            DMonoBehaviour[] cellData = userDate as DMonoBehaviour[];

            for (int i = 0; i < cellData.Length; i++)
            {
                if (cellData[i] is UIItem_IconShow) iconItem = cellData[i] as UIItem_IconShow;
            }

            InitOperators();
        }

        private void InitOperators()
        {
            //这里获取场景中所有的操作项，实例化icon
            for (int i = 0; i < allBObjects.Length; i++)
            {
                DMOperationItem[] opers = allBObjects[i].GetComponentsInChildren<DMOperationItem>();
                //拿到了所有操作项，为每一个操作项创建UIIcon进行显示
                for (int j = 0; j < opers.Length; j++)
                {
                    UIItem_IconShow item = Instantiate(iconItem, transform);
                    item.OnInit(opers[j], SendOperate);
                    item.gameObject.SetActive(true);
                }
            }
        }

        private int operatorId;
        private int operatorState;
        private string sourceId;
        private void SendOperate(int operatorId, int operatorState, string sourceId)
        {
            this.operatorId = operatorId;
            this.operatorState = operatorState;
            this.sourceId = sourceId;
            //把操作数据存一下，通知开启确认窗口，
            sender.RunSend(SendType.SubToMain,UIManager.Instance.main.BObjectId,666,$"{MyDataInfo.leadId}_是否确认上下车操作");
        }

        public void ReceiveOperate(int type,string info)
        {
            if(type==666)
            {
                //打开确认窗口
                UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation,info);
            }

            if (type == 888)
            {
                //确认了
                UIManager.Instance.HidePanel(UIName.UIConfirmation.ToString());
                SendUIInput(operatorId,operatorState,sourceId);
            }

            if (type==999)
            {
                //取消了
                UIManager.Instance.HidePanel(UIName.UIConfirmation.ToString());
            }
        }
        

        public void SendUIInput(int operatorId, int operatorState, string sourceId)
        {
            //todo: 要把main换成主角对象
            sender.LogError("触发了操作项事件,操作项ID：" + sourceId);
            //把操作项的ID和状态，还有操作者的ID传给操作项所在的对象
            Debug.LogError("触发操作项"+MyDataInfo.isHost);
            if (MyDataInfo.isHost)
                sender.RunSend(SendType.MainToAll, sourceId, operatorState, MyDataInfo.leadId + "," + operatorId);
            else
                sender.RunSend(SendType.SubToMain, sourceId, operatorState, MyDataInfo.leadId + "," + operatorId);

        }

        public override void HideMe()
        {
            base.HideMe();
        }
    }
}
