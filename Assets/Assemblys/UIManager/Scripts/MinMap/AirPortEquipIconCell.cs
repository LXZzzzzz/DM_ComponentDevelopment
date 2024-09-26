using System;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AirPortEquipIconCell : DMonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    private EquipBase eb;
    private Image icon;
    private Text nameText;
    private Slider progress;
    private GameObject zbObj;
    private bool isInit = false;

    private void InitView()
    {
        icon = transform.Find("icon").GetComponent<Image>();
        nameText = transform.Find("skillName").GetComponent<Text>();
        progress = transform.Find("progress").GetComponent<Slider>();
        zbObj = transform.Find("zbName").gameObject;
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
        var comData = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, equip.BeLongToCommanderId));
        icon.color = comData.MyColor;
        nameText.text = equip.name;
    }

    private void Update()
    {
        var comData = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, eb.BeLongToCommanderId));
        icon.color = comData.MyColor;

        if (eb == null || eb.currentSkill == SkillType.None)
        {
            if (progress == null)
            {
                Debug.LogError("进度条空");
                return;
            }

            if (progress.gameObject.activeSelf)
            {
                progress.gameObject.SetActive(false);
                zbObj.SetActive(false);
            }

            return;
        }

        if (!progress.gameObject.activeSelf)
        {
            progress.gameObject.SetActive(true);
            zbObj.SetActive(true);
        }

        progress.value = eb.skillProgress;
    }

    private void openRightClickView()
    {
        if (MyDataInfo.gameState != GameState.GameStart) return;
        if (eb?.currentSkill != SkillType.None) return;
        if (!string.Equals(MyDataInfo.leadId, eb.BeLongToCommanderId)) return;
#if UNITY_EDITOR

        RightClickShowInfo info1 = new RightClickShowInfo()
        {
            PointPos = GetComponent<RectTransform>().position, ShowSkillDatas =
                new List<SkillData>() { new() { isUsable = true, skillName = "测试技能", SkillType = SkillType.EndTask } },
            OnTriggerCallBack = null
        };
        UIManager.Instance.ShowPanel<UIRightClickMenuView>(UIName.UIRightClickMenuView, info1);
        return;
#endif
        RightClickShowInfo info = new RightClickShowInfo()
        {
            PointPos = GetComponent<RectTransform>().position,
            ShowSkillDatas = eb.GetSkillsData(), OnTriggerCallBack = eb.OnSelectSkill
        };
        UIManager.Instance.ShowPanel<UIRightClickMenuView>(UIName.UIRightClickMenuView, info);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            EventManager.Instance.EventTrigger(Enums.EventType.ChooseEquip.ToString(), eb.BObjectId);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        openRightClickView();
    }
}