using System;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.UI;

public class ChangeData_cellEquipChooseItem : DMonoBehaviour
{
    public Toggle tog;

    private string id;

    public void Init(EquipBase equip)
    {
        id = equip.BObjectId;
        tog.GetComponentInChildren<Text>(true).text = equip.name;
    }

    public string GetRunEquip()
    {
        return tog.isOn ? id : String.Empty;
    }
}