using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class OtherZiYuan : ZiYuanBase
{
    //需要一个属性，存储我的类型，让地图获取到，显示不同icon
    public void Init(int type, string id, string colorCode)
    {
        base.Init(id, 50, colorCode);
        //根据不同类型，改变自己的颜色
        ZiYuanType = (ZiYuanType)type;
    }

    public override void OnStart()
    {
    }

    protected override void OnReset()
    {
    }
}