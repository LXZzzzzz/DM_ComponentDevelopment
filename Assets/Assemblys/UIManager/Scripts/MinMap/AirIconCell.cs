using System;
using System.Collections.Generic;
using DG.Tweening;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Vectrosity;
using UiManager;
using UnityEngine.UI;

public class AirIconCell : IconCellBase
{
    private EquipBase equipGo;
    private RectTransform meRect;
    private GameObject rootObj;
    private Func<Vector3, Vector2> worldPosMapPosFunc;

    private UnityAction<bool, Vector2, Vector2> setRouteCb;

    // private GameObject showChooseState;
    private VectorLine currentMoveRoute;
    private List<Vector2> routePoints;
    private Slider skillProgressShow;
    private Text skillName;
    private Transform belongtoPart;
    private Slider currentOil;
    private Image oilPic;
    private GameObject water, goods, qPerson, zPerson;
    private GameObject airPort;
    private GameObject onGround;

    public void Init(EquipBase equipGo, string belongToId, UnityAction<string, PointerEventData.InputButton> chooseCb, Func<Vector3, Vector2> worldPosMapPosFunc, UnityAction<bool, Vector2, Vector2> setRouteCb)
    {
        base.Init(belongToId, chooseCb);
        this.equipGo = equipGo;
        this.worldPosMapPosFunc = worldPosMapPosFunc;
        this.setRouteCb = setRouteCb;
        meRect = GetComponent<RectTransform>();
        rootObj = transform.Find("Root").gameObject;
        // showChooseState = transform.Find("Root/Choose").gameObject;
        transform.Find("Root/mainPart/airType").GetComponent<Image>().sprite = equipGo.EquipIcon;
        transform.Find("Root/mainPart/equipName").GetComponent<Text>().text = equipGo.name;
        skillName = transform.Find("Root/skillBg/skillName").GetComponent<Text>();
        belongtoPart = transform.Find("Root/mainPart/belongToPart");
        currentOil = transform.Find("Root/currentInfoShow/oilPart/oilShow").GetComponent<Slider>();
        oilPic = transform.Find("Root/currentInfoShow/oilPart/pic").GetComponent<Image>();
        water = transform.Find("Root/currentInfoShow/waterPart").gameObject;
        goods = transform.Find("Root/currentInfoShow/goodsPart").gameObject;
        qPerson = transform.Find("Root/currentInfoShow/qPersonPart").gameObject;
        zPerson = transform.Find("Root/currentInfoShow/zPersonPart").gameObject;
        skillProgressShow = transform.Find("Root/skillBg/progressShow").GetComponent<Slider>();
        onGround = transform.Find("Root/mainPart/onGround").gameObject;
        initLine();
    }

    private void initLine()
    {
        routePoints = new List<Vector2>();
        routePoints.Add(Vector2.zero);
        routePoints.Add(Vector2.zero);
        currentMoveRoute = new VectorLine("Line" + equipGo.BObjectId, routePoints, 3, LineType.Continuous);
#if UNITY_EDITOR
        currentMoveRoute.SetCanvas(gameObject.GetComponentInParent<Canvas>());
#else
        currentMoveRoute.SetCanvas(UIManager.Instance.CurrentCanvans);
#endif
        currentMoveRoute.rectTransform.SetParent(transform.parent);
        currentMoveRoute.rectTransform.localPosition = Vector3.zero;
        currentMoveRoute.rectTransform.localScale = Vector3.one;
        currentMoveRoute.active = true;
        if (ColorUtility.TryParseHtmlString("#4dbaff", out Color color))
            currentMoveRoute.color = color;
        else currentMoveRoute.color = Color.cyan;
    }

    private void Update()
    {
        if (equipGo != null)
        {
            if (!equipGo.isDockingAtTheAirport)
                meRect.GetComponent<RectTransform>().anchoredPosition = worldPosMapPosFunc(equipGo.transform.position);
            else
                meRect.GetComponent<RectTransform>().anchoredPosition = worldPosMapPosFunc(getAirPort().transform.position);

            rootObj.SetActive(!equipGo.isDockingAtTheAirport);
        }

        selectChange(equipGo.isChooseMe);
        changeBelongtoShow();
        showSkillState();
        showAllMassInfo();
        showOilWarn();
    }

    private GameObject getAirPort()
    {
        if (airPort != null) return airPort;
        string airPortId = ProgrammeDataManager.Instance.GetEquipDataById(equipGo.BObjectId).airportId;
        for (int j = 0; j < allBObjects.Length; j++)
        {
            if (string.Equals(airPortId, allBObjects[j].BObject.Id) && allBObjects[j].GetComponent<ZiYuanBase>() != null)
            {
                airPort = allBObjects[j].gameObject;
                break;
            }
        }

        return airPort;
    }

    protected override IconInfoData GetBasicInfo()
    {
        equipGo.GetCurrentAllMass(out float currentOil, out float totalOil, out float water, out float goods, out float person, out int personType);
        IconInfoData data = new IconInfoData()
        {
            entityName = equipGo.name, entityInfo = $"飞机装备:{equipGo.name}", beUseCommanders = new List<string> { equipGo.BeLongToCommanderId },
            isAir = true, currentOilMass = currentOil, maxOilMass = totalOil, waterNum = water, goodsNum = goods, personNum = person, personType = personType
        };

        return data;
    }

    private bool isLastSelect;

