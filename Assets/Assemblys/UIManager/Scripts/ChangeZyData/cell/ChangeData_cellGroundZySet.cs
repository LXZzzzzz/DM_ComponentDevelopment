using System;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.UI;

public class ChangeData_cellGroundZySet : DMonoBehaviour
{
    public Text txtZyType;
    public InputField inputFName;

    private ChangeData_cellLongLatSet LonSet, LatSet;

    private ZiYuanBase _ziYuan;

    public void Init(ZiYuanBase zyData)
    {
        _ziYuan = zyData;
        txtZyType.text = getZyTypename();
        inputFName.text = zyData.ziYuanName;
        LonSet = transform.Find("long").GetComponent<ChangeData_cellLongLatSet>();
        LatSet = transform.Find("lat").GetComponent<ChangeData_cellLongLatSet>();

        SetDMS(zyData.latAndLon.x, LonSet);
        SetDMS(zyData.latAndLon.y, LatSet);
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


        float lon = DMSToFloat(int.Parse(LonSet.InputField_D.text), int.Parse(LonSet.InputField_M.text),
            int.Parse(LonSet.InputField_S.text));

        float lat = DMSToFloat(int.Parse(LatSet.InputField_D.text), int.Parse(LatSet.InputField_M.text),
            int.Parse(LatSet.InputField_S.text));

        Vector2 newPos = new Vector2(lon, lat);

        if (string.Equals(inputFName.text, _ziYuan.ziYuanName) && Vector2.Distance(newPos, _ziYuan.latAndLon) < 0.0001)
        {
            Debug.LogError(_ziYuan.ziYuanName + "数据未更改");
            return String.Empty;
        }

        string strData = _ziYuan.BobjectId + '_' + inputFName.text + '_' + lon + '_' + lat;
        return strData;
    }


    public void SetDMS(float lonlat, ChangeData_cellLongLatSet cellLongLatSet)
    {
        Vector3 V3 = FloatToDMS(lonlat);
        cellLongLatSet.InputField_D.text = V3.x.ToString();
        cellLongLatSet.InputField_M.text = V3.y.ToString();
        cellLongLatSet.InputField_S.text = V3.z.ToString();
    }


    /// <summary>
    /// float 转度分秒
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    public Vector3 FloatToDMS(float coordinate)
    {
        // 获取度数（整数部分）
        int degrees = Mathf.FloorToInt(coordinate);

        // 获取分数（剩余的小数部分 * 60）
        float minutesDecimal = (Mathf.Abs(coordinate) - Mathf.Abs(degrees)) * 60;
        int minutes = Mathf.FloorToInt(minutesDecimal);

        // 获取秒数（剩余的小数部分 * 60）
        float secondsDecimal = (minutesDecimal - minutes) * 60;
        int seconds = Mathf.FloorToInt(secondsDecimal);

        // 返回格式化的度分秒字符串
        return new Vector3(degrees, minutes, seconds);
    }

    /// <summary>
    /// 度分秒转化成 float
    /// </summary>
    /// <param name="degrees"></param>
    /// <param name="minutes"></param>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public float DMSToFloat(int degrees, int minutes, int seconds)
    {
        // 计算并返回十进制度表示
        return degrees + (float)minutes / 60 + (float)seconds / 3600;
    }
}