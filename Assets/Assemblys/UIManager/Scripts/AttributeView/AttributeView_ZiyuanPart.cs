using System;
using ToolsLibrary.EquipPart;
using UnityEngine.UI;

public class AttributeView_ZiyuanPart:DMonoBehaviour
{
    public void Init(ZiYuanBase data)
    {
        transform.Find("ziyuanType/ziyuanType").GetComponent<InputField>().text = data.ZiYuanType.ToString();
    }
}