using System;
using DM.Core.Map;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;

public class UICommanderView : BasePanel
{
    private RectTransform equipTypeParent;
    private RectTransform equipParent;
    private EquipTypeCell etcPrefab;
    private EquipCell ecPrefab;

    public override void Init()
    {
        base.Init();
        equipTypeParent = GetControl<ScrollRect>("EquipsTypes").content;
        etcPrefab = GetComponentInChildren<EquipTypeCell>(true);
        equipParent = GetControl<ScrollRect>("EquipsView").content;
        ecPrefab = GetComponentInChildren<EquipCell>(true);
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        int level = (int)userData;
        GetControl<Toggle>("tog_EquipTypeView").interactable = level == 1;
        showView();
        EventManager.Instance.AddEventListener<EquipBase>(Enums.EventType.CreatEquipCorrespondingIcon.ToString(), OnAddEquipView);
    }

    public override void HideMe()
    {
        base.HideMe();
        EventManager.Instance.RemoveEventListener<EquipBase>(Enums.EventType.CreatEquipCorrespondingIcon.ToString(), OnAddEquipView);
    }

    private void showView()
    {
        //获取场景中标识了模板的对象，展示出来
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 9) != null)
            {
                var itemObj = allBObjects[i];
                var itemCell = Instantiate(etcPrefab, equipTypeParent);
                itemCell.Init(itemObj.name, itemObj.BObject.Id, OnChooseEquipType);
                itemCell.gameObject.SetActive(true);
            }
        }
    }

    private void OnChooseEquipType(string id)
    {
        BObjectModel chooseObject = null;
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(id, allBObjects[i].BObject.Id))
            {
                chooseObject = allBObjects[i];
                break;
            }
        }

        if (chooseObject != null)
        {
            EventManager.Instance.EventTrigger<object>(Enums.EventType.SwitchCreatModel.ToString(), chooseObject.BObject.Id);
        }
    }

    private void OnAddEquipView(EquipBase equip)
    {
        var itemObj = equip;
        var itemCell = Instantiate(ecPrefab, equipParent);
        itemCell.Init(itemObj.name);
        itemCell.gameObject.SetActive(true);
    }
}