using DM.Core.Map;
using DM.Entity;
using DM.IFS;
using UnityEngine;

public class AirPortPointMain : ScriptManager
{
    private void Awake()
    {
        Properties = new DynamicProperty[]
        {
            new InputIntUnitProperty("水平间距", 10, "m"),
            new InputIntUnitProperty("竖直间距", 10, "m"),
            new InputIntUnitProperty("横向停放数量", 5, "架"),
            new InputIntUnitProperty("纵向停放数量", 5, "架"),
            new InputStringProperty("组件描述", "机场，飞机停靠和结束任务依据"),
            new InputStringProperty("组件Icon色值","#0B2D5F")
        };
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        AirPortPointLogic apl = gameObject.AddComponent<AirPortPointLogic>();
        apl.Init((Properties[0] as InputIntUnitProperty).Value, (Properties[1] as InputIntUnitProperty).Value,
            (Properties[2] as InputIntUnitProperty).Value, (Properties[3] as InputIntUnitProperty).Value, BObjectId,(Properties[5] as InputStringProperty).Value);
        apl.ziYuanName = GetComponent<BObjectModel>().BObject.Info.Name;
        apl.ziYuanDescribe = (Properties[4] as InputStringProperty).Value;
    }
}