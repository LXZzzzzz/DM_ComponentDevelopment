using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AirportAircraftCell : DMonoBehaviour
{
    public void Init(string equipId, string equipName,bool isMe, UnityAction<string> takeOffCb)
    {
        GetComponentInChildren<Text>().text = equipName;
        GetComponentInChildren<Button>().onClick.AddListener(() => takeOffCb(equipId));
        GetComponentInChildren<Button>().onClick.AddListener(() => UIManager.Instance.HidePanel(UIName.UIAirportAircraftShowView.ToString()));
        GetComponentInChildren<Button>().interactable = isMe;
    }
}