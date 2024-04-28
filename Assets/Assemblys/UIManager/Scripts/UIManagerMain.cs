﻿using System;
using UnityEngine;
using DM.IFS;
using UiManager;
using UiManager.IconShowPart;
using System.Collections;
using ToolsLibrary;
using UiManager.CursorShowPart;

public class UIManagerMain : ScriptManager, IMesRec
{
    public UIIconShow UIIconShow;
    public UICursorShow UICursorShow;
    public UIBarChartController UIBarChartController;
    public UIConfirmation UIConfirmation;

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
        Debug.LogError("运行脚本RunModeInitialized进入");
        Debug.LogError("UI系统打印：" + EventManager.Instance);
        EventManager.Instance.AddEventListener<string>(ToolsLibrary.EventType.ShowUI, OnShowUI);
        //todo:给这里改成UIManager初始化，并把go放到我节点下，并把自己传给他。把uimanager放到ToolsLibrary中，这样其他部分代码就可以直接通过单例拿到UI进行操作
        gameObject.AddComponent<UIManager>();
        //其他UI预制体脚本也需要在这里进行挂载
        uiprefabAddLogic();
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveEventListener<string>(ToolsLibrary.EventType.ShowUI, OnShowUI);
        UIManager.Instance.ClearUI();
    }

    private void uiprefabAddLogic()
    {
        UIIconShow = transform.Find("UiPrefab/IconItemPart/UIIconShow").gameObject.AddComponent<UIIconShow>();
        UICursorShow = transform.Find("UiPrefab/UICursorShow").gameObject.AddComponent<UICursorShow>();
        UIBarChartController = transform.Find("UiPrefab/UIBarChart").gameObject.GetComponent<UIBarChartController>();
        UIConfirmation = transform.Find("UiPrefab/UIConfirmation").gameObject.GetComponent<UIConfirmation>();
        testGet();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            OnShowUI("BarChart");
        }
    }

    private void OnShowUI(string uiName)
    {
        switch (uiName)
        {
            case "IconShow":

                UIManager.Instance.ShowPanel<UIIconShow>(UIName.UIIconShow, new DMonoBehaviour[] { itemIcon });
                break;
            case "CursorShow":

                UIManager.Instance.ShowPanel<UICursorShow>(UIName.UICursorShow, null);
                break;
            case "BarChart":
                UIManager.Instance.ShowPanel<UIBarChartController>(UIName.UIBarChart, null);
                break;
            case "Confirmation":
                UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, null);
                break;
            default:
                break;
        }
    }

    public void RecMessage(SendType type, GameObject senderObj, int eventType, string param)
    {
        if (type == SendType.SubToMain&&MyDataInfo.isHost)
        {
            sender.RunSend(SendType.MainToAll,BObjectId,eventType,param);
        }

        if (type==SendType.MainToAll)
        {
            switch (eventType)
            {
                case 666:
                    string[] data = param.Split('_');
                    if(data[0]==MyDataInfo.leadId)
                    {
                        UIManager.Instance.GetUIPanel<UIIconShow>(UIName.UIIconShow).ReceiveOperate(666,data[1]);
                    }
                    break;
                case 888:
                case 999:
                    if (param==MyDataInfo.leadId)
                    {
                        UIManager.Instance.GetUIPanel<UIIconShow>(UIName.UIIconShow).ReceiveOperate(eventType,"");
                    }
                    break;
            }
        }
    }
}