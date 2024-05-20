using DM.IFS;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class TestAirEquipMain : ScriptManager
{
    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        gameObject.AddComponent<TestEquip>().Init(BObjectId);
    }
}