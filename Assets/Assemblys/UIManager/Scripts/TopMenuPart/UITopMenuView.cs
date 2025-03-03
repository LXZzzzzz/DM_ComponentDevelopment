using System;
using System.Collections.Generic;
using DM.IFS;
using ToolsLibrary;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using EventType = Enums.EventType;

public class UITopMenuView : BasePanel
{
    private int mainLevel;
    private Text ProgrammName;
    private GameObject speedChangePart;
    private Text currentState, currentTime;
    private Button btn_start, btn_pause;
    private Dropdown speedChange;
    private List<float> dropdownValue;
    private Transform menuView;
    private GameObject zongPart, otherPart;
    private Text currentSpeed;


    public override void Init()
    {
        base.Init();
        _myUIType = UIType.upper;
        menuView = transform.Find("menuView");
        zongPart = transform.Find("SpeedChange/zongPart").gameObject;
        otherPart = transform.Find("SpeedChange/otherPart").gameObject;
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
        GetControl<Button>("btn_scbg").onClick.AddListener(() =>
        {
            putAwayMenu();
            EventManager.Instance.EventTrigger(EventType.GeneratePDF.ToString());
        });
        GetControl<Button>("btn_zgdf").onClick.AddListener(() =>
        {
            putAwayMenu();
            UIManager.Instance.ShowPanel<UISubjectiveRatingView>(UIName.UISubjectiveRatingView, null);
        });

        GetControl<Button>("btn_FaStart").onClick.AddListener(OnFaStart);
        GetControl<Button>("btn_FaTurnBack").onClick.AddListener(OnFaTurnBack);

        btn_start.onClick.AddListener(() => OnControlStartAndPause(false));
        btn_pause.onClick.AddListener(() => OnControlStartAndPause(true));
        GetControl<Button>("btn_stop").onClick.AddListener(OnContolStop);
        GetControl<Button>("btn_pdf").onClick.AddListener(OnGeneratePdf);
        GetControl<Button>("btn_upload").onClick.AddListener(OnUpLoad);
        GetControl<Button>("btn_CLose").onClick.AddListener(putAwayMenu);
        currentSpeed = GetControl<Text>("txt_speed");

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
        mainLevel = (int)userData;
        GetControl<Toggle>("Tog_Fazd").isOn = false;
        menuView.Find("PlanFormulation").gameObject.SetActive(mainLevel == 1);
        menuView.Find("CommandDeduction").gameObject.SetActive(mainLevel == 2);
        menuView.Find("ComprehensiveEvaluation").gameObject.SetActive(mainLevel == -1);
        speedChangePart.SetActive(mainLevel == 1);
        // EventManager.Instance.AddEventListener<string>(EventType.ShowProgrammeName.ToString(), ShowName);
        EventManager.Instance.AddEventListener<string>(EventType.ReceiveTask.ToString(), ReceiveTask);
        ShowName();

        dropdownValue = new List<float>() { 0.5f, 1.0f, 1.5f, 2.0f, 5.0f, 10.0f, 20.0f, 50.0f };
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
        // EventManager.Instance.RemoveEventListener<string>(EventType.ShowProgrammeName.ToString(), ShowName);
        EventManager.Instance.RemoveEventListener<string>(EventType.ReceiveTask.ToString(), ReceiveTask);
    }

    private void putAwayMenu()
    {
        foreach (var toggle in menuView.GetComponentsInChildren<Toggle>())
        {
            toggle.isOn = false;
        }

        GetControl<Button>("btn_CLose").gameObject.SetActive(false);
    }

    private void ShowName()
    {
        switch (MyDataInfo.MyLevel)
        {
            case -1:
                ProgrammName.text = "导教端";
                transform.Find("headNameBg").GetChild(0).gameObject.SetActive(true);
                break;
            case 1:
                ProgrammName.text = "值班领导端";
                transform.Find("headNameBg").GetChild(1).gameObject.SetActive(true);
                break;
            case 2:
                ProgrammName.text = "现场指挥端";
                transform.Find("headNameBg").GetChild(2).gameObject.SetActive(true);
                break;
            case 3:
                ProgrammName.text = "机长端";
                transform.Find("headNameBg").GetChild(3).gameObject.SetActive(true);
                break;
            case 4:
                ProgrammName.text = "态势端";
                transform.Find("headNameBg").GetChild(4).gameObject.SetActive(true);
                break;
        }
    }

    private void ReceiveTask(string info)
    {
        currentState.text = info;
    }

    private void newBuild()
    {
        putAwayMenu();
        if (MyDataInfo.gameState == GameState.None)
        {
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "收到任务背景后才可创建方案");
            return;
        }

