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

        var items = sceneAllZiyuan.FindAll(x => x.ZiYuanType == ZiYuanType.Airport);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                //第一次从机场起飞记为起飞时刻
                if (myRecordedData.takeOffTime < 1)
                    myRecordedData.takeOffTime = MyDataInfo.gameStartTime;
                Debug.LogError($"起飞时刻：{myRecordedData.takeOffTime}");
                break;
            }
        }

        for (int i = 0; i < anis.Length; i++)
        {
            anis[i].Play();
        }
        myass.ForEach(x=>x.gameObject.SetActive(true));
    }

    private void OnTOSuc()
    {
        myState = HelicopterState.hover;
        Vector3 startPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 endPos = new Vector3(transform.position.x, myAttributeInfo.zsjxhgd, transform.position.z);
        amountOfOil -= HeliPointFuel(startPos, endPos, myAttributeInfo.psl / 3.6f, myAttributeInfo.psyh);
        var itemPosition = transform.position;
        itemPosition = new Vector3(itemPosition.x, flyHight, itemPosition.z);
        transform.position = itemPosition;
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
        
        if (myass.Count == 0)
        {
            var ass = transform.GetComponentsInChildren<AudioSource>();
            for (int i = 0; i < ass.Length; i++)
            {
                if (ass[i].enabled) myass.Add(ass[i]);
            }
        }
        myass.ForEach(x=>x.gameObject.SetActive(false));
        mywms.ForEach(x => x.gameObject.SetActive(x.mark == 0));
    }

    private void OnLandSuc()
    {
        myState = HelicopterState.Landing;
        //降落就不用耗油了吧
        Vector3 startPos = new Vector3(transform.position.x, myAttributeInfo.zsjxhgd, transform.position.z);
        Vector3 endPos = new Vector3(transform.position.x, 0, transform.position.z);
        // amountOfOil -= HeliPointFuel(startPos, endPos, myAttributeInfo.psl / 3.6f, myAttributeInfo.psyh);
        var itemPosition = transform.position;
        itemPosition = new Vector3(itemPosition.x, GetCurrentGroundHeight() < 0 ? flyHight : GetCurrentGroundHeight(), itemPosition.z);
        transform.position = itemPosition;
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

    /// <summary>
    /// 获取当前位置地面的高度
    /// </summary>
    /// <returns></returns>
    private float GetCurrentGroundHeight()
    {
        // 射线的起点是当前物体的位置
        Ray ray = new Ray(transform.position, Vector3.down);

        // 存储射线碰撞信息的变量
        RaycastHit hit;

        // 检测射线是否碰撞到任何物体
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // 打印碰撞点的坐标
            Debug.Log("Hit Point: " + hit.point);
            return hit.point.y;
        }
        else
        {
            // 如果没有碰撞到任何物体
            Debug.Log("No hit");
            return -1;
        }
    }
}