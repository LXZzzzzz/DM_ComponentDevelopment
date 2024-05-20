using System.Collections;
using System.Collections.Generic;
using DM.IFS;
using UnityEngine;

public class HelicopterMain : ScriptManager
{
    public override void EditorModeInitialized()
    {
        base.EditorModeInitialized();
        //编辑模式下对该装备基本属性进行设置
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        gameObject.transform.GetChild(0).gameObject.AddComponent<HelicopterController>();
        //进入运行模式后，将一些基础属性通过控制器传给飞机，
    }
}
