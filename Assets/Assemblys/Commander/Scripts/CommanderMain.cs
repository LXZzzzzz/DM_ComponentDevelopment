using System;
using System.Collections.Generic;
using DM.Entity;
using DM.IFS;
using UnityEngine;

public class CommanderMain : ScriptManager, IControl
{
    private List<EnumDescription> commanderLevel;

    private void Awake()
    {
        commanderLevel = new List<EnumDescription>();
        commanderLevel.Add(new EnumDescription(1, "一级指挥官"));
        commanderLevel.Add(new EnumDescription(2, "二级指挥官"));
        List<EnumDescription> minMapSJY = new List<EnumDescription>();
        minMapSJY.Add(new EnumDescription(0,"中立"));
        minMapSJY.Add(new EnumDescription(1,"红方"));
        minMapSJY.Add(new EnumDescription(2,"蓝方"));
        Properties = new DynamicProperty[]
        {
            new DropDownProperty("指挥官等级", commanderLevel, 0),
            new InputStringProperty("任务名",""),
            new DropDownSceneSelectProperty("关联环境组件"),
            new OpenFileDialogProperty("评估报告路径",""),
            new ToggleProperty("启用经济系统",false),
            new LabelProperty("-------小地图属性"),
            new DropDownProperty("数据源",minMapSJY,0)
        };
    }

    public override void EditorModeInitialized()
    {
        base.EditorModeInitialized();
        sender.DebugMode = true;
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
    }

    public void Active(DevType type, bool playback)
    {
        //打开控制相机
        //根据自己的角色等级，告知UI展示谁
    }

    public void DeActive(DevType type, bool playback)
    {
    }
}