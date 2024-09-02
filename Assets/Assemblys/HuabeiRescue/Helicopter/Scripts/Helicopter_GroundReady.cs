using ToolsLibrary;
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
        openTimer(myAttributeInfo.qfqzbsj * 60, OnGRSuc, 4, OnGRStageComplete);
        //救援前准备、设备检查、登机、起飞前检查
    }

    private void OnGRStageComplete(int aa)
    {
        string stageInfo = "";
        switch (aa)
        {
            case 0:
                stageInfo = "起飞前准备";
                break;
            case 1:
                stageInfo = "设备检查";
                break;
            case 2:
                stageInfo = "登记";
                break;
            case 3:
                stageInfo = "起飞前检查";
                break;
            default: return;
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.TriggerOnlyShow, $"{BObjectId}_{stageInfo}");
    }

    public void BePutInStorage()
    {
        if (myState != HelicopterState.Landing) return;
        var items = sceneAllZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.Airport);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                myState = HelicopterState.NotReady;
                (items[i] as IAirPort).comeIn(BObjectId);
                // if (myRecordedData.endTaskTime < 1)
                myRecordedData.endTaskTime = MyDataInfo.gameStartTime;
                Debug.LogError($"结束时刻：{myRecordedData.endTaskTime}");
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