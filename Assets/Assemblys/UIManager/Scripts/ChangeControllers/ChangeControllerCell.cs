using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChangeControllerCell : DMonoBehaviour
{
    private string _comId;

    public string comId => _comId;
    public bool isChoose => GetComponentInChildren<Toggle>(true).isOn;

    public void Init(string comId, string comName,Color color)
    {
        _comId = comId;
        GetComponentInChildren<Text>(true).text = comName;
        GetComponentInChildren<Toggle>(true).isOn = false;
        GetComponent<Image>().color = color;
    }

    public void ChangeState(bool isShow)
    {
        GetComponentInChildren<Toggle>(true).isOn = isShow;
    }
}
