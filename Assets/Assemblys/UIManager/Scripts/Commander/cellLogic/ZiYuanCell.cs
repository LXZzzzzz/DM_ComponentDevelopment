using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using EventType = Enums.EventType;

public class ZiYuanCell : DraggingFunction
{
    public bool isRight;
    private ZiYuanBase _ziYuan;
    private float checkTimer;
    private GameObject chooseImg;
    private GameObject taskGo;
    private RectTransform myRect;

    public void Init(string pointName, string entityId, ZiYuanBase ziyuan, Func<string, string, bool, bool> changeDataCallBack, UnityAction<bool, string> ctrlCmCb)
    {
        base.Init(entityId, changeDataCallBack, ctrlCmCb);

        _ziYuan = ziyuan;
        myRect = GetComponent<RectTransform>();
        chooseImg = transform.Find("ChooseImg").gameObject;
        transform.Find("Text_name").GetComponent<Text>().text = _ziYuan.ziYuanName;
        transform.Find("Text_describe").GetComponent<Text>().text = _ziYuan.ziYuanDescribe;
        transform.Find("btn_change").GetComponent<Button>().onClick.AddListener(OnOpenChangeCom);
        transform.Find("btn_change").gameObject.SetActive(MyDataInfo.MyLevel == 1);
        transform.Find("btn_delete")?.GetComponent<Button>().onClick.AddListener(OnDeleZiYuan);
        transform.Find("btn_delete")?.gameObject.SetActive(MyDataInfo.MyLevel == -1);
        GetComponent<Button>().onClick.AddListener(() => EventManager.Instance.EventTrigger(Enums.EventType.ChooseZiyuan.ToString(), entityId));
    }

    private void LateUpdate()
    {
        if (_ziYuan == null) return;
        if (Time.time > checkTimer)
        {
            checkTimer = Time.time + 1 / 25f;
            chooseImg.SetActive(_ziYuan.isChooseMe);
            if (taskGo != null && taskGo.activeSelf != gameObject.activeSelf)
                taskGo.SetActive(gameObject.activeSelf);
        }
    }

    public override string GetMyName()
    {
        return _ziYuan.ziYuanName;
    }

    private void OnOpenChangeCom()
    {
        //打开选择权限的视图
        if (ProgrammeDataManager.Instance.GetCurrentData == null)
        {
            ConfirmatonInfo infob = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "请先创建方案再进行编辑！" };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infob);
            return;
        }
        var surePos = UIManager.Instance.GetUIPanel<UIMap>(UIName.UIMap).resolutionRatioNormalized_size(myRect.sizeDelta / 2 + (isRight ? new Vector2(-90, -myRect.sizeDelta.y) : Vector2.zero));
        ZyComsInfo zci = new ZyComsInfo() { pos = (Vector2)myRect.position + surePos, coms = _ziYuan.beUsedCommanderIds, changeComs = OnChangeComs };
        UIManager.Instance.ShowPanel<UIChangeControllers>(UIName.UIChangeControllers, zci);
    }

    private void OnDeleZiYuan()
    {
        EventManager.Instance.EventTrigger(EventType.DestoryZaiQuzy.ToString(), myEntityId);
    }

    private void OnChangeComs(List<string> data)
    {
        ShowComCtrls(data, false);
    }

    public void RefreshComShow()
    {
        ShowComCtrls(_ziYuan.beUsedCommanderIds);
    }

    public void SetTaskGo(GameObject go)
    {
        taskGo = go;
    }
}

public struct ZyComsInfo
{
    public Vector2 pos;
    public List<string> coms;
    public UnityAction<List<string>> changeComs;
}