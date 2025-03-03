using System;
using Enums;
using ToolsLibrary;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using EventType = Enums.EventType;

public class UISendTaskInfoShow : BasePanel
{
    private InputField airLineInfo;

    public override void Init()
    {
        base.Init();
        airLineInfo = GetControl<InputField>("InputF_AirLine");
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        if (userData != null)
        {
            airLineInfo.text = (string)userData;
            airLineInfo.interactable = false;

            GetControl<Button>("sure").onClick.AddListener(OnAccept);
            GetControl<Button>("cancel").onClick.AddListener(() => Close(UIName.UISendTaskInfoShow));
        }
        else
        {
            airLineInfo.interactable = true;
            GetControl<Button>("sure").onClick.AddListener(OnSure);
            GetControl<Button>("cancel").onClick.AddListener(() => Close(UIName.UISendTaskInfoShow));
        }
    }

    public override void HideMe()
    {
        base.HideMe();
        GetControl<Button>("sure").onClick.RemoveAllListeners();
        GetControl<Button>("cancel").onClick.RemoveAllListeners();
    }

    private void OnSure()
    {
        Close(UIName.UISendTaskInfoShow);

        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendProgramme, airLineInfo.text);
        EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "任务已下达");
    }

    private void OnAccept()
    {
        Close(UIName.UISendTaskInfoShow);
        if(MyDataInfo.gameState>= GameState.GameStart) return;
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendTrainPointSucInfo, TrainsPintType.XCZHGetTask.ToString());
    }
}