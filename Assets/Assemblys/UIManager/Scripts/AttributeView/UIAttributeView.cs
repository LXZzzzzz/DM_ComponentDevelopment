using System;
using System.Collections;
using System.Collections.Generic;
using DM.Core.Map;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using EventType = Enums.EventType;

public class UIAttributeView : BasePanel
{
    private InputField waterAmount;
    private Text watersName;
    private IWatersOperation operateObj;
    private Vector3 watersPos;
    private bool isReceiveMapInfo;
    private GameObject watersSkillView, equipInfoView;
    private AttributeView_ZiyuanPart ziYuanInfoView;
    private RectTransform msgParent;
    private msgCell msgObj;
    private List<msgCell> allMsgCells;
    private Transform equipObj, ziyuanObj;
    private Toggle messageT, infoT;


    private ZiYuanCell zycPrefab;
    private TaskCell taskPrefab;
    private RectTransform taskParent;
    private List<ZiYuanCell> allZyZqCells;
    private List<TaskCell> allTaskCells; //存储所有任务cell，方便后面数据修改

    public override void Init()
    {
        base.Init();
        waterAmount = GetControl<InputField>("input_waterAmount");
        watersName = GetControl<Text>("text_waters");
        watersSkillView = transform.Find("Skill_WaterIntaking/skillWatersInfoView").gameObject;
        equipInfoView = transform.Find("Skill_WaterIntaking/equipInfoView").gameObject;
        ziYuanInfoView = GetComponentInChildren<AttributeView_ZiyuanPart>(true);
        GetControl<Button>("btn_chooseWaters").onClick.AddListener(() => isReceiveMapInfo = true);
        GetControl<Button>("btn_sure").onClick.AddListener(OnSendSkillParameter);
        GetControl<Button>("btn_close").onClick.AddListener(() => Close(UIName.UIAttributeView));
        msgParent = GetControl<ScrollRect>("msgInfoView").content;
        msgObj = GetComponentInChildren<msgCell>(true);
        messageT = GetControl<Toggle>("messageT");
        infoT = GetControl<Toggle>("infoT");
        equipObj = transform.Find("msgInfoView/infoView/EquipInfo");
        ziyuanObj = transform.Find("msgInfoView/infoView/ZiyuanInfo");

        taskParent = GetControl<ScrollRect>("TaskListView").content;
        taskPrefab = GetComponentInChildren<TaskCell>(true);
        zycPrefab = GetComponentInChildren<ZiYuanCell>(true);
        EventManager.Instance.AddEventListener<string>(EventType.ShowAMsgInfo.ToString(), OnAddAMsg);
        EventManager.Instance.AddEventListener(EventType.ClearMsgBox.ToString(), OnCleraMsg);
        EventManager.Instance.AddEventListener<string>(EventType.ChangeCurrentCom.ToString(), OnChangeCom);
        EventManager.Instance.AddEventListener<int, string>(EventType.ChangeObjController.ToString(), OnRunningChangeObjCom);
        EventManager.Instance.AddEventListener<ZiYuanBase>(EventType.InitZiYuanBeUsed.ToString(), OnInitZiYuanBeUsed);
        // EventManager.Instance.AddEventListener<string>(EventType.ChooseEquip.ToString(), OnShowEquipInfo);
        allMsgCells = new List<msgCell>();
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        Debug.LogError("Attribute界面逻辑");
        StartCoroutine(ShowTaskView());

        if (userData == null)
        {
            OnShowEquipInfo(null);
            OnShowZiyuanInfo(null);
        }
        else
        {
            if (userData is EquipBase) OnShowEquipInfo(userData as EquipBase);
            if (userData is ZiYuanBase) OnShowZiyuanInfo(userData as ZiYuanBase);
        }

        return;
        //如果当前正在接收外部，则关闭其他属性展示逻辑
        if (isReceiveMapInfo) return;
        if (userData == null)
        {
            watersSkillView.SetActive(false);
            equipInfoView.SetActive(false);
            ziYuanInfoView.gameObject.SetActive(false);
        }

        if (userData is EquipBase)
        {
            EquipBase currentEquip = userData as EquipBase;
            switch (currentEquip.CurrentChooseSkillType)
            {
                case SkillType.None:
                    watersSkillView.SetActive(false);
                    equipInfoView.SetActive(true);
                    ziYuanInfoView.gameObject.SetActive(false);
                    equipInfoView.transform.Find("title").GetComponent<Text>().text = $"{currentEquip.name}-性能参数";
                    var showInfos = currentEquip.AttributeInfos;
                    var allInputf = equipInfoView.GetComponentsInChildren<InputField>(true);
                    for (int i = 0; i < allInputf.Length; i++)
                    {
                        allInputf[i].text = showInfos[i];
                    }

                    break;
                case SkillType.WaterIntaking:
                    watersSkillView.SetActive(true);
                    equipInfoView.SetActive(false);
                    ziYuanInfoView.gameObject.SetActive(false);
                    operateObj = (IWatersOperation)userData;
                    isReceiveMapInfo = false;
                    EventManager.Instance.AddEventListener<BObjectModel>(EventType.MapChooseIcon.ToString(), OnChooseWaters);
                    break;
            }
        }
        else if (userData is ZiYuanBase)
        {
            watersSkillView.SetActive(false);
            equipInfoView.SetActive(false);
            ziYuanInfoView.gameObject.SetActive(true);
            ziYuanInfoView.Init(userData as ZiYuanBase);
        }
    }

