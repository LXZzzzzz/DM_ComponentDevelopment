using System;
using System.Collections;
using System.Collections.Generic;
using DM.Entity;
using DM.IFS;
using UnityEngine;

public class OtherSubAssemblyMain : ScriptManager
{
    private void Awake()
    {
        Properties = new[]
        {
            new InputIntProperty("资源类型", 0)
        };
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        //进入运行模式后，传入参数，告诉他是什么类型的资源
        gameObject.AddComponent<OtherZiYuan>().Init((Properties[0] as InputIntProperty).Value, BObjectId);
    }
}
