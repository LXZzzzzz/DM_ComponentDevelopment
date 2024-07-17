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
        EventManager.Instance.AddEventListener<string>(Enums.EventType.DestoryEquip.ToString(), desAir);
    }

    public List<string> allDockingAircraft;

    public void comeIn(string equipId)
    {
        allDockingAircraft.Add(equipId);
        MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId)).isDockingAtTheAirport = true;
        for (int i = 0; i < allDockingAircraft.Count; i++)
        {
            GameObject itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, allDockingAircraft[i])).gameObject;
            if (i % 4 == 0)
            {
                itemEquip.transform.position = new Vector3(transform.position.x + (i / 4 + 1) * 10, itemEquip.transform.position.y, transform.position.z);
            }
            else if (i % 4 == 1)
            {
                itemEquip.transform.position = new Vector3(transform.position.x - (i / 4 + 1) * 10, itemEquip.transform.position.y, transform.position.z);
            }
            else if (i % 4 == 2)
            {
                itemEquip.transform.position = new Vector3(transform.position.x, itemEquip.transform.position.y, transform.position.z - (i / 4 + 1) * 15);
            }
            else if (i % 4 == 3)
            {
                itemEquip.transform.position = new Vector3(transform.position.x, itemEquip.transform.position.y, transform.position.z + (i / 4 + 1) * 15);
            }
        }
    }

    public List<string> GetAllEquips()
    {
        return allDockingAircraft;
    }

    public void goOut(string equipId)
    {
        //起飞指定飞机，并把他从机场移除出去
        allDockingAircraft.Remove(equipId);
        var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId));
        itemEquip.isDockingAtTheAirport = false;
        itemEquip.transform.position = new Vector3(transform.position.x, itemEquip.transform.position.y, transform.position.z);
    }

    private void desAir(string idid)
    {
        if (allDockingAircraft.Contains(idid))
            allDockingAircraft.Remove(idid);
    }

    public override void OnStart()
    {
    }

    protected override void OnReset()
    {
        allDockingAircraft.Clear();
        EventManager.Instance.AddEventListener<string>(Enums.EventType.DestoryEquip.ToString(), desAir);
    }
}