    IEnumerator ShowTaskView()
    {
        if (allZyZqCells != null) yield break;
        yield return new WaitForSeconds(1);
        allZyZqCells = new List<ZiYuanCell>();
        allTaskCells = new List<TaskCell>();

        for (int i = 0; i < allBObjects.Length; i++)
        {
            var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
            if (tagItem != null && tagItem.SubTags.Find(y => y.Id == 1 || y.Id == 5) != null)
            {
                var itemObj = allBObjects[i];
                ZiYuanBase zyObj = itemObj.GetComponent<ZiYuanBase>();
                bool isDisaster = zyObj.ZiYuanType == ZiYuanType.Hospital || zyObj.ZiYuanType == ZiYuanType.RescueStation ||
                                  zyObj.ZiYuanType == ZiYuanType.DisasterArea || zyObj.ZiYuanType == ZiYuanType.SourceOfAFire;
                if (!isDisaster) continue;
                var itemCell = Instantiate(zycPrefab, taskParent);
                itemCell.Init(itemObj.BObject.Info.Name, itemObj.BObject.Id, zyObj, UIManager.Instance.GetUIPanel<UICommanderView>(UIName.UICommanderView).OnChangeZiYuanBelongTo, null);
                itemCell.gameObject.SetActive(true);
                allZyZqCells.Add(itemCell);
            }
        }

        yield return new WaitForSeconds(1);
        int taskIndex = 0;
        for (int i = 0; i < allBObjects.Length; i++)
        {
            var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
            //任务列表展示
            if (tagItem != null && tagItem.SubTags.Find(y => y.Id == 3) != null)
            {
                taskIndex++;
                //任务与资源逻辑应该是一样的
                var itemObj = allBObjects[i];
                var itemzy = itemObj.gameObject.GetComponent<ZiYuanBase>();
                var itemCell = Instantiate(taskPrefab, taskParent);
                itemCell.Init("任务" + taskIndex, itemzy);
                itemCell.gameObject.SetActive(true);
                allTaskCells.Add(itemCell);

                yield return 1;
                string zqId = String.Empty;
                while (string.IsNullOrEmpty(zqId))
                {
                    zqId = (itemzy as ITaskProgress)?.getAssociationAssemblyId();
                    yield return 1;
                }

                int targetIndex = 0;
                for (int j = 0; j < taskParent.childCount; j++)
                {
                    if (string.Equals(zqId, taskParent.GetChild(j).GetComponent<ZiYuanCell>()?.myEntityId))
                    {
                        taskParent.GetChild(j).GetComponent<ZiYuanCell>().SetTaskGo(itemCell.gameObject);
                        targetIndex = j + 1;
                        break;
                    }
                }

                if (targetIndex >= 0 && targetIndex < taskParent.childCount)
                {
                    itemCell.transform.SetSiblingIndex(targetIndex);
                }
            }
        }
    }

