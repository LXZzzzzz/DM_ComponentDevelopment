using ToolsLibrary;
using UiManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHangShowInfo : BasePanel
{
    private RectTransform bgRect;
    private Text showName, showInfo, waterNum, goodsNum, personNum;
    private Transform comParent;
    private GameObject comCell;

    public override void Init()
    {
        base.Init();
        bgRect = transform.Find("point/bg").GetComponent<RectTransform>();
        showName = GetControl<Text>("text_Name");
        showInfo = GetControl<Text>("text_Info");
        waterNum = GetControl<Text>("text_WaterNum");
        goodsNum = GetControl<Text>("text_GoodsNum");
        personNum = GetControl<Text>("text_PersonNum");
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

        bgRect.sizeDelta = new Vector2(192, data.isAir ? 112 : 90);

        waterNum.gameObject.SetActive(data.isAir);
        goodsNum.gameObject.SetActive(data.isAir);
        personNum.gameObject.SetActive(data.isAir);
        waterNum.text = $"{data.waterNum}kg";
        goodsNum.text = $"{data.goodsNum}kg";
        personNum.text = $"{data.personNum}人";

        for (int i = 0; i < data.beUseCommanders?.Count; i++)
        {
            var itemCom = Instantiate(comCell, comParent);
            itemCom.GetComponentInChildren<Text>(true).text = getComName(data.beUseCommanders[i]);
            itemCom.GetComponentInChildren<Image>().color = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, data.beUseCommanders[i])).MyColor;
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