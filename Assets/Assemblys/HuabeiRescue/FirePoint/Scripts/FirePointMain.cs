using DM.Core.Map;
using DM.Entity;
using DM.IFS;
using UnityEngine;

public class FirePointMain : ScriptManager
{
    private void Awake()
    {
        Properties = new DynamicProperty[]
        {
            new InputFloatUnitProperty("风速", 5, "m/s"),
            new InputFloatUnitProperty("坡度", 5, "度"),
            new InputFloatUnitProperty("初始燃烧面积", 100, "平方米"),
            new InputStringProperty("组件描述","着火点，需要扑灭这里的火")
        };
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        FirePointLogic fpl = gameObject.AddComponent<FirePointLogic>();
        fpl.Init((Properties[0] as InputFloatUnitProperty).Value, (Properties[1] as InputFloatUnitProperty).Value,
            (Properties[2] as InputFloatUnitProperty).Value, BObjectId);

        fpl.ziYuanName = GetComponent<BObjectModel>().BObject.Info.Name;
        fpl.ziYuanDescribe = (Properties[3] as InputStringProperty).Value;
    }
}