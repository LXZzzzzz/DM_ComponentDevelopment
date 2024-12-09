using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using EventType = Enums.EventType;

public class EquipCell : DMonoBehaviour
{
    private Text showName;
    public GameObject zongPart, qianPart;
    public Dropdown changeEnable, changeJizu;
    public Button zyfp, rwfp; //资源分配和任务分配按钮
    private EquipBase _equip;
    private GameObject chooseImg;
    private UnityAction<AEquipData> changeCallBack;
    private Dictionary<int, string> dropDownSupplementInfo;
    private float checkTimer;

    public string equipObjectId => _equip.BObjectId;
    public string equipBeUseCommander => _equip.BeLongToCommanderId;

    public void Init(int myLevel, EquipBase equip, Dictionary<string, string> allCommanderInfos, UnityAction<AEquipData> changeCb)
    {
        zongPart.SetActive(myLevel == 1);
        qianPart.SetActive(myLevel == 2);
        showName = GetComponentInChildren<Text>();
        chooseImg = transform.Find("ChooseImg").gameObject;
        transform.Find("btn_track").GetComponent<Button>().onClick.AddListener(onTrack);
        changeCallBack = changeCb;
        _equip = equip;


        showName.text = equip.name;
        changeEnable.options.Clear();
        changeEnable.options.Add(new Dropdown.OptionData("不出动"));
        changeEnable.options.Add(new Dropdown.OptionData("出动"));
        changeJizu.options.Clear();
        changeJizu.options.Add(new Dropdown.OptionData("机组A"));
        changeJizu.options.Add(new Dropdown.OptionData("机组B"));
        changeJizu.options.Add(new Dropdown.OptionData("机组C"));

        changeEnable.onValueChanged.AddListener((a) => OnSendChangeData());
        changeJizu.onValueChanged.AddListener((a) => OnSendChangeData());
        zyfp.onClick.AddListener(OnShowZyList);
        rwfp.onClick.AddListener(OnShowRwList);

        // dropDownSupplementInfo = new Dictionary<int, string>();
        // foreach (var info in allCommanderInfos)
        // {
        //     //要加一个不出动的选项
        //     Dropdown.OptionData itemData = new Dropdown.OptionData(info.Value);
        //     changeEnable.options.Add(itemData);
        //     dropDownSupplementInfo.Add(changeEnable.options.Count - 1, info.Key);
        // }

        checkTimer = Time.time;
        GetComponent<Button>().onClick.AddListener(() => EventManager.Instance.EventTrigger(Enums.EventType.CameraControl.ToString(), 1, _equip.transform));
    }

    private void Update()
    {
        if (Time.time > checkTimer)
        {
            checkTimer = Time.time + 1 / 25f;
            chooseImg.SetActive(_equip.isChooseMe);
            changeEnable.interactable = (int)MyDataInfo.gameState < 2;
        }
    }

    public void RefreshComShow(AEquipData aed)
    {
        //这里只需要显示是否出动就可以了
        changeEnable.value = aed.isSetOut;
        changeJizu.value = aed.jiZuInfo;
        switch (MyDataInfo.MyLevel)
        {
            case -1:
                showName.text = _equip.name + (aed.isSetOut == 0 ? "(不出动)" : "(出动)");
                break;
            case 2:
                zyfp.interactable = rwfp.interactable = aed.isSetOut == 1;
                break;
        }
    }

    private AEquipData itemEquipData;

    private void OnSendChangeData()
    {
        if (itemEquipData == null) itemEquipData = new AEquipData();
        itemEquipData.myId = equipObjectId;
        itemEquipData.isSetOut = changeEnable.value;
        itemEquipData.jiZuInfo = changeJizu.value;
        changeCallBack?.Invoke(itemEquipData);
    }

    private void OnShowZyList()
    {
        ZyfpInfo zi = new ZyfpInfo() { type = 1, currentInfo = _equip.currentBindingZy, sureCb = OnChangeZyList };
        UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, zi);
    }

    private void OnChangeZyList(List<string> info)
    {
        string bindingIds = equipObjectId + '_';
        for (int i = 0; i < info.Count; i++)
        {
            bindingIds += info[i] + '_';
        }

        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendEquipBindingZiyuan, bindingIds);
    }

    private void OnShowRwList()
    {
        ZyfpInfo zi = new ZyfpInfo() { type = 2, currentInfo = _equip.currentBindingZy, sureCb = OnChangeZyList };
        UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, zi);
    }

    private void onTrack()
    {
        EventManager.Instance.EventTrigger(Enums.EventType.CameraControl.ToString(), 2, _equip.transform);
    }
}

public struct ZyfpInfo
{
    public int type;
    public List<string> currentInfo;
    public UnityAction<List<string>> sureCb;
}