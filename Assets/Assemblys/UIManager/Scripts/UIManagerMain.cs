using System;
using UnityEngine;
using DM.IFS;
using UiManager;
using UiManager.IconShowPart;
using System.Collections;
using ToolsLibrary;
using UiManager.CursorShowPart;
using UnityEngine.Events;

public class UIManagerMain : ScriptManager, IMesRec
{
    public UIIconShow UIIconShow;
    public UICursorShow UICursorShow;
    public UIBarChartController UIBarChartController;
    public UIConfirmation UIConfirmation;
    public UIMap UIMap;
    public UICommanderView uiCommanderView;
    public UITopMenuView UITopMenuView;
    public UIRightClickMenuView UIRightClickMenuView;
    public UIAttributeView UIAttributeView;
    public UIHangShowInfo UIHangShowInfo;
    public UIAirportAircraftShowView UIAirportAircraftShowView;

    private UIItem_IconShow itemIcon;

    //编辑器模式下，初始化UI组件,设为原点位置
    public override void EditorModeInitialized()
    {
        base.EditorModeInitialized();
        sender.DebugMode = true;
        transform.position = Vector3.zero;
    }

    private void Awake()
    {
        Debug.LogError("Awake进入");
    }

    private void Start()
    {
#if UNITY_EDITOR
        Debug.LogError("start进入");
        RunModeInitialized(false, null);
#else
        sender.LogError("start进入");
#endif
    }

    //把UIManager挂载到组件上
    public override void RunModeInitialized(bool isMain, SceneInfo info)
    {
        base.RunModeInitialized(isMain, info);
        EventManager.Instance.AddEventListener<string, object>(Enums.EventType.ShowUI.ToString(), OnShowUI);
        EventManager.Instance.AddEventListener<string>(Enums.EventType.ShowTipUI.ToString(), OnShowTip);
        EventManager.Instance.AddEventListener<string, UnityAction<bool>>(Enums.EventType.ShowConfirmUI.ToString(), OnShowConfirm);
        gameObject.AddComponent<UIManager>();
        if (info?.PicBObjects != null)
        {
            UIManager.Instance.PicBObjects = info.PicBObjects;
            UIManager.Instance.MisName = info.MisName;
            UIManager.Instance.terrainName = info.Mission.Terrain;
        }

        //其他UI预制体脚本也需要在这里进行挂载
        uiprefabAddLogic();
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener<string, object>(Enums.EventType.ShowUI.ToString(), OnShowUI);
        EventManager.Instance.RemoveEventListener<string>(Enums.EventType.ShowTipUI.ToString(), OnShowTip);
        EventManager.Instance.RemoveEventListener<string, UnityAction<bool>>(Enums.EventType.ShowConfirmUI.ToString(), OnShowConfirm);
        UIManager.Instance.ClearUI();
    }

    private void uiprefabAddLogic()
    {
        UIIconShow = transform.Find("UiPrefab/IconItemPart/UIIconShow").gameObject.AddComponent<UIIconShow>();
        UICursorShow = transform.Find("UiPrefab/UICursorShow").gameObject.AddComponent<UICursorShow>();
        UIBarChartController = transform.Find("UiPrefab/UIBarChart").gameObject.GetComponent<UIBarChartController>();
        UIConfirmation = transform.Find("UiPrefab/UIConfirmation").gameObject.GetComponent<UIConfirmation>();
        UIMap = transform.Find("UiPrefab/UIMinMap").gameObject.GetComponent<UIMap>();
        uiCommanderView = transform.Find("UiPrefab/UICommanderView").gameObject.GetComponent<UICommanderView>();
        UITopMenuView = transform.Find("UiPrefab/UITopMenuView").gameObject.GetComponent<UITopMenuView>();
        UIRightClickMenuView = transform.Find("UiPrefab/UIRightClickMenuView").gameObject.GetComponent<UIRightClickMenuView>();
        UIAttributeView = transform.Find("UiPrefab/UIAttributeView").gameObject.GetComponent<UIAttributeView>();
        UIHangShowInfo = transform.Find("UiPrefab/UIHangShowInfo").gameObject.GetComponent<UIHangShowInfo>();
        UIAirportAircraftShowView = transform.Find("UiPrefab/UIAirportAircraftShowView").gameObject.GetComponent<UIAirportAircraftShowView>();

        //todo: 作为某个UI用到的组件，可以放到该UI节点下，加载代码在UI里完成，这里只进行所有UIPanel的加载
        itemIcon = transform.Find("UiPrefab/IconItemPart/IconItem").gameObject.AddComponent<UIItem_IconShow>();
    }

