using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.UI;

public class ZaiQuTemplateCell : DMonoBehaviour
{
    private ZiYuanBase _ziYuan;

    public void Init(ZiYuanBase ziyuan)
    {
        _ziYuan = ziyuan;
        transform.Find("Text_name").GetComponent<Text>().text = _ziYuan.ziYuanName;
        transform.Find("Text_describe").GetComponent<Text>().text = _ziYuan.ziYuanDescribe;
        GetComponentInChildren<Button>().onClick.AddListener(() =>
            EventManager.Instance.EventTrigger<object>(Enums.EventType.TransferEditingInfo.ToString(), _ziYuan.BobjectId));
    }
}