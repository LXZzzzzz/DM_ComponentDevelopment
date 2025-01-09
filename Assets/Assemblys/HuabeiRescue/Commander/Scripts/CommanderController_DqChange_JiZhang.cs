using System.Collections;
using System.Collections.Generic;
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
    private Queue<string> pendingRunSkill;

    public void Init(string myId)
    {
        //这里应该是主角数据初始化已经走过了，直接去直升机列表中找自己的id
        myEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BeLongToCommanderId, myId));
        guzhangInfo = new[] { "无故障", "巡航时单发故障", "燃油压力警报灯亮", "主变速箱系统故障", "尾旋翼控制系统故障", "电瓶超温" };
        pendingRunSkill = new Queue<string>();
        StartCoroutine(OnRunSkill());
    }

    public void OnOpenPlanningMode()
    {
        EventManager.Instance.EventTrigger<string, UnityAction>(Enums.EventType.ShowTipUIAndCb.ToString(), "接到任务信息和开始任务规划的命令",
            () => OnSendSkillInfo((int)MessageID.SendTrainPointSucInfo, TrainsPintType.JZSureTaskInfo.ToString()));
        EventManager.Instance.EventTrigger(Enums.EventType.SwitchMapModel.ToString(), 2);
    }

    public void OnFerryFlights()
    {
        //收到了转场飞行的指令.
        (myEquip as IDqChangePart)?.GoFerryFlights();
    }

    public void OnReturnBack()
    {
        //收到了返航的指令.
        (myEquip as IDqChangePart)?.GoReturnBack();
    }

    public void OnReturnRepair(string data)
    {
        Debug.LogError("收到了同意申请" + data);
        var infos = data.Split('_');
        if (int.Parse(infos[1]) == 2) (MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, infos[0])) as IDqChangePart)?.StopRunTime();
        if (MyDataInfo.MyLevel != 3) return;
        if (string.Equals(infos[0], myEquip.BObjectId))
        {
            switch (int.Parse(infos[1]))
            {
                case 1:
                    Debug.LogError("调用返修");
                    (myEquip as IDqChangePart)?.GoReturnRepair();
                    break;
                case 2:
                    Debug.LogError("调用返航");
                    (myEquip as IDqChangePart)?.GoReturnBack();
                    break;
            }
        }
    }

    public void OnAddSkillUseSuc(string param)
    {
        Debug.LogError("收到技能使用" + param);
        pendingRunSkill.Enqueue(param);
    }

    private IEnumerator OnRunSkill()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (pendingRunSkill.Count > 0)
            {
                Debug.LogError("加入了一个待执行点" + pendingRunSkill.Peek());
                MyDataInfo.SkillsToBeConfirmed.Add(pendingRunSkill.Dequeue());
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