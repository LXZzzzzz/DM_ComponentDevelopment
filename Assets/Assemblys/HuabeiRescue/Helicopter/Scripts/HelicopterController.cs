using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using ToolsLibrary;
using ToolsLibrary.EffectivenessEvaluation;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.Events;
using EventType = Enums.EventType;

public partial class HelicopterController : EquipBase, IWatersOperation, IGroundReady, ITakeOffAndLand, IGoodsOperation, IRescuePersonnelOperation, ISupply
{
    private bool isWaitArrive;
    [HideInInspector] public bool isTS, isYSWZ, isYSRY, isSJJY;
    public HelicopterInfo myAttributeInfo;
    private List<SkillData> mySkills;
    private HelicopterState myState;
    private UnityAction OnCountdownEndsCallBack;
    private int currentTargetType;
    private RecordedData myRecordedData;
    private List<ZiYuanBase> sceneAllZiyuan;

    private bool isRunTimer;
    private float timer;
    private float timeDuration;

    private Animation[] anis;


    public override void Init(EquipBase baseData,List<ZiYuanBase> sceneAllZiyuan)
    {
        base.Init(baseData,sceneAllZiyuan);
        this.sceneAllZiyuan = sceneAllZiyuan;
        InitData(baseData);
        EventManager.Instance.AddEventListener<int>(EventType.ChooseEquipToZiYuanType.ToString(), OnSetTargetType);
        myRecordedData = new RecordedData();
        myRecordedData.eachSortieData = new List<SingleSortieData>();
        mySkills = new List<SkillData>();
        //初始化飞机的基本属性
        mySkills.Add(new SkillData() { SkillType = SkillType.GroundReady, isUsable = false, skillName = "起飞前准备" });
        mySkills.Add(new SkillData() { SkillType = SkillType.BePutInStorage, isUsable = false, skillName = "入库" });
        mySkills.Add(new SkillData() { SkillType = SkillType.TakeOff, isUsable = false, skillName = "起飞" });
        mySkills.Add(new SkillData() { SkillType = SkillType.Supply, isUsable = false, skillName = "补给" });
        mySkills.Add(new SkillData() { SkillType = SkillType.Landing, isUsable = false, skillName = "降落" });
        if (isTS)
        {
            mySkills.Add(new SkillData() { SkillType = SkillType.WaterIntaking, isUsable = false, skillName = "取水" });
            mySkills.Add(new SkillData() { SkillType = SkillType.WaterPour, isUsable = false, skillName = "投水" });
        }

        if (isYSWZ)
        {
            mySkills.Add(new SkillData() { SkillType = SkillType.LadeGoods, isUsable = false, skillName = "装载物资" });
            mySkills.Add(new SkillData() { SkillType = SkillType.UnLadeGoods, isUsable = false, skillName = "卸载物资" });
            mySkills.Add(new SkillData() { SkillType = SkillType.AirdropGoods, isUsable = false, skillName = "空投物资" });
        }

        if (isYSRY)
        {
            mySkills.Add(new SkillData() { SkillType = SkillType.Manned, isUsable = false, skillName = "装载人员" });
            mySkills.Add(new SkillData() { SkillType = SkillType.PlacementOfPersonnel, isUsable = false, skillName = "安置人员" });
        }

        if (isSJJY)
        {
            mySkills.Add(new SkillData() { SkillType = SkillType.CableDescentRescue, isUsable = false, skillName = "索降救援" });
            if (mySkills.Find(x => x.SkillType == SkillType.PlacementOfPersonnel) == null)
                mySkills.Add(new SkillData() { SkillType = SkillType.PlacementOfPersonnel, isUsable = false, skillName = "安置人员" });
        }

        mySkills.Add(new SkillData() { SkillType = SkillType.ReturnFlight, isUsable = true, skillName = "返航" });
        mySkills.Add(new SkillData() { SkillType = SkillType.EndTask, isUsable = true, skillName = "结束任务" });

        currentSkill = SkillType.None;
        myState = HelicopterState.NotReady;
        isWaitArrive = false;
        isRunTimer = false;
        currentTargetType = -1;
        anis = transform.GetComponentsInChildren<Animation>();
        for (int i = 0; i < anis.Length; i++)
        {
            anis[i].Stop();
        }
    }

