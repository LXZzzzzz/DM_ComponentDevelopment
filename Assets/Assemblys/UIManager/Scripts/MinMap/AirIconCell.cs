using System;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
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
    private GameObject showChooseState;
    private VectorLine currentMoveRoute;
    private List<Vector2> routePoints;
    private Image skillProgressShow;
    private Text skillName;
    private Image belongtoShow;

    public void Init(EquipBase equipGo, string belongToId, UnityAction<string, PointerEventData.InputButton> chooseCb, Func<Vector3, Vector2> worldPosMapPosFunc)
    {
        base.Init(belongToId, chooseCb);
        this.equipGo = equipGo;
        this.worldPosMapPosFunc = worldPosMapPosFunc;
        meRect = GetComponent<RectTransform>();
        rootObj = transform.Find("Root").gameObject;
        showChooseState = transform.Find("Root/Choose").gameObject;
        transform.Find("Root/airType").GetComponent<Image>().sprite = equipGo.EquipIcon;
        transform.Find("Root/equipName").GetComponent<Text>().text = equipGo.name;
        skillProgressShow = transform.Find("Root/progress").GetComponent<Image>();
        skillName = transform.Find("Root/skillName").GetComponent<Text>();
        belongtoShow = transform.Find("Root/belongTo").GetComponent<Image>();
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
        currentMoveRoute.color = Color.cyan;
    }

    private void Update()
    {
        if (equipGo != null)
        {
            meRect.GetComponent<RectTransform>().anchoredPosition = worldPosMapPosFunc(equipGo.transform.position);
            rootObj.SetActive(!equipGo.isDockingAtTheAirport);
        }

        selectChange(equipGo.isChooseMe);
        changeBelongtoShow();
        showSkillState();
    }

    protected override IconInfoData GetBasicInfo()
    {
        IconInfoData data = new IconInfoData()
        {
            entityName = equipGo.name, entityInfo = "飞机飞机",
            beUseCommanders = new List<string> { equipGo.BeLongToCommanderId }
        };

        return data;
    }

    private bool isLastSelect;

    private void selectChange(bool isSelect)
    {
        if (isSelect && routePoints != null)
        {
            routePoints[0] = meRect.anchoredPosition;
            routePoints[1] = equipGo.TargetPos == Vector3.zero ? meRect.anchoredPosition : worldPosMapPosFunc(equipGo.TargetPos);
            currentMoveRoute.Draw();
        }

        if (isLastSelect == isSelect) return;
        isLastSelect = isSelect;
        showChooseState.SetActive(isSelect);
        if (isSelect && MyDataInfo.gameState == GameState.GameStart)
        {
            currentMoveRoute.active = true;
        }
        else
        {
            currentMoveRoute.active = false;
        }
    }

    private void changeBelongtoShow()
    {
        if ((int)MyDataInfo.gameState < 2)
        {
            belongtoShow.color = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, equipGo.BeLongToCommanderId)).MyColor;
        }
    }

    private void showSkillState()
    {
        if (equipGo.currentSkill == SkillType.None)
        {
            if (skillName.gameObject.activeSelf) skillName.gameObject.SetActive(false);
            if (skillProgressShow.gameObject.activeSelf) skillProgressShow.gameObject.SetActive(false);
            return;
        }

        if (!skillName.gameObject.activeSelf) skillName.gameObject.SetActive(true);
        if (!skillProgressShow.gameObject.activeSelf) skillProgressShow.gameObject.SetActive(true);

        switch (equipGo.currentSkill)
        {
            case SkillType.WaterIntaking:
                skillName.text = "正在取水...";
                break;
        }

        skillProgressShow.fillAmount = equipGo.skillProgress;
    }

    public override void DestroyMe()
    {
        base.DestroyMe();
        currentMoveRoute.active = false;
        Destroy(currentMoveRoute.rectTransform.gameObject);
    }
}