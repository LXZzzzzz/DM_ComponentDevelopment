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
            new InputIntUnitProperty("受灾需转运总人数(救援)", 10, "kg/㎡"),
            new InputFloatUnitProperty("最大巡航速度", 255, "km/h"),
            new InputFloatUnitProperty("单次取水和投水时间", 0.05f, "h"),
            new InputFloatUnitProperty("单次物资装载时间", 0.008f, "h"),
            new InputFloatUnitProperty("单次物资投放时间", 0.0014f, "h"),
            new InputFloatUnitProperty("单次人员吊救时间(救援)", 0.0014f, "h"),
            new InputIntUnitProperty("单次最大运载人数(救援)", 10, "人"),
            new InputFloatUnitProperty("吊桶单次最大装载量/单次最大运载物资重量", 5000, "kg"),
            new InputFloatUnitProperty("直升机每飞行小时耗油量", 1000, "kg"),
            new InputStringProperty("选中色号", "#5B52FF"),
            new InputStringProperty("icon底色色号", "#3C387D"),
            new InputStringProperty("进度条标识", "总")
        };
    }

    private Vector2 CalcAndSetLonLat(Vector3 pos)
    {
        //基准点经纬度,基准经纬度默认Type=DMLonLatType.Normal，LonType=E,LatType=N
        if (mDMLonLat == null) return Vector2.zero;
        var zeroLon = mDMLonLat.HGetField("Longitude");
        var zeroLat = mDMLonLat.HGetField("Latitude");
        double mLon = zeroLon.GetType() != typeof(double) ? 116.4 : (double)zeroLon;
        double mLat = zeroLat.GetType() != typeof(double) ? 39.9 : (double)zeroLat;
        int mScaleRate = (int)mDMLonLat.HGetField("ScaleRate");
        //计算并设置经纬度
        double lat = HarvenSin.GetLatByDis(mLat, pos.z);
        double lon = HarvenSin.GetLonByDis(mLon, pos.x, lat);
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
            int clientLevel = -1;
            string clientLevelName = "";
            Color clientColor = Color.white;
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
                    progrId = (itemMain.Properties[15] as InputStringProperty).Value;
                    
                    clientColorCode = (itemMain.Properties[1] as InputStringProperty).Value;
                    if (ColorUtility.TryParseHtmlString(clientColorCode, out Color color))
                    {
                        clientColor = color;
                    }
                    var itemCodeC = (itemMain.Properties[13] as InputStringProperty).Value;
                    if (ColorUtility.TryParseHtmlString(itemCodeC, out Color colorc))
                    {
                        chooseColor = colorc;
                    }
                    var itemCodeI = (itemMain.Properties[14] as InputStringProperty).Value;
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
                ClientLevelName = clientLevelName, MyColor = clientColor, ColorCode = clientColorCode, ChooseColor = chooseColor, IconBgColor = iconBgColor, progressId = progrId
            });
        }

        InitRecordData();
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
        mDMLonLat = GameObject.Find("DMLonLat").HGetScript("DMLonLat");
        float mapLength = float.Parse(mDMLonLat.HGetField("TerLength").ToString());
        float mapWidth = float.Parse(mDMLonLat.HGetField("TerWidth").ToString());
        mapSizeData = new Vector2(mapLength, mapWidth);
        _commanderController.Init(CalcAndSetLonLat);
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
        cameraObject = GetComponentInChildren<Camera>(true).gameObject;
        cameraObject?.gameObject.SetActive(true);

        yield return 1;
        int myLevel = MyDataInfo.MyLevel = (Properties[0] as DropDownProperty).Selected.Enum;
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "IconShow", null);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "MinMap", mapSizeData);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "CommanderView", myLevel);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "TopMenuView", myLevel);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", null);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "ThreeDIconView", null);
        if (MyDataInfo.isPlayBack)
            EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "CursorShow", null);

        MyDataInfo.sceneAllEquips = new List<EquipBase>();
        yield return new WaitForSeconds(1);
        _commanderController.SendTaskSureMsg();
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
            sender.RunSend(SendType.MainToAll, senderObj.GetComponent<ScriptManager>().BObjectId, eventType, param);
            return;
        }

        switch ((MessageID)eventType)
        {
            case MessageID.SendReceiveTask:
                if (MyDataInfo.leadId != BObjectId) break;
                MyDataInfo.gameState = GameState.FirstLevelCommanderEditor;
                MyDataInfo.speedMultiplier = 1;
                MyDataInfo.gameStartTime = 0;
                _commanderController.Receive_TextMsgRecord("总指挥接受了任务，开始指定方案");
                EventManager.Instance.EventTrigger(EventType.ReceiveTask.ToString());
                break;
            case MessageID.SendProgramme:
                if (MyDataInfo.leadId != BObjectId) break;
                int myLevel = (Properties[0] as DropDownProperty).Selected.Enum;
                sender.LogError(myLevel != 1 ? "我需要接收场景装备数据" : "我就是数据编辑者");
                MyDataInfo.gameState = GameState.Preparation;
                if (myLevel != 1 || MyDataInfo.isPlayBack)
                    _commanderController.Receive_ProgrammeData(param);
                else _commanderController.Receive_TextMsgRecord("下达二级任务");
                break;
            case MessageID.SendGameStart:
                if (MyDataInfo.leadId != BObjectId) break;
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
                if (MyDataInfo.leadId != BObjectId) break;
                MyDataInfo.gameState = int.Parse(param) == 1 ? GameState.GamePause : GameState.GameStart;
                break;
            case MessageID.SendGameStop:
                if (MyDataInfo.leadId != BObjectId) break;
                MyDataInfo.gameState = GameState.GameStop;
                MyDataInfo.gameStartTime = gameStartTimePoint / 1000.0f;
                MyDataInfo.speedMultiplier = 1;
                _commanderController.Receive_GameStop();
                break;
            case MessageID.SendChangeSpeed:
                if (MyDataInfo.leadId != BObjectId) break;
                MyDataInfo.speedMultiplier = float.Parse(param);
                break;
        }

        if (eventType >= 1100)
            _commanderController.Receive_TriggerSkill((MessageID)eventType, param);
    }
}