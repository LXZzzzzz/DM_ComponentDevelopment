using Enums;
using UnityEngine;
using UnityEngine.UI;

public class TrainCell : DMonoBehaviour
{
    public Toggle togComplete;
    public Text titleText;
    private string id;
    private string type;

    public void Init(string id, string title, string type)
    {
        this.id = id;
        titleText.text = title;
        this.type = type;
    }

    public void SetComplete(string comType)
    {
        if (string.Equals(type, comType)) togComplete.isOn = true;
    }
}