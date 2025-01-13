using DataTranfsers;
using Enums;
using Newtonsoft.Json;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class msgCell : DMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string info;
    private Button showDataBtn;
    private string showData;

    public void Init(string time, string msgInfo, string data)
    {
        transform.GetChild(0).GetComponent<Text>().text = time;
        transform.GetChild(1).GetComponent<Text>().text = msgInfo;
        showDataBtn = transform.GetChild(2).GetComponent<Button>();
        showDataBtn.onClick.AddListener(showDataClick);
        info = msgInfo;
        showData = data;
    }

    public void ChangeController(string ctrlName)
    {
        if (ctrlName.Contains('总'))
        {
            gameObject.SetActive(true);
            return;
        }

        gameObject.SetActive(info.Contains(ctrlName));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(showData)) showDataBtn.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(showData)) showDataBtn.gameObject.SetActive(false);
    }

    private void showDataClick()
    {
        string showJsonStr=AESUtils.Decrypt(showData);
        var data = JsonConvert.DeserializeObject<ZbldZqqr>(showJsonStr);
        UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowStrInputData((int)data.szdt, showJsonStr));
    }
}