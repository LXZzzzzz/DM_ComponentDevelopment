using ToolsLibrary;
using ToolsLibrary.FrameSync;

public class SyncLogicController
{
    private FrameSyncLogicBase currentSyncLogic;

    public void Init(bool isMain)
    {
        if (isMain)
            currentSyncLogic = new ServerLogic();
        else
            currentSyncLogic = new ClientLogic();

        MyDataInfo.netLogic = currentSyncLogic;
    }

    public void Update()
    {
        currentSyncLogic?.OnUpdate();
    }
}