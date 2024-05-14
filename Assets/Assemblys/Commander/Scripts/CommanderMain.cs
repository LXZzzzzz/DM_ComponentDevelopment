using System;
using System.Collections;
using System.Collections.Generic;
using DM.Entity;
using DM.IFS;
using ToolsLibrary;
using ToolsLibrary.PathPart;
using UnityEngine;
using EventType = ToolsLibrary.EventType;

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
        sender.LogError("进入编辑模式");
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        sender.LogError("进入运行模式");
        _commanderController = gameObject.AddComponent<CommanderController>();
        isMain = isRoomCreator;
    }

    public void Active(DevType type, bool playback)
    {
        //打开控制相机
        //根据自己的角色等级，告知UI展示谁
        sender.LogError($"{name}:以我为主角运行");
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
        EventManager.Instance.EventTrigger<string,object>(EventType.ShowUI, "IconShow",null);
        if (MyDataInfo.isPlayBack)
            EventManager.Instance.EventTrigger<string,object>(EventType.ShowUI, "CursorShow",null);

        MyDataInfo.sceneAllEquips = new List<EquipBase>();
        sender.LogError("找到了装备组件多少个"+root.GetComponentsInChildren<EquipBase>().Length);
        MyDataInfo.sceneAllEquips.AddRange(root.GetComponentsInChildren<EquipBase>());
    }

    public void DeActive(DevType type, bool playback)
    {
    }

    private void OnDestroy()
    {
        _commanderController.Terminate();
        _commanderController = null;
    }

    private GameObject cameraObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            EventManager.Instance.EventTrigger<string,object>(EventType.ShowUI, "MinMap",MyDataInfo.sceneAllEquips);
        }
    }

    public void RecMessage(SendType type, GameObject senderObj, int eventType, string param)
    {
        if (type == SendType.SubToMain)
            sender.RunSend(SendType.MainToAll, senderObj.GetComponent<CommanderMain>().BObjectId, eventType, param);

        switch ((MessageID)eventType)
        {
            case MessageID.MoveToTarget:
                _commanderController.MoveEquipToTarget(param);
                break;
        }
    }


    public enum MessageID
    {
        MoveToTarget
    }
}