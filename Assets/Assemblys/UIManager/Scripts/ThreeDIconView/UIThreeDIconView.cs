using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using Enums;
using EventType = Enums.EventType;

public class UIThreeDIconView : BasePanel
{
    private Transform parent;
    private ThreeDIconCell tdic;
    private Dictionary<string,ThreeDIconCell> allIconCells;

    public override void Init()
    {
        base.Init();
        EventManager.Instance.AddEventListener<EquipBase>(EventType.CreatEquipCorrespondingIcon.ToString(), creatAirCell);
        EventManager.Instance.AddEventListener<string>(Enums.EventType.DestoryEquip.ToString(), desAir);
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        parent = transform.Find("IconObjParent");
        tdic = transform.Find("IconPrefabs/ThreeDIconCell").GetComponent<ThreeDIconCell>();
        allIconCells = new Dictionary<string, ThreeDIconCell>();
    }

    private void creatAirCell(EquipBase equip)
    {
        var itemIcon = Instantiate(tdic, parent);
        itemIcon.Init(equip);
        allIconCells.Add(equip.BObjectId,itemIcon);
    }

    private void desAir(string id)
    {
        foreach (var iconCell in allIconCells)
        {
            if (string.Equals(iconCell.Key, id))
            {
                Destroy(iconCell.Value.gameObject);
                allIconCells.Remove(id);
                break;
            }
        }
    }

    public override void HideMe()
    {
        base.HideMe();
        EventManager.Instance.RemoveEventListener<EquipBase>(EventType.CreatEquipCorrespondingIcon.ToString(), creatAirCell);
        EventManager.Instance.RemoveEventListener<string>(Enums.EventType.DestoryEquip.ToString(), desAir);
    }
}