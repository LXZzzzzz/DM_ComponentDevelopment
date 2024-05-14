using DM.IFS;
using ToolsLibrary.PathPart;
using UnityEngine;

public class TestAirEquipMain : ScriptManager
{
    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        gameObject.AddComponent<TestEquip>().Init(BObjectId);
    }
}

public class TestEquip : EquipBase
{
    public void Init(string myid)
    {
        BObjectId = myid;
        Debug.LogError("测试组件装备正常挂载继承Mono的脚本"+myid);
    }
}