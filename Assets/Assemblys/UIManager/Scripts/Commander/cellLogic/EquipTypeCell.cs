using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EquipTypeCell : DMonoBehaviour
{
    private Text showText;
    private string belongtoId;
    private Button btn;
    public void Init(string name, string id, UnityAction<string> cb)
    {
        showText = GetComponentInChildren<Text>(true);
        btn = GetComponentInChildren<Button>(true);
        
        //展示名字和点击按钮后的回调
        showText.text = name;
        belongtoId = id;
        btn.onClick.AddListener(() => cb(belongtoId));
    }
}