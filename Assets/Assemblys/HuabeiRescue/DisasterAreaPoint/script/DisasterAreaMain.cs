using System.Collections.Generic;
using DM.Core.Map;
using DM.Entity;
using DM.IFS;
using UnityEngine;

public class DisasterAreaMain : ScriptManager
{
    private void Awake()
    {
        var woundedPersonnelType = new List<EnumDescription>();
        woundedPersonnelType.Add(new EnumDescription(1, "受灾群众"));
        woundedPersonnelType.Add(new EnumDescription(2, "伤员"));
        Properties = new DynamicProperty[]
        {
            new InputIntUnitProperty("需要救助人数", 5000, "人"),
            new InputStringProperty("组件描述", "灾区点，需要在这救助人员"),
            new DropDownProperty("人员类型", woundedPersonnelType, 0),
            new InputStringProperty("资源Icon色值", "#800049"),
            new InputStringProperty("资源Icon选中色值", "#cb488f"),
            new InputStringProperty("无用选项","这里是为了更新组件，无逻辑用处")
        };
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        DisasterAreaLogic rsl = gameObject.AddComponent<DisasterAreaLogic>();
        rsl.Init(BObjectId, (Properties[0] as InputIntUnitProperty).Value, (Properties[2] as DropDownProperty).Selected.Enum,
            (Properties[3] as InputStringProperty).Value, (Properties[4] as InputStringProperty).Value);

        rsl.ziYuanName = GetComponent<BObjectModel>().BObject.Info.Name;
        rsl.ziYuanDescribe = (Properties[1] as InputStringProperty).Value;
        rsl.ZiyuanIcon = info.PicBObjects[BObjectId];
    }
}