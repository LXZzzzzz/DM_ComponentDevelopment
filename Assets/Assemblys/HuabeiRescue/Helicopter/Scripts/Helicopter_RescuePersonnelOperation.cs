using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EffectivenessEvaluation;
using ToolsLibrary.EquipPart;
using UnityEngine;

public partial class HelicopterController
{
    private int amountOfPerson;

    private int itemPersonNum;

    private int personType;

    /// <summary>
    /// 装载人员
    /// </summary>
    public void Manned()
    {
        if (myState != HelicopterState.Landing) return;
        //找到场景中所有灾区点，判断距离
        var items = sceneAllZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.DisasterArea);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                currentSkill = SkillType.Manned;
                // float itemgoods = myAttributeInfo.zdyxzh - amountOfPerson * myAttributeInfo.cnrpjtz - amountOfGoods;
                // int itemperson = Mathf.Min(myAttributeInfo.zdzkl, (int)Mathf.Floor(itemgoods / myAttributeInfo.cnrpjtz));
                itemPersonNum = (items[i] as IDisasterArea).rescuePerson(myAttributeInfo.zdzkl - amountOfPerson);
                Debug.LogError(myAttributeInfo.ldzzrysj * 3600f);
                openTimer(myAttributeInfo.ldzzrysj * 3600f, OnZZRYSuc);

                myRecordedData.eachSortieData.Add(new SingleSortieData());
                if (myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstRescuePersonTime < 1)
                    myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstRescuePersonTime = MyDataInfo.gameStartTime;
                myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].lastRescuePersonTime = MyDataInfo.gameStartTime;
                isGoods = 1;
                PickupPoint = items[i].transform.position;
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "当前位置超出装载人员距离，请前往灾区点再进行操作");
    }

    private void OnZZRYSuc()
    {
        amountOfPerson += itemPersonNum;
        myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].numberOfRescues += amountOfPerson;
    }

    /// <summary>
    /// 安置人员
    /// </summary>
    public void PlacementOfPersonnel()
    {
        if (myState != HelicopterState.Landing) return;
        //找到场景中所有安置点，判断距离
        var items = sceneAllZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.RescueStation);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                currentSkill = SkillType.PlacementOfPersonnel;
                itemPersonNum = (items[i] as IRescueStation).placementOfPersonnel(amountOfPerson);
                // Debug.LogError(amountOfPerson / myAttributeInfo.azsysl * 60);
                myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].personDistance = Vector3.Distance(PickupPoint, items[i].transform.position);
                myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].placementOfPersonTime = MyDataInfo.gameStartTime;
                openTimer(myAttributeInfo.azsysj * 3600f, OnAZSYSuc);
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "当前位置超出安置人员距离，请前往救助站再进行操作");
    }

    private void OnAZSYSuc()
    {
        amountOfPerson -= itemPersonNum;
    }

    /// <summary>
    /// 索降救援
    /// </summary>
    public void CableDescentRescue()
    {
        if (myState != HelicopterState.hover) return;
        //找到场景中所有灾区点，判断距离
        var items = sceneAllZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.DisasterArea);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                currentSkill = SkillType.CableDescentRescue;
                // float itemgoods = myAttributeInfo.zdyxzh - amountOfPerson - amountOfGoods;
                // int itemperson = Mathf.Min(myAttributeInfo.zdzkl, (int)Mathf.Floor(itemgoods / myAttributeInfo.cnrpjtz));
                itemPersonNum = (items[i] as IDisasterArea).rescuePerson(myAttributeInfo.zdzkl - amountOfPerson);
                // Debug.LogError(itemPersonNum / myAttributeInfo.sjjrsl * 60);
                //索降救援、伤情评估与地面人员配合救援
                openTimer(myAttributeInfo.sjjrsj * 3600f, OnZZRYSuc, 3, OnCDRStageComplete);

                myRecordedData.eachSortieData.Add(new SingleSortieData());
                if (myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstRescuePersonTime < 1)
                    myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstRescuePersonTime = MyDataInfo.gameStartTime;
                myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].lastRescuePersonTime = MyDataInfo.gameStartTime;
                isGoods = 1;
                PickupPoint = items[i].transform.position;
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "当前位置超出装载人员距离，请前往灾区点再进行操作");
    }

    private void OnCDRStageComplete(int aa)
    {
        string stageInfo = "";
        switch (aa)
        {
            case 0:
                stageInfo = "索降救援";
                break;
            case 1:
                stageInfo = "伤情评估";
                break;
            case 2:
                stageInfo = "地面人员配合救援";
                break;
            default: return;
        }

        EventManager.Instance.EventTrigger(Enums.EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.TriggerOnlyShow, $"{BObjectId}_{stageInfo}");
    }
}