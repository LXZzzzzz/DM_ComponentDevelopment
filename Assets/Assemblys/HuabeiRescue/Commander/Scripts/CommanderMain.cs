using System;
using System.Collections;
using System.Collections.Generic;
using DM.Entity;
using DM.IFS;
using Enums;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using EventType = Enums.EventType;

public class CommanderMain : ScriptManager, IControl, IMesRec
{
    private List<EnumDescription> commanderLevel, taskType;
    private CommanderController _commanderController;
    private bool isMain;
    private int gameStartTimePoint;
    private MonoBehaviour mDMLonLat;

    private void Awake()
    {
        commanderLevel = new List<EnumDescription>();
        commanderLevel.Add(new EnumDescription(1, "总指挥端"));
        commanderLevel.Add(new EnumDescription(2, "前线指挥端"));
        commanderLevel.Add(new EnumDescription(3, "机长端"));
        commanderLevel.Add(new EnumDescription(4, "态势端"));
        commanderLevel.Add(new EnumDescription(-1, "导教端"));
        taskType = new List<EnumDescription>();
        taskType.Add(new EnumDescription(1, "灭火"));
        taskType.Add(new EnumDescription(2, "救援"));
        List<EnumDescription> minMapSJY = new List<EnumDescription>();
        minMapSJY.Add(new EnumDescription(0, "中立"));
        minMapSJY.Add(new EnumDescription(1, "红方"));
        minMapSJY.Add(new EnumDescription(2, "蓝方"));
        Properties = new DynamicProperty[]
        {
            new DropDownProperty("指挥官等级", commanderLevel, 0),
            new InputStringProperty("十六进制标志色号", "#3C387D"),
            new DropDownProperty("任务类型", taskType, 0),

            new InputFloatUnitProperty("单位燃烧面积投水需求/人均救援物资需求", 2.5f, "kg/㎡(kg/人)"),
            new InputStringProperty("默认色号", "#5B52FF"),
            new InputStringProperty("选中色号", "#5B52FF"),
            new InputStringProperty("icon底色色号", "#3C387D"),
            new InputStringProperty("进度条标识", "总")
        };
    }

    private Vector2 Pos2LonLat(Vector3 pos)
    {
        //基准点经纬度,基准经纬度默认Type=DMLonLatType.Normal，LonType=E,LatType=N
        Debug.LogError("移动调用" + pos);
        if (mDMLonLat == null) return Vector2.zero;
        var zeroLon = mDMLonLat.HGetField("Longitude");
        Debug.LogError("Long数值：" + zeroLon);
        var zeroLat = mDMLonLat.HGetField("Latitude");
        Debug.LogError(zeroLon + "--" + zeroLat);
        double mLon = double.Parse(zeroLon.ToString()); //zeroLon.GetType() != typeof(double) ? 116.4 : (double)zeroLon;
        double mLat = double.Parse(zeroLat.ToString()); //zeroLat.GetType() != typeof(double) ? 39.9 : (double)zeroLat;
        int mScaleRate = (int)mDMLonLat.HGetField("ScaleRate");
        Debug.LogError("mScaleRate:" + mScaleRate);
        //计算并设置经纬度
        double lat = HarvenSin.GetLatByDis(mLat, pos.z);
        double lon = HarvenSin.GetLonByDis(mLon, pos.x, lat);
        Debug.LogError("Lon:" + (float)lon + "Lat:" + (float)lat);
        return new Vector2((float)lon, (float)lat);
    }

    private Vector3 LonLat2Pos(Vector2 lonLat)
    {
        if (mDMLonLat == null) return Vector2.zero;
        var zeroLon = mDMLonLat.HGetField("Longitude");
        var zeroLat = mDMLonLat.HGetField("Latitude");
        double mLon = double.Parse(zeroLon.ToString()); //zeroLon.GetType() != typeof(double) ? 116.4 : (double)zeroLon;
        double mLat = double.Parse(zeroLat.ToString()); //zeroLat.GetType() != typeof(double) ? 39.9 : (double)zeroLat;
        int mScaleRate = (int)mDMLonLat.HGetField("ScaleRate");
        Vector3 point = mDMLonLat.transform.position;
        float vecX = (float)HarvenSin.DisLon(mLon, lonLat.x, mLat);
        float vecZ = (float)HarvenSin.DisLat(mLat, lonLat.y);
        return new Vector3(vecX / mScaleRate, 0, vecZ / mScaleRate) + point;
    }

