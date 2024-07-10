using System.Collections;
using System.Collections.Generic;
using Enums;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using EventType = Enums.EventType;

public partial class HelicopterController
{
    private float amountOfWater;

    public void WaterIntaking(Vector3 pos, float range, float amount, bool isExecuteImmediately)
    {
        Debug.LogError("取水技能参数回传" + isExecuteImmediately);
        if (!isExecuteImmediately)
        {
            //把参数传给主角，将参数传给所有客户端，统一执行
            EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerWaterIntaking, MsgSend_WaterIntaking(BObjectId, pos, amount));
            return;
        }

        //判断自己是否处于取水地的范围内，不在的话调move机动到目的地，然后，执行取水逻辑
        if (Vector3.Distance(transform.position, pos) > range)
        {
            isWaitArrive = true;
            MoveToTarget(pos);
        }
        // else StartCoroutine(quWater());
    }

    public void WaterIntaking_New()
    {
        if (myState != HelicopterState.hover) return;
        //找到场景中所有水源点，判断距离
        var items = sceneAllZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.Waters);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                currentSkill = SkillType.WaterIntaking;
                Debug.LogError(myAttributeInfo.qssj * 60);
                if (myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstLoadingGoodsTime < 1)
                    myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstLoadingGoodsTime = MyDataInfo.gameStartTime;
                openTimer(myAttributeInfo.qssj * 60f, OnQSSuc);
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "当前位置超出取水距离，请前往水源点再进行操作");
    }


    public void WaterPour(Vector3 pos)
    {
        currentSkill = SkillType.WaterPour;
        transform.position = pos;
        float minDis = float.MaxValue;
        ZiYuanBase targetZy = null;
        var items = sceneAllZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.SourceOfAFire);
        Debug.LogError("找到火场：" + items.Count);
        for (int i = 0; i < items.Count; i++)
        {
            Debug.LogError(items[i].ziYuanName);
        }

        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < minDis)
            {
                targetZy = items[i];
                minDis = Vector3.Distance(transform.position, zyPos);
            }
        }

        //找到离我最近的火场，作用于它
        if (targetZy != null && minDis < targetZy.DetectionRange)
        {
            Debug.LogError("找到了符合条件的火场");
            //如果当前没有水了，就不给他水，如果有就直接给喷洒面积
            (targetZy as ISourceOfAFire).waterPour(MyDataInfo.gameStartTime, amountOfWater, amountOfWater);
        }

        currentSkill = SkillType.WaterPour;
        if (myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstOperationTime < 1)
            myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstOperationTime = MyDataInfo.gameStartTime;
        myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].lastOperationTime = MyDataInfo.gameStartTime;
        openTimer(myAttributeInfo.sssj * 60, OnSSSuc);
    }

    private void OnQSSuc()
    {
        currentSkill = SkillType.None;
        amountOfWater = myAttributeInfo.dszl; //Mathf.Min(amountOfWater + myAttributeInfo.dszl, myAttributeInfo.zdzsl);
    }

    private void OnSSSuc()
    {
        myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].totalWeight += amountOfWater;
        currentSkill = SkillType.None;
        amountOfWater = 0;
    }

    #region 数据组装

    private string MsgSend_WaterIntaking(string id, Vector3 pos, float amount)
    {
        return string.Format($"{pos.x}_{pos.y}_{pos.z}_{amount}_{id}");
    }

    private string MsgSend_WaterPour(string id, Vector3 pos)
    {
        return string.Format($"{pos.x}_{pos.y}_{pos.z}_{id}");
    }

    #endregion
}