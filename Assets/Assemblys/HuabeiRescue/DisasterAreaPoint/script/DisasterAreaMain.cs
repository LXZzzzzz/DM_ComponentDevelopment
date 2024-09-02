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
        woundedPersonnelType.Add(new EnumDescription(1, "轻伤员"));
        woundedPersonnelType.Add(new EnumDescription(2, "重伤员"));
        Properties = new DynamicProperty[]
        {
            new InputIntUnitProperty("需要救助人数", 5000, "人"),
            new InputStringProperty("组件描述", "灾区点，需要在这救助人员"),
            new DropDownProperty("伤员类型", woundedPersonnelType, 0),
            new InputStringProperty("资源Icon色值", "#800049"),
            new InputStringProperty("资源Icon选中色值", "#cb488f"),
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
    }
}