    public override void EditorModeInitialized()
    {
        base.EditorModeInitialized();
        sender.DebugMode = true;
        sender.LogError("进入编辑模式" + (Properties[0] as DropDownProperty).Selected.Enum);
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        sender.LogError("进入运行模式:" + (Properties[0] as DropDownProperty).Selected.Enum);
        _commanderController = gameObject.AddComponent<CommanderController>();
        isMain = isRoomCreator;
        MyDataInfo.playerInfos = new List<ClientInfo>();
        for (int i = 0; i < info.ClientInfos.Count; i++)
        {
            int clientLevel = 0;
            string clientLevelName = "";
            Color clientColor = Color.white;
            Color normalColor = Color.white;
            Color chooseColor = Color.white;
            Color iconBgColor = Color.white;
            string clientColorCode = "";
            string progrId = "";
            for (int j = 0; j < allBObjects.Length; j++)
            {
                var itemMain = allBObjects[j].GetComponent<ScriptManager>();
                if (itemMain != null && string.Equals(itemMain.BObjectId, info.ClientInfos[i].RoleId))
                {
                    clientLevel = (itemMain.Properties[0] as DropDownProperty).Selected.Enum;
                    clientLevelName = allBObjects[j].BObject.Info.Name;
                    progrId = (itemMain.Properties[7] as InputStringProperty).Value;

                    clientColorCode = (itemMain.Properties[1] as InputStringProperty).Value;
                    if (ColorUtility.TryParseHtmlString(clientColorCode, out Color color))
                    {
                        clientColor = color;
                    }

                    var itemCodeN = (itemMain.Properties[4] as InputStringProperty).Value;
                    if (ColorUtility.TryParseHtmlString(itemCodeN, out Color colorn))
                    {
                        normalColor = colorn;
                    }

                    var itemCodeC = (itemMain.Properties[5] as InputStringProperty).Value;
                    if (ColorUtility.TryParseHtmlString(itemCodeC, out Color colorc))
                    {
                        chooseColor = colorc;
                    }

                    var itemCodeI = (itemMain.Properties[6] as InputStringProperty).Value;
                    if (ColorUtility.TryParseHtmlString(itemCodeI, out Color colori))
                    {
                        iconBgColor = colori;
                    }


                    sender.LogError((itemMain.Properties[1] as InputStringProperty).Value + ":" + clientColor);
                    break;
                }
            }

            sender.LogError(info.ClientInfos[i].Name + "等级：" + clientLevel);
            MyDataInfo.playerInfos.Add(new ClientInfo()
            {
                PlayerName = info.ClientInfos[i].Name, RoleId = info.ClientInfos[i].RoleId, UID = info.ClientInfos[i].UID, ClientLevel = clientLevel,
                ClientLevelName = clientLevelName, MyColor = clientColor, ColorCode = clientColorCode, NormalColor = normalColor, ChooseColor = chooseColor, IconBgColor = iconBgColor, progressId = progrId
            });
        }

        InitRecordData();
        mDMLonLat = GameObject.Find("DMLonLat").HGetScript("DMLonLat");
        _commanderController.misName = info.MisName;
        _commanderController.misDescription = info.MisDescription;
    }

    private void InitRecordData()
    {
        _commanderController.cdata = new ComanderData();
        _commanderController.cdata.dwrsmjtsxq = (Properties[3] as InputFloatUnitProperty).Value;
        MyDataInfo.gameScene = _commanderController.gameType = (Properties[2] as DropDownProperty).Selected.Enum;

        // var fields = _commanderController.cdata.GetType().GetFields();
        // for (int i = 3; i < 12; i++)
        // {
        //     if (fields[i - 3].FieldType == typeof(Int32))
        //     {
        //         fields[i - 3].SetValue(_commanderController.cdata, (Properties[i] as InputIntUnitProperty).Value);
        //     }
        //     else
        //     {
        //         fields[i - 3].SetValue(_commanderController.cdata, (Properties[i] as InputFloatUnitProperty).Value);
        //     }
        // }
    }

    public override void PropertiesChanged(DynamicProperty[] pros)
    {
        base.PropertiesChanged(pros);
        // sender.LogError($"{name}:修改了属性:"+(pros[0] as InputIntProperty).Selected.Enum);
    }

    private Vector2 mapSizeData;

