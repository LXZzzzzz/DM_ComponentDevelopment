using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZiYuanCell : DMonoBehaviour
{
    private Text showName;
    public void Init(string pointName,string entityId,List<string> canBeUseCommanders)
    {
        //显示名称，存储对应实体ID，界面显示自己都可被哪些人使用
        showName = GetComponentInChildren<Text>();
        showName.text = pointName;
    }
}
