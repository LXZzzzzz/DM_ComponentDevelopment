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
            new InputFloatProperty("指挥官等级临时", 1),
            new DropDownProperty("指挥官等级", commanderLevel, 0),
            new InputStringProperty("任务名", ""),
            new DropDownSceneSelectProperty("关联环境组件"),
            new OpenFileDialogProperty("评估报告路径", ""),
            new ToggleProperty("启用经济系统", false),
            new LabelProperty("-------小地图属性"),
            new DropDownProperty("数据源", minMapSJY, 0)
        };
    }

    public override void EditorModeInitialized()
    {
        base.EditorModeInitialized();
        sender.DebugMode = true;
        sender.LogError("进入编辑模式" + (Properties[0] as InputFloatProperty).Value);
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        sender.LogError("进入运行模式:" + (Properties[0] as InputFloatProperty).Value);
        _commanderController = gameObject.AddComponent<CommanderController>();
        isMain = isRoomCreator;
    }

    public override void PropertiesChanged(DynamicProperty[] pros)
    {
        base.PropertiesChanged(pros);
        // sender.LogError($"{name}:修改了属性:"+(pros[0] as InputIntProperty).Selected.Enum);
    }

    public void Active(DevType type, bool playback)
    {
        //打开控制相机
        //根据自己的角色等级，告知UI展示谁
        sender.LogError($"{name}:以我为主角运行:" + (Properties[0] as InputFloatProperty).Value);
        MyDataInfo.isHost = isMain;
        MyDataInfo.leadId = BObjectId;
        MyDataInfo.isPlayBack = playback;
        _commanderController.Init();
        StartCoroutine(InitMain());
    }

    IEnumerator InitMain()
    {
        yield return 1;

        cameraObject = new GameObject("Main Camera");
        cameraObject.transform.parent = transform.parent;
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = transform.position;
        cameraObject.AddComponent<Camera>();

        yield return 1;
        int myLevel = (int)(Properties[0] as InputFloatProperty).Value;
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "IconShow", null);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "MinMap", null);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "CommanderView", myLevel);
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "TopMenuView", myLevel);
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
        if (cameraObject != null)
            Destroy(cameraObject.gameObject);
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
                int myLevel = (int)(Properties[0] as InputFloatProperty).Value;
                sender.LogError(myLevel != 1 ? "我需要接收场景装备数据" : "我就是数据编辑者");
                if (myLevel != 1)
                    _commanderController.Receive_ProgrammeData(param);
                break;
            case MessageID.MoveToTarget:
                sender.LogError("收到了移动的指令"+type);
                _commanderController.Receive_MoveEquipToTarget(param);
                break;
            case MessageID.TriggerWaterIntaking:
                sender.LogError("收到了取水的指令"+type);
                _commanderController.Receive_TriggerWaterIntaking(param);
                break;
        }
    }
}