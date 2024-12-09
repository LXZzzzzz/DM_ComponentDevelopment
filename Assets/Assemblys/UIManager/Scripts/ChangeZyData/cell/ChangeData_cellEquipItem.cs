using UnityEngine;
using UnityEngine.UI;

public class ChangeData_cellEquipItem : DMonoBehaviour
{
    [HideInInspector] public string myName;
    [HideInInspector] public string myId;
    public Text nameTxt;
    public Dropdown dp;

    public void Init(string name, string id, int state)
    {
        myName = name;
        myId = id;
        nameTxt.text = name;
        dp.value = state;
    }

    public int GetState()
    {
        return dp.value;
    }
}