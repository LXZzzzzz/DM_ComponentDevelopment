using System.Collections;
using System.Collections.Generic;
using DM.Core.Map;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Vectrosity;
using EventType = Enums.EventType;

public class MapOperate_Normal : MapOperateLogicBase
{
    private VectorLine showLine;
    private List<Vector2> linePoss;
    private bool isShow;
    private RectTransform chooseEquip;
    private string creatTargetTemplate;
    private bool isPressDownCtrl;

    public override void OnEnter()
    {
        EventManager.Instance.AddEventListener<int, string>(EventType.ChangeObjController.ToString(), OnRunningChangeObjCom);
        EventManager.Instance.AddEventListener<object>(EventType.TransferEditingInfo.ToString(), OnParsingData);
        EventManager.Instance.AddEventListener<Vector2>(EventType.ShowMarkMapPoint.ToString(), OnShowMarkPoint);
        EventManager.Instance.AddEventListener<string>(EventType.crashIcon.ToString(), OnCrash);
        linePoss = new List<Vector2>();
        linePoss.Add(Vector2.zero);
        linePoss.Add(Vector2.zero);
        showLine = new VectorLine("Line", linePoss, 2, LineType.Continuous);
#if UNITY_EDITOR
        showLine.SetCanvas(mainLogic.gameObject.GetComponentInParent<Canvas>());
#else
        showLine.SetCanvas(UIManager.Instance.CurrentCanvans);
#endif
        showLine.rectTransform.SetParent(mainLogic.iconCellParent);
        showLine.rectTransform.localPosition = Vector3.zero;
        showLine.rectTransform.localScale = Vector3.one;
        showLine.active = true;
        if (mainLogic.dashedLineMat == null)
        {
            Debug.LogError("虚线材质没找到");
            if (ColorUtility.TryParseHtmlString("#FF0000", out Color color))
                showLine.color = color;
        }
        else showLine.material = mainLogic.dashedLineMat;

        isShow = false;
    }

    public override void OnLeftClickIcon(IconCellBase targetIconCell)
    {
        //查看他的类型

        if (targetIconCell is AirIconCell)
        {
            //选中对象
            EventManager.Instance.EventTrigger(EventType.ChooseEquip.ToString(), targetIconCell.belongToId);
            //打开装备数据展示界面
            isShow = true;
            showLine.active = true;
            chooseEquip = targetIconCell.GetComponent<RectTransform>();
        }

        if (targetIconCell is PointIconCell)
        {
            //选中的是标点
#if UNITY_EDITOR
            Debug.Log("选中的标点是" + targetIconCell.belongToId + "的点；" + "名字是：" + targetIconCell.name);
            Debug.Log($"经过了{(targetIconCell as PointIconCell).allViaPointIds?.Count}个点");
#else
                    mainLogic.sender.LogError("选中的标点是" + targetIconCell.belongToId + "的点；" + "名字是：" + targetIconCell.name);
#endif
        }

        if (targetIconCell is ZiYuanIconCell)
        {
            EventManager.Instance.EventTrigger(EventType.ChooseZiyuan.ToString(), targetIconCell.belongToId);

            for (int i = 0; i < mainLogic.allBObjects.Length; i++)
            {
                if (string.Equals(mainLogic.allBObjects[i].BObject.Id, targetIconCell.belongToId))
                {
                    EventManager.Instance.EventTrigger(EventType.MapChooseIcon.ToString(), mainLogic.allBObjects[i]);
                    break;
                }
            }

            #region 无用逻辑

            return;
            //如果是机场的话，弹出飞机列表，飞机列表点击起飞，把飞机从机场中去除，并把飞机状态设为起飞状态。 
            ZiYuanBase zy = (targetIconCell as ZiYuanIconCell).ziYuanItem;
            if (MyDataInfo.MyLevel != 1 &&
                (zy.beUsedCommanderIds == null || zy.beUsedCommanderIds.Find(x => string.Equals(x, MyDataInfo.leadId)) == null)) return;
            if (zy.ZiYuanType == ZiYuanType.Airport)
            {
                AirportAircraftsInfo aai = new AirportAircraftsInfo()
                {
                    PointPos = targetIconCell.GetComponent<RectTransform>().anchoredPosition, AircraftDatas = (zy as IAirPort)?.GetAllEquips(),
                    OnRightClickCallBack = a => openRightClickView(a, targetIconCell.GetComponent<RectTransform>().anchoredPosition)
                };
                UIManager.Instance.ShowPanel<UIAirportAircraftShowView>(UIName.UIAirportAircraftShowView, aai);
            }

            #endregion
        }
    }