    public void Active(DevType type, bool playback)
    {
        //打开控制相机
        //根据自己的角色等级，告知UI展示谁
        sender.LogError($"{name}:以我为主角运行:" + (Properties[0] as DropDownProperty).Selected.Enum);
        MyDataInfo.isHost = isMain;
        MyDataInfo.leadId = BObjectId;
        MyDataInfo.isPlayBack = playback;
        MyDataInfo.SceneGoParent = transform.Find("AllGoParent");
        MyDataInfo.SceneGoParent.position = Vector3.zero;
        MyDataInfo.gameState = GameState.None;
        MyDataInfo.sceneAllEquips = new List<EquipBase>();
        MyDataInfo.SkillsToBeConfirmed = new List<string>();
        MyDataInfo.TaskPlanningCompletedPersons = new List<string>();
        gameStartTimePoint = -1;
        if (playback) OnInitPlayBackPlayerInfos();
        float mapLength = float.Parse(mDMLonLat.HGetField("TerLength").ToString());
        float mapWidth = float.Parse(mDMLonLat.HGetField("TerWidth").ToString());
        mapSizeData = new Vector2(mapLength, mapWidth);
        cameraObject = GetComponentInChildren<Camera>(true).gameObject;
        cameraObject?.gameObject.SetActive(true);
        StartCoroutine(InitMain());
    }

