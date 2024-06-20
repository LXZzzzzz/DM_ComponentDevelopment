using System.Collections;
using System.Collections.Generic;
using DM.Entity;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class OtherZiYuan : ZiYuanBase,IAirPort
{
    //需要一个属性，存储我的类型，让地图获取到，显示不同icon
    public void Init(int type)
    {
        //根据不同类型，改变自己的颜色
        allDockingAircraft = new List<string>();
        ZiYuanType = (ZiYuanType)type;
    }

    public List<string> allDockingAircraft;
    public void AddEquip(string equipId)
    {
        allDockingAircraft.Add(equipId);
    }

    public List<string> GetAllEquips()
    {
        return allDockingAircraft;
    }
}