        ConfirmatonInfo info = new ConfirmatonInfo()
        {
            type = showType.newScheme, sureCallBack = (a) =>
                ProgrammeDataManager.Instance.CreatProgramme((string)a)
        };
        UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, info);
    }

    private void save()
    {
        putAwayMenu();
        ProgrammeDataManager.Instance.SaveProgramme(Application.dataPath + "/MapLib/Scheme");
    }

    private void saveAs()
    {
        putAwayMenu();
        ProgrammeDataManager.Instance.SaveProgramme();
    }

    private void load()
    {
        putAwayMenu();
        var data = ProgrammeDataManager.Instance.LoadProgramme(Application.dataPath + "/MapLib/Scheme");

        if (data != null)
        {
            EventManager.Instance.EventTrigger(Enums.EventType.LoadProgrammeDataSuc.ToString(), data);
        }
        else
        {
            ConfirmatonInfo infoa = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "所选文件解析失败" };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
        }
    }

    private void release()
    {
        if (MyDataInfo.gameState < GameState.AgreeAirLine)
        {
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "还未到下达任务阶段");
            return;
        }

        if (MyDataInfo.gameState != GameState.AgreeAirLine)
        {
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "申报航线后才可下达任务");
            return;
        }

        if (MyDataInfo.gameState > GameState.AgreeAirLine)
        {
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "任务已下达");
            return;
        }

        // if (ProgrammeDataManager.Instance.GetCurrentData == null)
        // {
        //     ConfirmatonInfo infor = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "当前未创建方案无法发布" };
        //     UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infor);
        //     return;
        // }

        putAwayMenu();
        // string packedData = ProgrammeDataManager.Instance.PackedData();

        UIManager.Instance.ShowPanel<UISendTaskInfoShow>(UIName.UISendTaskInfoShow, null);
    }

    private void standAlone()
    {
        putAwayMenu();
        if (MyDataInfo.gameState == GameState.GameStart || MyDataInfo.gameState == GameState.GamePause)
        {
            ConfirmatonInfo infob = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "推演已经开始！！！" };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infob);
            return;
        }

        string packedData = ProgrammeDataManager.Instance.PackedData();
        sender.RunSend(SendType.MainToAll, MyDataInfo.leadId, (int)Enums.MessageID.SendProgramme, packedData);

        sender.RunSend(SendType.MainToAll, MyDataInfo.leadId, (int)Enums.MessageID.SendGameStart, ((int)(MyDataInfo.gameStartTime * 1000)).ToString());

        currentState.text = "实时指挥 > 单机";
        btn_start.gameObject.SetActive(false);
        btn_pause.gameObject.SetActive(true);
    }

    private void onLine()
    {
        putAwayMenu();
        // switch (MyDataInfo.gameState)
        // {
        //     case GameState.FirstLevelCommanderEditor:
        //         ConfirmatonInfo infoa = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "请先发布方案，再通知开始" };
        //         UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
        //         return;
        //     case GameState.GameStart:
        //     case GameState.GamePause:
        //         ConfirmatonInfo infob = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "推演已经开始！！！" };
        //         UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infob);
        //         return;
        //     // case GameState.GameStop:
        //     //     ConfirmatonInfo infoc = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "方案已经停止！！！" };
        //     //     UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoc);
        //     //     break;
        // }

        //只有在准备阶段才能发送开始

        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendGameStart, ((int)(MyDataInfo.gameStartTime * 1000)).ToString());

        currentState.text = "实时指挥 > 联机";
        btn_start.gameObject.SetActive(false);
        btn_pause.gameObject.SetActive(true);
    }

    private void OnFaStart()
    {
        putAwayMenu();
        //开始推演指令
        // for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        // {
        //     bool isOut = ProgrammeDataManager.Instance.GetEquipDataById(MyDataInfo.sceneAllEquips[i].BObjectId)?.isSetOut == 1;
        //     if (isOut && !MyDataInfo.TaskPlanningCompletedPersons.Contains(MyDataInfo.sceneAllEquips[i].BObjectId))
        //     {
        //         ConfirmatonInfo ci = new ConfirmatonInfo() { showStrInfo = "需等到所有出动直升机都完成任务规划才能开始", type = showType.tipView };
        //         UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, ci);
        //         return;
        //     }
        // }

        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendGameStart, ((int)(MyDataInfo.gameStartTime * 1000)).ToString());
    }

    private void OnFaTurnBack()
    {
        putAwayMenu();
        //返航指令

        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendTurnBack, "");
    }

    private void OnControlStartAndPause(bool isPause)
    {
        if (MyDataInfo.gameState < GameState.GameStart)
        {
            ConfirmatonInfo infob = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "推演未开始！！" };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infob);
            return;
        }

        btn_pause.gameObject.SetActive(!isPause);
        btn_start.gameObject.SetActive(isPause);
        //执行逻辑传给所有人

        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendGamePause, (isPause ? 1 : 0).ToString());
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


        if (MyDataInfo.sceneAllEquips.Find(x => !x.isCrash && !x.isDockingAtTheAirport) != null)
        {
            EventManager.Instance.EventTrigger<string, UnityAction<bool>>(EventType.ShowConfirmUI.ToString(), "当前有飞机未入库机场，数据将无法生成报告，是否确认丢弃本次推演数据？",
                (a) => { EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendGameStop, ""); });
            return;
        }

        btn_start.gameObject.SetActive(true);
        btn_pause.gameObject.SetActive(false);
        speedChange.value = 1;

        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendGameStop, "");
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

        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendChangeSpeed, changeSpeed.ToString());
    }

    private void Update()
    {
        currentTime.text = ConvertSecondsToHHMMSS(MyDataInfo.gameStartTime);

        //速度页签要在时间进行阶段显示，zongPart要在总指挥开始推演阶段显示，otherPart要在其他指挥开始推演阶段显示
        speedChangePart.SetActive(MyDataInfo.gameState >= GameState.None);

        if (MyDataInfo.isPlayBack)
        {
            zongPart.SetActive(false);
            otherPart.SetActive(false);
        }
        else
        {
            if (mainLevel == 1) zongPart.SetActive(true);
            else otherPart.SetActive(true);
        }

        currentSpeed.text = $"{MyDataInfo.speedMultiplier:0.0} X";
    }

    private string ConvertSecondsToHHMMSS(float seconds)
    {
        int hours = (int)(seconds / 3600); // 计算小时数
        int minutes = (int)(seconds % 3600 / 60); // 计算分钟数
        float remainingSeconds = seconds % 60; // 计算剩余秒数

        // 格式化为“时：分：秒”字符串
        return $"{hours:00}:{minutes:00}:{(int)remainingSeconds:00}";
    }
}