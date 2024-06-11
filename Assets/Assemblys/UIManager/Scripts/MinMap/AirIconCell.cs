using System;
using System.Collections.Generic;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Vectrosity;

public class AirIconCell : IconCellBase
{
    private EquipBase equipGo;
    private RectTransform iconShow;
    private Func<Vector3, Vector2> worldPosMapPosFunc;
    private GameObject showChooseState;
    private VectorLine currentMoveRoute;
    private List<Vector2> routePoints;
    private RectTransform meRect;

    public void Init(EquipBase equipGo, string belongToId, UnityAction<string, PointerEventData.InputButton> chooseCb, Func<Vector3, Vector2> worldPosMapPosFunc)
    {
        base.Init(belongToId, chooseCb);
        this.equipGo = equipGo;
        this.worldPosMapPosFunc = worldPosMapPosFunc;
        iconShow = GetComponent<RectTransform>();
        showChooseState = transform.GetChild(1).gameObject;
        meRect = GetComponent<RectTransform>();
        initLine();
    }

    private void initLine()
    {
        routePoints = new List<Vector2>();
        routePoints.Add(Vector2.zero);
        routePoints.Add(Vector2.zero);
        VectorLine itemLine = new VectorLine("Line" + equipGo.BObjectId, routePoints, 10, LineType.Continuous);
#if UNITY_EDITOR
        itemLine.SetCanvas(gameObject.GetComponentInParent<Canvas>());
#else
        itemLine.SetCanvas(UIManager.Instance.CurrentCanvans);
#endif
        itemLine.rectTransform.SetParent(transform);
        itemLine.rectTransform.localPosition = Vector3.zero;
        itemLine.rectTransform.localScale = Vector3.one;
        itemLine.active = true;
        itemLine.color = Color.cyan;
    }

    private void Update()
    {
        if (equipGo != null)
            iconShow.GetComponent<RectTransform>().anchoredPosition = worldPosMapPosFunc(equipGo.transform.position);

        selectChange(equipGo.isChooseMe);
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
        if (isSelect) routePoints[0] = meRect.anchoredPosition;
        if (isLastSelect == isSelect) return;
        isLastSelect = isSelect;
        showChooseState.SetActive(isSelect);
        if (isSelect)
        {
            currentMoveRoute.active = true;
            //todo:这里要在equipBase中给目标点增加一个属性
            // routePoints[1]=equipGo.
        }
        else
        {
            currentMoveRoute.active = false;
        }
    }
}