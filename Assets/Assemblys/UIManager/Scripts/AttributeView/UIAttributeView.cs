using DM.Core.Map;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using EventType = Enums.EventType;
using IWaterIntaking = ToolsLibrary.EquipPart.IWaterIntaking;

public class UIAttributeView : BasePanel
{
    private InputField waterAmount;
    private Text watersName;
    private IWaterIntaking operateObj;
    private Vector3 watersPos;
    private bool isReceiveMapInfo;
    private GameObject watersSkillView, equipInfoView;

    public override void Init()
    {
        base.Init();
        waterAmount = GetControl<InputField>("input_waterAmount");
        watersName = GetControl<Text>("text_waters");
        watersSkillView = transform.Find("Skill_WaterIntaking/skillWatersInfoView").gameObject;
        equipInfoView = transform.Find("Skill_WaterIntaking/equipInfoView").gameObject;
        GetControl<Button>("btn_chooseWaters").onClick.AddListener(() => isReceiveMapInfo = true);
        GetControl<Button>("btn_sure").onClick.AddListener(OnSendSkillParameter);
        GetControl<Button>("btn_close").onClick.AddListener(() => Close(UIName.UIAttributeView));
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        if (userData is IWaterIntaking)
        {
            watersSkillView.SetActive(true);
            equipInfoView.SetActive(false);
            operateObj = (IWaterIntaking)userData;
            isReceiveMapInfo = false;
            EventManager.Instance.AddEventListener<BObjectModel>(EventType.MapChooseIcon.ToString(), OnChooseWaters);
        }
        else
        {
            watersSkillView.SetActive(false);
            equipInfoView.SetActive(true);
        }
    }


    private void OnChooseWaters(BObjectModel bom)
    {
        if (!isReceiveMapInfo || bom == null) return;

        var coms = bom.GetComponent<ZiYuanBase>().beUsedCommanderIds;
        var isMeControl = coms?.Find(x => string.Equals(x, MyDataInfo.leadId));
        sender.LogError(bom.name + "我的级别：" + MyDataInfo.MyLevel);

        if (isMeControl == null)
        {
            //这里的目的是：当资源归属人为空，那就交给一级指挥官控制
            if (MyDataInfo.MyLevel != 1 || coms != null && coms.Count != 0)
            {
                sender.LogError("选择的取水点不可为我所用");
                watersPos = Vector3.zero;
                return;
            }
        }

        //todo:后期加上类型保护
        watersName.text = bom.BObject.Info.Name;
        watersPos = bom.gameObject.transform.position;
    }

    private void OnSendSkillParameter()
    {
        if (string.IsNullOrEmpty(waterAmount.text) || watersPos == Vector3.zero)
        {
            sender.LogError("取水参数错误");
            return;
        }

        isReceiveMapInfo = false;
        float itemWaterAmount = int.Parse(waterAmount.text);
        if (operateObj.CheckCapacity() > itemWaterAmount)
            operateObj.WaterIntaking(watersPos, 1, itemWaterAmount, false);
    }

    public override void HideMe()
    {
        base.HideMe();
        EventManager.Instance.RemoveEventListener<BObjectModel>(EventType.MapChooseIcon.ToString(), OnChooseWaters);
    }
}