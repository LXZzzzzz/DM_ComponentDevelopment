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
    private InputField airPoint1;
    private InputField airPoint2;
    private InputField airPoint3;
    private InputField airPoint4;

    public override void Init()
    {
        base.Init();
        airLineInfo = GetControl<InputField>("InputF_AirLine");
        airPoint1 = GetControl<InputField>("airPoint1");
        airPoint2 = GetControl<InputField>("airPoint2");
        airPoint3 = GetControl<InputField>("airPoint3");
        airPoint4 = GetControl<InputField>("airPoint4");
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        if (userData != null)
        {
            var airLineData = ((string)userData).Split('_');
            airLineInfo.text = airLineData[0];
            airPoint1.text = airLineData[1];
            airPoint2.text = airLineData[2];
            airPoint3.text = airLineData[3];
            airPoint4.text = airLineData[4];
            airLineInfo.interactable = false;
            airPoint1.interactable = false;
            airPoint2.interactable = false;
            airPoint3.interactable = false;
            airPoint4.interactable = false;

            GetControl<Button>("sure").onClick.AddListener(OnAgree);
            GetControl<Button>("cancel").onClick.AddListener(OnRefuse);
        }
        else
        {
            airLineInfo.interactable = true;
            airPoint1.interactable = true;
            airPoint2.interactable = true;
            airPoint3.interactable = true;
            airPoint4.interactable = true;
            airLineInfo.text = String.Empty;
            airPoint1.text = String.Empty;
            airPoint2.text = String.Empty;
            airPoint3.text = String.Empty;
            airPoint4.text = String.Empty;
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
        string airLineData = $"{airLineInfo.text}_{airPoint1.text}_{airPoint2.text}_{airPoint3.text}_{airPoint4.text}";
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendAskForAirLine, airLineData);
    }

    private void OnAgree()
    {
        Close(UIName.UIAirLineInfoShow);
        if(MyDataInfo.gameState>= GameState.GameStart) return;
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendAgreeAirLine, "1");
    }

    private void OnRefuse()
    {
        Close(UIName.UIAirLineInfoShow);
        if(MyDataInfo.gameState>= GameState.GameStart) return;
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendAgreeAirLine, "0");
    }
}