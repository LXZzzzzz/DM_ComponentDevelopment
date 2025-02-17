using Enums;
using UnityEngine;
using UnityEngine.UI;

public class TrainCell : DMonoBehaviour
{
    public Toggle togComplete;
    public Text titleText;
    private string id;
    private string type;
    private string score;

    public void Init(string id, string title, string type,string score)
    {
        this.id = id;
        titleText.text = title;
        this.type = type;
        this.score = score;
    }

    public void SetComplete(string comType)
    {
        if (string.Equals(type, comType)) togComplete.isOn = true;
    }
}