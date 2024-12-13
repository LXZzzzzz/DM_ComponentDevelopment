using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public partial class CommanderController
{
    private EquipBase myEquip;

    public void Init(string myId)
    {
        //这里应该是主角数据初始化已经走过了，直接去直升机列表中找自己的id
        myEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BeLongToCommanderId, myId));
    }

    public void OnOpenPlanningMode()
    {
        EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "请开始为直升机规划任务");
        EventManager.Instance.EventTrigger(Enums.EventType.SwitchMapModel.ToString(), 2);
    }

    public void OnGetTurnBack()
    {
        //收到了返航的指令.
        (myEquip as IDqChangePart)?.GoReturnBack();
    }
}