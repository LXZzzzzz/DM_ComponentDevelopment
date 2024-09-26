using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelOnPointEnter : DMonoBehaviour, IPointerEnterHandler
{
    public GameObject go;

    public void OnPointerEnter(PointerEventData eventData)
    {
        go.SetActive(true);
        var tog = GetComponent<Toggle>();
        if (tog) tog.isOn = true;
    }
}