    private void openRightClickView(string id, Vector2 pos)
    {
        EventManager.Instance.EventTrigger(EventType.ChooseEquip.ToString(), id);
        var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, id));
        if (itemEquip.currentSkill != SkillType.None) return;
        RightClickShowInfo info = new RightClickShowInfo()
        {
            PointPos = pos, ShowSkillDatas = itemEquip.GetSkillsData(), OnTriggerCallBack = itemEquip.OnSelectSkill
        };
        UIManager.Instance.ShowPanel<UIRightClickMenuView>(UIName.UIRightClickMenuView, info);
    }

    public override void OnRightClickIcon(IconCellBase clickIcon)
    {
        //打开指令集页面
        if (clickIcon is AirIconCell)
        {
            var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, clickIcon.belongToId));
            if (!string.Equals(MyDataInfo.leadId, itemEquip.BeLongToCommanderId)) return;
            if (itemEquip.currentSkill != SkillType.None) return;
            EventManager.Instance.EventTrigger(EventType.ChooseEquip.ToString(), clickIcon.belongToId);
            RightClickShowInfo info = new RightClickShowInfo()
            {
                PointPos = (Vector2)clickIcon.GetComponent<RectTransform>().position + new Vector2(50, 40),
                ShowSkillDatas = itemEquip.GetSkillsData(), OnTriggerCallBack = itemEquip.OnSelectSkill
            };
            if (info.ShowSkillDatas.Find(x => x.isUsable) != null)
                UIManager.Instance.ShowPanel<UIRightClickMenuView>(UIName.UIRightClickMenuView, info);
        }
        else if (clickIcon is ZiYuanIconCell)
        {
            EventManager.Instance.EventTrigger(EventType.MoveToTarget.ToString(), clickIcon.belongToId, (clickIcon as ZiYuanIconCell).ziYuanItem.gameObject.transform.position);
            EventManager.Instance.EventTrigger(EventType.ChooseEquipToZiYuanType.ToString(), (int)(clickIcon as ZiYuanIconCell).ziYuanItem.ZiYuanType);
        }
    }

    public override void OnUpdate()
    {
        showXvLine();
        showTargetTemplate();
        isPressDownCtrl = Input.GetKey(KeyCode.LeftControl);
    }

    private void showXvLine()
    {
        if (!isShow || chooseEquip) return;
#if UNITY_EDITOR
        linePoss[0] = Vector2.zero;
#else
                linePoss[0] = chooseEquip.anchoredPosition;
#endif
        var rectPos = mainLogic.resolutionRatioNormalized(Input.mousePosition);
        linePoss[1] = mainLogic.mousePos2UI(rectPos);
        showLine.Draw();
        float dis = Vector2.Distance(linePoss[0], linePoss[1]);
        mainLogic.dashedLineMat.SetTextureScale("_MainTex", new Vector2(dis / 80, 0));
        showLine.active = true;
    }

    private void showTargetTemplate()
    {
        if (!string.IsNullOrEmpty(creatTargetTemplate))
        {
            var rectPos = mainLogic.resolutionRatioNormalized(Input.mousePosition);
            mainLogic.TempIcon.anchoredPosition = mainLogic.mousePos2UI(rectPos);
        }
    }

    public override void OnLeftClickMap(Vector2 pos)
    {
        if (isPressDownCtrl)
        {
            EventManager.Instance.EventTrigger(EventType.MarkMapPoints.ToString(), pos);

#if UNITY_EDITOR
            OnShowMarkPoint(pos);
#endif
            return;
        }

#if UNITY_EDITOR
        var itemPoint = GameObject.Instantiate(mainLogic.pointIconPrefab, mainLogic.iconCellParent);
        itemPoint.enabled = false;
        var itemGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        itemGo.transform.position = uiPos2WorldPos(pos);
        itemPoint.GetComponent<RectTransform>().anchoredPosition = worldPos2UiPos(uiPos2WorldPos(pos));
        itemPoint.gameObject.SetActive(true);
#endif
        EventManager.Instance.EventTrigger(EventType.ChooseEquip.ToString(), string.Empty);
        EventManager.Instance.EventTrigger(EventType.ChooseZiyuan.ToString(), string.Empty);
        isShow = false;
        showLine.active = false;

        //导教端在场景中创建灾区
        if (string.IsNullOrEmpty(creatTargetTemplate)) return;
        //通知主角在场景对应位置创建实体
        EventManager.Instance.EventTrigger(EventType.CreatZaiQuZy.ToString(), creatTargetTemplate, uiPos2WorldPos(pos));

        //这里先去数据管理器里申请创建，然后将数据ID传给创建者
        // string equipId = ProgrammeDataManager.Instance.AddEquip(creatTargetTemplate, uiPos2WorldPos(pos));
        // ProgrammeDataManager.Instance.GetEquipDataById(equipId).controllerId = MyDataInfo.leadId;
    }

    public override void OnRightClickMap(Vector2 pos)
    {
        mainLogic.TempIcon.gameObject.SetActive(false);
        creatTargetTemplate = string.Empty;
        EventManager.Instance.EventTrigger(EventType.MoveToTarget.ToString(), string.Empty, uiPos2WorldPos(pos));
        EventManager.Instance.EventTrigger(EventType.ChooseEquipToZiYuanType.ToString(), -1);
    }

    public override void OnExit()
    {
        EventManager.Instance.RemoveEventListener<int, string>(EventType.ChangeObjController.ToString(), OnRunningChangeObjCom);
        EventManager.Instance.RemoveEventListener<object>(EventType.TransferEditingInfo.ToString(), OnParsingData);
        EventManager.Instance.RemoveEventListener<Vector2>(EventType.ShowMarkMapPoint.ToString(), OnShowMarkPoint);
        EventManager.Instance.RemoveEventListener<string>(EventType.crashIcon.ToString(), OnCrash);
        showLine.active = false;
        isShow = false;
    }

    private void OnRunningChangeObjCom(int type, string id)
    {
        if (mainLogic.allIconCells.ContainsKey(id))
            mainLogic.allIconCells[id].RefreshView();
    }

    private void OnParsingData(object data)
    {
        if (data is string)
        {
            mainLogic.TempIcon.gameObject.SetActive(true);
            creatTargetTemplate = (string)data;
            mainLogic.TempIcon.GetChild(0).GetComponent<Image>().sprite = UIManager.Instance.PicBObjects[creatTargetTemplate];
        }
    }

    private void OnShowMarkPoint(Vector2 pos)
    {
        GameObject markPointGo = GameObject.Instantiate(mainLogic.markPointPrefab, mainLogic.iconCellParent);
        markPointGo.GetComponent<RectTransform>().anchoredPosition = worldPos2UiPos(uiPos2WorldPos(pos));
        markPointGo.SetActive(true);
        GameObject.Destroy(markPointGo, 5);
    }

    private void OnCrash(string id)
    {
        mainLogic.allIconCells[id].gameObject.SetActive(false);
    }

    public MapOperate_Normal(UIMap mainLogic) : base(mainLogic)
    {
    }
}

public struct RightClickShowInfo
{
    public Vector2 PointPos;
    public List<SkillData> ShowSkillDatas;
    public UnityAction<SkillType> OnTriggerCallBack;
}

public struct AirportAircraftsInfo
{
    public Vector2 PointPos;
    public List<string> AircraftDatas;
    public UnityAction<string> OnRightClickCallBack;
}