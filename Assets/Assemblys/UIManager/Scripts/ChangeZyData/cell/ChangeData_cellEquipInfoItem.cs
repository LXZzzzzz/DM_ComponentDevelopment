using System;
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

    public InputField InputField_pos, InputField_cycle, InputField_time;

    public Dropdown dp_state;

    public Dropdown jz, bz;

    private EquipBase _equip;

    public void Init(EquipBase data)
    {
        _equip = data;
        jz.interactable = (MyDataInfo.MyLevel == 1);
        bz.interactable = (MyDataInfo.MyLevel == 1);
        toggle.interactable = MyDataInfo.MyLevel == -1;
        InputField_pos.interactable = MyDataInfo.MyLevel == -1;
        dp_state.interactable = MyDataInfo.MyLevel == -1;
        InputField_cycle.interactable = MyDataInfo.MyLevel == -1;
        InputField_time.interactable = MyDataInfo.MyLevel == -1;
        toggle.GetComponentInChildren<Text>().text = _equip.name;
        if (string.IsNullOrEmpty(_equip.textInfo)) return;
        groupInit();
        var strs = _equip.textInfo.Split('_');
        toggle.isOn = int.Parse(strs[1]) == 1;
        InputField_pos.text = strs[2];
        dp_state.value = int.Parse(strs[3]);
        InputField_cycle.text = strs[4];
        InputField_time.text = strs[5];
        jz.value = int.Parse(strs[6]);
        bz.value = int.Parse(strs[7]);

        jz.interactable = (MyDataInfo.MyLevel == 1 && toggle.isOn);
        bz.interactable = (MyDataInfo.MyLevel == 1 && toggle.isOn);
    }

    private void groupInit()
    {
        jz.options.Clear();
        for (int i = 0; i < MyDataInfo.BeUsedJizus.Count; i++)
        {
            jz.options.Add(new Dropdown.OptionData(MyDataInfo.BeUsedJizus[i]));
        }

        bz.options.Clear();
        for (int i = 0; i < MyDataInfo.BeUsedBaozhangs.Count; i++)
        {
            bz.options.Add(new Dropdown.OptionData(MyDataInfo.BeUsedBaozhangs[i]));
        }
    }

    public string getData()
    {
        Debug.LogError(_equip.BObjectId);
        Debug.LogError(toggle.name);
        Debug.LogError(InputField_pos.name);
        return _equip.BObjectId + '_' + (toggle.isOn ? 1 : 0) + '_' + InputField_pos.text + '_' + dp_state.value + '_' +
               InputField_cycle.text + '_' + InputField_time.text + '_' + jz.value.ToString() + '_' + bz.value.ToString();
    }
}