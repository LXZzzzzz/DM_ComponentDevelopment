using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class OtherZiYuan : ZiYuanBase, IAirPort
{
    //需要一个属性，存储我的类型，让地图获取到，显示不同icon
    public void Init(int type, string id)
    {
        base.Init(id, 50);
        //根据不同类型，改变自己的颜色
        allDockingAircraft = new List<string>();
        ZiYuanType = (ZiYuanType)type;
    }

    public List<string> allDockingAircraft;

    public void comeIn(string equipId)
    {
        allDockingAircraft.Add(equipId);
        MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId)).isDockingAtTheAirport = true;
    }

    public List<string> GetAllEquips()
    {
        return allDockingAircraft;
    }
    public void goOut(string equipId)
    {
        //起飞指定飞机，并把他从机场移除出去
        allDockingAircraft.Remove(equipId);
        MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId)).isDockingAtTheAirport = false;
    }

    protected override void OnReset()
    {
        allDockingAircraft.Clear();
    }
}