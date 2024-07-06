using DM.Core.Map;
using DM.Entity;
using DM.IFS;
using UnityEngine;

public class RescueStationMain : ScriptManager
{
    private void Awake()
    {
        Properties = new DynamicProperty[]
        {
            new InputFloatUnitProperty("人均救援物资需求", 17.7f, "kg"),
            new InputIntUnitProperty("可安置人数",100,"人"),
            new InputStringProperty("组件描述","安置点，需要给这里安排伤员和物资")
        };
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        RescueStationLogic rsl = gameObject.AddComponent<RescueStationLogic>();
        rsl.Init(BObjectId, (Properties[0] as InputFloatUnitProperty).Value, (Properties[1] as InputIntUnitProperty).Value);

        rsl.ziYuanName = GetComponent<BObjectModel>().BObject.Info.Name;
        rsl.ziYuanDescribe = (Properties[2] as InputStringProperty).Value;
    }
}
