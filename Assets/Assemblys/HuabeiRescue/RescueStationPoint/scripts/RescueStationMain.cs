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
            new InputFloatUnitProperty("需要物资数量", 5000, "kg"),
            new InputIntUnitProperty("可安置人数",100,"人")
        };
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        RescueStationLogic rsl = gameObject.AddComponent<RescueStationLogic>();
        rsl.Init(BObjectId, (Properties[0] as InputFloatUnitProperty).Value, (Properties[1] as InputIntUnitProperty).Value);

        rsl.ziYuanName = GetComponent<BObjectModel>().BObject.Info.Name;
    }
}
