using System;
using DataTranfsers;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 装备信息界面选项
/// </summary>
public class ChangeData_cellEquipInfoItem : DMonoBehaviour
{
    public Toggle toggle;

    public Text text_name;

    public InputField InputField_pos, InputField_cycle, InputField_time;

    public Dropdown dp_state;

    public Dropdown jz, bz;

    private EquipBase _equip;

    public string GetUsedEquipId => toggle.isOn ? _equip.BObjectId : string.Empty;
    public string GetId => _equip.BObjectId;
    public bool GetIsUse => toggle.isOn;

    public void Init(EquipBase data)
    {
        _equip = data;
        jz.gameObject.SetActive(MyDataInfo.MyLevel == 1);
        bz.gameObject.SetActive(MyDataInfo.MyLevel == 1);
        toggle.gameObject.SetActive(MyDataInfo.MyLevel == 1);
        InputField_pos.interactable = MyDataInfo.MyLevel == -1;
        dp_state.interactable = MyDataInfo.MyLevel == -1;
        InputField_cycle.interactable = MyDataInfo.MyLevel == -1;
        InputField_time.interactable = MyDataInfo.MyLevel == -1;
        text_name.text = _equip.name;
        if (string.IsNullOrEmpty(_equip.textInfo)) return;
        groupInit();
        var strs = _equip.textInfo.Split('_');
        InputField_pos.text = strs[1];
        dp_state.value = int.Parse(strs[2]);
        InputField_cycle.text = strs[3];
        InputField_time.text = strs[4];
        jz.value = int.Parse(strs[5]);
        bz.value = int.Parse(strs[6]);

        jz.interactable = (MyDataInfo.MyLevel == 1 && toggle.isOn);
        bz.interactable = (MyDataInfo.MyLevel == 1 && toggle.isOn);
    }

    public void ShowInfo(zbcellInfo zi)
    {
        if (zi == null) return;

        groupInit();
        jz.gameObject.SetActive(true);
        bz.gameObject.SetActive(true);
        toggle.gameObject.SetActive(true);
        InputField_pos.interactable = false;
        dp_state.interactable = false;
        InputField_cycle.interactable = false;
        InputField_time.interactable = false;
        jz.interactable = false;
        bz.interactable = false;
        toggle.isOn = zi.isUse;
        jz.value = zi.chooseJz;
        bz.value = zi.chooseBzz;
    }

    private void groupInit()
    {
        jz.options.Clear();
        for (int i = 0; i < MyDataInfo.BeUsedJizus?.Count; i++)
        {
            jz.options.Add(new Dropdown.OptionData(MyDataInfo.BeUsedJizus[i]));
        }

        bz.options.Clear();
        for (int i = 0; i < MyDataInfo.BeUsedBaozhangs?.Count; i++)
        {
            bz.options.Add(new Dropdown.OptionData(MyDataInfo.BeUsedBaozhangs[i]));
        }
    }

    public string getData()
    {
        Debug.LogError(_equip.BObjectId);
        Debug.LogError(text_name.name);
        Debug.LogError(InputField_pos.name);
        return _equip.BObjectId + '_' + InputField_pos.text + '_' + dp_state.value + '_' +
               InputField_cycle.text + '_' + InputField_time.text + '_' + jz.value.ToString() + '_' + bz.value.ToString();
    }
}