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
    private Transform comPointItem;
    private Transform comsParent;
    private RectTransform comsParentRect;
    private List<Transform> currentBelongToInfos;
    private Transform tipShowPart;

    // private GameObject chooseImg;

    //todo:后面有时间，把这个单独拆成一个Go，如果机场有飞机，就加载出来
    private GameObject airPortMarkView;
    private AirPortEquipIconCell aec;
    private Transform equipParent;
    private int currAllEquipInfoCount;
    private int currentPageNum, allPageNum;
    private Button backBtn, nextBtn;

    public ZiYuanBase ziYuanItem => _ziYuanItem;

    private void Start()
    {
        zyTypePart = transform.Find("MainPart/zyTypePart");
        tipShowPart = transform.Find("MainPart/TipShowPart");
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
            ChoosePart = transform.Find("MainPart/belongToPart");
            transform.Find("MainPart/zyName").GetComponent<Text>().text = ziYuanItem.ziYuanName;
            for (int i = 0; i < ChoosePart.childCount; i++)
            {
                if (i == 1 || i == 3) continue;
                ChoosePart.GetChild(i).GetComponent<Image>().color = ziYuanItem.MyColor;
            }

            comPointItem = transform.Find("MainPart/comPointItem");
            comsParent = transform.Find("MainPart/BelongtoShowPart");
            comsParentRect = comsParent.GetComponent<RectTransform>();
            currentBelongToInfos = new List<Transform>();
        }

        #region 机场相关

        airPortMarkView = transform.Find("MainPart/airPortMarkView").gameObject;
        aec = airPortMarkView.GetComponentInChildren<AirPortEquipIconCell>(true);
        equipParent = airPortMarkView.transform.Find("equipsParent");
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

        switch (type)
        {
            case ZiYuanType.SourceOfAFire:
                tipShowPart.gameObject.SetActive(true);
                tipShowPart.GetChild(1).gameObject.SetActive(true);
                tipShowPart.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(135, 34);
                break;
            case ZiYuanType.RescueStation:
                tipShowPart.gameObject.SetActive(true);
                tipShowPart.GetChild(5).gameObject.SetActive(true);
                tipShowPart.GetChild(10).gameObject.SetActive(true);
                tipShowPart.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(135 + 21.5f, 34);
                break;
            case ZiYuanType.Hospital:
                tipShowPart.gameObject.SetActive(true);
                tipShowPart.GetChild(4).gameObject.SetActive(true);
                tipShowPart.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(135, 34);
                break;
            case ZiYuanType.DisasterArea:
                tipShowPart.gameObject.SetActive(true);
                tipShowPart.GetChild((_ziYuanItem as IDisasterArea).getWoundedPersonnelType() == 1 ? 6 : 7).gameObject.SetActive(true);
                tipShowPart.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(135, 34);
                break;
            case ZiYuanType.Waters:
                tipShowPart.gameObject.SetActive(true);
                tipShowPart.GetChild(2).gameObject.SetActive(true);
                tipShowPart.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(135, 34);
                break;
            case ZiYuanType.Airport:
                tipShowPart.gameObject.SetActive(true);
                tipShowPart.GetChild(3).gameObject.SetActive(true);
                tipShowPart.GetChild(11).gameObject.SetActive(true);
                tipShowPart.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(135 + 21.5f, 34);
                break;
            case ZiYuanType.Supply:
                tipShowPart.gameObject.SetActive(true);
                tipShowPart.GetChild(8).gameObject.SetActive(true);
                tipShowPart.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(135, 34);
                break;
            case ZiYuanType.GoodsPoint:
                tipShowPart.gameObject.SetActive(true);
                tipShowPart.GetChild(9).gameObject.SetActive(true);
                tipShowPart.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(135, 34);
                break;
            default:
                tipShowPart.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(110, 40);
                break;
        }
    }

    protected override IconInfoData GetBasicInfo()
    {
        string progressInfo = String.Empty;
        switch (_ziYuanItem.ZiYuanType)
        {
            case ZiYuanType.SourceOfAFire:
                (_ziYuanItem as ISourceOfAFire).getFireData(out float ghmj, out float rsmj, out float csghmj, out float csrsmj, out float tszl);
                progressInfo = $"该火源点需求水量为：{(int)(rsmj < 0 ? 0 : rsmj)}kg";
                break;
            case ZiYuanType.RescueStation:
                (_ziYuanItem as IRescueStation).getTaskProgress(out int currentPersonNum, out int maxPersonNum, out float currentGoodsNum, out float maxGoodsNum);
                progressInfo = $"安置点当前安置轻伤员:{currentPersonNum}人，总共可安置：{maxPersonNum}人\n当前已有物资:{currentGoodsNum}kg，总共需求物资：{maxGoodsNum}kg";
                break;
            case ZiYuanType.Hospital:
                (_ziYuanItem as IRescueStation).getTaskProgress(out int currentPersonNum1, out int maxPersonNum1, out float currentGoodsNum1, out float maxGoodsNum1);
                progressInfo = $"医院当前救治重伤员:{currentPersonNum1}人";
                break;
            case ZiYuanType.DisasterArea:
                (_ziYuanItem as IDisasterArea).getTaskProgress(out int currentNum, out int maxNum);
                string personType = (_ziYuanItem as IDisasterArea).getWoundedPersonnelType() == 1 ? "轻伤员" : "重伤员";
                progressInfo = $"{personType}灾区还剩余需转运人数:{currentNum}人，总共受灾人数：{maxNum}人";
                break;
            default:
                progressInfo = _ziYuanItem.ziYuanDescribe;
                break;
        }

        IconInfoData data = new IconInfoData()
        {
            entityName = _ziYuanItem.name, entityInfo = progressInfo, beUseCommanders = _ziYuanItem.beUsedCommanderIds
        };
        return data;
    }

    private void Update()
    {
        if (Time.time > checkTimer)
        {
            checkTimer = Time.time + 1 / 25f;
            if (ChoosePart != null && ziYuanItem != null)
            {
                ChoosePart.GetChild(0).GetComponent<Image>().color = ziYuanItem.isChooseMe ? ziYuanItem.ChooseColor : ziYuanItem.MyColor;
                if (ColorUtility.TryParseHtmlString("#D7D7D7", out Color color))
                {
                    ChoosePart.GetChild(1).GetComponent<Image>().color = ziYuanItem.isChooseMe ? Color.white : color;
                }
            }

            AirPortShowLogic();
            if (ziYuanItem != null) ChangeBelongToShow(_ziYuanItem.beUsedCommanderIds);
        }
    }

    private void ChangeBelongToShow(List<string> coms)
    {
        if (coms == null || coms.Count == currentBelongToInfos.Count) return;
        for (int i = 0; i < coms.Count; i++)
        {
            if (currentBelongToInfos.Find(x => string.Equals(x.name, coms[i])) == null)
            {
                var itemCom = Instantiate(comPointItem, comsParent);
                itemCom.GetChild(0).GetComponent<Image>().color = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, coms[i])).MyColor;
                itemCom.name = coms[i];
                itemCom.gameObject.SetActive(true);
                currentBelongToInfos.Add(itemCom.transform);
            }
        }

        for (int i = 0; i < currentBelongToInfos.Count; i++)
        {
            if (coms.Find(x => string.Equals(x, currentBelongToInfos[i].name)) == null)
            {
                Destroy(currentBelongToInfos[i].gameObject);
                currentBelongToInfos.Remove(currentBelongToInfos[i]);
                i--;
            }
        }

        if (currentBelongToInfos.Count == 0)
        {
            comsParent.gameObject.SetActive(false);
            // comsParent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(110, 40);
        }
        else
        {
            comsParent.gameObject.SetActive(true);
            // comsParent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(135 + (currentBelongToInfos.Count - 1) * 21.5f, 34);
        }
    }

    #region 机场相关

    private bool isUsePageTurning = false;

    private void AirPortShowLogic()
    {
        if (ziYuanItem?.ZiYuanType != ZiYuanType.Airport) return;
        if (MyDataInfo.MyLevel != 1 &&
            (ziYuanItem.beUsedCommanderIds == null || ziYuanItem.beUsedCommanderIds.Find(x => string.Equals(x, MyDataInfo.leadId)) == null)) return;

        var itemInfo = (ziYuanItem as IAirPort)?.GetAllEquips();
        bool isRefresh = currAllEquipInfoCount != itemInfo.Count;
        if (isRefresh)
        {
            Debug.LogError("刷新了" + itemInfo.Count);
            currAllEquipInfoCount = itemInfo.Count;
            if (isUsePageTurning)
            {
                //翻页逻辑
                currentPageNum = 1;
                allPageNum = currAllEquipInfoCount / equipParent.childCount + (currAllEquipInfoCount % equipParent.childCount == 0 ? 0 : 1);
                refreshCurrentPageInfo(1);
            }
            else
            {
                //排列逻辑
                for (int i = 0; i < equipParent.childCount; i++)
                {
                    Destroy(equipParent.GetChild(i).gameObject);
                }

                for (int i = 0; i < itemInfo.Count; i++)
                {
                    var itemCell = Instantiate(aec, equipParent);
                    var item = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, itemInfo[i]));
                    itemCell.Init(item);
                    itemCell.gameObject.SetActive(true);
                }
            }
        }

        airPortMarkView.SetActive(itemInfo.Count != 0);
        comsParentRect.anchoredPosition = new Vector2(0, itemInfo.Count != 0 ? -26 : 26);
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

        backBtn.interactable = pageNum != 1;
        nextBtn.interactable = pageNum != allPageNum;
    }

    #endregion
}