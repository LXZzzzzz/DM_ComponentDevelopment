using System;
using ToolsLibrary.EquipPart;
using UnityEngine.UI;

public class AttributeView_ZiyuanPart : DMonoBehaviour
{
    public void Init(ZiYuanBase data)
    {
        transform.Find("ziyuanType/ziyuanType").GetComponent<InputField>().text = type2Str(data.ZiYuanType);
        transform.Find("ziyuanName/ziyuanName").GetComponent<InputField>().text = data.ziYuanName;
        transform.Find("ziyuanDescribe/ziyuanDescribe").GetComponent<InputField>().text = data.ziYuanDescribe;
    }

    private string type2Str(ZiYuanType zt)
    {
        switch (zt)
        {
            case ZiYuanType.Airport:
                return "机场";
            case ZiYuanType.Hospital:
                return "医院";
            case ZiYuanType.Supply:
                return "补给点";
            case ZiYuanType.Waters:
                return "取水点";
            case ZiYuanType.DisasterArea:
                return "灾区点";
            case ZiYuanType.GoodsPoint:
                return "物资点";
            case ZiYuanType.RescueStation:
                return "救助站";
            case ZiYuanType.SourceOfAFire:
                return "火灾点";
            case ZiYuanType.TaskPoint:
                return "任务点";
        }

        return "---";
    }
}