using System;
using System.Collections;
using System.Collections.Generic;
using DM.Core.Map;
using DM.Entity;
using DM.IFS;
using UnityEngine;

public class OtherSubAssemblyMain : ScriptManager
{
    private void Awake()
    {
        Properties = new DynamicProperty[]
        {
            new InputIntProperty("资源类型", 0),
            new InputStringProperty("组件描述","组件的基本描述。。。")
        };
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        //进入运行模式后，传入参数，告诉他是什么类型的资源
        OtherZiYuan oz = gameObject.AddComponent<OtherZiYuan>();
        oz.Init((Properties[0] as InputIntProperty).Value, BObjectId);
        oz.ziYuanName = GetComponent<BObjectModel>().BObject.Info.Name;
    }
}