    private void InitData(EquipBase baseData)
    {
        isTS = (baseData as HelicopterController).isTS;
        isYSWZ = (baseData as HelicopterController).isYSWZ;
        isYSRY = (baseData as HelicopterController).isYSRY;
        isSJJY = (baseData as HelicopterController).isSJJY;
        myAttributeInfo = JsonUtility.FromJson<HelicopterInfo>(JsonUtility.ToJson((baseData as HelicopterController).myAttributeInfo));
        amountOfOil = myAttributeInfo.zyl;
        amountOfWater = 0;
        amountOfGoods = 0;
        amountOfPerson = 0;
        speed = myAttributeInfo.zsjxhsd / 3.6f;
    }

    private void OnSetTargetType(int zyt)
    {
        if (isChooseMe) currentTargetType = zyt;
    }

    public override List<SkillData> GetSkillsData()
    {
        for (int i = 0; i < mySkills.Count; i++)
        {
            if (mySkills[i].SkillType == SkillType.GroundReady)
                mySkills[i].isUsable = myState == HelicopterState.NotReady;
            if (mySkills[i].SkillType == SkillType.BePutInStorage)
            {
                bool isArriveTargetType = currentTargetType != -1 && isArrive && (ZiYuanType)currentTargetType == ZiYuanType.Airport;
                mySkills[i].isUsable = isArriveTargetType && myState == HelicopterState.Landing;
            }

            if (mySkills[i].SkillType == SkillType.TakeOff)
                mySkills[i].isUsable = myState == HelicopterState.Landing;
            if (mySkills[i].SkillType == SkillType.Landing)
            {
                ZiYuanType itemType = (ZiYuanType)currentTargetType;
                bool isArriveTargetType = currentTargetType != -1 && isArrive && itemType != ZiYuanType.SourceOfAFire && itemType != ZiYuanType.Waters;
                mySkills[i].isUsable = isArriveTargetType && myState == HelicopterState.hover;
            }

            if (mySkills[i].SkillType == SkillType.Supply)
            {
                bool isArriveTargetType = currentTargetType != -1 && isArrive && (ZiYuanType)currentTargetType == ZiYuanType.Supply;
                mySkills[i].isUsable = isArriveTargetType && myState == HelicopterState.Landing;
            }

            if (mySkills[i].SkillType == SkillType.WaterIntaking)
            {
                bool isArriveTargetType = currentTargetType != -1 && isArrive && (ZiYuanType)currentTargetType == ZiYuanType.Waters;
                mySkills[i].isUsable = isArriveTargetType && myState == HelicopterState.hover;
            }

            if (mySkills[i].SkillType == SkillType.WaterPour)
                mySkills[i].isUsable = myState == HelicopterState.flying || myState == HelicopterState.hover;
            if (mySkills[i].SkillType == SkillType.LadeGoods)
            {
                bool isArriveTargetType = currentTargetType != -1 && isArrive && (ZiYuanType)currentTargetType == ZiYuanType.GoodsPoint;
                mySkills[i].isUsable = isArriveTargetType && myState == HelicopterState.Landing;
            }

            if (mySkills[i].SkillType == SkillType.UnLadeGoods)
            {
                ZiYuanType itemType = (ZiYuanType)currentTargetType;
                bool isArriveTargetType = currentTargetType != -1 && isArrive && (itemType == ZiYuanType.GoodsPoint || itemType == ZiYuanType.RescueStation);
                mySkills[i].isUsable = isArriveTargetType && myState == HelicopterState.Landing;
            }

            if (mySkills[i].SkillType == SkillType.AirdropGoods)
                mySkills[i].isUsable = myState == HelicopterState.flying || myState == HelicopterState.hover;
            if (mySkills[i].SkillType == SkillType.Manned)
            {
                bool isArriveTargetType = currentTargetType != -1 && isArrive && (ZiYuanType)currentTargetType == ZiYuanType.DisasterArea;
                mySkills[i].isUsable = isArriveTargetType && myState == HelicopterState.Landing;
            }

            if (mySkills[i].SkillType == SkillType.PlacementOfPersonnel)
            {
                ZiYuanType itemType = (ZiYuanType)currentTargetType;
                bool isArriveTargetType = currentTargetType != -1 && isArrive && (itemType == ZiYuanType.Hospital || itemType == ZiYuanType.RescueStation);
                mySkills[i].isUsable = isArriveTargetType && myState == HelicopterState.Landing;
            }

            if (mySkills[i].SkillType == SkillType.CableDescentRescue)
            {
                bool isArriveTargetType = currentTargetType != -1 && isArrive && (ZiYuanType)currentTargetType == ZiYuanType.DisasterArea;
                mySkills[i].isUsable = isArriveTargetType && myState == HelicopterState.hover;
            }
        }

        //检测所有技能的显示条件
        return mySkills;
    }

