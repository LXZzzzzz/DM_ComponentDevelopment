using DM.Entity;
using DM.IFS;

public class TaskMain : ScriptManager
{
    
    private void Awake()
    {
        Properties = new DynamicProperty[]
        {
            new InputStringProperty("任务名", "对某某点进行救火")
        };
    }
    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        gameObject.AddComponent<ZiYuan_Task>();
        //进入运行模式后，将一些基础属性通过控制器传给任务点，
    }
}
