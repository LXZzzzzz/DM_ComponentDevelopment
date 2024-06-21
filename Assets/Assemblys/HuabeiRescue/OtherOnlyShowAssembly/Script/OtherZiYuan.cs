using System.Collections;
using System.Collections.Generic;
using DM.Entity;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class OtherZiYuan : ZiYuanBase, IAirPort
{
    //需要一个属性，存储我的类型，让地图获取到，显示不同icon
    public void Init(int type, string id)
    {
        //根据不同类型，改变自己的颜色
        allDockingAircraft = new List<string>();
        ZiYuanType = (ZiYuanType)type;
        BobjectId = id;
    }

    public List<string> allDockingAircraft;

    public void AddEquip(string equipId)
    {
        allDockingAircraft.Add(equipId);
        MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId)).isDockingAtTheAirport = true;
    }

    public List<string> GetAllEquips()
    {
        return allDockingAircraft;
    }

    public void OnTakeOff(string equipId, bool isExecuteImmediately)
    {
        if (!isExecuteImmediately)
        {
            //要把指令分发出去，再执行，保证同步
            EventManager.Instance.EventTrigger(Enums.EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.TriggerTakeOff, equipId);
            return;
        }

        //起飞指定飞机，并把他从机场移除出去
        allDockingAircraft.Remove(equipId);
        MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId)).isDockingAtTheAirport = false;
    }
}