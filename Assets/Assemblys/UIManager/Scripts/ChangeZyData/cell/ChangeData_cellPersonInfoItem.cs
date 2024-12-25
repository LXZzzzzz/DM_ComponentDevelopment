using System;
using UnityEngine;
using UnityEngine.UI;

public class ChangeData_cellPersonInfoItem : DMonoBehaviour
{
    public Toggle toggle;

    public Text Text_toggle;

    public InputField InputField_postion, InputField_state;

    public string personName => Text_toggle.text;

    public void Init(string data)
    {
        var itemInfos = data.Split('_');
        toggle.isOn = string.Equals(itemInfos[1], "1");
        InputField_postion.text = itemInfos[2];
        InputField_state.text = itemInfos[3];

        toggle.interactable = false;
        InputField_postion.interactable = false;
        InputField_state.interactable = false;
    }

    public string GetData()
    {
        Debug.LogError(toggle.name);
        Debug.LogError(toggle.GetComponentInChildren<Text>().name);
        Debug.LogError(InputField_postion.name);
        Debug.LogError(InputField_state.name);
        return personName + '_' + (toggle.isOn ? 1 : 0) + '_' + InputField_postion.text + '_' + InputField_state.text;
    }
}