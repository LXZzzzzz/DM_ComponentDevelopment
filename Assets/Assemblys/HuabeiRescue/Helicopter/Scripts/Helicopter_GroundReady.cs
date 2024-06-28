using System.Collections.Generic;
using ToolsLibrary.EquipPart;
using UnityEngine;

public partial class HelicopterController
{
    private IAirPort _airPort;

    public void GroundReady(IAirPort airPort)
    {
        if (myState != HelicopterState.NotReady) return;
        Debug.LogError("起飞前准备" + myAttributeInfo.qfqzbsj * 60);
        _airPort = airPort;
        currentSkill = SkillType.GroundReady;
        openTimer(myAttributeInfo.qfqzbsj * 60, OnGRSuc);
    }

    public void BePutInStorage(List<ZiYuanBase> allZiyuan)
    {
        if (myState != HelicopterState.Landing) return;
        var items = allZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.Airport);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                myState = HelicopterState.NotReady;
                (items[i] as IAirPort).comeIn(BObjectId);
                return;
            }
        }
    }

    private void OnGRSuc()
    {
        Debug.LogError("起飞准备完成");
        currentSkill = SkillType.None;
        myState = HelicopterState.Landing;
        _airPort.goOut(BObjectId);
        currentTargetType = (int)ZiYuanType.Airport;
    }
}