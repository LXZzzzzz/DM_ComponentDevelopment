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
    private Transform zyTypePart;
    private Transform ChoosePart;

    // private GameObject chooseImg;

    //todo:后面有时间，把这个单独拆成一个Go，如果机场有飞机，就加载出来
    private GameObject airPortMarkView;
    private Transform equipParent;
    private int currAllEquipInfoCount;
    private int currentPageNum, allPageNum;
    private Button backBtn, nextBtn;

    public ZiYuanBase ziYuanItem => _ziYuanItem;

    private void Start()
    {
        zyTypePart = transform.Find("Root/MainPart/zyTypePart");
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

        if (ziYuanItem != null)
        {
            // chooseImg = transform.Find("Choose").gameObject;
            ChoosePart = transform.Find("Root/MainPart/belongToPart");
            transform.Find("Root/MainPart/zyName").GetComponent<Text>().text = ziYuanItem.ziYuanName;
            for (int i = 0; i < ChoosePart.childCount; i++)
            {
                ChoosePart.GetChild(i).GetComponent<Image>().color = ziYuanItem.MyColor;
            }
        }

        #region 机场相关

        airPortMarkView = transform.Find("airPortMarkView").gameObject;
        equipParent = airPortMarkView.transform.Find("equipsListView");
        backBtn = airPortMarkView.transform.Find("back").GetComponent<Button>();
        nextBtn = airPortMarkView.transform.Find("next").GetComponent<Button>();
        backBtn.onClick.AddListener(() => pageTurning(false));
        nextBtn.onClick.AddListener(() => pageTurning(true));
        allPageNum = currentPageNum = 1;

        airPortMarkView.SetActive(false);
        // airPortMarkView.transform.SetParent(transform.parent);

        #endregion
    }

    private void changeIcon(ZiYuanType type)
    {
        for (int i = 0; i < zyTypePart.childCount; i++)
        {
            zyTypePart.GetChild(i).gameObject.SetActive(false);
        }

        int index = -1;
        switch (type)
        {
            case ZiYuanType.Supply:
                index = 7;
                break;
            case ZiYuanType.RescueStation:
                index = 6;
                break;
            case ZiYuanType.Airport:
                index = 2;
                break;
            case ZiYuanType.Hospital:
                index = 1;
                break;
            case ZiYuanType.GoodsPoint:
                index = 5;
                break;
            case ZiYuanType.Waters:
                index = 0;
                break;
            case ZiYuanType.DisasterArea:
                index = 3;
                break;
            case ZiYuanType.SourceOfAFire:
                index = 4;
                break;
        }

        if (index != -1) zyTypePart.GetChild(index).gameObject.SetActive(true);
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
            if (ChoosePart != null && ziYuanItem != null)
                ChoosePart.GetChild(1).GetComponent<Image>().color = ziYuanItem.isChooseMe ? Color.white : ziYuanItem.MyColor;
            airPortShowLogic();
        }
    }

    #region 机场相关

    private void airPortShowLogic()
    {
        if (ziYuanItem?.ZiYuanType != ZiYuanType.Airport) return;
        if (MyDataInfo.MyLevel != 1 &&
            (ziYuanItem.beUsedCommanderIds == null || ziYuanItem.beUsedCommanderIds.Find(x => string.Equals(x, MyDataInfo.leadId)) == null)) return;

        var itemInfo = (ziYuanItem as IAirPort)?.GetAllEquips();
        bool isRefresh = currAllEquipInfoCount != itemInfo.Count;
        if (isRefresh)
        {
            currAllEquipInfoCount = itemInfo.Count;
            currentPageNum = 1;
            allPageNum = currAllEquipInfoCount / equipParent.childCount + (currAllEquipInfoCount % equipParent.childCount == 0 ? 0 : 1);
            Debug.LogError($"{currAllEquipInfoCount}+{equipParent.childCount}+{(currAllEquipInfoCount / equipParent.childCount)}+{(currAllEquipInfoCount % equipParent.childCount == 0 ? 0 : 1)}");
            refreshCurrentPageInfo(1);
        }

        airPortMarkView.SetActive(itemInfo.Count != 0);
    }

    private void pageTurning(bool isNext)
    {
        if (isNext)
        {
            if (currentPageNum < allPageNum) refreshCurrentPageInfo(++currentPageNum);
        }
        else
        {
            if (currentPageNum > 1) refreshCurrentPageInfo(--currentPageNum);
        }
    }

    private void refreshCurrentPageInfo(int pageNum)
    {
        var currentAirportEquips = MyDataInfo.sceneAllEquips.FindAll(x => x.isDockingAtTheAirport);
        for (int i = 0; i < equipParent.childCount; i++)
        {
            int currentIndex = equipParent.childCount * (pageNum - 1) + i;
            if (currentIndex < currentAirportEquips.Count)
            {
                equipParent.GetChild(i).GetComponent<AirPortEquipIconCell>().Init(currentAirportEquips[currentIndex]);
                equipParent.GetChild(i).gameObject.SetActive(true);
            }
            else equipParent.GetChild(i).gameObject.SetActive(false);
        }

        Debug.LogError($"当前页数{pageNum}总页数{allPageNum}");
        backBtn.interactable = pageNum != 1;
        nextBtn.interactable = pageNum != allPageNum;
    }

    #endregion
}