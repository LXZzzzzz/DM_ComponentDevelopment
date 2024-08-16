using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;

public class ThreeD_ZiYuanIconCell : DMonoBehaviour
{
    private ZiYuanBase _ziYuanItem;
    private float checkTimer;
    private Transform zyTypePart;
    private Transform ChoosePart;
    private Image comPointItem;
    private Transform comsParent;
    private List<Transform> currentBelongToInfos;
    private Vector3 initialScale = Vector3.zero;

    // private GameObject chooseImg;

    //todo:后面有时间，把这个单独拆成一个Go，如果机场有飞机，就加载出来
    private GameObject airPortMarkView;
    private Transform equipParent;
    private int currAllEquipInfoCount;
    private int currentPageNum, allPageNum;
    private Button backBtn, nextBtn;

    public ZiYuanBase ziYuanItem => _ziYuanItem;


    public void Init(ZiYuanBase zy)
    {
        _ziYuanItem = zy;
        if (_ziYuanItem == null) return;
        zyTypePart = transform.Find("MainPart/zyTypePart");
        changeIcon(_ziYuanItem.ZiYuanType);
        // chooseImg = transform.Find("Choose").gameObject;
        ChoosePart = transform.Find("MainPart/belongToPart");
        transform.Find("MainPart/zyName").GetComponent<Text>().text = ziYuanItem.ziYuanName;
        for (int i = 0; i < ChoosePart.childCount; i++)
        {
            if (i == 3) continue;
            ChoosePart.GetChild(i).GetComponent<Image>().color = ziYuanItem.MyColor;
        }

        comPointItem = transform.Find("MainPart/comPointItem").GetComponent<Image>();
        comsParent = transform.Find("MainPart/BelongtoShowPart");
        currentBelongToInfos = new List<Transform>();
        initialScale = transform.localScale;
    }

    private void Start()
    {
        #region 机场相关

        airPortMarkView = transform.Find("MainPart/airPortMarkView").gameObject;
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

    private void Update()
    {
        if (Time.time > checkTimer)
        {
            checkTimer = Time.time + 1 / 25f;
            if (ChoosePart != null && ziYuanItem != null)
                ChoosePart.GetChild(1).GetComponent<Image>().color = ziYuanItem.isChooseMe ? Color.white : ziYuanItem.MyColor;
            AirPortShowLogic();
            if (ziYuanItem != null) ChangeBelongToShow(_ziYuanItem.beUsedCommanderIds);
        }

        controlView();
    }

    private void ChangeBelongToShow(List<string> coms)
    {
        if (coms == null || coms.Count == currentBelongToInfos.Count) return;
        for (int i = 0; i < coms.Count; i++)
        {
            if (currentBelongToInfos.Find(x => string.Equals(x.name, coms[i])) == null)
            {
                var itemCom = Instantiate(comPointItem, comsParent);
                itemCom.color = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, coms[i])).MyColor;
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
            comsParent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(132, 40);
        }
        else
        {
            comsParent.gameObject.SetActive(true);
            comsParent.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(142 + (currentBelongToInfos.Count - 1) * 13, 40);
        }
    }

    private void controlView()
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(ziYuanItem.transform.position + ziYuanItem.transform.up * 30);
        bool isShow = Vector3.Angle(Camera.main.transform.forward, Vector3.Normalize(ziYuanItem.transform.position - Camera.main.transform.position)) < 60;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(isShow);
        }

        Vector2 pointUGUIPos = new Vector2();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(UIManager.Instance.CurrentCanvans.transform as RectTransform, screenPoint, null, out pointUGUIPos))
            transform.GetComponent<RectTransform>().anchoredPosition = pointUGUIPos;

        if (Camera.main != null && initialScale != Vector3.zero)
        {
            float distance = Vector3.Distance(ziYuanItem.transform.position, Camera.main.transform.position);
            if (distance > 1000)
            {
                float scaleFactor = distance / 1000;
                transform.localScale = initialScale / scaleFactor;
            }
            else
            {
                transform.localScale = initialScale;
            }
        }
    }

    #region 机场相关

    private void AirPortShowLogic()
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

        backBtn.interactable = pageNum != 1;
        nextBtn.interactable = pageNum != allPageNum;
    }

    #endregion
}