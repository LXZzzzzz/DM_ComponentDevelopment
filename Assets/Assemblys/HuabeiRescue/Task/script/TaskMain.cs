using DM.Entity;
using DM.IFS;
using ToolsLibrary.EquipPart;

public class TaskMain : ScriptManager
{
    private void Awake()
    {
        Properties = new DynamicProperty[]
        {
            new InputStringProperty("任务名", "对某某点进行救火"),
            new InputStringProperty("任务描述", "任务描述描述描述"),
            new DropDownSceneBObjectsProperty("任务关联的资源")
        };
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        ZiYuan_Task task = gameObject.AddComponent<ZiYuan_Task>();
        //进入运行模式后，将一些基础属性通过控制器传给任务点，
        task.Init(BObjectId, (Properties[2] as DropDownSceneBObjectsProperty).Value);
        task.ziYuanName = (Properties[0] as InputStringProperty).Value;
        task.ziYuanDescribe = (Properties[1] as InputStringProperty).Value;
    }
}