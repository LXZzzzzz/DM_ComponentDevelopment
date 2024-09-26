using System;
using System.Collections;
using System.Collections.Generic;
using DM.Core.Map;
using DM.Entity;
using DM.IFS;
using UnityEngine;

public class OtherSubAssemblyMain : ScriptManager
{
    private List<EnumDescription> ziyuanType;

    private void Awake()
    {
        ziyuanType = new List<EnumDescription>();
        ziyuanType.Add(new EnumDescription(1, "水源点"));
        ziyuanType.Add(new EnumDescription(6, "补给点"));
        ziyuanType.Add(new EnumDescription(7, "物资点"));
        Properties = new DynamicProperty[]
        {
            new DropDownProperty("资源类型", ziyuanType, 0),
            new InputStringProperty("组件描述", "组件的基本描述。。。"),
            new InputStringProperty("组件icon色值", "#0B2D5F"),
            new InputStringProperty("组件icon选中色值", "#366baf"),
        };
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        //进入运行模式后，传入参数，告诉他是什么类型的资源
        OtherZiYuan oz = gameObject.AddComponent<OtherZiYuan>();
        oz.Init((Properties[0] as DropDownProperty).Selected.Enum, BObjectId, (Properties[2] as InputStringProperty).Value, (Properties[3] as InputStringProperty).Value);

        oz.ziYuanName = GetComponent<BObjectModel>().BObject.Info.Name;
        oz.ziYuanDescribe = (Properties[1] as InputStringProperty).Value;
    }
}