using System;
using Enums;
using ToolsLibrary;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using EventType = Enums.EventType;

public class UIAirLineInfoShow : BasePanel
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

            GetControl<Button>("sure").onClick.AddListener(OnAgree);
            GetControl<Button>("cancel").onClick.AddListener(OnRefuse);
        }
        else
        {
            airLineInfo.interactable = true;
            airLineInfo.text = String.Empty;
            GetControl<Button>("sure").onClick.AddListener(OnSure);
            GetControl<Button>("cancel").onClick.AddListener(() => Close(UIName.UIAirLineInfoShow));
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
        Close(UIName.UIAirLineInfoShow);
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendAskForAirLine, airLineInfo.text);
    }

    private void OnAgree()
    {
        Close(UIName.UIAirLineInfoShow);
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendAgreeAirLine, "1");
    }

    private void OnRefuse()
    {
        Close(UIName.UIAirLineInfoShow);
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendAgreeAirLine, "0");
    }
}