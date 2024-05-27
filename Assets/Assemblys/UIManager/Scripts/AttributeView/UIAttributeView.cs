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
    private IWaterIntaking operateObj;
    private Vector3 watersPos;
    private bool isReceiveMapInfo;

    public override void Init()
    {
        base.Init();
        waterAmount = GetControl<InputField>("input_waterAmount");
        watersName = GetControl<Text>("text_waters");
        GetControl<Button>("btn_chooseWaters").onClick.AddListener(() => isReceiveMapInfo = true);
        GetControl<Button>("btn_sure").onClick.AddListener(OnSendSkillParameter);
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        operateObj = (IWaterIntaking)userData;
        isReceiveMapInfo = false;
        EventManager.Instance.AddEventListener<BObjectModel>(EventType.MapChooseIcon.ToString(), OnChooseWaters);
    }


    private void OnChooseWaters(BObjectModel bom)
    {
        if (!isReceiveMapInfo || bom == null) return;
        //todo:后期加上类型保护
        watersName.text = bom.BObject.Info.Name;
        watersPos = bom.gameObject.transform.position;
    }

    private void OnSendSkillParameter()
    {
        isReceiveMapInfo = false;
        float itemWaterAmount = int.Parse(waterAmount.text);
        if (operateObj.CheckCapacity() < itemWaterAmount)
            operateObj.WaterIntaking(watersPos, 1, itemWaterAmount, false);
    }

    public override void HideMe()
    {
        base.HideMe();
        EventManager.Instance.RemoveEventListener<BObjectModel>(EventType.MapChooseIcon.ToString(), OnChooseWaters);
    }
}