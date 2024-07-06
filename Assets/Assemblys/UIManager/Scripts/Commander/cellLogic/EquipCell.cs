using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EquipCell : DMonoBehaviour
{
    private Text showName;
    private Dropdown changeCtrl;
    private Text noDpShowName;
    private EquipBase _equip;
    private GameObject chooseImg;
    private UnityAction<string, string> changeCallBack;
    private Dictionary<int, string> dropDownSupplementInfo;
    private float checkTimer;
    private Button deleBtn;

    public string equipObjectId => _equip.BObjectId;
    public string equipBeUseCommander => _equip.BeLongToCommanderId;

    public void Init(EquipBase equip, Dictionary<string, string> allCommanderInfos, UnityAction<string, string> changeCb)
    {
        showName = GetComponentInChildren<Text>();
        changeCtrl = GetComponentInChildren<Dropdown>();
        chooseImg = transform.Find("ChooseImg").gameObject;
        deleBtn = transform.Find("btn_Dele").GetComponent<Button>();
        transform.Find("btn_positioning").GetComponent<Button>().onClick.AddListener(onPositioning);
        transform.Find("btn_track").GetComponent<Button>().onClick.AddListener(onTrack);
        deleBtn.onClick.AddListener(deleEquip);
        noDpShowName = transform.Find("noDpShowName").GetComponent<Text>();
        changeCallBack = changeCb;
        _equip = equip;

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

    private void deleEquip()
    {
        ConfirmatonInfo infob = new ConfirmatonInfo
        {
            type = showType.secondConfirm, showStrInfo = "请确认是否删除", sureCallBack = a =>
            {
                bool isdele = ProgrammeDataManager.Instance.DeleEquip(equipObjectId);
                if (isdele)
                    MyDataInfo.sceneAllEquips.Find(b => string.Equals(b.BObjectId, equipObjectId)).Destroy();
                else
                {
                    ConfirmatonInfo infoc = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "数据错误，请退出重试！！" };
                    UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoc);
                }
            }
        };
        UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infob);
    }

    private void dropDownInit()
    {
        string controllerId = _equip.BeLongToCommanderId;
        if (string.IsNullOrEmpty(controllerId))
            changeCtrl.value = 0;
        else
        {
            if (dropDownSupplementInfo.Count == 0)
            {
                //证明是二级指挥端，显示自己名字即可
                noDpShowName.gameObject.SetActive(true);
                changeCtrl.gameObject.SetActive(false);
                noDpShowName.text = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, MyDataInfo.leadId)).ClientLevelName;
                return;
            }

            noDpShowName.gameObject.SetActive(false);
            changeCtrl.gameObject.SetActive(true);
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
            changeCtrl.interactable = (int)MyDataInfo.gameState < 2;
            deleBtn.gameObject.SetActive(MyDataInfo.gameState == GameState.FirstLevelCommanderEditor);
        }
    }

    private void onPositioning()
    {
        EventManager.Instance.EventTrigger(Enums.EventType.CameraControl.ToString(), 1, _equip.transform);
    }

    private void onTrack()
    {
        EventManager.Instance.EventTrigger(Enums.EventType.CameraControl.ToString(), 2, _equip.transform);
    }
}