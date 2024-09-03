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
    private int splitNum;
    private UnityAction<int> stageComplete;

    private Animation[] anis;

    private Vector3 PickupPoint;
    private int isGoods; //0物资 1人员 2灭火

    private bool isSendCrash; //记录是否已经发送过坠毁信息

    private float flyHight;

    private List<AudioSource> myass;
    private List<WingMark> mywms;

    private LineRenderer flyLine;
    private Vector3[] positions;

    private Vector3 initialScale = Vector3.zero;


    public override void Init(EquipBase baseData, List<ZiYuanBase> sceneAllZiyuan)
    {
        base.Init(baseData, sceneAllZiyuan);
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

        // mySkills.Add(new SkillData() { SkillType = SkillType.EndTask, isUsable = true, skillName = "结束任务" });

        currentSkill = SkillType.None;
        myState = HelicopterState.NotReady;
        isWaitArrive = false;
        isRunTimer = false;
        currentTargetType = -1;
        var position = transform.position;
        flyHight = position.y;
        position = new Vector3(position.x, GetCurrentGroundHeight() < 0 ? flyHight : GetCurrentGroundHeight(), position.z);
        transform.position = position;
        anis = transform.GetComponentsInChildren<Animation>();
        for (int i = 0; i < anis.Length; i++)
        {
            anis[i].Stop();
        }

        flyLine = gameObject.AddComponent<LineRenderer>();
        flyLine.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        // 设置线宽
        flyLine.startWidth = 1f;
        flyLine.endWidth = 1f;
        // 设置线段颜色
        flyLine.startColor = Color.red;
        flyLine.endColor = Color.blue;
        positions = new Vector3[2];
        initialScale = transform.localScale;
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
        isSendCrash = false;
        myass = new List<AudioSource>();
        mywms = new List<WingMark>();

        mywms.AddRange(transform.GetComponentsInChildren<WingMark>(true));
    }

    private void OnSetTargetType(int zyt)
    {
        if (isChooseMe && (myState == HelicopterState.flying || myState == HelicopterState.hover) && !isCrash)
            currentTargetType = zyt;
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
                mySkills[i].isUsable = (myState == HelicopterState.flying || myState == HelicopterState.hover) && amountOfWater > 1;
            if (mySkills[i].SkillType == SkillType.LadeGoods)
            {
                bool isArriveTargetType = currentTargetType != -1 && isArrive && (ZiYuanType)currentTargetType == ZiYuanType.GoodsPoint;
                mySkills[i].isUsable = isArriveTargetType && myState == HelicopterState.Landing && amountOfPerson == 0;
            }

            if (mySkills[i].SkillType == SkillType.UnLadeGoods)
            {
                ZiYuanType itemType = (ZiYuanType)currentTargetType;
                bool isArriveTargetType = currentTargetType != -1 && isArrive && (itemType == ZiYuanType.GoodsPoint || itemType == ZiYuanType.RescueStation);
                mySkills[i].isUsable = isArriveTargetType && myState == HelicopterState.Landing && amountOfGoods > 1;
            }

            if (mySkills[i].SkillType == SkillType.AirdropGoods)
            {
                ZiYuanType itemType = (ZiYuanType)currentTargetType;
                bool isArriveTargetType = itemType != ZiYuanType.Hospital;
                mySkills[i].isUsable = isArriveTargetType &&(myState == HelicopterState.flying || myState == HelicopterState.hover) && amountOfGoods > 1;
            }
            if (mySkills[i].SkillType == SkillType.Manned)
            {
                bool isArriveTargetType = currentTargetType != -1 && isArrive && (ZiYuanType)currentTargetType == ZiYuanType.DisasterArea;
                mySkills[i].isUsable = isArriveTargetType && myState == HelicopterState.Landing && amountOfGoods < 1;
            }

            if (mySkills[i].SkillType == SkillType.PlacementOfPersonnel)
            {
                ZiYuanType itemType = (ZiYuanType)currentTargetType;
                bool isArriveTargetType = currentTargetType != -1 && isArrive && (itemType == ZiYuanType.Hospital || itemType == ZiYuanType.RescueStation);
                mySkills[i].isUsable = isArriveTargetType && myState == HelicopterState.Landing && amountOfPerson > 0;
            }

            if (mySkills[i].SkillType == SkillType.CableDescentRescue)
            {
                bool isArriveTargetType = currentTargetType != -1 && isArrive && (ZiYuanType)currentTargetType == ZiYuanType.DisasterArea;
                mySkills[i].isUsable = isArriveTargetType && myState == HelicopterState.hover && amountOfGoods < 1;
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
                if (amountOfGoods < myAttributeInfo.zdyxzh)
                    EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerLadeGoods, BObjectId);
                else EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "已达到最大载荷");
                break;
            case SkillType.UnLadeGoods:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerUnLadeGoods, BObjectId);
                break;
            case SkillType.AirdropGoods:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerAirDropGoods, MsgSend_WaterPour(BObjectId, transform.position));
                break;
            case SkillType.Manned:
                if (amountOfPerson < myAttributeInfo.zdzkl)
                    EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerManned, BObjectId);
                else EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "已达到最大载客量");
                break;
            case SkillType.PlacementOfPersonnel:
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerPlacementOfPersonnel, BObjectId);
                break;
            case SkillType.CableDescentRescue:
                if (amountOfPerson < myAttributeInfo.zdzkl)
                    EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerCableDescentRescue, BObjectId);
                else EventManager.Instance.EventTrigger(Enums.EventType.ShowTipUI.ToString(), "已达到最大载客量");
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

        if (!ismove) EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "装备未起飞！");

        if (isCrash)
        {
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "装备油量耗尽，已坠毁！");
            return false;
        }

        if (currentSkill != SkillType.None)
        {
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "当前正在执行任务，无法机动");
            return false;
        }

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

    public override void GetCurrentAllMass(out float currentOil, out float totalOil, out float water, out float goods, out float person, out int personType)
    {
        currentOil = amountOfOil;
        totalOil = myAttributeInfo.zyl;
        water = amountOfWater;
        goods = amountOfGoods;
        person = amountOfPerson;
        personType = this.personType;
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

        DrawLine();

        if (!isCrash && !isSendCrash && amountOfOil <= 0)
        {
            EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.TriggerEquipCrash, BObjectId);
            isSendCrash = true;
        }
    }

    private void switchMyState()
    {
        if (myState == HelicopterState.flying || myState == HelicopterState.hover)
        {
            myState = isArrive ? HelicopterState.hover : HelicopterState.flying;
        }
    }

    private Vector3 lastPos = Vector3.zero;

    private void calculationData()
    {
        if (isCrash) return;
        if (myState == HelicopterState.flying)
        {
            myRecordedData.allDistanceTravelled += speed * Time.deltaTime * MyDataInfo.speedMultiplier;
            if (lastPos != Vector3.zero)
            {
                float ifc = HeliPointFuel(transform.position, lastPos, speed * MyDataInfo.speedMultiplier, myAttributeInfo.xhyh);
                amountOfOil -= ifc;
            }

            lastPos = transform.position;
        }

        if (myState == HelicopterState.hover)
        {
            amountOfOil -= myAttributeInfo.xtyh / 3600f * Time.deltaTime * MyDataInfo.speedMultiplier;
        }
    }

    private void DrawLine()
    {
        Vector3 myUpPos = transform.position + transform.up * 1.5f;
        positions[0] = myUpPos; // 起点
        positions[1] = TargetPos == Vector3.zero ? myUpPos : TargetPos; // 终点

        // 设置线段顶点
        flyLine.SetPositions(positions);

        if (Camera.main != null && initialScale != Vector3.zero)
        {
            float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            if (distance > 500)
            {
                float scaleFactor = distance / 500;
                transform.localScale = initialScale * scaleFactor;
            }
            else
            {
                transform.localScale = initialScale;
            }
        }
    }

    /// <summary>
    /// 直升机到目标点之间的消耗油耗
    /// </summary>
    /// <param name="StartVect">起点的位置</param>
    /// <param name="TargetVect">目标点的位置</param>
    /// <param name="HeliVelocity">直升机速度（巡航，爬升，下降）</param>
    /// <param name="SegmentFlightFuelConsumption">耗油率</param>
    /// <returns></returns>
    private float HeliPointFuel(Vector3 StartVect, Vector3 TargetVect, float HeliVelocity, float SegmentFlightFuelConsumption)
    {
        float RemainingFuel = 0f;
        float SegmentFlightTime; //航段所需时间
        float distanceab = Vector3.Distance(StartVect, TargetVect);
        if (distanceab > 0)
        {
            SegmentFlightTime = distanceab / HeliVelocity; //千米每小时转换成米每秒
            RemainingFuel = SegmentFlightTime * SegmentFlightFuelConsumption / 3600.0f;
        }

        return RemainingFuel;
    }


    private void openTimer(float duration, UnityAction cb, int splitNum = 1, UnityAction<int> stageComplete = null)
    {
        timer = 0;
        timeDuration = duration;
        skillProgress = 0;
        OnCountdownEndsCallBack = cb;
        this.stageComplete = stageComplete;
        currentStage = 0;
        stageDuration = duration / splitNum;
        isRunTimer = true;
    }

    private int currentStage; //当前分段
    private float stageDuration; //每分段时长

    private void runTimer()
    {
        timer += Time.deltaTime * MyDataInfo.speedMultiplier;
        skillProgress = timer / timeDuration;

        if (stageComplete != null)
        {
            if (timer >= stageDuration * currentStage)
            {
                stageComplete.Invoke(currentStage++);
            }
        }

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
    /// 装载物资时间
    /// </summary>
    public float zzwzsj;

    /// <summary>
    /// 卸载物资时间
    /// </summary>
    public float xzwzsj;

    /// <summary>
    /// 空投物资时间
    /// </summary>
    public float ktwzsj;

    /// <summary>
    /// 落地装载人员时间
    /// </summary>
    public float ldzzrysj;

    /// <summary>
    /// 索降救人时间
    /// </summary>
    public float sjjrsj;

    /// <summary>
    /// 绞车型号
    /// </summary>
    public string jcxh;

    /// <summary>
    /// 绞车收放速度
    /// </summary>
    public string jcsfsd;

    /// <summary>
    /// 安置伤员时间
    /// </summary>
    public float azsysj;

    /// <summary>
    /// 补给时间
    /// </summary>
    public float bjsj;

    /// <summary>
    /// 成年人平均体重
    /// </summary>
    public float cnrpjtz;

    /// <summary>
    /// 直升机价格(只是用来效能评估，取出来算最小单价)
    /// </summary>
    public float zsjjg;

    /// <summary>
    /// 最低每小时耗油量(只是用来效能评估，取出来算最小耗油量)
    /// </summary>
    /// <returns></returns>
    public float zdmxshyl;
}