    private void testGet()
    {
        Transform itemGo = transform.Find("UiPrefab/UIBarChart");
        var datas = itemGo.GetComponents<MonoBehaviour>();
#if !UNITY_EDITOR
        sender.LogError($"获取到了{datas.Length}个脚本");
#endif
        for (int i = 0; i < datas.Length; i++)
        {
            if (datas[i] != null)
            {
                Debug.LogError(datas[i].GetType());
            }
        }
    }

    private void OnShowUI(string uiName, object dataInfo = null)
    {
        switch (uiName)
        {
            case "IconShow":
                UIManager.Instance.ShowPanel<UIIconShow>(UIName.UIIconShow, new DMonoBehaviour[] { itemIcon });
                break;
            case "CursorShow":
                UIManager.Instance.ShowPanel<UICursorShow>(UIName.UICursorShow, dataInfo);
                break;
            case "BarChart":
                UIManager.Instance.ShowPanel<UIBarChartController>(UIName.UIBarChart, dataInfo);
                break;
            case "Confirmation":
                UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, dataInfo);
                break;
            case "MinMap":
                UIManager.Instance.ShowPanel<UIMap>(UIName.UIMap, dataInfo);
                break;
            case "CommanderView":
                UIManager.Instance.ShowPanel<UICommanderView>(UIName.UICommanderView, dataInfo);
                break;
            case "TopMenuView":
                UIManager.Instance.ShowPanel<UITopMenuView>(UIName.UITopMenuView, dataInfo);
                break;
            case "RightClickMenuView":
                UIManager.Instance.ShowPanel<UIRightClickMenuView>(UIName.UIRightClickMenuView, dataInfo);
                break;
            case "AttributeView":
                UIManager.Instance.ShowPanel<UIAttributeView>(UIName.UIAttributeView, dataInfo);
                break;
            default:
                break;
        }
    }

    private void OnShowTip(string tipInfo)
    {
        ConfirmatonInfo info = new ConfirmatonInfo() { type = showType.tipView, showStrInfo = tipInfo };
        UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, info);
    }

    private void OnShowConfirm(string showInfo, UnityAction<bool> cb)
    {
        ConfirmatonInfo infob = new ConfirmatonInfo
        {
            type = showType.secondConfirm, showStrInfo = showInfo, sureCallBack = a => { cb(true); }
        };
        UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infob);
    }

    public void RecMessage(SendType type, GameObject senderObj, int eventType, string param)
    {
        if (type == SendType.SubToMain && MyDataInfo.isHost)
        {
            sender.RunSend(SendType.MainToAll, BObjectId, eventType, param);
        }

        if (type == SendType.MainToAll)
        {
            switch (eventType)
            {
                case 666:
                    string[] data = param.Split('_');
                    if (data[0] == MyDataInfo.leadId)
                    {
                        UIManager.Instance.GetUIPanel<UIIconShow>(UIName.UIIconShow).ReceiveOperate(666, data[1]);
                    }

                    break;
                case 888:
                case 999:
                    if (param == MyDataInfo.leadId)
                    {
                        UIManager.Instance.GetUIPanel<UIIconShow>(UIName.UIIconShow).ReceiveOperate(eventType, "");
                    }

                    break;
            }
        }
    }
}