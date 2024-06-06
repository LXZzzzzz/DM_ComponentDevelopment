using UiManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHangShowInfo : BasePanel
{
    private Text showName, showInfo;
    private Transform comParent;
    private GameObject comCell;

    public override void Init()
    {
        base.Init();
        showName = GetControl<Text>("text_Name");
        showInfo = GetControl<Text>("text_Info");
        comParent = GetControl<ScrollRect>("commandersListView").content;
        comCell = transform.Find("point/bg/commanderCell").gameObject;
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);

        var data = (IconInfoData)userData;

        transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = data.pointPos;
        showName.text = data.entityName;
        showInfo.text = data.entityInfo;

        for (int i = 0; i < data.beUseCommanders?.Count; i++)
        {
            var itemCom = Instantiate(comCell, comParent);
            itemCom.GetComponentInChildren<Text>(true).text = getComName(data.beUseCommanders[i]);
            itemCom.gameObject.SetActive(true);
        }
    }


    private string getComName(string id)
    {
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(allBObjects[i].BObject.Id, id))
            {
                return allBObjects[i].BObject.Info.Name;
            }
        }

        return "未找到指定ID指挥端";
    }

    public override void HideMe()
    {
        base.HideMe();

        for (int i = 0; i < comParent.childCount; i++)
        {
            Destroy(comParent.GetChild(i).gameObject);
        }
    }
}