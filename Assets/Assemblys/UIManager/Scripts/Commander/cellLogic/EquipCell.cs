using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EquipCell : DMonoBehaviour
{
    private Text showName;
    private Dropdown changeCtrl;
    private EquipBase _equip;
    private GameObject chooseImg;
    private UnityAction<string, string> changeCallBack;
    private Dictionary<int, string> dropDownSupplementInfo;
    private float checkTimer;
    public string equipBeUseCommander => _equip.BeLongToCommanderId;

    public void Init(EquipBase equip, Dictionary<string, string> allCommanderInfos, UnityAction<string, string> changeCb)
    {
        showName = GetComponentInChildren<Text>();
        changeCtrl = GetComponentInChildren<Dropdown>();
        chooseImg = transform.Find("ChooseImg").gameObject;
        transform.Find("btn_positioning").GetComponent<Button>().onClick.AddListener(onPositioning);
        transform.Find("btn_track").GetComponent<Button>().onClick.AddListener(onTrack);
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
        checkTimer = Time.time;
        GetComponent<Button>().onClick.AddListener(() => EventManager.Instance.EventTrigger(Enums.EventType.ChooseEquip.ToString(), equip.BObjectId));
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
                if (string.Equals(controllerId, info.Value))
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

    private void Update()
    {
        if (Time.time > checkTimer)
        {
            checkTimer = Time.time + 1 / 25f;
            chooseImg.SetActive(_equip.isChooseMe);
        }
    }

    private void onPositioning()
    {
        EventManager.Instance.EventTrigger(Enums.EventType.CameraControl.ToString(),1,_equip.transform);
    }

    private void onTrack()
    {
        EventManager.Instance.EventTrigger(Enums.EventType.CameraControl.ToString(),2,_equip.transform);
    }
}