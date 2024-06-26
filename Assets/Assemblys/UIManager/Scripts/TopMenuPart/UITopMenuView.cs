using System;
using System.Collections.Generic;
using DM.IFS;
using ToolsLibrary;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using EventType = Enums.EventType;

public class UITopMenuView : BasePanel
{
    private Text ProgrammName;
    private GameObject speedChangePart;
    private Text currentState, currentTime;
    private Button btn_start, btn_pause;
    private Dropdown speedChange;
    private List<float> dropdownValue;
    private Transform menuView;


    public override void Init()
    {
        base.Init();
        _myUIType = UIType.upper;
        menuView = transform.Find("menuView");
        ProgrammName = GetControl<Text>("text_PName");
        speedChangePart = transform.Find("SpeedChange").gameObject;
        currentState = GetControl<Text>("currentState");
        currentTime = GetControl<Text>("currentTime");
        btn_start = GetControl<Button>("btn_start");
        btn_pause = GetControl<Button>("btn_pause");
        speedChange = GetControl<Dropdown>("speedChange");
        GetControl<Button>("btn_NewBuild").onClick.AddListener(newBuild);
        GetControl<Button>("btn_Save").onClick.AddListener(save);
        GetControl<Button>("btn_SaveAs").onClick.AddListener(saveAs);
        GetControl<Button>("btn_Load").onClick.AddListener(load);
        GetControl<Button>("btn_Release").onClick.AddListener(release);
        GetControl<Button>("btn_StandAlone").onClick.AddListener(standAlone);
        GetControl<Button>("btn_Online").onClick.AddListener(onLine);

        btn_start.onClick.AddListener(() => OnControlStartAndPause(false));
        btn_pause.onClick.AddListener(() => OnControlStartAndPause(true));
        GetControl<Button>("btn_stop").onClick.AddListener(OnContolStop);
        GetControl<Button>("btn_pdf").onClick.AddListener(OnGeneratePdf);
        GetControl<Button>("btn_upload").onClick.AddListener(OnUpLoad);
        GetControl<Button>("btn_CLose").onClick.AddListener(putAwayMenu);
        GetControl<Toggle>("Tog_Zhty").onValueChanged.AddListener(a =>
        {
            if (!a)
            {
                foreach (var toggle in GetControl<Toggle>("Tog_Zhty").transform.parent.GetComponentsInChildren<Toggle>(true))
                {
                    toggle.isOn = false;
                }
            }
        });
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        int mainLevel = (int)userData;
        GetControl<Toggle>("Tog_Fazd").isOn = false;
        GetControl<Toggle>("Tog_Fazd").transform.parent.gameObject.SetActive(mainLevel == 1);
        speedChangePart.SetActive(mainLevel == 1);
        EventManager.Instance.AddEventListener<string>(EventType.ShowProgrammeName.ToString(), ShowName);
        ProgrammName.text = UIManager.Instance.MisName;

        dropdownValue = new List<float>() { 0.5f, 1.0f, 1.5f, 2.0f, 5.0f };
        speedChange.options.Clear();
        for (int i = 0; i < dropdownValue.Count; i++)
        {
            speedChange.options.Add(new Dropdown.OptionData($"{dropdownValue[i]}X"));
        }

        speedChange.value = 1;
        speedChange.onValueChanged.AddListener(OnChangeSpeed);
    }

    public override void HideMe()
    {
        base.HideMe();
        EventManager.Instance.RemoveEventListener<string>(EventType.ShowProgrammeName.ToString(), ShowName);
    }

    private void putAwayMenu()
    {
        foreach (var toggle in menuView.GetComponentsInChildren<Toggle>())
        {
            toggle.isOn = false;
        }

        GetControl<Button>("btn_CLose").gameObject.SetActive(false);
    }

    private void ShowName(string pName)
    {
        ProgrammName.text = $"{UIManager.Instance.MisName}（{pName}）";
    }

