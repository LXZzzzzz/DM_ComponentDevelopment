using ToolsLibrary;
using UiManager;
using UnityEngine;
using UnityEngine.UI;

public class UIAirportAircraftShowView : BasePanel
{
    private RectTransform viewPoint;
    private Transform menusParent;
    private AirportAircraftCell aacell;

    public override void Init()
    {
        base.Init();
        viewPoint = transform.GetChild(0).GetComponent<RectTransform>();
        menusParent = GetControl<ScrollRect>("menuListView").content;
        aacell = GetComponentInChildren<AirportAircraftCell>(true);
        GetComponent<Button>().onClick.AddListener(() => Close(UIName.UIAirportAircraftShowView));
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        AirportAircraftsInfo info = (AirportAircraftsInfo)userData;

        viewPoint.anchoredPosition = info.PointPos;
        //将所有飞机列表展示出来
        for (int i = 0; i < info.AircraftDatas.Count; i++)
        {
            var itemaa = Instantiate(aacell, menusParent);
            var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, info.AircraftDatas[i]));
            itemaa.gameObject.SetActive(true);
            itemaa.Init(info.AircraftDatas[i], itemEquip.name, string.Equals(itemEquip.BeLongToCommanderId, MyDataInfo.leadId), info.OnTakeOffCallBack);
        }
    }

    public override void HideMe()
    {
        base.HideMe();
        for (int i = 0; i < menusParent.childCount; i++)
        {
            Destroy(menusParent.GetChild(i).gameObject);
        }
    }
}