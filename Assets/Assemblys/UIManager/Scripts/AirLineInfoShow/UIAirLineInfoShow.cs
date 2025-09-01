using System;
using System.Collections;
using System.IO;
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
            LoadDefaultData(out string lineStr, out string linePoints);
            airLineInfo.text = lineStr;
            var points = linePoints.Split('_');
            airPoint1.text = points.Length > 0 ? points[0] : String.Empty;
            airPoint2.text = points.Length > 1 ? points[1] : String.Empty;
            airPoint3.text = points.Length > 2 ? points[2] : String.Empty;
            airPoint4.text = points.Length > 3 ? points[3] : String.Empty;
            GetControl<Button>("sure").onClick.AddListener(OnSure);
            GetControl<Button>("cancel").onClick.AddListener(() => Close(UIName.UIAirLineInfoShow));
        }

        if (MyDataInfo.isPlayBack) StartCoroutine(closeMe());
    }

    private void LoadDefaultData(out string lineStr, out string linePoints)
    {
        string filePath = Path.Combine(Application.dataPath, "MapLib", "DefaultData", "AirLineData.txt");
        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            lineStr = linePoints = String.Empty;
            return;
        }

        // 读取文件内容
        string fileContent = File.ReadAllText(filePath);
        var dataSplit = fileContent.Split(':');
        if (dataSplit.Length < 2)
        {
            lineStr = fileContent;
            linePoints = string.Empty;
        }
        else
        {
            lineStr = dataSplit[0];
            linePoints = dataSplit[1];
        }
    }

    IEnumerator closeMe()
    {
        yield return new WaitForSeconds(3);
        Close(UIName.UIAirLineInfoShow);
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
        if (MyDataInfo.gameState >= GameState.GameStart) return;
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendAgreeAirLine, "1");
    }

    private void OnRefuse()
    {
        Close(UIName.UIAirLineInfoShow);
        if (MyDataInfo.gameState >= GameState.GameStart) return;
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendAgreeAirLine, "0");
    }
}