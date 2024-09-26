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
        commanderLevel.Add(new EnumDescription(1, "一级指挥官"));
        commanderLevel.Add(new EnumDescription(2, "二级指挥官"));
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
            new InputIntUnitProperty("无用--受灾需转运总人数(救援)", 10, "kg/㎡"),
            new InputFloatUnitProperty("无用--最大巡航速度", 255, "km/h"),
            new InputFloatUnitProperty("无用--单次取水和投水时间", 0.05f, "h"),
            new InputFloatUnitProperty("无用--单次物资装载时间", 0.008f, "h"),
            new InputFloatUnitProperty("无用--单次物资投放时间", 0.0014f, "h"),
            new InputFloatUnitProperty("无用--单次人员吊救时间(救援)", 0.0014f, "h"),
            new InputIntUnitProperty("无用--单次最大运载人数(救援)", 10, "人"),
            new InputFloatUnitProperty("无用--吊桶单次最大装载量/单次最大运载物资重量", 5000, "kg"),
            new InputFloatUnitProperty("无用--直升机每飞行小时耗油量", 1000, "kg"),
            new InputStringProperty("默认色号", "#5B52FF"),
            new InputStringProperty("选中色号", "#5B52FF"),
            new InputStringProperty("icon底色色号", "#3C387D"),
            new InputStringProperty("进度条标识", "总")
        };
    }

    private Vector2 CalcAndSetLonLat(Vector3 pos)
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
                    progrId = (itemMain.Properties[16] as InputStringProperty).Value;

                    clientColorCode = (itemMain.Properties[1] as InputStringProperty).Value;
                    if (ColorUtility.TryParseHtmlString(clientColorCode, out Color color))
                    {
                        clientColor = color;
                    }

                    var itemCodeN = (itemMain.Properties[13] as InputStringProperty).Value;
                    if (ColorUtility.TryParseHtmlString(itemCodeN, out Color colorn))
                    {
                        normalColor = colorn;
                    }

                    var itemCodeC = (itemMain.Properties[14] as InputStringProperty).Value;
                    if (ColorUtility.TryParseHtmlString(itemCodeC, out Color colorc))
                    {
                        chooseColor = colorc;
                    }

                    var itemCodeI = (itemMain.Properties[15] as InputStringProperty).Value;
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
        var fields = _commanderController.cdata.GetType().GetFields();
        for (int i = 3; i < 12; i++)
        {
            if (fields[i - 3].FieldType == typeof(Int32))
            {
                fields[i - 3].SetValue(_commanderController.cdata, (Properties[i] as InputIntUnitProperty).Value);
            }
            else
            {
                fields[i - 3].SetValue(_commanderController.cdata, (Properties[i] as InputFloatUnitProperty).Value);
            }
        }

        _commanderController.gameType = (Properties[2] as DropDownProperty).Selected.Enum;
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
        if (myLevel == -1) EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "CommanderDirector", null);
        else EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "CommanderView", myLevel);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "TopMenuView", myLevel);
        if (myLevel != -1) EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", null);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "ThreeDIconView", null);
        // if (MyDataInfo.isPlayBack)
        //     EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "CursorShow", null);

        yield return 1;
        _commanderController.Init(CalcAndSetLonLat);
        MyDataInfo.sceneAllEquips = new List<EquipBase>();
        yield return new WaitForSeconds(1);
        if (myLevel == 1)
            _commanderController.SendTaskSureMsg();
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
                progrId = (itemMain.Properties[16] as InputStringProperty).Value;

                clientColorCode = (itemMain.Properties[1] as InputStringProperty).Value;
                if (ColorUtility.TryParseHtmlString(clientColorCode, out Color color))
                {
                    clientColor = color;
                }

                var itemCodeN = (itemMain.Properties[13] as InputStringProperty).Value;
                if (ColorUtility.TryParseHtmlString(itemCodeN, out Color colorn))
                {
                    normalColor = colorn;
                }

                var itemCodeC = (itemMain.Properties[14] as InputStringProperty).Value;
                if (ColorUtility.TryParseHtmlString(itemCodeC, out Color colorc))
                {
                    chooseColor = colorc;
                }

                var itemCodeI = (itemMain.Properties[15] as InputStringProperty).Value;
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
            case MessageID.SendReceiveTask:
                MyDataInfo.gameState = GameState.FirstLevelCommanderEditor;
                MyDataInfo.speedMultiplier = 1;
                MyDataInfo.gameStartTime = 0;
                _commanderController.Receive_TextMsgRecord("总指挥接受了任务，开始指定方案");
                EventManager.Instance.EventTrigger(EventType.ReceiveTask.ToString());
                break;
            case MessageID.SendProgramme:
                int myLevel = (Properties[0] as DropDownProperty).Selected.Enum;
                sender.LogError(myLevel != 1 ? "我需要接收场景装备数据" : "我就是数据编辑者");
                MyDataInfo.gameState = GameState.Preparation;
                if (myLevel != 1 || MyDataInfo.isPlayBack)
                    _commanderController.Receive_ProgrammeData(param);
                else _commanderController.Receive_TextMsgRecord("下达二级任务");
                break;
            case MessageID.SendGameStart:
                MyDataInfo.gameState = GameState.GameStart;
                if (gameStartTimePoint < 0) gameStartTimePoint = int.Parse(param);
                MyDataInfo.speedMultiplier = 1;
                MyDataInfo.gameStartTime = gameStartTimePoint / 1000.0f;
                EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 0);
                _commanderController.Receive_TextMsgRecord("推演开始！");
                EventManager.Instance.EventTrigger(EventType.SetMyEquipIconLayer.ToString());
                _commanderController.Receive_GameStart();
                break;
            case MessageID.MoveToTarget:
                sender.LogError("收到了移动的指令" + type);
                _commanderController.Receive_MoveEquipToTarget(param);
                break;
            case MessageID.SendGamePause:
                MyDataInfo.gameState = int.Parse(param) == 1 ? GameState.GamePause : GameState.GameStart;
                break;
            case MessageID.SendGameStop:
                MyDataInfo.gameState = GameState.GameStop;
                MyDataInfo.gameStartTime = gameStartTimePoint / 1000.0f;
                MyDataInfo.speedMultiplier = 1;
                _commanderController.Receive_GameStop();
                break;
            case MessageID.SendChangeSpeed:
                MyDataInfo.speedMultiplier = float.Parse(param);
                break;
            case MessageID.SendChangeController:
                _commanderController.Receive_ChangeController(param);
                break;
            case MessageID.SendChangeZaiqu:
                sender.LogError("收到创建灾区的消息");
                _commanderController.Receive_CreatZaiqu(param);
                break;
            case MessageID.SendMarkMapPoint:
                _commanderController.Receive_ShowMarkPoint(param);
                break;
        }

        if (eventType >= 1100)
            _commanderController.Receive_TriggerSkill((MessageID)eventType, param);
    }
}