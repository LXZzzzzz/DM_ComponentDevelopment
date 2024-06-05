using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZiYuanDeCell : DraggingFunction
{
    public void Init(string pointName,string entityId, Func<string, string, bool, bool> changeDataCallBack)
    {
        base.Init(entityId, changeDataCallBack);

        transform.Find("Text_name").GetComponent<Text>().text = pointName;
        transform.Find("Text_describe").GetComponent<Text>().text = pointName;
    }
}
