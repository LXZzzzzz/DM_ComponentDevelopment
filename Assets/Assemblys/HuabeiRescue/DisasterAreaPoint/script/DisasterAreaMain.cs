using DM.Core.Map;
using DM.Entity;
using DM.IFS;
using UnityEngine;

public class DisasterAreaMain : ScriptManager
{
    private void Awake()
    {
        Properties = new DynamicProperty[]
        {
            new InputIntUnitProperty("需要救助人数", 5000, "人"),
            new InputStringProperty("组件描述","灾区点，需要在这救助人员")
        };
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        DisasterAreaLogic rsl = gameObject.AddComponent<DisasterAreaLogic>();
        rsl.Init(BObjectId, (Properties[0] as InputIntUnitProperty).Value);

        rsl.ziYuanName = GetComponent<BObjectModel>().BObject.Info.Name;
        rsl.ziYuanDescribe = (Properties[1] as InputStringProperty).Value;
    }
}
