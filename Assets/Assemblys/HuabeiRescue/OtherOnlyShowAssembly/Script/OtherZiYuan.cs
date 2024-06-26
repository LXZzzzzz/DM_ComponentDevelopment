using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class OtherZiYuan : ZiYuanBase, IAirPort,ISourceOfAFire,IDisasterArea
{
    //需要一个属性，存储我的类型，让地图获取到，显示不同icon
    public void Init(int type, string id)
    {
        //根据不同类型，改变自己的颜色
        allDockingAircraft = new List<string>();
        ZiYuanType = (ZiYuanType)type;
        BobjectId = id;
        detectionRange = 50;
    }

    public List<string> allDockingAircraft;

    public void comeIn(string equipId)
    {
        allDockingAircraft.Add(equipId);
        MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId)).isDockingAtTheAirport = true;
    }

    public List<string> GetAllEquips()
    {
        Debug.LogError("机场有多少飞机：" + allDockingAircraft.Count);
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

    public void waterPour(float time, float squareMeasure)
    {
        Debug.LogError($"火源点在{time}时刻受到{squareMeasure}kg的水");
    }

    public void airdropGoods(float time, float squareMeasure)
    {
        Debug.LogError($"灾区点在{time}时刻受到{squareMeasure}kg的物资");
    }
}