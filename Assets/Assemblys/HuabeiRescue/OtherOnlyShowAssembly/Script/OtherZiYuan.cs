using System.Collections;
using System.Collections.Generic;
using DM.Entity;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class OtherZiYuan : ZiYuanBase
{
    //需要一个属性，存储我的类型，让地图获取到，显示不同icon
    public void Init(int type)
    {
        //根据不同类型，改变自己的颜色

        Debug.LogError("我的类型是" + type);

        ZiYuanType = (ZiYuanType)type;
    }
}