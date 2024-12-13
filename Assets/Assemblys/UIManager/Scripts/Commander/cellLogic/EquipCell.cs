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
    private Text showName, equipState;
    public GameObject zongPart, qianPart, otherPart;
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
        otherPart.SetActive(myLevel != 1 && myLevel != 2);
        showName = GetComponentInChildren<Text>(true);
        equipState = otherPart.GetComponentInChildren<Text>(true);
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
            changeEnable.interactable = (int)MyDataInfo.gameState < 1;
            showEquipState();
        }
    }

    public void RefreshComShow(AEquipData aed)
    {
        //这里只需要显示是否出动就可以了
        changeEnable.value = aed.isSetOut;
        changeJizu.value = aed.jiZuInfo;
        showName.text = _equip.name + (aed.isSetOut == 0 ? "(不出动)" : "(出动)");
        switch (MyDataInfo.MyLevel)
        {
            case 2:
                zyfp.interactable = rwfp.interactable = aed.isSetOut == 1;
                break;
        }
    }

    private void showEquipState()
    {
        switch (_equip.currentSkill)
        {
            case SkillType.GroundReady:
                equipState.text = "起飞前准备";
                break;
            case SkillType.BePutInStorage:
                equipState.text = "入库";
                break;
            case SkillType.TakeOff:
                equipState.text = "起飞";
                break;
            case SkillType.Landing:
                equipState.text = "降落";
                break;
            case SkillType.Supply:
                equipState.text = "补给";
                break;
            case SkillType.WaterIntaking:
                equipState.text = "取水";
                break;
            case SkillType.WaterPour:
                equipState.text = "投水";
                break;
            case SkillType.LadeGoods:
                equipState.text = "装载物资";
                break;
            case SkillType.UnLadeGoods:
                equipState.text = "卸载物资";
                break;
            case SkillType.AirdropGoods:
                equipState.text = "空投物资";
                break;
            case SkillType.Manned:
                equipState.text = "装载人员";
                break;
            case SkillType.PlacementOfPersonnel:
                equipState.text = "安置人员";
                break;
            case SkillType.CableDescentRescue:
                equipState.text = "索降救援";
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