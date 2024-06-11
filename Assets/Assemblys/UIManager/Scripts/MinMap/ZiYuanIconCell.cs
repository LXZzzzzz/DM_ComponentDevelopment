using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class ZiYuanIconCell : IconCellBase
{
    //为该类型的对象就等于在场景中存在对应组件，所以可以直接通过belongtoID获取组件

    private ZiYuanBase ziYuanItem;

    private void Start()
    {
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(allBObjects[i].BObject.Id, belongToId))
            {
                ziYuanItem = allBObjects[i].GetComponent<ZiYuanBase>();
                if (ziYuanItem == null) return;
                changeIcon(ziYuanItem.ZiYuanType);
                break;
            }
        }

        transform.GetChild(8).gameObject.SetActive(true);
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
            case ZiYuanType.Accident:
                index = 7;
                break;
            case ZiYuanType.Airport:
                index = 3;
                break;
            case ZiYuanType.Hospital:
                index = 2;
                break;
            case ZiYuanType.Supply:
                index = 6;
                break;
            case ZiYuanType.Waters:
                index = 1;
                break;
            case ZiYuanType.DisasterArea:
                index = 4;
                break;
            case ZiYuanType.SourceOfAFire:
                index = 5;
                break;
        }

        if (index != -1) transform.GetChild(index).gameObject.SetActive(true);
    }

    protected override IconInfoData GetBasicInfo()
    {
        IconInfoData data = new IconInfoData()
        {
            entityName = ziYuanItem.name, entityInfo = "资源资源", beUseCommanders = ziYuanItem.beUsedCommanderIds
        };
        return data;
    }
}