    IEnumerator InitMain()
    {
        yield return 1;

        // cameraObject = new GameObject("Main Camera");
        // cameraObject.transform.parent = transform.parent;
        // cameraObject.tag = "MainCamera";
        // cameraObject.transform.position = transform.position+Vector3.up*10;
        // cameraObject.AddComponent<Camera>();
        // cameraObject = GetComponentInChildren<Camera>(true).gameObject;
        // cameraObject?.gameObject.SetActive(true);

        yield return 1;
        int myLevel = MyDataInfo.MyLevel = (Properties[0] as DropDownProperty).Selected.Enum;
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "IconShow", null);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "MinMap", mapSizeData);
        // if (myLevel == -1) EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "CommanderDirector", null);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "CommanderView", myLevel);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "TopMenuView", myLevel);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", null);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "ThreeDIconView", null);
        // if (MyDataInfo.isPlayBack)
        //     EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "CursorShow", null);

        yield return 1;
        _commanderController.Init(Pos2LonLat, LonLat2Pos);
        //大庆版本下的初始化，为了适应改变后的直升机创建模式
        _commanderController.Init();
        if (myLevel == 3)
            _commanderController.Init(BObjectId);
        yield return new WaitForSeconds(1);
        // if (myLevel == 1)
        //     _commanderController.SendTaskSureMsg();
    }

    private void OnInitPlayBackPlayerInfos()
    {
        MyDataInfo.playerInfos = new List<ClientInfo>();
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 8) == null) continue;

            var itemMain = allBObjects[i].GetComponent<ScriptManager>();
            if (itemMain != null)
            {
                int clientLevel = -1;
                string clientLevelName = allBObjects[i].BObject.Info.Name;
                Color clientColor = Color.white;
                Color normalColor = Color.white;
                Color chooseColor = Color.white;
                Color iconBgColor = Color.white;
                string clientColorCode = "";
                string progrId = "";

                clientLevel = (itemMain.Properties[0] as DropDownProperty).Selected.Enum;
                progrId = (itemMain.Properties[7] as InputStringProperty).Value;

                clientColorCode = (itemMain.Properties[1] as InputStringProperty).Value;
                if (ColorUtility.TryParseHtmlString(clientColorCode, out Color color))
                {
                    clientColor = color;
                }

                var itemCodeN = (itemMain.Properties[4] as InputStringProperty).Value;
                if (ColorUtility.TryParseHtmlString(itemCodeN, out Color colorn))
                {
                    normalColor = colorn;
                }

                var itemCodeC = (itemMain.Properties[5] as InputStringProperty).Value;
                if (ColorUtility.TryParseHtmlString(itemCodeC, out Color colorc))
                {
                    chooseColor = colorc;
                }

                var itemCodeI = (itemMain.Properties[6] as InputStringProperty).Value;
                if (ColorUtility.TryParseHtmlString(itemCodeI, out Color colori))
                {
                    iconBgColor = colori;
                }

                MyDataInfo.playerInfos.Add(new ClientInfo()
                {
                    PlayerName = "回放空玩家", RoleId = itemMain.BObjectId, UID = "无用ID", ClientLevel = clientLevel,
                    ClientLevelName = clientLevelName, MyColor = clientColor, ColorCode = clientColorCode,
                    NormalColor = normalColor, ChooseColor = chooseColor, IconBgColor = iconBgColor, progressId = progrId
                });
            }
        }
    }

    public void DeActive(DevType type, bool playback)
    {
    }

    private void OnDestroy()
    {
        _commanderController?.Terminate();
        _commanderController = null;
        for (int i = 0; i < MyDataInfo.sceneAllEquips?.Count; i++)
        {
            Destroy(MyDataInfo.sceneAllEquips[i].gameObject);
        }

        MyDataInfo.sceneAllEquips?.Clear();
        cameraObject?.gameObject.SetActive(false);
    }

    private GameObject cameraObject;

    public void RecMessage(SendType type, GameObject senderObj, int eventType, string param)
    {
        if (type == SendType.SubToMain)
        {
            for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
            {
                sender.LogError(MyDataInfo.playerInfos.Count + "发送给" + MyDataInfo.playerInfos[i].PlayerName);
                sender.RunSend(SendType.MainToAll, MyDataInfo.playerInfos[i].RoleId, eventType, param);
            }

            return;
        }

        if (MyDataInfo.leadId != BObjectId) return;

        switch ((MessageID)eventType)
        {
            case MessageID.SendProgramme:
                MyDataInfo.gameState = GameState.ReleaseProgramme;
                _commanderController.Receive_ProgrammeData(param);
                MyDataInfo.speedMultiplier = 1;
                MyDataInfo.gameStartTime = 0;
                _commanderController.Receive_TextMsgRecord("值班领导下达了任务");
                break;
            case MessageID.SendGameStart:
                Debug.LogError("收到了开始");
                MyDataInfo.gameState = GameState.GameStart;
                if (gameStartTimePoint < 0) gameStartTimePoint = int.Parse(param);
                MyDataInfo.speedMultiplier = 1;
                MyDataInfo.gameStartTime = gameStartTimePoint / 1000.0f;
                _commanderController.Receive_TextMsgRecord("前线指挥控制推演开始！");
                _commanderController.Receive_GameStart();
                break;
            case MessageID.MoveToTarget:
                sender.LogError("收到了移动的指令" + type);
                _commanderController.Receive_MoveEquipToTarget(param);
                break;
            case MessageID.SendGamePause:
                MyDataInfo.gameState = int.Parse(param) == 1 ? GameState.GamePause : GameState.GameStart;
                break;
            case MessageID.SendChangeSpeed:
                MyDataInfo.speedMultiplier = float.Parse(param);
                break;
            case MessageID.SendChangeZaiqu:
                sender.LogError("收到创建灾区的消息");
                _commanderController.Receive_CreatZaiqu(param);
                break;
            case MessageID.SendPathPlanningData:
                //收到规划数据，展示到界面上，
                _commanderController.Receive_PathPlanningData(param);
                break;
            case MessageID.SendSkillConfirmation:
                MyDataInfo.SkillsToBeConfirmed.Add(param);
                break;
            case MessageID.SendEquipBindingZiyuan:
                _commanderController.Receive_ChangeEquipBindings(param);
                break;
            case MessageID.SendEquipState:
                _commanderController.Receive_ChangeEquipState(param);
                _commanderController.Receive_TextMsgRecord("特情信息：直升机装备发生故障");
                break;
            case MessageID.SendTianQi:
                _commanderController.OnChangeTianQi(param);
                break;
            case MessageID.SendAskForAirLine:
                //如果是导教端，就弹出航线申报消息，让他选择是否同意
                if (MyDataInfo.MyLevel == -1)
                    EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AirLineInfoShow", param);

                _commanderController.Receive_TextMsgRecord("值班领导进行航线申报");
                break;
            case MessageID.SendAgreeAirLine:
                //这里如果是总指挥，就弹提示窗，告知航线申请反馈，如果同意就进入下一阶段
                if (int.Parse(param) == 1)
                {
                    MyDataInfo.gameState = GameState.AgreeAirLine;
                    if (MyDataInfo.MyLevel == 1)
                        EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "航线确认成功");
                }
                else
                {
                    if (MyDataInfo.MyLevel == 1)
                        EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "航线信息有误，请重新申报");
                }

                break;
            case MessageID.SendAskForTaskExecute:
                //这里如果是总指挥，就弹二次确认窗口，询问是否同意任务执行，让他选择是否同意
                _commanderController.OnAskTaskExecute();
                _commanderController.Receive_TextMsgRecord("前线指挥员申请任务执行");
                break;
            case MessageID.SendAgreeTaskExecute:
                //如果是机长，就让他的地图模式改为Plane模式，并弹窗提示可以开始任务规划
                MyDataInfo.gameState = GameState.AgreeTaskExecute;
                if (MyDataInfo.MyLevel == 3)
                    _commanderController.OnOpenPlanningMode();
                break;
            case MessageID.SendTaskPlanningCompleted:
                //前指收到这个通知，存起来，如果每架飞机都收到，那就可以点击开始推演
                _commanderController.OnReceiveRwghwc(param);
                break;
            case MessageID.SendTurnBack:
                //如果是机长，就让其控制直升机执行返回机场并入库操作
                _commanderController.OnReturnBack();
                _commanderController.Receive_TextMsgRecord("前线指挥员发送返航指令");
                break;
            case MessageID.SendTaskBgInfo:
                _commanderController.Receive_SetTaskBg(param);
                break;
            case MessageID.SendCompleteTaskBgSet:
                MyDataInfo.gameState = GameState.CompleteTaskBgSet;
                _commanderController.Receive_CompleteBgSet();
                _commanderController.Receive_TextMsgRecord("导教端完成任务设置");
                break;
            case MessageID.SendZySetData:
                _commanderController.OnSetZyInfo(param);
                break;
            case MessageID.SendUseEquips:
                //这里的数据是出动直升机信息，要让不出动的直升机在列表和地图不显示
                _commanderController.OnSetEquipShow(param);
                break;
            case MessageID.SendUsePersons:
                //这里的数据是可用机组和保障信息，要让总指挥页面的下拉框修改一下
                _commanderController.OnSetPersonInfo(param);
                break;
            case MessageID.SendAskForReturn:
                _commanderController.OnAskForReturn(param);
                break;
            case MessageID.SendAgreeReturn:
                _commanderController.OnReturnRepair(param);
                break;
            case MessageID.SendRwghData:
                if (MyDataInfo.MyLevel == -1) _commanderController.OnShowRwghData(param);
                break;
            case MessageID.SendDiscoverNewDisaster:
                //收到上报新灾情，让前指处理
                _commanderController.OnDiscoverNewDisaster(param);
                break;
            case MessageID.SendAgreeDiscoverNewDisaster:
                if (MyDataInfo.MyLevel != 3) EventManager.Instance.EventTrigger(EventType.HideGoIcon.ToString(), string.Empty);
                break;
            case MessageID.SendChangeEquipOilAndLoad:
                _commanderController.OnChangeEquipInfo(param);
                break;
            case MessageID.SendChangeZiyuanData:
                _commanderController.OnChangeZiyuanInfo(param);
                break;


            //这下面的case逻辑不需要了
            case MessageID.SendReceiveTask:
                // MyDataInfo.gameState = GameState.FirstLevelCommanderEditor;
                MyDataInfo.speedMultiplier = 1;
                MyDataInfo.gameStartTime = 0;
                _commanderController.Receive_TextMsgRecord("总指挥接受了任务，开始指定方案");
                EventManager.Instance.EventTrigger(EventType.ReceiveTask.ToString(), "总指挥制定方案中");
                break;
            case MessageID.SendChangeController:
                _commanderController.Receive_ChangeController(param);
                break;
            case MessageID.SendGetChangeZQPower:
                //暂停进度，并打开地图编辑模式
                _commanderController.Receive_GetChangeZiyPower();
                break;
            case MessageID.SendLoseChangeZQPower:
                //恢复进度，并关闭地图编辑模式
                _commanderController.Receive_LoseChangeZiyPower();
                break;
            case MessageID.SendGameStop:
                MyDataInfo.gameState = GameState.GameStop;
                MyDataInfo.gameStartTime = gameStartTimePoint / 1000.0f;
                MyDataInfo.speedMultiplier = 1;
                _commanderController.Receive_GameStop();
                break;
            case MessageID.SendMarkMapPoint:
                _commanderController.Receive_ShowMarkPoint(param);
                break;
        }

        if (eventType >= 1100)
            _commanderController.Receive_TriggerSkill((MessageID)eventType, param);
    }
}