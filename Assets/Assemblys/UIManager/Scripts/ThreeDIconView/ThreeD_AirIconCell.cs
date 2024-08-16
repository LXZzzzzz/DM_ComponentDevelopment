using System;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;

public class ThreeD_AirIconCell : DMonoBehaviour
{
    private EquipBase equipGo;
    private Slider skillProgressShow;
    private Text skillName;
    private Transform belongtoPart;
    private Image currentOil;
    private GameObject water, goods, qPerson, zPerson;
    private GameObject airPort;
    private Vector3 initialScale = Vector3.zero;

    public void Init(EquipBase equipGo)
    {
        this.equipGo = equipGo;
        transform.Find("Root/mainPart/airType").GetComponent<Image>().sprite = equipGo.EquipIcon;
        transform.Find("Root/mainPart/equipName").GetComponent<Text>().text = equipGo.name;
        skillName = transform.Find("Root/skillBg/skillName").GetComponent<Text>();
        belongtoPart = transform.Find("Root/mainPart/belongToPart");
        currentOil = transform.Find("Root/currentInfoShow/oilPart/oilShow").GetComponent<Image>();
        water = transform.Find("Root/currentInfoShow/waterPart").gameObject;
        goods = transform.Find("Root/currentInfoShow/goodsPart").gameObject;
        qPerson = transform.Find("Root/currentInfoShow/qPersonPart").gameObject;
        zPerson = transform.Find("Root/currentInfoShow/zPersonPart").gameObject;

        var comData = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, equipGo.BeLongToCommanderId));
        skillProgressShow = transform.Find($"Root/progressPart/{comData.progressId}").GetComponent<Slider>();
        initialScale = transform.localScale;
    }

    private void Update()
    {
        if (equipGo == null) return;

        selectChange(equipGo.isChooseMe);
        changeBelongtoShow();
        showSkillState();
        showAllMassInfo();
    }

    private void LateUpdate()
    {
        controlView();
    }

    private bool isLastSelect;

    private void selectChange(bool isSelect)
    {
        if (isLastSelect == isSelect) return;
        isLastSelect = isSelect;
        var comData = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, equipGo.BeLongToCommanderId));
        belongtoPart.GetChild(0).GetComponent<Image>().color = isSelect ? comData.ChooseColor : comData.MyColor;
        belongtoPart.GetChild(1).GetComponent<Image>().color = isSelect ? Color.white : comData.MyColor;
    }

    private string belongtoCom;

    private void changeBelongtoShow()
    {
        if ((int)MyDataInfo.gameState < 2)
        {
            if (equipGo.BeLongToCommanderId != belongtoCom)
            {
                belongtoCom = equipGo.BeLongToCommanderId;
                var comData = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, equipGo.BeLongToCommanderId));
                belongtoPart.GetChild(0).GetComponent<Image>().color = comData.MyColor;
                belongtoPart.GetChild(1).GetComponent<Image>().color = comData.MyColor;
                belongtoPart.GetChild(2).GetComponent<Image>().color = comData.IconBgColor;
                belongtoPart.GetChild(3).GetComponent<Image>().color = comData.IconBgColor;
                skillProgressShow = transform.Find($"Root/progressPart/{comData.progressId}").GetComponent<Slider>();
            }
        }
    }

    private void showSkillState()
    {
        if (equipGo.currentSkill == SkillType.None)
        {
            if (skillName.transform.parent.gameObject.activeSelf) skillName.transform.parent.gameObject.SetActive(false);
            if (skillProgressShow != null && skillProgressShow.gameObject.activeSelf) skillProgressShow.gameObject.SetActive(false);
            return;
        }

        if (!skillName.transform.parent.gameObject.activeSelf) skillName.transform.parent.gameObject.SetActive(true);
        if (skillProgressShow != null && !skillProgressShow.gameObject.activeSelf) skillProgressShow.gameObject.SetActive(true);

        switch (equipGo.currentSkill)
        {
            case SkillType.GroundReady:
                skillName.text = "正在起飞前准备...";
                break;
            case SkillType.BePutInStorage:
                skillName.text = "正在入库...";
                break;
            case SkillType.TakeOff:
                skillName.text = "正在起飞...";
                break;
            case SkillType.Landing:
                skillName.text = "正在降落...";
                break;
            case SkillType.Supply:
                skillName.text = "正在补给...";
                break;
            case SkillType.WaterIntaking:
                skillName.text = "正在取水...";
                break;
            case SkillType.WaterPour:
                skillName.text = "正在投水...";
                break;
            case SkillType.LadeGoods:
                skillName.text = "正在装载物资...";
                break;
            case SkillType.UnLadeGoods:
                skillName.text = "正在卸载物资...";
                break;
            case SkillType.AirdropGoods:
                skillName.text = "正在空投物资...";
                break;
            case SkillType.Manned:
                skillName.text = "正在装载人员...";
                break;
            case SkillType.PlacementOfPersonnel:
                skillName.text = "正在安置人员...";
                break;
            case SkillType.CableDescentRescue:
                skillName.text = "正在索降救援...";
                break;
        }

        skillProgressShow.value = equipGo.skillProgress;
    }

    private void showAllMassInfo()
    {
        equipGo.GetCurrentAllMass(out float currentOil, out float totalOil, out float water, out float goods, out float person, out int personType);
        this.currentOil.fillAmount = currentOil / totalOil;
        // float aPartOil = totalOil / this.currentOil.childCount;
        // for (int i = 0; i < this.currentOil.childCount; i++)
        // {
        //     this.currentOil.GetChild(i).gameObject.SetActive(currentOil >= aPartOil * (i + 1));
        // }

        this.water.SetActive(water > 1);
        this.goods.SetActive(goods > 1);
        qPerson.SetActive(personType == 1 && person > 1);
        zPerson.SetActive(personType == 2 && person > 1);
    }

    private void controlView()
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(equipGo.transform.position + equipGo.transform.up * 3);
        bool isShow = Vector3.Angle(Camera.main.transform.forward, Vector3.Normalize(equipGo.transform.position - Camera.main.transform.position)) < 60;
        transform.GetChild(0).gameObject.SetActive(isShow);

        Vector2 pointUGUIPos = new Vector2();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(UIManager.Instance.CurrentCanvans.transform as RectTransform, screenPoint, null, out pointUGUIPos))
            transform.GetComponent<RectTransform>().anchoredPosition = pointUGUIPos;

        if (Camera.main != null && initialScale != Vector3.zero)
        {
            float distance = Vector3.Distance(equipGo.transform.position, Camera.main.transform.position);
            if (distance > 500)
            {
                float scaleFactor = distance / 500;
                transform.localScale = initialScale / scaleFactor;
            }
            else
            {
                transform.localScale = initialScale;
            }
        }
    }
}