    public override RecordedData GetRecordedData()
    {
        //获取记录的数据
        return myRecordedData;
    }

    public override void OnSelectSkill(SkillType st)
    {
        if (mySkills.Find(x => x.SkillType == st) == null) return; //技能数据错误
        CurrentChooseSkillType = st;
        switch (st)
        {
            case SkillType.GroundReady:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerGroundReady, BObjectId);
                break;
            case SkillType.BePutInStorage:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerBePutInStorage, BObjectId);
                break;
            case SkillType.TakeOff:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerTakeOff, BObjectId);
                break;
            case SkillType.Landing:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerLanding, BObjectId);
                break;
            case SkillType.Supply:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerSupply, BObjectId);
                break;
            case SkillType.WaterIntaking:
                //这里需要打开取水UI界面，并把自己以接口形式传过去(这里需求更改，取水直接执行)
                // EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", this);
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerWaterIntaking, BObjectId);
                break;
            case SkillType.WaterPour:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerWaterPour, MsgSend_WaterPour(BObjectId, transform.position));
                break;
            case SkillType.LadeGoods:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerLadeGoods, BObjectId);
                break;
            case SkillType.UnLadeGoods:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerUnLadeGoods, BObjectId);
                break;
            case SkillType.AirdropGoods:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerAirDropGoods, MsgSend_WaterPour(BObjectId, transform.position));
                break;
            case SkillType.Manned:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerManned, BObjectId);
                break;
            case SkillType.PlacementOfPersonnel:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerPlacementOfPersonnel, BObjectId);
                break;
            case SkillType.CableDescentRescue:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerCableDescentRescue, BObjectId);
                break;
            case SkillType.ReturnFlight:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerReturnFlight, BObjectId);
                break;
            case SkillType.EndTask:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerEndTask, BObjectId);
                break;
        }
    }

    public override bool OnCheckIsMove()
    {
        // if (currentSkill != SkillType.None)
        // {
        //     currentSkill = SkillType.None;
        //     isRunTimer = false;
        // }

        bool ismove = myState == HelicopterState.flying || myState == HelicopterState.hover;

        if (!ismove) EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "装备未起飞");
        return ismove;
    }

    public override void OnNullCommand(int type)
    {
        if (type == 0)
        {
            myRecordedData.eachSortieData[myRecordedData.eachSortieData.Count - 1].returnFlightTime = MyDataInfo.gameStartTime;
        }

        if (type == 1)
        {
            if (myRecordedData.endTaskTime < 1)
                myRecordedData.endTaskTime = MyDataInfo.gameStartTime;
        }
    }

    protected override void OnClose()
    {
        EventManager.Instance.EventTrigger(EventType.DestoryEquip.ToString(), BObjectId);
        EventManager.Instance.RemoveEventListener<int>(EventType.ChooseEquipToZiYuanType.ToString(), OnSetTargetType);
    }

    private void LateUpdate()
    {
        if (MyDataInfo.gameState == GameState.GamePause || MyDataInfo.gameState == GameState.GameStop) return;

        if (isRunTimer) runTimer();

        switchMyState();

        calculationData();
    }

    private void switchMyState()
    {
        if (myState == HelicopterState.flying || myState == HelicopterState.hover)
        {
            myState = isArrive ? HelicopterState.hover : HelicopterState.flying;
        }
    }

    private void calculationData()
    {
        if (myState == HelicopterState.flying)
        {
            myRecordedData.allDistanceTravelled += speed * Time.deltaTime * MyDataInfo.speedMultiplier;
            //todo:应该还得加油耗计算，飞行油耗，悬浮油耗
        }
    }


    private void openTimer(float duration, UnityAction cb)
    {
        timer = 0;
        timeDuration = duration;
        skillProgress = 0;
        OnCountdownEndsCallBack = cb;
        isRunTimer = true;
    }

    private void runTimer()
    {
        timer += Time.deltaTime * MyDataInfo.speedMultiplier;
        skillProgress = timer / timeDuration;
        if (timer >= timeDuration)
        {
            // 计时结束，执行相关操作
            skillProgress = 1;
            timer = 0; // 重置计时器
            isRunTimer = false;
            currentSkill = SkillType.None;
            OnCountdownEndsCallBack?.Invoke();
        }
    }
}

