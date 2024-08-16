using UiManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRightClickMenuView : BasePanel, IPointerDownHandler
{
    private RectTransform viewPoint;
    private Transform menusParent;
    private RightMenuCell rmc;

    public override void Init()
    {
        base.Init();
        viewPoint = transform.GetChild(0).GetComponent<RectTransform>();
        menusParent = GetControl<ScrollRect>("menuListView").content;
        rmc = GetComponentInChildren<RightMenuCell>(true);
        GetComponent<Button>().onClick.AddListener(() => Close(UIName.UIRightClickMenuView));
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);

        var showInfo = (RightClickShowInfo)userData;

        viewPoint.position = showInfo.PointPos + new Vector2(12, -15);

        for (int i = 0; i < showInfo.ShowSkillDatas.Count; i++)
        {
            var itemRmc = Instantiate(rmc, menusParent);
            itemRmc.Init(showInfo.ShowSkillDatas[i], showInfo.OnTriggerCallBack);
            // itemRmc.gameObject.SetActive(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Close(UIName.UIRightClickMenuView);
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