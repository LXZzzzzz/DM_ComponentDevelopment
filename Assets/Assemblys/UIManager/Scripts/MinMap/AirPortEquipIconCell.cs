using System;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AirPortEquipIconCell : DMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private EquipBase eb;
    private Image icon;
    private GameObject namePart;
    private Slider progress;
    private bool isInit=false;

    private void InitView()
    {
        icon = transform.Find("icon").GetComponent<Image>();
        namePart = transform.Find("NamePart").gameObject;
        progress = transform.Find("progress").GetComponent<Slider>();
        namePart.GetComponentInChildren<Text>().text = "";
        transform.GetComponentInChildren<Button>().onClick.AddListener(openRightClickView);
        isInit = true;
    }

    private void Start()
    {
#if UNITY_EDITOR
        InitView();
#endif
    }

    public void Init(EquipBase equip)
    {
        if (!isInit) InitView();
        eb = equip;
        icon.sprite = equip.EquipIcon;
        namePart.GetComponentInChildren<Text>().text = equip.name;
    }

    private void Update()
    {
        if (eb == null || eb.currentSkill == SkillType.None)
        {
            if (progress.gameObject.activeSelf) progress.gameObject.SetActive(false);
            return;
        }

        if (!progress.gameObject.activeSelf) progress.gameObject.SetActive(true);
        progress.value = eb.skillProgress;
    }

    private void openRightClickView()
    {
        Vector2 iconPos = transform.GetComponentInParent<IconCellBase>().GetComponent<RectTransform>().anchoredPosition;
        Vector2 apViewPos = transform.GetComponentInParent<IconCellBase>().transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition;
        Vector2 parentPos = transform.parent.GetComponent<RectTransform>().anchoredPosition;
#if UNITY_EDITOR

        RightClickShowInfo info1 = new RightClickShowInfo()
        {
            PointPos = iconPos + apViewPos + parentPos + GetComponent<RectTransform>().anchoredPosition, ShowSkillDatas =
                new List<SkillData>() { new() { isUsable = true, skillName = "测试技能", SkillType = SkillType.EndTask } },
            OnTriggerCallBack = null
        };
        UIManager.Instance.ShowPanel<UIRightClickMenuView>(UIName.UIRightClickMenuView, info1);
        return;
#endif
        EventManager.Instance.EventTrigger(Enums.EventType.ChooseEquip.ToString(), eb.BObjectId);
        RightClickShowInfo info = new RightClickShowInfo()
        {
            PointPos = iconPos + apViewPos + parentPos + GetComponent<RectTransform>().anchoredPosition,
            ShowSkillDatas = eb.GetSkillsData(), OnTriggerCallBack = eb.OnSelectSkill
        };
        UIManager.Instance.ShowPanel<UIRightClickMenuView>(UIName.UIRightClickMenuView, info);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        namePart.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        namePart.gameObject.SetActive(false);
    }
}