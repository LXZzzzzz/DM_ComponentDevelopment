using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public partial class HelicopterController
{
    private int amountOfPerson;

    public void Manned(List<ZiYuanBase> allZiyuan)
    {
        if (myState != HelicopterState.Landing) return;
        //找到场景中所有灾区点，判断距离
        var items = allZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.DisasterArea);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                currentSkill = SkillType.Manned;
                float itemgoods = myAttributeInfo.zdyxzh - amountOfPerson - amountOfGoods;
                int itemperson = Mathf.Min(myAttributeInfo.zdzkl, (int)Mathf.Floor(itemgoods / myAttributeInfo.cnrpjtz));
                Debug.LogError(itemperson / myAttributeInfo.ldzzrysl * 60);
                openTimer(itemperson / myAttributeInfo.ldzzrysl * 60f, OnZZRYSuc);
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "当前位置超出装载人员距离，请前往灾区点再进行操作");
    }

    private void OnZZRYSuc()
    {
        float itemgoods = myAttributeInfo.zdyxzh - amountOfPerson - amountOfGoods;
        amountOfPerson = Mathf.Min(myAttributeInfo.zdzkl, (int)Mathf.Floor(itemgoods / myAttributeInfo.cnrpjtz));
    }

    public void PlacementOfPersonnel(List<ZiYuanBase> allZiyuan)
    {
        if (myState != HelicopterState.Landing) return;
        //找到场景中所有灾区点，判断距离
        var items = allZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.RescueStation || x.ZiYuanType == ZiYuanType.Hospital);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                currentSkill = SkillType.PlacementOfPersonnel;
                Debug.LogError(amountOfPerson / myAttributeInfo.azsysl * 60);
                openTimer(amountOfPerson / myAttributeInfo.azsysl * 60f, OnAZSYSuc);
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "当前位置超出安置人员距离，请前往医院或救助站再进行操作");
    }

    private void OnAZSYSuc()
    {
        myRecordedData.eachSortieData[^1].numberOfRescues += amountOfPerson;
        amountOfPerson = 0;
    }

    public void CableDescentRescue(List<ZiYuanBase> allZiyuan)
    {
        if (myState != HelicopterState.hover) return;
        //找到场景中所有灾区点，判断距离
        var items = allZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.DisasterArea);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                currentSkill = SkillType.CableDescentRescue;
                float itemgoods = myAttributeInfo.zdyxzh - amountOfPerson - amountOfGoods;
                int itemperson = Mathf.Min(myAttributeInfo.zdzkl, (int)Mathf.Floor(itemgoods / myAttributeInfo.cnrpjtz));
                Debug.LogError(itemperson / myAttributeInfo.sjjrsl * 60);
                openTimer(itemperson / myAttributeInfo.sjjrsl * 60f, OnZZRYSuc);
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "当前位置超出装在人员距离，请前往灾区点再进行操作");
    }
}