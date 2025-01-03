using System;
using UnityEngine;
using UnityEngine.UI;

public class ChangeData_cellPersonInfoItem : DMonoBehaviour
{
    public Toggle toggle;

    public InputField InputField_name, InputField_postion;

    public Dropdown dp_state;

    public void Init(string data)
    {
        var itemInfos = data.Split('_');
        InputField_name.text = itemInfos[0];
        toggle.isOn = string.Equals(itemInfos[1], "1");
        InputField_postion.text = itemInfos[2];
        dp_state.value = int.Parse(itemInfos[3]);

        toggle.interactable = false;
        InputField_name.interactable = false;
        InputField_postion.interactable = false;
        dp_state.interactable = false;
    }

    public string GetData()
    {
        return InputField_name.text + '_' + (toggle.isOn ? 1 : 0) + '_' + InputField_postion.text + '_' + dp_state.value;
    }
}