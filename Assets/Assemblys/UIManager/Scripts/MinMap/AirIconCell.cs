using System;
using System.Collections.Generic;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AirIconCell : IconCellBase
{
    private EquipBase equipGo;
    private RectTransform iconShow;
    private Func<Vector3, Vector2> worldPosMapPosFunc;
    private GameObject showChooseState;
    private float checkTimer;

    public void Init(EquipBase equipGo, string belongToId, UnityAction<string, PointerEventData.InputButton> chooseCb, Func<Vector3, Vector2> worldPosMapPosFunc)
    {
        base.Init(belongToId, chooseCb);
        this.equipGo = equipGo;
        this.worldPosMapPosFunc = worldPosMapPosFunc;
        iconShow = GetComponent<RectTransform>();
        showChooseState = transform.GetChild(1).gameObject;
        checkTimer = Time.time;
    }

    private void Update()
    {
        if (equipGo != null)
            iconShow.GetComponent<RectTransform>().anchoredPosition = worldPosMapPosFunc(equipGo.transform.position);

        if (Time.time > checkTimer)
        {
            checkTimer = Time.time + 1/25f;
            showChooseState.SetActive(equipGo.isChooseMe);
        }
    }

    protected override IconInfoData GetBasicInfo()
    {
        IconInfoData data = new IconInfoData()
        {
            entityName = equipGo.name, entityInfo = "飞机飞机", beUseCommanders = new List<string> { equipGo.BeLongToCommanderId }
        };

        return data;
    }
}