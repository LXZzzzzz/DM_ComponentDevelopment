using DM.Core.Map;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
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
        EventManager.Instance.AddEventListener<string>(EventType.ShowAMsgInfo.ToString(), OnAddAMsg);
        EventManager.Instance.AddEventListener(EventType.ClearMsgBox.ToString(), OnCleraMsg);
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);

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


    private void OnAddAMsg(string info)
    {
        var itemCell = Instantiate(msgObj, msgParent);
        itemCell.Init(ConvertSecondsToHHMMSS(MyDataInfo.gameStartTime), info);
        itemCell.gameObject.SetActive(true);
    }

    private void OnCleraMsg()
    {
        for (int i = 0; i < msgParent.childCount; i++)
        {
            Destroy(msgParent.GetChild(i).gameObject);
        }
    }

    private string ConvertSecondsToHHMMSS(float seconds)
    {
        int hours = (int)(seconds / 3600); // 计算小时数
        int minutes = (int)(seconds % 3600 / 60); // 计算分钟数
        float remainingSeconds = seconds % 60; // 计算剩余秒数

        // 格式化为“时：分：秒”字符串
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, (int)remainingSeconds);
    }

    public override void HideMe()
    {
        base.HideMe();
        EventManager.Instance.RemoveEventListener<BObjectModel>(EventType.MapChooseIcon.ToString(), OnChooseWaters);
        EventManager.Instance.RemoveEventListener<string>(EventType.ShowAMsgInfo.ToString(), OnAddAMsg);
        EventManager.Instance.RemoveEventListener(EventType.ClearMsgBox.ToString(), OnCleraMsg);
    }
}