    private void OnInitZiYuanBeUsed(ZiYuanBase data)
    {
        var itemZiyuan = allZyZqCells.Find(x => string.Equals(x.myEntityId, data.main.BObjectId));
        itemZiyuan?.ShowComCtrls(data.beUsedCommanderIds);
        // var itemTask = allTaskCells.Find(x => string.Equals(x.myEntityId, data.main.BObjectId));
        // itemTask?.ShowComCtrls(data.beUsedCommanderIds);

        if (MyDataInfo.MyLevel != 1)
        {
            if (itemZiyuan != null)
            {
                bool isShow = itemZiyuan.allcoms.Find(x => string.Equals(x.comId, MyDataInfo.leadId));
                itemZiyuan.gameObject.SetActive(isShow);
            }
        }

        StartCoroutine(dalayCall());
    }

    public void OnChooseCommander(string id)
    {
        var currentCommander = MyDataInfo.playerInfos.Find(x => string.Equals(id, x.RoleId));
        if (currentCommander.ClientLevel == 1)
        {
            for (int i = 0; i < allZyZqCells.Count; i++)
            {
                allZyZqCells[i].gameObject.SetActive(true);
            }

            for (int i = 0; i < allTaskCells.Count; i++)
            {
                allTaskCells[i].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < allZyZqCells.Count; i++)
            {
                bool isShow = allZyZqCells[i].allcoms.Find(x => string.Equals(x.comId, id));
                allZyZqCells[i].gameObject.SetActive(isShow);
            }

            for (int i = 0; i < allTaskCells.Count; i++)
            {
                bool isShow = allTaskCells[i].allcoms.Find(x => string.Equals(x.comId, id));
                allTaskCells[i].gameObject.SetActive(isShow);
            }
        }
    }

    private void OnRunningChangeObjCom(int type, string id)
    {
        var itemZyCell = allZyZqCells.Find(x => string.Equals(x.myEntityId, id));
        if (itemZyCell != null) itemZyCell.RefreshComShow();

        StartCoroutine(dalayCall());
    }

    IEnumerator dalayCall()
    {
        yield return 1;
        OnChooseCommander(MyDataInfo.leadId);
    }


    // ////////////////////////////////////////////////////////////消息列表逻辑/////////////////////////////////////////////////////////////////


    #region 技能选择逻辑，现在无用了

    private void OnChooseWaters(BObjectModel bom)
    {
        if (!isReceiveMapInfo || bom == null) return;

        var coms = bom.GetComponent<ZiYuanBase>().beUsedCommanderIds;
        var isMeControl = coms?.Find(x => string.Equals(x, MyDataInfo.leadId));
        sender.LogError(bom.name + "我的级别：" + MyDataInfo.MyLevel);

        if (bom.GetComponent<ZiYuanBase>().ZiYuanType != ZiYuanType.Waters)
        {
            ConfirmatonInfo infoa = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "当前选择并不是取水点" };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
            watersPos = Vector3.zero;
            return;
        }

