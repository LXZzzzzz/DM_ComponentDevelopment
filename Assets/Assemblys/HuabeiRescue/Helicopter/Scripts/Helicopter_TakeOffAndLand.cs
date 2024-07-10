using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EffectivenessEvaluation;
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
        openTimer(myAttributeInfo.zsjxhgd / (myAttributeInfo.psl * 3.6f), OnTOSuc);

        
        myRecordedData.eachSortieData.Add(new SingleSortieData());
        myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].takeOffTime = MyDataInfo.gameStartTime;
        
        // var items = sceneAllZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.Airport);
        // for (int i = 0; i < items.Count; i++)
        // {
        //     Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
        //     if (Vector3.Distance(transform.position, zyPos) < 10)
        //     {
        //         //在机场起飞才算一个架次
        //         myRecordedData.eachSortieData.Add(new SingleSortieData());
        //         myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].takeOffTime = MyDataInfo.gameStartTime;
        //         return;
        //     }
        // }

        for (int i = 0; i < anis.Length; i++)
        {
            anis[i].Play();
        }
    }

    private void OnTOSuc()
    {
        myState = HelicopterState.hover;
        Vector3 startPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 endPos = new Vector3(transform.position.x, myAttributeInfo.zsjxhgd, transform.position.z);
        amountOfOil -= HeliPointFuel(startPos, endPos, myAttributeInfo.psl / 3.6f, myAttributeInfo.psyh);
    }

    public void Landing()
    {
        if (myState != HelicopterState.hover) return;
        currentSkill = SkillType.Landing;
        openTimer(myAttributeInfo.zsjxhgd / (myAttributeInfo.psl * 3.6f), OnLandSuc);

        for (int i = 0; i < anis.Length; i++)
        {
            anis[i].Stop();
        }
    }

    private void OnLandSuc()
    {
        myState = HelicopterState.Landing;
        //降落就不用耗油了吧
        Vector3 startPos = new Vector3(transform.position.x, myAttributeInfo.zsjxhgd, transform.position.z);
        Vector3 endPos = new Vector3(transform.position.x, 0, transform.position.z);
        // amountOfOil -= HeliPointFuel(startPos, endPos, myAttributeInfo.psl / 3.6f, myAttributeInfo.psyh);
    }

    public void Supply()
    {
        if (myState != HelicopterState.Landing) return;
        //找到场景中补给点，判断距离
        var items = sceneAllZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.Supply);
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