    private void newBuild()
    {
        putAwayMenu();
        ConfirmatonInfo info = new ConfirmatonInfo() { type = showType.newScheme, sureCallBack = runNewScheme };
        UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, info);
    }

    private void runNewScheme(object _info)
    {
        string schemeName = (string)_info;
        ProgrammeDataManager.Instance.CreatProgramme(schemeName);
        EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 1);
        EventManager.Instance.EventTrigger(EventType.ClearProgramme.ToString());
        MyDataInfo.gameState = GameState.FirstLevelCommanderEditor;
        ShowName(schemeName);
    }

    private void save()
    {
        putAwayMenu();
        ProgrammeDataManager.Instance.SaveProgramme();
    }

    private void saveAs()
    {
        putAwayMenu();
        ProgrammeDataManager.Instance.SaveProgramme();
    }

    private void load()
    {
        putAwayMenu();
        var data = ProgrammeDataManager.Instance.LoadProgramme();

        if (data != null)
        {
            EventManager.Instance.EventTrigger(Enums.EventType.LoadProgrammeDataSuc.ToString(), data);
            MyDataInfo.gameState = GameState.FirstLevelCommanderEditor;
        }
        else
        {
            ConfirmatonInfo infoa = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "所选文件解析失败" };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
        }
    }

    private void release()
    {
        putAwayMenu();
        string packedData = ProgrammeDataManager.Instance.PackedData();

        for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
        {
            sender.RunSend(SendType.MainToAll, MyDataInfo.playerInfos[i].RoleId, (int)Enums.MessageID.SendProgramme, packedData);
        }

        EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 0);
        // for (int i = 0; i < allBObjects.Length; i++)
        // {
        //     if (allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 8) != null)
        //     {
        //         sender.RunSend(SendType.MainToAll, allBObjects[i].BObject.Id, (int)Enums.MessageID.SendProgramme, packedData);
        //     }
        // }
    }

    private void standAlone()
    {
        putAwayMenu();
        if ((int)MyDataInfo.gameState >= 2)
        {
            ConfirmatonInfo infob = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "推演已经开始！！！" };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infob);
            return;
        }

        sender.RunSend(SendType.MainToAll, MyDataInfo.leadId, (int)Enums.MessageID.SendGameStart, "");

        currentState.text = "实时指挥 > 单机";
        btn_start.gameObject.SetActive(false);
        btn_pause.gameObject.SetActive(true);
    }

    private void onLine()
    {
        putAwayMenu();
        switch (MyDataInfo.gameState)
        {
            case GameState.FirstLevelCommanderEditor:
                ConfirmatonInfo infoa = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "请先发布方案，再通知开始" };
                UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
                return;
            case GameState.GameStart:
            case GameState.GamePause:
            case GameState.GameStop:
                ConfirmatonInfo infob = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "推演已经开始！！！" };
                UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infob);
                return;
        }

        //只有在准备阶段才能发送开始
        for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
        {
            sender.RunSend(SendType.MainToAll, MyDataInfo.playerInfos[i].RoleId, (int)Enums.MessageID.SendGameStart, "");
        }

        currentState.text = "实时指挥 > 联机";
        btn_start.gameObject.SetActive(false);
        btn_pause.gameObject.SetActive(true);
    }

    private void OnControlStartAndPause(bool isPause)
    {
        if ((int)MyDataInfo.gameState < 2)
        {
            ConfirmatonInfo infob = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "推演未开始！！" };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infob);
            return;
        }

        btn_pause.gameObject.SetActive(!isPause);
        btn_start.gameObject.SetActive(isPause);
        //执行逻辑传给所有人
        for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
        {
            sender.RunSend(SendType.MainToAll, MyDataInfo.playerInfos[i].RoleId, (int)Enums.MessageID.SendGamePause, (isPause ? 1 : 0).ToString());
        }
    }

    private void OnContolStop()
    {
        if ((int)MyDataInfo.gameState < 2)
        {
            ConfirmatonInfo infob = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "推演未开始！！" };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infob);
            return;
        }

        if (MyDataInfo.gameState == GameState.GameStop) return;
        btn_start.gameObject.SetActive(true);
        btn_pause.gameObject.SetActive(false);
        for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
        {
            sender.RunSend(SendType.MainToAll, MyDataInfo.playerInfos[i].RoleId, (int)Enums.MessageID.SendGameStop, "");
        }
    }

    private void OnGeneratePdf()
    {
        EventManager.Instance.EventTrigger(EventType.GeneratePDF.ToString());
    }

    private void OnUpLoad()
    {
    }

    private void OnChangeSpeed(int index)
    {
        float changeSpeed = dropdownValue[index];
        if ((int)(changeSpeed * 100) == (int)(MyDataInfo.speedMultiplier * 100)) return;
        for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
        {
            sender.RunSend(SendType.MainToAll, MyDataInfo.playerInfos[i].RoleId, (int)Enums.MessageID.SendChangeSpeed, changeSpeed.ToString());
        }
    }

    private void Update()
    {
        if (MyDataInfo.gameState == GameState.GameStart)
        {
            MyDataInfo.gameStartTime += Time.deltaTime * MyDataInfo.speedMultiplier;
            currentTime.text = ConvertSecondsToHHMMSS(MyDataInfo.gameStartTime);
        }
    }

    private string ConvertSecondsToHHMMSS(float seconds)
    {
        int hours = (int)(seconds / 3600); // 计算小时数
        int minutes = (int)(seconds % 3600 / 60); // 计算分钟数
        float remainingSeconds = seconds % 60; // 计算剩余秒数

        // 格式化为“时：分：秒”字符串
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, (int)remainingSeconds);
    }
}