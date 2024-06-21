using System.Collections;
using System.Collections.Generic;
using DM.Entity;
using DM.IFS;
using UnityEngine;

public class HelicopterMain : ScriptManager
{
    private List<EnumDescription> instructionSetModel;
    public override void EditorModeInitialized()
    {
        base.EditorModeInitialized();
        //编辑模式下对该装备基本属性进行设置
    }
    private void Awake()
    {
        instructionSetModel = new List<EnumDescription>();
        instructionSetModel.Add(new EnumDescription(1, "灭火任务指令"));
        instructionSetModel.Add(new EnumDescription(2, "救援任务指令"));
        Properties = new DynamicProperty[]
        {
            new DropDownProperty("设置飞机指令集", instructionSetModel, 0)
        };
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        var logic = gameObject.transform.GetChild(0).gameObject.AddComponent<HelicopterController>();
        //进入运行模式后，将一些基础属性通过控制器传给飞机，
        logic.EquipIcon = info.PicBObjects[BObjectId];
        logic.instructionSetModel = (Properties[0] as DropDownProperty).Selected.Enum;
    }
}
