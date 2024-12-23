using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.Events;
using EventType = Enums.EventType;
using Enums;

public partial class CommanderController
{
    private EquipBase myEquip;
    private string[] guzhangInfo;

    public void Init(string myId)
    {
        //这里应该是主角数据初始化已经走过了，直接去直升机列表中找自己的id
        myEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BeLongToCommanderId, myId));
        guzhangInfo = new[] { "无故障", "巡航时单发故障", "燃油压力警报灯亮", "主变速箱系统故障", "尾旋翼控制系统故障", "电瓶超温" };
    }

    public void OnOpenPlanningMode()
    {
        EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "请开始为直升机规划任务");
        EventManager.Instance.EventTrigger(Enums.EventType.SwitchMapModel.ToString(), 2);
    }

    public void OnReturnBack()
    {
        //收到了返航的指令.
        (myEquip as IDqChangePart)?.GoReturnBack();
    }

    public void OnReturnRepair(string data)
    {
        var infos = data.Split('_');
        if (string.Equals(infos[0], myEquip.BObjectId))
        {
            switch (int.Parse(infos[1]))
            {
                case 1:
                    (myEquip as IDqChangePart)?.GoReturnRepair();
                    break;
                case 2:
                    (myEquip as IDqChangePart)?.GoReturnBack();
                    break;
            }
        }
    }

    //显示当前我的飞机的状态
    private void ShowMyEquipState(int state)
    {
        EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), $"当前直升机状态：{guzhangInfo[state]}");
    }

    private void OnAskForReturn(int state)
    {
        switch (state)
        {
            case 1: //返修
                EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "已申请返修，等待批准");
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendAskForReturn, $"{myEquip.BObjectId}_1");
                break;
            case 2: //返航
                EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "已申请返航，等待批准");
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendAskForReturn, $"{myEquip.BObjectId}_2");
                break;
        }
    }
}