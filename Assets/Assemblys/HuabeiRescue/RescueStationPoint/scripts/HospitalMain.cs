using DM.Core.Map;
using DM.Entity;
using DM.IFS;

public class HospitalMain : ScriptManager
{
    private void Awake()
    {
        Properties = new DynamicProperty[]
        {
            new InputFloatUnitProperty("人均救援物资需求", 17.7f, "kg"),
            new InputIntUnitProperty("可救治人数", 100, "人"),
            new InputStringProperty("组件描述", "医院，需要给这里安排重伤员和物资"),
            new InputStringProperty("组件icon色值", "#0B2D5F"),
            new InputStringProperty("组件icon选中色值", "#366baf"),
        };
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        RescueStationLogic rsl = gameObject.AddComponent<RescueStationLogic>();
        rsl.Init(BObjectId, 3, (Properties[0] as InputFloatUnitProperty).Value, (Properties[1] as InputIntUnitProperty).Value
            , (Properties[3] as InputStringProperty).Value, (Properties[4] as InputStringProperty).Value);

        rsl.ziYuanName = GetComponent<BObjectModel>().BObject.Info.Name;
        rsl.ziYuanDescribe = (Properties[2] as InputStringProperty).Value;
        rsl.ZiyuanIcon = info.PicBObjects[BObjectId];
    }
}