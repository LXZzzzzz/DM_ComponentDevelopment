using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using EventType = Enums.EventType;

public class HelicopterController : EquipBase, IWaterIntaking
{
    private bool isWaitArrive;
    private float quWaterDuration = 3;


    public override void Init()
    {
        base.Init();
        //初始化飞机的基本属性
        mySkills.Add(new SkillData() { SkillType = SkillType.WaterIntaking, skillName = "取水" });
        mySkills.Add(new SkillData() { SkillType = SkillType.WaterIntaking, skillName = "投水" });
        mySkills.Add(new SkillData() { SkillType = SkillType.WaterIntaking, skillName = "盘旋" });
        mySkills.Add(new SkillData() { SkillType = SkillType.WaterIntaking, skillName = "补给" });

        currentSkill = SkillType.None;
        isWaitArrive = false;
    }

    public override void OnSelectSkill(SkillType st)
    {
        if (mySkills.Find(x => x.SkillType == st) == null) return; //技能数据错误
        CurrentChooseSkillType = st;
        switch (st)
        {
            case SkillType.WaterIntaking:
                //这里需要打开取水UI界面，并把自己以接口形式传过去(这里需求更改，取水直接执行)
                // EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", this);

                EventManager.Instance.EventTrigger(EventType.SendWaterInfoToControler.ToString(), BObjectId);

                break;
        }
    }

    protected override void OnClose()
    {
        EventManager.Instance.EventTrigger(EventType.DestoryEquip.ToString(), BObjectId);
    }

    public void WaterIntaking(Vector3 pos, float range, float amount, bool isExecuteImmediately)
    {
        Debug.LogError("取水技能参数回传" + isExecuteImmediately);
        if (!isExecuteImmediately)
        {
            //把参数传给主角，将参数传给所有客户端，统一执行
            EventManager.Instance.EventTrigger(EventType.SendWaterInfoToControler.ToString(), MsgSend_Water(BObjectId, pos, amount));
            return;
        }

        //判断自己是否处于取水地的范围内，不在的话调move机动到目的地，然后，执行取水逻辑
        if (Vector3.Distance(transform.position, pos) > range)
        {
            isWaitArrive = true;
            MoveToTarget(pos);
        }
        else StartCoroutine(quWater());
    }

    public float CheckCapacity()
    {
        Debug.LogError("检查数量");
        return 10;
    }

    public void WaterIntaking_New(List<ZiYuanBase> allzy)
    {
        //找到场景中所有水源点，判断距离
        var items = allzy.FindAll(x => x.ZiYuanType == ZiYuanType.Waters);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                StartCoroutine(quWater());
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "当前位置超出取水距离，请前往水源地再进行操作");
    }


    private void LateUpdate()
    {
        if (MyDataInfo.gameState == GameState.GamePause || MyDataInfo.gameState == GameState.GameStop) return;
        if (isWaitArrive && isArrive)
        {
            isWaitArrive = false;
            StartCoroutine(quWater());
        }
    }

    IEnumerator quWater()
    {
        currentSkill = SkillType.WaterIntaking;
        skillProgress = 0;
        float endTime = Time.time + quWaterDuration / MyDataInfo.speedMultiplier;
        Transform waterSphere = transform.GetChild(1);
        while (true)
        {
            waterSphere.Translate(Vector3.up * .8f);
            if (waterSphere.localPosition.y >= 0) waterSphere.localPosition = Vector3.up * -20;
            yield return 1;
            skillProgress = 1 - (endTime - Time.time) / (quWaterDuration / MyDataInfo.speedMultiplier);
            if (Time.time > endTime) break;
        }

        waterSphere.localPosition = Vector3.zero;
        currentSkill = SkillType.None;
    }


    private string MsgSend_Water(string id, Vector3 pos, float amount)
    {
        return string.Format($"{pos.x}_{pos.y}_{pos.z}_{amount}_{id}");
    }
}