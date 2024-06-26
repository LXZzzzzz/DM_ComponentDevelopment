﻿using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

//把补给也包含在起飞降落里面完成。逻辑较少
public partial class HelicopterController
{
    private float amountOfOil;

    public void TakeOff()
    {
        if (myState != HelicopterState.Landing) return;
        currentSkill = SkillType.TakeOff;
        myRecordedData.eachSortieData.Add(new SingleSortieData());
        openTimer(myAttributeInfo.zsjxhgd / (myAttributeInfo.psl * 3.6f), () => myState = HelicopterState.hover);
    }

    public void Landing()
    {
        if (myState != HelicopterState.hover) return;
        currentSkill = SkillType.Landing;
        openTimer(myAttributeInfo.zsjxhgd / (myAttributeInfo.psl * 3.6f), () => myState = HelicopterState.Landing);
    }

    public void Supply(List<ZiYuanBase> allzy)
    {
        if (myState != HelicopterState.Landing) return;
        //找到场景中补给点，判断距离
        var items = allzy.FindAll(x => x.ZiYuanType == ZiYuanType.Supply);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                currentSkill = SkillType.Supply;
                openTimer(myAttributeInfo.bjsj * 60f, () => amountOfOil = myAttributeInfo.zyl);
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "当前位置超出补给距离，请前往补给点再进行操作");
    }
}