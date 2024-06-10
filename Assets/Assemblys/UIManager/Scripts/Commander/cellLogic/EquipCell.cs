using System.Collections;
using System.Collections.Generic;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EquipCell : DMonoBehaviour
{
    private Text showName;
    private Dropdown changeCtrl;
    private EquipBase _equip;
    private UnityAction<string, string> changeCallBack;
    private Dictionary<int, string> dropDownSupplementInfo;
    public string equipBeUseCommander => _equip.BeLongToCommanderId;

    public void Init(EquipBase equip, Dictionary<string, string> allCommanderInfos, UnityAction<string, string> changeCb)
    {
        showName = GetComponentInChildren<Text>();
        changeCtrl = GetComponentInChildren<Dropdown>();
        changeCallBack = changeCb;
        this._equip = equip;

        showName.text = equip.name;
        changeCtrl.options = new List<Dropdown.OptionData>();
        dropDownSupplementInfo = new Dictionary<int, string>();
        foreach (var info in allCommanderInfos)
        {
            Dropdown.OptionData itemData = new Dropdown.OptionData(info.Value);
            changeCtrl.options.Add(itemData);
            dropDownSupplementInfo.Add(changeCtrl.options.Count - 1, info.Key);
        }
        dropDownInit();
    }

    private void dropDownInit()
    {
        string controllerId = _equip.BeLongToCommanderId;
        if (string.IsNullOrEmpty(controllerId))
            changeCtrl.value = 0;
        else
        {
            //找到这个id的下标，赋值
            foreach (var info in dropDownSupplementInfo)
            {
                if (string.Equals(controllerId,info.Value))
                {
                    changeCtrl.value = info.Key;
                    break;
                }
            }
        }

        changeCtrl.onValueChanged.AddListener(OnChange);
    }

    private void OnChange(int select)
    {
        changeCallBack(_equip.BObjectId, dropDownSupplementInfo[select]);
        _equip.BeLongToCommanderId = dropDownSupplementInfo[select];
    }
}