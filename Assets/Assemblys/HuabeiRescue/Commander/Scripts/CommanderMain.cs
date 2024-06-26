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
    private List<EnumDescription> commanderLevel;
    private CommanderController _commanderController;
    private bool isMain;

    private void Awake()
    {
        commanderLevel = new List<EnumDescription>();
        commanderLevel.Add(new EnumDescription(1, "一级指挥官"));
        commanderLevel.Add(new EnumDescription(2, "二级指挥官"));
        List<EnumDescription> minMapSJY = new List<EnumDescription>();
        minMapSJY.Add(new EnumDescription(0, "中立"));
        minMapSJY.Add(new EnumDescription(1, "红方"));
        minMapSJY.Add(new EnumDescription(2, "蓝方"));
        Properties = new DynamicProperty[]
        {
            new DropDownProperty("指挥官等级", commanderLevel, 0),
            new InputStringProperty("十六进制标志色号", "")
        };
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
            for (int j = 0; j < allBObjects.Length; j++)
            {
                var itemMain = allBObjects[j].GetComponent<ScriptManager>();
                if (itemMain != null && string.Equals(itemMain.BObjectId, info.ClientInfos[i].RoleId))
                {
                    clientLevel = (itemMain.Properties[0] as DropDownProperty).Selected.Enum;
                    clientLevelName = allBObjects[j].BObject.Info.Name;
                    if (ColorUtility.TryParseHtmlString((itemMain.Properties[1] as InputStringProperty).Value, out Color color))
                    {
                        clientColor = color;
                    }

                    sender.LogError((itemMain.Properties[1] as InputStringProperty).Value + ":" + clientColor);
                    break;
                }
            }

            sender.LogError(info.ClientInfos[i].Name + "等级：" + clientLevel);
            MyDataInfo.playerInfos.Add(new ClientInfo()
            {
                PlayerName = info.ClientInfos[i].Name, RoleId = info.ClientInfos[i].RoleId, UID = info.ClientInfos[i].UID, ClientLevel = clientLevel,
                ClientLevelName = clientLevelName, MyColor = clientColor
            });
        }
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
        float mapLength = float.Parse(GameObject.Find("DMLonLat").HGetScript("DMLonLat").HGetField("TerLength").ToString());
        float mapWidth = float.Parse(GameObject.Find("DMLonLat").HGetScript("DMLonLat").HGetField("TerWidth").ToString());
        mapSizeData = new Vector2(mapLength, mapWidth);
        _commanderController.Init();
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
        if (MyDataInfo.isPlayBack)
            EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "CursorShow", null);

        MyDataInfo.sceneAllEquips = new List<EquipBase>();
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
            case MessageID.SendProgramme:
                if (MyDataInfo.leadId != BObjectId) break;
                int myLevel = (Properties[0] as DropDownProperty).Selected.Enum;
                sender.LogError(myLevel != 1 ? "我需要接收场景装备数据" : "我就是数据编辑者");
                MyDataInfo.gameState = GameState.Preparation;
                if (myLevel != 1)
                    _commanderController.Receive_ProgrammeData(param);
                break;
            case MessageID.SendGameStart:
                if (MyDataInfo.leadId != BObjectId) break;
                MyDataInfo.gameState = GameState.GameStart;
                MyDataInfo.speedMultiplier = 1;
                MyDataInfo.gameStartTime = 0;
                EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 0);
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
                MyDataInfo.gameStartTime = 0;
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