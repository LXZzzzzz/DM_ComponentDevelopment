using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipCell : DMonoBehaviour
{
    private Text showName;
    public void Init(string name)
    {
        showName = GetComponentInChildren<Text>();
        showName.text = name;
        
    }
}
