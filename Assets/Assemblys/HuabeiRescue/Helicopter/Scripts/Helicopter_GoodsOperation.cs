﻿using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EffectivenessEvaluation;
using ToolsLibrary.EquipPart;
using UnityEngine;

public partial class HelicopterController
{
    private float amountOfGoods;

    /// <summary>
    /// 装载物资
    /// </summary>
    public void LadeGoods()
    {
        if (myState != HelicopterState.Landing) return;
        //找到场景中所有物资点，判断距离
        var items = sceneAllZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.GoodsPoint);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                currentSkill = SkillType.LadeGoods;
                // float itemgoods = myAttributeInfo.zdyxzh - amountOfGoods - amountOfPerson;
                // Debug.LogError(itemgoods / myAttributeInfo.zzwzsl * 60);
                myRecordedData.eachSortieData.Add(new SingleSortieData());
                if (myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstLoadingGoodsTime < 1)
                    myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstLoadingGoodsTime = MyDataInfo.gameStartTime;
                isGoods = 0;
                PickupPoint = items[i].transform.position;
                openTimer(myAttributeInfo.zzwzsj * 3600f, OnZZWZSuc);
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "当前位置超出装载物资距离，请前往物资点再进行操作");
    }

    private void OnZZWZSuc()
    {
        amountOfGoods = myAttributeInfo.zdyxzh;
    }

    /// <summary>
    /// 卸载物资
    /// </summary>
    public void UnLadeGoods()
    {
        if (myState != HelicopterState.Landing) return;
        //找到场景中所有物资点，判断距离
        var items = sceneAllZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.RescueStation);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                currentSkill = SkillType.UnLadeGoods;
                // Debug.LogError(amountOfGoods / myAttributeInfo.xzwzsl * 60);
                if (myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstOperationTime < 1)
                    myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstOperationTime = MyDataInfo.gameStartTime;
                myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].lastOperationTime = MyDataInfo.gameStartTime;
                if (items[i] is IRescueStation) (items[i] as IRescueStation).goodsPour(amountOfGoods);
                openTimer(myAttributeInfo.xzwzsj * 3600f, OnXZWZSuc);
                myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].goodsDistance = Vector3.Distance(PickupPoint, items[i].transform.position);
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "当前位置超出装载物资距离，请前往物资点再进行操作");
    }

    private void OnXZWZSuc()
    {
        myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].totalWeight += amountOfGoods;
        amountOfGoods = 0;
    }

    /// <summary>
    /// 空投物资
    /// </summary>
    /// <param name="pos"></param>
    public void AirdropGoods(Vector3 pos)
    {
        transform.position = pos;
        float minDis = float.MaxValue;
        ZiYuanBase targetZy = null;
        var items = sceneAllZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.RescueStation);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < minDis)
            {
                targetZy = items[i];
                minDis = Vector3.Distance(transform.position, zyPos);
            }
        }

        //找到离我最近的安置点，作用于它
        if (targetZy != null && minDis < targetZy.DetectionRange)
        {
            (targetZy as IRescueStation).goodsPour(amountOfGoods);
        }

        currentSkill = SkillType.AirdropGoods;
        if (myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstOperationTime < 1)
            myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstOperationTime = MyDataInfo.gameStartTime;
        myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].lastOperationTime = MyDataInfo.gameStartTime;
        myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].goodsDistance = Vector3.Distance(PickupPoint, targetZy.transform.position);
        openTimer(myAttributeInfo.ktwzsj * 3600f, OnXZWZSuc);
    }
}