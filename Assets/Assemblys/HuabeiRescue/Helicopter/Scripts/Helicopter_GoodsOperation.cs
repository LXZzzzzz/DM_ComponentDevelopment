using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public partial class HelicopterController
{
    private float amountOfGoods;

    public void LadeGoods(List<ZiYuanBase> allZiyuan)
    {
        if (myState != HelicopterState.Landing) return;
        //找到场景中所有物资点，判断距离
        var items = allZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.GoodsPoint);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                currentSkill = SkillType.LadeGoods;
                float itemgoods = myAttributeInfo.zdyxzh - amountOfGoods - amountOfPerson;
                Debug.LogError(itemgoods / myAttributeInfo.zzwzsl * 60);
                if (myRecordedData.eachSortieData[^1].firstLoadingGoodsTime < 1)
                    myRecordedData.eachSortieData[^1].firstLoadingGoodsTime = MyDataInfo.gameStartTime;
                openTimer(itemgoods / myAttributeInfo.zzwzsl * 60f, OnZZWZSuc);
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "当前位置超出装载物资距离，请前往物资点再进行操作");
    }

    private void OnZZWZSuc()
    {
        amountOfGoods = myAttributeInfo.zdyxzh - amountOfGoods - amountOfPerson;
    }

    public void UnLadeGoods(List<ZiYuanBase> allZiyuan)
    {
        if (myState != HelicopterState.Landing) return;
        //找到场景中所有物资点，判断距离
        var items = allZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.GoodsPoint);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                currentSkill = SkillType.UnLadeGoods;
                Debug.LogError(amountOfGoods / myAttributeInfo.xzwzsl * 60);
                if (myRecordedData.eachSortieData[^1].firstOperationTime < 1)
                    myRecordedData.eachSortieData[^1].firstOperationTime = MyDataInfo.gameStartTime;
                myRecordedData.eachSortieData[^1].lastOperationTime = MyDataInfo.gameStartTime;
                openTimer(amountOfGoods / myAttributeInfo.xzwzsl * 60f, OnXZWZSuc);
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "当前位置超出装载物资距离，请前往物资点再进行操作");
    }

    private void OnXZWZSuc()
    {
        myRecordedData.eachSortieData[^1].totalWeight += amountOfGoods;
        amountOfGoods = 0;
    }

    public void AirdropGoods(Vector3 pos, List<ZiYuanBase> allZiyuan)
    {
        transform.position = pos;
        float minDis = float.MaxValue;
        ZiYuanBase targetZy = null;
        var items = allZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.DisasterArea);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < minDis)
            {
                targetZy = items[i];
                minDis = Vector3.Distance(transform.position, zyPos);
            }
        }

        //找到离我最近的灾区，作用于它
        if (targetZy != null && minDis < targetZy.DetectionRange)
        {
            (targetZy as IDisasterArea).airdropGoods(MyDataInfo.gameStartTime, amountOfGoods);
        }

        currentSkill = SkillType.AirdropGoods;
        if (myRecordedData.eachSortieData[^1].firstOperationTime < 1)
            myRecordedData.eachSortieData[^1].firstOperationTime = MyDataInfo.gameStartTime;
        myRecordedData.eachSortieData[^1].lastOperationTime = MyDataInfo.gameStartTime;
        openTimer(amountOfGoods / myAttributeInfo.ktwzsl * 60, OnXZWZSuc);
    }
}