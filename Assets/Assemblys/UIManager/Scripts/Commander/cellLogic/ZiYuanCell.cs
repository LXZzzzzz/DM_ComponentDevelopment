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

public class ZiYuanCell : DMonoBehaviour
{
    private ZiYuanBase _ziYuan;
    private float checkTimer;
    private GameObject chooseImg;
    private UnityAction<AZiYuanData> changeDataCb;
    private Text nameTxt;


    public string myEntityId => _ziYuan.BobjectId;

    public void Init(int myLevel, ZiYuanBase ziyuan, UnityAction<AZiYuanData> changeDataCallBack)
    {
        _ziYuan = ziyuan;
        changeDataCb = changeDataCallBack;
        chooseImg = transform.Find("ChooseImg").gameObject;
        nameTxt = transform.Find("Text_name").GetComponent<Text>();
        transform.Find("Text_describe").GetComponent<Text>().text = _ziYuan.ziYuanDescribe;
        transform.Find("btn_changeData").GetComponent<Button>().onClick.AddListener(OnOpenChangeCom);
        transform.Find("btn_changeData").gameObject.SetActive(myLevel == 1 && (_ziYuan.ZiYuanType == ZiYuanType.Supply || _ziYuan.ZiYuanType == ZiYuanType.GoodsPoint));
        GetComponent<Button>().onClick.AddListener(() => EventManager.Instance.EventTrigger(EventType.DqChooseGo.ToString(), myEntityId));
    }

    private void Update()
    {
        if (_ziYuan == null) return;
        if (Time.time > checkTimer)
        {
            checkTimer = Time.time + 1 / 25f;
            nameTxt.text = _ziYuan.ziYuanName;
            chooseImg.SetActive(_ziYuan.isChooseMe);
        }
    }

    private void OnOpenChangeCom()
    {
        //打开选择权限的视图
        // if (ProgrammeDataManager.Instance.GetCurrentData == null)
        // {
        //     ConfirmatonInfo infob = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "请先创建方案再进行编辑！" };
        //     UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infob);
        //     return;
        // }

        Debug.LogError("资源类型：" + _ziYuan.ZiYuanType);

        ZyComsInfo zci = new ZyComsInfo() { zyType = _ziYuan.ZiYuanType, currentData = _ziYuan.GetVariableData(), changeComs = OnSendChangeData };
        UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, zci);
    }

    private AZiYuanData itemZyData;

    private void OnSendChangeData(ZyVariableDataBase data)
    {
        if (itemZyData == null) itemZyData = new AZiYuanData();
        itemZyData.myId = myEntityId;

        if (_ziYuan.ZiYuanType == ZiYuanType.Supply) itemZyData.zyNum = ((SupplyVariableData)data).oilNum;
        if (_ziYuan.ZiYuanType == ZiYuanType.GoodsPoint) itemZyData.zyNum = ((GoodsPointVariableData)data).goodsNum;
        changeDataCb?.Invoke(itemZyData);
    }
}

public class ZyComsInfo
{
    public ZiYuanType zyType;
    public ZyVariableDataBase currentData;
    public UnityAction<ZyVariableDataBase> changeComs;
}