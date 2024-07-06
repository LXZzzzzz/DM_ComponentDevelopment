using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.UI;

public class ZiYuanIconCell : IconCellBase
{
    //为该类型的对象就等于在场景中存在对应组件，所以可以直接通过belongtoID获取组件

    private ZiYuanBase _ziYuanItem;
    private float checkTimer;

    private GameObject chooseImg;

    //todo:后面有时间，把这个单独拆成一个Go，如果机场有飞机，就加载出来
    private GameObject airPortMarkView;
    private RectTransform equipParent;
    private AirPortEquipIconCell aec;
    private int currAllEquipInfoCount;

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

        chooseImg = transform.Find("Choose").gameObject;
        airPortMarkView = transform.Find("airPortMarkView").gameObject;
        equipParent = airPortMarkView.GetComponentInChildren<ScrollRect>(true).content;
        aec = airPortMarkView.GetComponentInChildren<AirPortEquipIconCell>(true);
        airPortMarkView.SetActive(false);
        airPortMarkView.transform.SetParent(transform.parent);
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
                index = 8;
                break;
            case ZiYuanType.RescueStation:
                index = 7;
                break;
            case ZiYuanType.Airport:
                index = 3;
                break;
            case ZiYuanType.Hospital:
                index = 2;
                break;
            case ZiYuanType.GoodsPoint:
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
            entityName = _ziYuanItem.name, entityInfo = _ziYuanItem.ziYuanDescribe, beUseCommanders = _ziYuanItem.beUsedCommanderIds
        };
        return data;
    }

    private void Update()
    {
        if (Time.time > checkTimer)
        {
            checkTimer = Time.time + 1 / 25f;
            chooseImg.SetActive(_ziYuanItem.isChooseMe);
            airPortShowLogic();
        }
    }


    private void airPortShowLogic()
    {
        if (ziYuanItem.ZiYuanType != ZiYuanType.Airport) return;
        if (MyDataInfo.MyLevel != 1 && 
            (ziYuanItem.beUsedCommanderIds == null || ziYuanItem.beUsedCommanderIds.Find(x => string.Equals(x, MyDataInfo.leadId)) == null)) return;

        var itemInfo = (ziYuanItem as IAirPort)?.GetAllEquips();
        bool isRefresh = currAllEquipInfoCount != itemInfo.Count;
        if (isRefresh)
        {
            Debug.LogError("刷新" + currAllEquipInfoCount + ":" + itemInfo.Count);
            currAllEquipInfoCount = itemInfo.Count;
            for (int i = 0; i < equipParent.childCount; i++)
            {
                Destroy(equipParent.GetChild(i).gameObject);
            }

            for (int i = 0; i < itemInfo.Count; i++)
            {
                var itemCell = Instantiate(aec, equipParent);
                var itemIcon = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, itemInfo[i])).EquipIcon;
                itemCell.Init(itemInfo[i], itemIcon);
                itemCell.gameObject.SetActive(true);
            }
        }

        airPortMarkView.SetActive(itemInfo.Count != 0);
    }
}