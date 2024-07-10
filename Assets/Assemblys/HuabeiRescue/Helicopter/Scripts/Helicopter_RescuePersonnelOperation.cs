using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public partial class HelicopterController
{
    private int amountOfPerson;

    private int itemPersonNum;

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
                itemPersonNum = (items[i] as IDisasterArea).rescuePerson(myAttributeInfo.zdzkl);
                Debug.LogError(myAttributeInfo.ldzzrysj * 3600f);
                openTimer(myAttributeInfo.ldzzrysj * 3600f, OnZZRYSuc);

                if (myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstRescuePersonTime < 1)
                    myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstRescuePersonTime = MyDataInfo.gameStartTime;
                myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].lastRescuePersonTime = MyDataInfo.gameStartTime;
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "当前位置超出装载人员距离，请前往灾区点再进行操作");
    }

    private void OnZZRYSuc()
    {
        amountOfPerson = itemPersonNum;
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
                (items[i] as IRescueStation).placementOfPersonnel(amountOfPerson);
                // Debug.LogError(amountOfPerson / myAttributeInfo.azsysl * 60);
                openTimer(myAttributeInfo.azsysj * 3600f, OnAZSYSuc);
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "当前位置超出安置人员距离，请前往救助站再进行操作");
    }

    private void OnAZSYSuc()
    {
        amountOfPerson = 0;
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
                itemPersonNum = (items[i] as IDisasterArea).rescuePerson(myAttributeInfo.zdzkl);
                // Debug.LogError(itemPersonNum / myAttributeInfo.sjjrsl * 60);
                openTimer(myAttributeInfo.sjjrsj * 3600f, OnZZRYSuc);

                if (myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstRescuePersonTime < 1)
                    myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].firstRescuePersonTime = MyDataInfo.gameStartTime;
                myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].lastRescuePersonTime = MyDataInfo.gameStartTime;
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "当前位置超出装载人员距离，请前往灾区点再进行操作");
    }
}