public enum HelicopterState
{
    /// <summary>
    /// 未就绪
    /// </summary>
    NotReady,

    /// <summary>
    /// 降落
    /// </summary>
    Landing,

    /// <summary>
    /// 飞行
    /// </summary>
    flying,

    /// <summary>
    /// 悬停
    /// </summary>
    hover
}

public class HelicopterInfo
{
    /// <summary>
    /// 起飞前准备时间
    /// </summary>
    public float qfqzbsj;

    /// <summary>
    /// 最大起飞重量
    /// </summary>
    public float zdqfzl;

    /// <summary>
    /// 空机重量
    /// </summary>
    public float kjzl;

    /// <summary>
    /// 最大航程
    /// </summary>
    public float zdhc;

    /// <summary>
    /// 最大有效载荷
    /// </summary>
    public float zdyxzh;

    /// <summary>
    /// 载油量
    /// </summary>
    public float zyl;

    /// <summary>
    /// 最大载客量
    /// </summary>
    public int zdzkl;

    /// <summary>
    /// 最大载水量
    /// </summary>
    public float zdzsl;

    /// <summary>
    /// 最大时速
    /// </summary>
    public float zdss;

    /// <summary>
    /// 直升机巡航速度
    /// </summary>
    public float zsjxhsd;

    /// <summary>
    /// 直升机巡航高度
    /// </summary>
    public float zsjxhgd;

    /// <summary>
    /// 巡航油耗
    /// </summary>
    public float xhyh;

    /// <summary>
    /// 爬升率
    /// </summary>
    public float psl;

    /// <summary>
    /// 爬升油耗
    /// </summary>
    public float psyh;

    /// <summary>
    /// 悬停油耗
    /// </summary>
    public float xtyh;

    /// <summary>
    /// 吊水重量
    /// </summary>
    public float dszl;

    /// <summary>
    /// 取水时间
    /// </summary>
    public float qssj;

    /// <summary>
    /// 洒水时间
    /// </summary>
    public float sssj;

    /// <summary>
    /// 加油时间
    /// </summary>
    public float jysj;

    /// <summary>
    /// 装载物资速率
    /// </summary>
    public float zzwzsl;

    /// <summary>
    /// 卸载物资速率
    /// </summary>
    public float xzwzsl;

    /// <summary>
    /// 空投物资速率
    /// </summary>
    public float ktwzsl;

    /// <summary>
    /// 落地装载人员速率
    /// </summary>
    public float ldzzrysl;

    /// <summary>
    /// 索降救人速率
    /// </summary>
    public float sjjrsl;

    /// <summary>
    /// 安置伤员速率
    /// </summary>
    public float azsysl;

    /// <summary>
    /// 补给时间
    /// </summary>
    public float bjsj;

    /// <summary>
    /// 成年人平均体重
    /// </summary>
    public float cnrpjtz;

    /// <summary>
    /// 洒水喷洒面积
    /// </summary>
    public float psmj;

    /// <summary>
    /// 直升机价格
    /// </summary>
    public float zsjjg;

    /// <summary>
    /// 最低每小时耗油量
    /// </summary>
    /// <returns></returns>
    public float zdmxshyl;
}