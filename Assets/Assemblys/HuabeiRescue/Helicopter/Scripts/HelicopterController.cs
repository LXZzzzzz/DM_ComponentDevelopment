using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using EventType = Enums.EventType;

public class HelicopterController : EquipBase, IWaterIntaking, IGroundReady, IWaterPour, ISupply, IReturnFlight
{
    private bool isWaitArrive;
    private float quWaterDuration = 3; //取水时长
    private float groundReadyDuration = 6; //地面准备时长
    [HideInInspector] public int instructionSetModel;


    public override void Init()
    {
        base.Init();
        //初始化飞机的基本属性
        switch (instructionSetModel)
        {
            case 1:
                mySkills.Add(new SkillData() { SkillType = SkillType.GroundReady, skillName = "地面准备" });
                mySkills.Add(new SkillData() { SkillType = SkillType.WaterIntaking, skillName = "取水" });
                mySkills.Add(new SkillData() { SkillType = SkillType.WaterPour, skillName = "投水" });
                mySkills.Add(new SkillData() { SkillType = SkillType.Supply, skillName = "补给" });
                mySkills.Add(new SkillData() { SkillType = SkillType.ReturnFlight, skillName = "返航" });
                break;
            case 2:
                mySkills.Add(new SkillData() { SkillType = SkillType.GroundReady, skillName = "地面准备" });
                mySkills.Add(new SkillData() { SkillType = SkillType.LadeGoods, skillName = "装载物资" });
                mySkills.Add(new SkillData() { SkillType = SkillType.Manned, skillName = "载人" });
                mySkills.Add(new SkillData() { SkillType = SkillType.UnLadeGoods, skillName = "卸载物资" });
                mySkills.Add(new SkillData() { SkillType = SkillType.AirdropGoods, skillName = "空投物资" });
                mySkills.Add(new SkillData() { SkillType = SkillType.Supply, skillName = "补给" });
                mySkills.Add(new SkillData() { SkillType = SkillType.ReturnFlight, skillName = "返航" });
                break;
        }

        currentSkill = SkillType.None;
        isWaitArrive = false;
    }

    public override void OnSelectSkill(SkillType st)
    {
        if (mySkills.Find(x => x.SkillType == st) == null) return; //技能数据错误
        CurrentChooseSkillType = st;
        switch (st)
        {
            case SkillType.GroundReady:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.TriggerGroundReady, BObjectId);
                break;
            case SkillType.WaterIntaking:
                //这里需要打开取水UI界面，并把自己以接口形式传过去(这里需求更改，取水直接执行)
                // EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", this);

                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.TriggerWaterIntaking, BObjectId);

                break;
            case SkillType.WaterPour:

                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.TriggerWaterPour, BObjectId);
                break;
            case SkillType.Supply:

                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.TriggerSupply, BObjectId);
                break;
            case SkillType.ReturnFlight:

                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.TriggerReturnFlight, BObjectId);
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
            EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.TriggerWaterIntaking, MsgSend_WaterIntaking(BObjectId, pos, amount));
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
            StartCoroutine(returnFlight(3));
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

    public void GroundReady()
    {
        StartCoroutine(onlyShowUI(SkillType.GroundReady, groundReadyDuration));
    }

    IEnumerator onlyShowUI(SkillType type, float duration)
    {
        currentSkill = type;
        skillProgress = 0;
        float endTime = Time.time + duration / MyDataInfo.speedMultiplier;
        while (true)
        {
            yield return 1;
            skillProgress = 1 - (endTime - Time.time) / (quWaterDuration / MyDataInfo.speedMultiplier);
            if (Time.time > endTime) break;
        }

        currentSkill = SkillType.None;
    }

    public void WaterPour()
    {
        StartCoroutine(onlyShowUI(SkillType.WaterPour, 3));
    }

    public void Supply(List<ZiYuanBase> allzy)
    {
        var items = allzy.FindAll(x => x.ZiYuanType == ZiYuanType.Supply);
        for (int i = 0; i < items.Count; i++)
        {
            Vector3 zyPos = new Vector3(items[i].transform.position.x, transform.position.y, items[i].transform.position.z);
            if (Vector3.Distance(transform.position, zyPos) < 10)
            {
                StartCoroutine(onlyShowUI(SkillType.Supply, 5));
                return;
            }
        }

        if (string.Equals(BeLongToCommanderId, MyDataInfo.leadId))
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "当前位置超出补给距离，请前往补给点再进行操作");
    }

    public void ReturnFlight(ZiYuanBase ziyuan)
    {
        if (ziyuan==null)
        {
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "当前飞机没有初始机场，不可返航");
            return;
        }
        
        isWaitArrive = true;
        returnFlightObj = ziyuan;
        MoveToTarget(ziyuan.transform.position);
    }

    private ZiYuanBase returnFlightObj;
    IEnumerator returnFlight(float duration)
    {
        currentSkill = SkillType.ReturnFlight;
        skillProgress = 0;
        float endTime = Time.time + duration / MyDataInfo.speedMultiplier;
        while (true)
        {
            yield return 1;
            skillProgress = 1 - (endTime - Time.time) / (quWaterDuration / MyDataInfo.speedMultiplier);
            if (Time.time > endTime) break;
        }

        (returnFlightObj as IAirPort).AddEquip(BObjectId);
        currentSkill = SkillType.None;
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