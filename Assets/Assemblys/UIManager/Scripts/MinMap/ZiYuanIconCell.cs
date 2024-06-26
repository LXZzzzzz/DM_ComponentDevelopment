using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class ZiYuanIconCell : IconCellBase
{
    //为该类型的对象就等于在场景中存在对应组件，所以可以直接通过belongtoID获取组件

    private ZiYuanBase _ziYuanItem;
    private float checkTimer;
    private GameObject chooseImg;

    public ZiYuanBase ziYuanItem => _ziYuanItem;

    private void Start()
    {
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(allBObjects[i].BObject.Id, belongToId))
            {
                _ziYuanItem = allBObjects[i].GetComponent<ZiYuanBase>();
                if (_ziYuanItem == null) return;
                changeIcon(_ziYuanItem.ZiYuanType);
                break;
            }
        }

        transform.GetChild(1).gameObject.SetActive(true);
        chooseImg = transform.Find("Choose").gameObject;
    }

    private void changeIcon(ZiYuanType type)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        int index = -1;
        switch (type)
        {
            case ZiYuanType.Supply:
                index = 9;
                break;
            case ZiYuanType.RescueStation:
                index = 8;
                break;
            case ZiYuanType.Airport:
                index = 4;
                break;
            case ZiYuanType.Hospital:
                index = 3;
                break;
            case ZiYuanType.GoodsPoint:
                index = 7;
                break;
            case ZiYuanType.Waters:
                index = 2;
                break;
            case ZiYuanType.DisasterArea:
                index = 5;
                break;
            case ZiYuanType.SourceOfAFire:
                index = 6;
                break;
        }

        if (index != -1) transform.GetChild(index).gameObject.SetActive(true);
    }

    protected override IconInfoData GetBasicInfo()
    {
        IconInfoData data = new IconInfoData()
        {
            entityName = _ziYuanItem.name, entityInfo = "资源资源", beUseCommanders = _ziYuanItem.beUsedCommanderIds
        };
        return data;
    }
    private void Update()
    {
        if (Time.time > checkTimer)
        {
            checkTimer = Time.time + 1 / 25f;
            chooseImg.SetActive(_ziYuanItem.isChooseMe);
        }
    }
}