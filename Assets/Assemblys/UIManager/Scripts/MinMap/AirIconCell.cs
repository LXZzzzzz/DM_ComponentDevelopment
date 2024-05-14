using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary.PathPart;
using UnityEngine;
using UnityEngine.Events;

public class AirIconCell : IconCellBase
{
    private EquipBase equipGo;
    private RectTransform iconShow;
    private Func<Vector3, Vector2> worldPosMapPosFunc;
    public void Init(EquipBase equipGo,string belongToId, UnityAction<string> chooseCb, Func<Vector3, Vector2> worldPosMapPosFunc)
    {
        base.Init(belongToId, chooseCb);
        this.equipGo = equipGo;
        this.worldPosMapPosFunc = worldPosMapPosFunc;
        iconShow = GetComponent<RectTransform>();
    }

    private void Update()
    {
        iconShow.GetComponent<RectTransform>().anchoredPosition = worldPosMapPosFunc(equipGo.transform.position);
    }
}
