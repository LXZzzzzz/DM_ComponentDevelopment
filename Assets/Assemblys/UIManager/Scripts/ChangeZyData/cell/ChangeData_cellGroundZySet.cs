using System;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.UI;

public class ChangeData_cellGroundZySet : DMonoBehaviour
{
    public Text txtZyType;
    public InputField inputFName, inputFLong, inputFLat;

    private ZiYuanBase _ziYuan;

    public void Init(ZiYuanBase zyData)
    {
        _ziYuan = zyData;
        txtZyType.text = getZyTypename();
        inputFName.text = zyData.ziYuanName;
        inputFLong.text = zyData.latAndLon.x.ToString();
        inputFLat.text = zyData.latAndLon.y.ToString();
    }

    private string getZyTypename()
    {
        switch (_ziYuan.ZiYuanType)
        {
            case ZiYuanType.Airport:
                return "机场";
            case ZiYuanType.Hospital:
                return "医院";
            case ZiYuanType.Supply:
                return "补给点";
            case ZiYuanType.Waters:
                return "水源点";
            case ZiYuanType.DisasterArea:
                return "灾区";
            case ZiYuanType.GoodsPoint:
                return "物资点";
            case ZiYuanType.RescueStation:
                return "救助站";
            case ZiYuanType.SourceOfAFire:
                return "火场位置";
            default: return "";
        }
    }

    public string GetSaveData()
    {
        //组装数据并发出
        Vector2 newPos = new Vector2(float.Parse(inputFLong.text), float.Parse(inputFLat.text));
        if (string.Equals(inputFName.text, _ziYuan.ziYuanName) && Vector2.Distance(newPos, _ziYuan.latAndLon) < 0.0001)
        {
            Debug.LogError(_ziYuan.ziYuanName + "数据未更改");
            return String.Empty;
        }

        string strData = _ziYuan.BobjectId + '_' + inputFName.text + '_' + inputFLong.text + '_' + inputFLat.text;
        return strData;
    }
}