using System.Collections;
using System.Collections.Generic;
using DM.IFS;
using UnityEngine;

public class WaterPointMain : ScriptManager
{
    //可能需要存一个水量

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        gameObject.AddComponent<ZiYuan_WaterPoinnt>();
        //进入运行模式后，将一些基础属性通过控制器传给飞机，
    }
}