    private void selectChange(bool isSelect)
    {
        if (isSelect && routePoints != null) setRouteCb?.Invoke(Vector2.Distance(routePoints[0], routePoints[1]) > 50, routePoints[0], routePoints[1]);

        if (routePoints != null)
        {
            routePoints[0] = meRect.anchoredPosition;
            routePoints[1] = equipGo.TargetPos == Vector3.zero ? meRect.anchoredPosition : worldPosMapPosFunc(equipGo.TargetPos);
            currentMoveRoute.Draw();
        }

        if (isLastSelect == isSelect) return;
        isLastSelect = isSelect;
        // showChooseState.SetActive(isSelect);
        var comData = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, equipGo.BeLongToCommanderId));
        belongtoPart.GetChild(0).GetComponent<Image>().color = isSelect ? comData.ChooseColor : comData.MyColor;
        belongtoPart.GetChild(1).GetComponent<Image>().color = isSelect ? Color.white : comData.MyColor;

        if (isSelect && MyDataInfo.gameState == GameState.GameStart)
        {
            currentMoveRoute.active = true;
        }
        else
        {
            // currentMoveRoute.active = false;
            setRouteCb?.Invoke(false, Vector2.zero, Vector2.zero);
        }
    }

    private string belongtoCom;

    private void changeBelongtoShow()
    {
        if (true)
        {
            if (equipGo.BeLongToCommanderId != belongtoCom)
            {
                belongtoCom = equipGo.BeLongToCommanderId;
                var comData = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, equipGo.BeLongToCommanderId));
                belongtoPart.GetChild(0).GetComponent<Image>().color = comData.MyColor;
                belongtoPart.GetChild(1).GetComponent<Image>().color = comData.MyColor;
                belongtoPart.GetChild(2).GetComponent<Image>().color = comData.IconBgColor;
                belongtoPart.GetChild(3).GetComponent<Image>().color = comData.IconBgColor;
                // skillProgressShow = transform.Find($"Root/progressPart/{comData.progressId}").GetComponent<Slider>();
            }
        }
    }

    private void showSkillState()
    {
        if (equipGo.currentSkill == SkillType.None)
        {
            if (skillName.transform.parent.gameObject.activeSelf) skillName.transform.parent.gameObject.SetActive(false);
            if (skillProgressShow != null && skillProgressShow.gameObject.activeSelf) skillProgressShow.gameObject.SetActive(false);
            return;
        }

        if (!skillName.transform.parent.gameObject.activeSelf) skillName.transform.parent.gameObject.SetActive(true);
        if (skillProgressShow != null && !skillProgressShow.gameObject.activeSelf) skillProgressShow.gameObject.SetActive(true);

        switch (equipGo.currentSkill)
        {
            case SkillType.GroundReady:
                skillName.text = "正在起飞前准备...";
                break;
            case SkillType.BePutInStorage:
                skillName.text = "正在入库...";
                break;
            case SkillType.TakeOff:
                skillName.text = "正在起飞...";
                onGround.SetActive(false);
                break;
            case SkillType.Landing:
                skillName.text = "正在降落...";
                onGround.SetActive(true);
                break;
            case SkillType.Supply:
                skillName.text = "正在补给...";
                break;
            case SkillType.WaterIntaking:
                skillName.text = "正在取水...";
                break;
            case SkillType.WaterPour:
                skillName.text = "正在投水...";
                break;
            case SkillType.LadeGoods:
                skillName.text = "正在装载物资...";
                break;
            case SkillType.UnLadeGoods:
                skillName.text = "正在卸载物资...";
                break;
            case SkillType.AirdropGoods:
                skillName.text = "正在空投物资...";
                break;
            case SkillType.Manned:
                skillName.text = "正在装载人员...";
                break;
            case SkillType.PlacementOfPersonnel:
                skillName.text = "正在安置人员...";
                break;
            case SkillType.CableDescentRescue:
                skillName.text = "正在索降救援...";
                break;
        }

        skillProgressShow.value = equipGo.skillProgress;
    }

    private void showAllMassInfo()
    {
        equipGo.GetCurrentAllMass(out float currentOil, out float totalOil, out float water, out float goods, out float person, out int personType);
        this.currentOil.value = currentOil / totalOil;
        // float aPartOil = totalOil / this.currentOil.childCount;
        // for (int i = 0; i < this.currentOil.childCount; i++)
        // {
        //     this.currentOil.GetChild(i).gameObject.SetActive(currentOil >= aPartOil * (i + 1));
        // }

        this.water.SetActive(water > 1);
        this.goods.SetActive(goods > 1);
        qPerson.SetActive(personType == 1 && person > 1);
        zPerson.SetActive(personType == 2 && person > 1);
    }

    private bool isShowWarn;
    private Tweener currentTweener;
    private void showOilWarn()
    {
        if (isShowWarn)
        {
            equipGo.GetCurrentAllMass(out float currentOil, out float totalOil, out float water, out float goods, out float person, out int personType);
            float itemOil = currentOil / totalOil;
            if (itemOil > .2f)
            {
                isShowWarn = false;
                currentTweener?.Kill();
                oilPic.color = Color.white;
            }
        }
        else
        {
            equipGo.GetCurrentAllMass(out float currentOil, out float totalOil, out float water, out float goods, out float person, out int personType);
            float itemOil = currentOil / totalOil;
            if (itemOil < .2f)
            {
                isShowWarn = true;
                currentTweener = oilPic.DOColor(Color.red, 1).SetLoops(-1, LoopType.Yoyo);
            }
        }
    }

    public override void DestroyMe()
    {
        base.DestroyMe();
        currentMoveRoute.active = false;
        Destroy(currentMoveRoute.rectTransform.gameObject);
    }
}