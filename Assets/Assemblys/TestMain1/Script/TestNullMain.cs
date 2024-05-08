using System;
using System.Collections.Generic;
using DM.Entity;
using DM.IFS;
using UnityEngine;

public class TestNullMain : ScriptManager,IControl, IContainer, IMesRec
{
    private void Awake()
    {
        Debug.LogError("为什么啊");
        Properties = new DynamicProperty[]
        {
            new ToggleGroupProperty("toggleGroup字段",new List<ToggleProperty>()
                {new ToggleProperty("aa",true),new ToggleProperty("bb",false),new ToggleProperty("cc",false)},true,0),
            new InputIntUnitProperty("带单位的int输入", 10, "元"),
            new InputFloatUnitProperty("带单位的float输入", 10.5f, "斤"),
            new InputIntUnitLimitProperty("带单位的，有取值范围的输入", 10, "个", 0, 80),
            new InputFloatUnitLimitProperty("带单位的，有取值范围的输入", 10.5f, "斤", 0, 80.5f),
            new DropDownSceneSelectProperty("阵营，分组，组件列表的级联"),
            new DropDownSceneBObjectsProperty("场景中全部组件"),
            new OpenFileDialogProperty("接收文件路径", ""),
            new PanelProperty("Panel字段", new DynamicProperty[] { new InputIntUnitProperty("带单位的int输入", 10, "元") }),
            new FoldoutProperty("foldOut字段", new DynamicProperty[] { new InputFloatUnitProperty("带单位的float输入", 10.5f, "斤") })
        };
        // sender.LogError("测试初始化结果"+Properties[1].Name);
    }

    public void Active(DevType type, bool playback)
    {
        
    }

    public void DeActive(DevType type, bool playback)
    {
        
    }

    public void ExternalView()
    {
    
    }

    public void InternalView()
    {
        
    }

    public void RecMessage(SendType type, GameObject senderObj, int eventType, string param)
    {
        
    }
}
