using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeData_cellZyItem : DMonoBehaviour
{
    [HideInInspector] public string zyName;
    [HideInInspector] public string zyId;
    private Toggle tog;

    public void Init(string name, string id, bool isChoose)
    {
        zyName = name;
        zyId = id;
        GetComponentInChildren<Text>(true).text = name;
        tog = GetComponentInChildren<Toggle>(true);
        tog.isOn = isChoose;
    }

    public void OnchangeChoose(List<string> bindingZy)
    {
        tog.isOn = bindingZy != null && bindingZy.Contains(zyId);
    }

    public bool GetIsChoose()
    {
        return tog.isOn;
    }
}