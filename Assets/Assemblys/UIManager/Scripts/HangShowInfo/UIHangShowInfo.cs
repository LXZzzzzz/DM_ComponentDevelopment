using ToolsLibrary;
using UiManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHangShowInfo : BasePanel
{
    private RectTransform bgRect;
    private Text showName, showInfo, oilMass, waterNum, goodsNum, qPersonNum, zPersonNum;
    private Transform comParent;
    private GameObject comCell;

    public override void Init()
    {
        base.Init();
        bgRect = transform.Find("point/bg").GetComponent<RectTransform>();
        showName = GetControl<Text>("text_Name");
        showInfo = GetControl<Text>("text_Info");
        oilMass = GetControl<Text>("text_OilMass");
        waterNum = GetControl<Text>("text_WaterNum");
        goodsNum = GetControl<Text>("text_GoodsNum");
        qPersonNum = GetControl<Text>("text_qPersonNum");
        zPersonNum = GetControl<Text>("text_zPersonNum");
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

        bgRect.sizeDelta = new Vector2(291, data.isAir ? 226 : 174);

        oilMass.gameObject.SetActive(data.isAir);
        waterNum.gameObject.SetActive(data.isAir);
        goodsNum.gameObject.SetActive(data.isAir);
        qPersonNum.gameObject.SetActive(data.isAir);
        zPersonNum.gameObject.SetActive(data.isAir);
        oilMass.text = $"{(int)data.currentOilMass}/{(int)data.maxOilMass}";
        waterNum.text = $"{data.waterNum}kg";
        goodsNum.text = $"{data.goodsNum}kg";
        qPersonNum.text = $"{(data.personType == 1 ? data.personNum : 0)}人";
        zPersonNum.text = $"{(data.personType == 2 ? data.personNum : 0)}人";

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