        if (isMeControl == null)
        {
            //这里的目的是：当资源归属人为空，那就交给一级指挥官控制
            if (MyDataInfo.MyLevel != 1 || coms != null && coms.Count != 0)
            {
                ConfirmatonInfo infoa = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "选择的取水点不可为我所用" };
                UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
                watersPos = Vector3.zero;
                return;
            }
        }

        watersName.text = bom.BObject.Info.Name;
        watersPos = bom.gameObject.transform.position;
        EventManager.Instance.RemoveEventListener<BObjectModel>(EventType.MapChooseIcon.ToString(), OnChooseWaters);
    }

    private void OnSendSkillParameter()
    {
        if (string.IsNullOrEmpty(waterAmount.text) || watersPos == Vector3.zero)
        {
            ConfirmatonInfo infoa = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "取水参数错误" };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
            return;
        }

        isReceiveMapInfo = false;
        // float itemWaterAmount = int.Parse(waterAmount.text);
        // if (operateObj.CheckCapacity() > itemWaterAmount)
        //     operateObj.WaterIntaking(watersPos, 1, itemWaterAmount, false);
        // else
        // {
        //     ConfirmatonInfo infoa = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "取水量超出飞机最大核载水量" };
        //     UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
        // }
    }

    #endregion

    private EquipBase currentEquip;
    private Slider oilSlider;
    private Text oilValue;
    private Slider waterSlider, goodsSlider, zsySlider, qsySlider;
    private Text waterText, goodsText, zsyText, qsyText;


    private void OnAddAMsg(string info)
    {
        var itemCell = Instantiate(msgObj, msgParent);
        itemCell.Init(ConvertSecondsToHHMMSS(MyDataInfo.gameStartTime), info);
        itemCell.gameObject.SetActive(true);
        allMsgCells.Add(itemCell);
        StartCoroutine(setSV());
    }

    IEnumerator setSV()
    {
        yield return 1;
        yield return 1;
        GetControl<ScrollRect>("msgInfoView").verticalScrollbar.value = 0;
    }

    private void OnCleraMsg()
    {
        for (int i = 0; i < allMsgCells.Count; i++)
        {
            Destroy(allMsgCells[i].gameObject);
        }

        allMsgCells.Clear();
    }

    private void OnChangeCom(string com)
    {
        allMsgCells.ForEach(a => a.ChangeController(com));
    }

    private string ConvertSecondsToHHMMSS(float seconds)
    {
        int hours = (int)(seconds / 3600); // 计算小时数
        int minutes = (int)(seconds % 3600 / 60); // 计算分钟数
        float remainingSeconds = seconds % 60; // 计算剩余秒数

        // 格式化为“时：分：秒”字符串
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, (int)remainingSeconds);
    }

    private void Update()
    {
        if (currentEquip)
        {
            currentEquip.GetCurrentAllMass(out float currentOil, out float totalOil, out float water, out float goods, out float person, out int personType);
            oilSlider.value = currentOil / totalOil;
            oilValue.text = (int)(currentOil / totalOil * 100) + "%";
            waterSlider.value = water / float.Parse(currentEquip.AttributeInfos[15]);
            waterText.text = water + "/" + currentEquip.AttributeInfos[15] + "Kg";
            goodsSlider.value = goods / float.Parse(currentEquip.AttributeInfos[4]);
            goodsText.text = goods + "/" + currentEquip.AttributeInfos[4] + "Kg";
            zsySlider.value = (personType == 1 ? 0 : person) / float.Parse(currentEquip.AttributeInfos[6]);
            zsyText.text = (personType == 1 ? 0 : person) + "/" + currentEquip.AttributeInfos[6] + "人";
            qsySlider.value = (personType == 1 ? person : 0) / float.Parse(currentEquip.AttributeInfos[6]);
            qsyText.text = (personType == 1 ? person : 0) + "/" + currentEquip.AttributeInfos[6] + "人";
        }
    }

    private void OnShowEquipInfo(EquipBase equip)
    {
        if (equip == null)
        {
            messageT.isOn = true;
            currentEquip = null;
            return;
        }

        infoT.isOn = true;
        equipObj.gameObject.SetActive(true);
        ziyuanObj.gameObject.SetActive(false);

        currentEquip = equip;
        equipObj.Find("equipNameView/airType").GetComponent<Image>().sprite = equip.EquipIcon;
        equipObj.Find("equipNameView/equipName").GetComponent<Text>().text = equip.name;
        var comData = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, equip.BeLongToCommanderId));
        equipObj.Find("equipNameView/bg").GetComponent<Image>().color = comData.MyColor;
        equipObj.Find("equipNameView/AirTypeBg").GetComponent<Image>().color = comData.MyColor;
        if (oilSlider == null) oilSlider = equipObj.Find("equipNameView/oilPart/oilShow").GetComponent<Slider>();
        if (oilValue == null) oilValue = equipObj.Find("equipNameView/oilPart/oilValue").GetComponent<Text>();
        equip.GetCurrentAllMass(out float currentOil, out float totalOil, out float water, out float goods, out float person, out int personType);
        oilSlider.value = currentOil / totalOil;
        oilValue.text = (int)(currentOil / totalOil * 100) + "%";
        //飞机参数
        equipObj.Find("equipParameter/parameterInfo/personPart").GetComponentInChildren<Text>().text = equip.AttributeInfos[6] + "人";
        equipObj.Find("equipParameter/parameterInfo/hangcPart").GetComponentInChildren<Text>().text = equip.AttributeInfos[3] + "Km";
        equipObj.Find("equipParameter/parameterInfo/speedPart").GetComponentInChildren<Text>().text = equip.AttributeInfos[9] + "km/h";
        equipObj.Find("equipParameter/parameterInfo/oilPart").GetComponentInChildren<Text>().text = equip.AttributeInfos[5] + "Kg";
        equipObj.Find("equipParameter/parameterInfo/waterPart").GetComponentInChildren<Text>().text = equip.AttributeInfos[15] + "Kg";
        equipObj.Find("equipParameter/parameterInfo/goodsPart").GetComponentInChildren<Text>().text = equip.AttributeInfos[4] + "Kg";
        //机载携带
        if (waterSlider == null) waterSlider = equipObj.Find("carryParameter/parameterInfo/waterPart").GetComponentInChildren<Slider>();
        if (waterText == null) waterText = equipObj.Find("carryParameter/parameterInfo/waterPart").GetComponentInChildren<Text>();
        if (goodsSlider == null) goodsSlider = equipObj.Find("carryParameter/parameterInfo/goodsPart").GetComponentInChildren<Slider>();
        if (goodsText == null) goodsText = equipObj.Find("carryParameter/parameterInfo/goodsPart").GetComponentInChildren<Text>();
        if (zsySlider == null) zsySlider = equipObj.Find("carryParameter/parameterInfo/zsyPart").GetComponentInChildren<Slider>();
        if (zsyText == null) zsyText = equipObj.Find("carryParameter/parameterInfo/zsyPart").GetComponentInChildren<Text>();
        if (qsySlider == null) qsySlider = equipObj.Find("carryParameter/parameterInfo/qsyPart").GetComponentInChildren<Slider>();
        if (qsyText == null) qsyText = equipObj.Find("carryParameter/parameterInfo/qsyPart").GetComponentInChildren<Text>();
        waterSlider.value = water / float.Parse(equip.AttributeInfos[15]);
        waterText.text = water + "/" + equip.AttributeInfos[15] + "Kg";
        goodsSlider.value = goods / float.Parse(equip.AttributeInfos[4]);
        goodsText.text = goods + "/" + equip.AttributeInfos[4] + "Kg";
        zsySlider.value = (personType == 1 ? 0 : person) / float.Parse(equip.AttributeInfos[6]);
        zsyText.text = (personType == 1 ? 0 : person) + "/" + equip.AttributeInfos[6] + "人";
        qsySlider.value = (personType == 1 ? person : 0) / float.Parse(equip.AttributeInfos[6]);
        qsyText.text = (personType == 1 ? person : 0) + "/" + equip.AttributeInfos[6] + "人";
    }

    private void OnShowZiyuanInfo(ZiYuanBase ziyuan)
    {
        if (ziyuan == null)
        {
            messageT.isOn = true;
            return;
        }

        equipObj.gameObject.SetActive(false);
        ziyuanObj.gameObject.SetActive(true);
        infoT.isOn = true;

        ziyuanObj.Find("ziyuanNameView/equipName").GetComponent<Text>().text = ziyuan.ziYuanName;
        ziyuanObj.Find("ziyuanNameView/bg").GetComponent<Image>().color = ziyuan.MyColor;
        ziyuanObj.Find("ziyuanNameView/AirTypeBg").GetComponent<Image>().color = ziyuan.MyColor;
        changeIcon(ziyuan.ZiYuanType);
        ziyuanObj.Find("ziyuanInfo/ziyuanInfo").GetComponent<Text>().text = getZiyuanInfo(ziyuan);
        ziyuanObj.Find("ziyuanJWD").GetComponent<Text>().text = ziyuan.latAndLon.ToString();
    }

    private void changeIcon(ZiYuanType type)
    {
        Transform zyTypePart = ziyuanObj.Find("ziyuanNameView/zyTypePart").transform;
        for (int i = 0; i < zyTypePart.childCount; i++)
        {
            zyTypePart.GetChild(i).gameObject.SetActive(false);
        }

        int index = -1;
        switch (type)
        {
            case ZiYuanType.Supply:
                index = 7;
                break;
            case ZiYuanType.RescueStation:
                index = 6;
                break;
            case ZiYuanType.Airport:
                index = 2;
                break;
            case ZiYuanType.Hospital:
                index = 1;
                break;
            case ZiYuanType.GoodsPoint:
                index = 5;
                break;
            case ZiYuanType.Waters:
                index = 0;
                break;
            case ZiYuanType.DisasterArea:
                index = 3;
                break;
            case ZiYuanType.SourceOfAFire:
                index = 4;
                break;
        }

        if (index != -1) zyTypePart.GetChild(index).gameObject.SetActive(true);
    }

    private string getZiyuanInfo(ZiYuanBase _ziYuanItem)
    {
        string progressInfo = String.Empty;
        switch (_ziYuanItem.ZiYuanType)
        {
            case ZiYuanType.SourceOfAFire:
                (_ziYuanItem as ISourceOfAFire).getFireData(out float ghmj, out float rsmj, out float csghmj, out float csrsmj, out float tszl);
                progressInfo = $"该火源点需求水量为：{(int)(rsmj < 0 ? 0 : rsmj)}kg";
                break;
            case ZiYuanType.RescueStation:
                (_ziYuanItem as IRescueStation).getTaskProgress(out int currentPersonNum, out int maxPersonNum, out float currentGoodsNum, out float maxGoodsNum);
                progressInfo = $"安置点当前安置人员:{currentPersonNum}人，总共可安置：{maxPersonNum}人\n当前已有物资:{currentGoodsNum}kg，总共需求物资：{maxGoodsNum}kg";
                break;
            case ZiYuanType.Hospital:
                (_ziYuanItem as IRescueStation).getTaskProgress(out int currentPersonNum1, out int maxPersonNum1, out float currentGoodsNum1, out float maxGoodsNum1);
                progressInfo = $"医院当前救治人员:{currentPersonNum1}人，总共可救治：{maxPersonNum1}人\n当前已有物资:{currentGoodsNum1}kg，总共需求物资：{maxGoodsNum1}kg";
                break;
            case ZiYuanType.DisasterArea:
                (_ziYuanItem as IDisasterArea).getTaskProgress(out int currentNum, out int maxNum);
                string personType = (_ziYuanItem as IDisasterArea).getWoundedPersonnelType() == 1 ? "轻伤员" : "重伤员";
                progressInfo = $"{personType}灾区还剩余需转运人数:{currentNum}人，总共受灾人数：{maxNum}人";
                break;
            default:
                progressInfo = _ziYuanItem.ziYuanDescribe;
                break;
        }

        return progressInfo;
    }

    public override void HideMe()
    {
        base.HideMe();
        EventManager.Instance.RemoveEventListener<BObjectModel>(EventType.MapChooseIcon.ToString(), OnChooseWaters);
        EventManager.Instance.RemoveEventListener<string>(EventType.ShowAMsgInfo.ToString(), OnAddAMsg);
        EventManager.Instance.RemoveEventListener(EventType.ClearMsgBox.ToString(), OnCleraMsg);
        EventManager.Instance.RemoveEventListener<string>(EventType.ChangeCurrentCom.ToString(), OnChangeCom);
        EventManager.Instance.RemoveEventListener<int, string>(EventType.ChangeObjController.ToString(), OnRunningChangeObjCom);
        EventManager.Instance.RemoveEventListener<ZiYuanBase>(EventType.InitZiYuanBeUsed.ToString(), OnInitZiYuanBeUsed);
        // EventManager.Instance.RemoveEventListener<string>(EventType.ChooseEquip.ToString(), OnShowEquipInfo);
    }
}