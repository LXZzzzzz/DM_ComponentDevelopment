using System.Collections.Generic;
using System.IO;
using Enums;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.PathPart;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EventType = Enums.EventType;
using System.Collections;

public enum OperatorState
{
    Normal,
    CreatAndEditor,
    PlanningPath,
    DqNormal
}

public class UIMap : BasePanel, IPointerClickHandler
{
    [HideInInspector] public RectTransform mapView;
    [HideInInspector] public Transform iconCellParent;
    [HideInInspector] public IconCellBase airIconPrefab, pointIconPrefab, ziYuanIconPrefab;
    [HideInInspector] public GameObject markPointPrefab;

    private Vector2 uiCameraSize;
    public Dictionary<string, IconCellBase> allIconCells; //存储地图上的所有点
    [HideInInspector] public float mapBLx, mapBLz;
    public Material dashedLineMat;

    private Dictionary<OperatorState, MapOperateLogicBase> mapLogics;
    private MapOperateLogicBase currentMapLogic;

    public RectTransform TempIcon;
    private GameObject routeDecorateGo;
    private RectTransform startPoint, middlePoint, endPoint;
    public OperatorState CurrentState;

    private List<ZiYuanBase> zaiquTemplates;

    private string personData, misDescriptionData, kongguanData, tianqiData;

    private GameObject leftPart, rightPart;
    public List<RectTransform> beRefreshView;
    [HideInInspector] public Toggle distanceMeasurementTog;
    [HideInInspector] public Transform meaDisMask;

    public override void Init()
    {
        base.Init();
        mapView = transform.Find("maxMap/map").GetComponent<RectTransform>();
        iconCellParent = transform.Find("maxMap/objects").GetComponent<RectTransform>();
        meaDisMask = transform.Find("maxMap/meaDisMask").GetComponent<RectTransform>();
        airIconPrefab = transform.Find("prefabs/airCell").GetComponent<AirIconCell>();
        pointIconPrefab = transform.Find("prefabs/pointCell").GetComponent<PointIconCell>();
        ziYuanIconPrefab = transform.Find("prefabs/ziyuanCell").GetComponent<ZiYuanIconCell>();
        markPointPrefab = transform.Find("prefabs/markPoint").gameObject;
        TempIcon = transform.Find("maxMap/TempIcon").GetComponent<RectTransform>();
        leftPart = transform.Find("LeftUpPart").gameObject;
        rightPart = transform.Find("RightUpPart").gameObject;
        GetControl<Toggle>("tog_Map").onValueChanged.AddListener(OnCloseMap);
        GetControl<Button>("Btn_CreatFirePoint").onClick.AddListener(() => OnOpenCreatZaiqu(1));
        GetControl<Button>("Btn_CreatDisaster").onClick.AddListener(() => OnOpenCreatZaiqu(2));
        GetControl<Button>("Btn_DeleteDis").onClick.AddListener(() => UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowNoInputData((int)ShowZyDataType.deleteDisShow)));
        GetControl<Button>("Btn_ChangeTQ").onClick.AddListener(() => UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowNoInputData((int)ShowZyDataType.TqChange)));
        GetControl<Button>("Btn_Malfunction").onClick.AddListener(() => UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowNoInputData((int)ShowZyDataType.zbgzChange)));
        GetControl<Button>("Btn_Zqxx").onClick.AddListener(() => OnClickZqxx(1));
        GetControl<Button>("Btn_Zbxx").onClick.AddListener(() => OnClickZqxx(2));
        GetControl<Button>("Btn_Ryxx").onClick.AddListener(() => OnClickZqxx(3));
        GetControl<Button>("Btn_Kgxx").onClick.AddListener(() => OnClickZqxx(4));
        GetControl<Button>("Btn_Rwxx").onClick.AddListener(() => OnClickZqxx(5));
        GetControl<Button>("Btn_Hxsb").onClick.AddListener(OnClickHxsb);
        GetControl<Button>("Btn_Rwqzb").onClick.AddListener(() => OnClickZqxx(6));
        GetControl<Button>("Btn_Zcfx").onClick.AddListener(OnClickZcfx);
        GetControl<Button>("Btn_Xdrw").onClick.AddListener(OnClickSqrwzx);
        GetControl<Button>("Btn_Dmzb").onClick.AddListener(() => OnClickZqxx(7));
        GetControl<Button>("Btn_Rwghwc").onClick.AddListener(OnClickRwghwc);
        GetControl<Button>("Btn_Export").onClick.AddListener(() => OnImportAndExportData(false));
        GetControl<Button>("Btn_Import").onClick.AddListener(() => OnImportAndExportData(true));
        GetControl<Button>("Btn_ReturnBack").onClick.AddListener(() => OnAskForReturn(2));
        GetControl<Button>("Btn_ReturnRepair").onClick.AddListener(() => OnAskForReturn(1));
        GetControl<Button>("Btn_NewDisaster").onClick.AddListener(OnSendNewDisaster);
        GetControl<Button>("Btn_Tqbhsb").onClick.AddListener(() => OnSendReport(1));
        GetControl<Button>("Btn_Zbgzsb").onClick.AddListener(() => OnSendReport(2));
        GetControl<Button>("Btn_Set").onClick.AddListener(OnSetData);
        GetControl<Button>("Btn_TaskBg").onClick.AddListener(() => UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowNoInputData((int)ShowZyDataType.TaskBgShow)));
        GetControl<Button>("Btn_CompleteBgSet").onClick.AddListener(
            () =>
            {
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendCompleteTaskBgSet, "");
                EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "已完成任务设置，可以开始训练");
            });
        GetControl<Button>("Btn_GroundSupport").onClick.AddListener(() => UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowNoInputData((int)ShowZyDataType.GroundSupport)));
        GetControl<Button>("Btn_GroundDisaster").onClick.AddListener(() => UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowNoInputData((int)ShowZyDataType.GroundDisaster)));
        GetControl<Button>("Btn_Equips").onClick.AddListener(() => UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowNoInputData((int)ShowZyDataType.EquipsShow)));
        GetControl<Button>("Btn_Person").onClick.AddListener(() => UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowNoInputData((int)ShowZyDataType.PersonShow)));
        distanceMeasurementTog = GetControl<Toggle>("distanceMeasurementTog");
        
        routeDecorateGo = transform.Find("maxMap/objects/routeDecorate").gameObject;
        startPoint = transform.Find("maxMap/objects/routeDecorate/startPoint").GetComponent<RectTransform>();
        middlePoint = transform.Find("maxMap/objects/routeDecorate/middlePoint").GetComponent<RectTransform>();
        endPoint = transform.Find("maxMap/objects/routeDecorate/endPoint").GetComponent<RectTransform>();
        dashedLineMat = transform.Find("Cube").GetComponent<MeshRenderer>().material;

        mapLogics = new Dictionary<OperatorState, MapOperateLogicBase>();
        allIconCells = new Dictionary<string, IconCellBase>();
        zaiquTemplates = new List<ZiYuanBase>();
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        //刚开始都要显示地图的，都为普通模式
        //当一级点击创建某个装备时，切换为创建模式，并传过来要创建对象的ID
        //当一级发布方案后，把自己状态切换为普通，二级收到消息后切换为创建，创建完立刻切回普通

        LoadMap(Application.dataPath + $"/LibRes/TerrainLib/{UIManager.Instance.terrainName}/{UIManager.Instance.terrainName}.png");

        Debug.LogError("地图大小："+(Vector2)userData);
        mapBLx = ((Vector2)userData).x / mapView.sizeDelta.x;
        mapBLz = ((Vector2)userData).y / mapView.sizeDelta.y;

        uiCameraSize = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
        EventManager.Instance.AddEventListener<int>(EventType.SwitchMapModel.ToString(), SwithMode);

        //刚开始切换为编辑模式，并通知初始化场景
        SwitchMapLogic(OperatorState.CreatAndEditor);
        // EventManager.Instance.EventTrigger<object>(EventType.TransferEditingInfo.ToString(), allBObjects);
        EventManager.Instance.AddEventListener(EventType.SetMyEquipIconLayer.ToString(), setAirCellMaxLayer);
        EventManager.Instance.AddEventListener<List<string>>(EventType.ChangeJiZhangView.ToString(), OnChangeZyShow);
        EventManager.Instance.AddEventListener<string>(EventType.HideGoIcon.ToString(), OnHideZyShow);
        EventManager.Instance.AddEventListener<string>(EventType.TransferPersonData.ToString(), OnGetPersonData);
        EventManager.Instance.AddEventListener<string>(EventType.TransferMisDescription.ToString(), OnGetmisDescription);
        EventManager.Instance.AddEventListener<string>(EventType.TransferKongguanData.ToString(), OnGetKongGuanData);
        EventManager.Instance.AddEventListener<string>(EventType.TransferTianqiData.ToString(), OnGetTianqiData);
        EventManager.Instance.AddEventListener(EventType.OpenMap.ToString(), OnOpenMap);
        EventManager.Instance.AddEventListener(EventType.captureMap.ToString(), OnCaptureMap);
        // 当前UI对象的局部Y轴
        localYAxis = middlePoint.transform.up;

        GetAllZaiquTemplate();
        GetControl<Toggle>("xxqrTog").gameObject.SetActive(MyDataInfo.MyLevel == 1);
        GetControl<Toggle>("jzOperatorTog").gameObject.SetActive(MyDataInfo.MyLevel == 3);
        GetControl<Button>("Btn_CreatFirePoint").gameObject.SetActive(MyDataInfo.gameScene == 1);
        GetControl<Button>("Btn_CreatDisaster").gameObject.SetActive(MyDataInfo.gameScene == 2);
        
        rightPart.SetActive(!MyDataInfo.isPlayBack);
    }

    private void GetAllZaiquTemplate()
    {
        if (zaiquTemplates?.Count != 0) return;
        //获取场景全部组件，并找到灾区模板
        for (int i = 0; i < allBObjects.Length; i++)
        {
            var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
            if (tagItem == null || tagItem.SubTags.Find(y => y.Id == 6 || y.Id == 4) == null) continue;

            if (allBObjects[i].transform.GetChild(0).GetComponent<ZiYuanBase>() != null)
            {
                var zyItem = allBObjects[i].transform.GetChild(0).GetComponent<ZiYuanBase>();
                zaiquTemplates.Add(zyItem);
            }
        }
    }

    private Texture2D m_Tex;

    private void LoadMap(string path)
    {
#if !UNITY_EDITOR
        m_Tex = new Texture2D(1, 1);
        //读取图片字节流
        m_Tex.LoadImage(ReadPNG(path));

        //变换格式
        Sprite tempSprite = Sprite.Create(m_Tex, new Rect(0, 0, m_Tex.width, m_Tex.height), new Vector2(10, 10));
        mapView.GetComponent<Image>().sprite = tempSprite; //赋值 
#endif
    }

    private byte[] ReadPNG(string path)
    {
        Debug.Log(path);
        FileStream fileStream = new FileStream(path, FileMode.Open, System.IO.FileAccess.Read);

        fileStream.Seek(0, SeekOrigin.Begin);
        //创建文件长度的buffer
        byte[] binary = new byte[fileStream.Length];
        fileStream.Read(binary, 0, (int)fileStream.Length);
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        return binary;
    }

    public override void HideMe()
    {
        base.HideMe();
        currentMapLogic?.OnExit();
        EventManager.Instance.RemoveEventListener<int>(EventType.SwitchMapModel.ToString(), SwithMode);
        EventManager.Instance.RemoveEventListener(EventType.SetMyEquipIconLayer.ToString(), setAirCellMaxLayer);
        EventManager.Instance.RemoveEventListener<List<string>>(EventType.ChangeJiZhangView.ToString(), OnChangeZyShow);
        EventManager.Instance.RemoveEventListener<string>(EventType.HideGoIcon.ToString(), OnHideZyShow);
        EventManager.Instance.RemoveEventListener<string>(EventType.TransferPersonData.ToString(), OnGetPersonData);
        EventManager.Instance.RemoveEventListener<string>(EventType.TransferMisDescription.ToString(), OnGetmisDescription);
        EventManager.Instance.RemoveEventListener<string>(EventType.TransferKongguanData.ToString(), OnGetKongGuanData);
        EventManager.Instance.RemoveEventListener<string>(EventType.TransferTianqiData.ToString(), OnGetTianqiData);
        EventManager.Instance.RemoveEventListener(EventType.OpenMap.ToString(), OnOpenMap);
        EventManager.Instance.RemoveEventListener(EventType.captureMap.ToString(), OnCaptureMap);
    }

    private void SwithMode(int mode)
    {
        switch (mode)
        {
            case 0:
                SwitchMapLogic(OperatorState.Normal);
                break;
            case 1:
                SwitchMapLogic(OperatorState.CreatAndEditor);
                break;
            case 2:
                SwitchMapLogic(OperatorState.PlanningPath);
                break;
            case 3:
                SwitchMapLogic(OperatorState.DqNormal);
                break;
        }
    }

    public void SwitchMapLogic(OperatorState targetState)
    {
#if !UNITY_EDITOR
     sender.LogError("当前地图模式："+targetState);
#else
        Debug.LogError("当前地图模式：" + targetState);
#endif
        bool isCreat = !mapLogics.ContainsKey(targetState);
        switch (targetState)
        {
            case OperatorState.Normal:
                if (isCreat) mapLogics.Add(OperatorState.Normal, new MapOperate_Normal(this));
                break;
            case OperatorState.PlanningPath:
                if (isCreat) mapLogics.Add(OperatorState.PlanningPath, new MapOperate_PlanningPath(this));
                break;
            case OperatorState.CreatAndEditor:
                if (isCreat) mapLogics.Add(OperatorState.CreatAndEditor, new MapOperate_CreatAndEditor(this));
                break;
            case OperatorState.DqNormal:
                if (isCreat) mapLogics.Add(OperatorState.DqNormal, new MapOperate_DqNormal(this));
                break;
        }

        mapLogics[targetState].setCanvanceSize(uiCameraSize);

        currentMapLogic?.OnExit();
        currentMapLogic = mapLogics[targetState];
        currentMapLogic?.OnEnter();
        CurrentState = targetState;
    }

    private void OnCloseMap(bool isShowMap)
    {
        //点击了切换三维地图或二维地图
        EventManager.Instance.EventTrigger(EventType.CameraSwitch.ToString(), !isShowMap);
        // UIManager.Instance.GetUIPanel<UIAttributeView>(UIName.UIAttributeView).gameObject.SetActive(isShowMap);
    }

    private void OnOpenCreatZaiqu(int type)
    {
        SwitchMapLogic(OperatorState.CreatAndEditor);
        for (int i = 0; i < zaiquTemplates.Count; i++)
        {
            switch (type)
            {
                case 1:
                    //找到火灾
                    if (zaiquTemplates[i].ZiYuanType == ZiYuanType.SourceOfAFire)
                    {
                        EventManager.Instance.EventTrigger<object>(Enums.EventType.TransferEditingInfo.ToString(), zaiquTemplates[i].BobjectId);
                        return;
                    }

                    break;
                case 2:
                    //找到灾区
                    if (zaiquTemplates[i].ZiYuanType == ZiYuanType.DisasterArea)
                    {
                        EventManager.Instance.EventTrigger<object>(Enums.EventType.TransferEditingInfo.ToString(), zaiquTemplates[i].BobjectId);

                        return;
                    }

                    break;
            }
        }

        SwitchMapLogic(OperatorState.DqNormal);
        UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, new ConfirmatonInfo() { showStrInfo = "选择的灾情类型该地图不存在", type = showType.tipView });
    }


    private EquipBase[] sceneAllObjs;

    private void Start()
    {
#if UNITY_EDITOR
        MyDataInfo.sceneAllEquips = new List<EquipBase>();
        // uiCameraSize = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
        Debug.Log("uiCameraSize：" + uiCameraSize);
        sceneAllObjs = GameObject.FindObjectsOfType<EquipBase>();
        Debug.LogError("场景中有：" + sceneAllObjs.Length);
        for (int i = 0; i < sceneAllObjs?.Length; i++)
        {
            var item = sceneAllObjs[i];
            item.BObjectId = ((i + 1) * 11111111).ToString();
            MyDataInfo.sceneAllEquips.Add(item);
        }

        allIconCells = new Dictionary<string, IconCellBase>();
        mapLogics = new Dictionary<OperatorState, MapOperateLogicBase>();

        SwitchMapLogic(OperatorState.PlanningPath);
        EventManager.Instance.EventTrigger<object>(EventType.TransferEditingInfo.ToString(), MyDataInfo.sceneAllEquips);
#endif
    }

    //该段逻辑是点击了地图上任意图标的回调，包含两部分：1.点击装备图表 2.点击非装备图标(包含系统实体、本地实体)
    public void OnChooseObj(string objId, PointerEventData.InputButton button)
    {
        IconCellBase targetIconCell = null;
        foreach (var iconCell in allIconCells)
        {
            if (string.Equals(iconCell.Key, objId))
            {
                targetIconCell = iconCell.Value;
                break;
            }
        }

        if (targetIconCell == null) return;

        switch (button)
        {
            case PointerEventData.InputButton.Left:
                currentMapLogic?.OnLeftClickIcon(targetIconCell);
                break;
            case PointerEventData.InputButton.Right:
                currentMapLogic?.OnRightClickIcon(targetIconCell);
                break;
        }
    }

    private void Update()
    {
        GetControl<Button>("Btn_TaskBgSetting").gameObject.SetActive(MyDataInfo.MyLevel == -1 && MyDataInfo.gameState == GameState.None);
        GetControl<Button>("Btn_CompleteBgSet").gameObject.SetActive(MyDataInfo.MyLevel == -1 && MyDataInfo.gameState == GameState.None);
        GetControl<Button>("Btn_PeculiarSetting").gameObject.SetActive(MyDataInfo.MyLevel == -1 && MyDataInfo.gameState >= GameState.GameStart);
        GetControl<Button>("Btn_Hxsb").gameObject.SetActive(MyDataInfo.MyLevel == 1 && MyDataInfo.gameState == GameState.CompleteTaskBgSet);
        if (!(MyDataInfo.MyLevel == 1 && MyDataInfo.gameState == GameState.CompleteTaskBgSet)) GetControl<Toggle>("xxqrTog").isOn = false;
        GetControl<Toggle>("xxqrTog").gameObject.SetActive(MyDataInfo.MyLevel == 1 && MyDataInfo.gameState == GameState.CompleteTaskBgSet);
        GetControl<Button>("Btn_Rwqzb").gameObject.SetActive(MyDataInfo.MyLevel == 2 && MyDataInfo.gameState == GameState.ReleaseProgramme);
        GetControl<Button>("Btn_Zcfx").gameObject.SetActive(MyDataInfo.MyLevel == 2 && MyDataInfo.gameState == GameState.ReleaseProgramme);
        GetControl<Button>("Btn_Xdrw").gameObject.SetActive(MyDataInfo.MyLevel == 2 && MyDataInfo.gameState == GameState.ReleaseProgramme);
        GetControl<Button>("Btn_Dmzb").gameObject.SetActive(MyDataInfo.MyLevel == 3 && MyDataInfo.gameState >= GameState.AgreeTaskExecute);
        GetControl<Button>("Btn_Rwghwc").gameObject.SetActive(MyDataInfo.MyLevel == 3 && MyDataInfo.gameState >= GameState.AgreeTaskExecute);
        GetControl<Button>("Btn_Export").gameObject.SetActive(MyDataInfo.MyLevel == 3 && MyDataInfo.gameState >= GameState.GameStart);
        GetControl<Button>("Btn_Import").gameObject.SetActive(MyDataInfo.MyLevel == 3 && MyDataInfo.gameState >= GameState.AgreeTaskExecute);
        GetControl<Button>("Btn_Set").gameObject.SetActive(MyDataInfo.MyLevel == 3 && MyDataInfo.gameState >= GameState.GameStart);
        GetControl<Button>("Btn_ReturnBack").gameObject.SetActive(MyDataInfo.MyLevel == 3 && MyDataInfo.gameState >= GameState.GameStart);
        GetControl<Button>("Btn_ReturnRepair").gameObject.SetActive(false);
        GetControl<Button>("Btn_NewDisaster").gameObject.SetActive(MyDataInfo.MyLevel == 3 && MyDataInfo.gameState >= GameState.GameStart);
        GetControl<Button>("Btn_Tqbhsb").gameObject.SetActive(MyDataInfo.MyLevel == 3 && MyDataInfo.gameState >= GameState.GameStart);
        GetControl<Button>("Btn_Zbgzsb").gameObject.SetActive(MyDataInfo.MyLevel == 3 && MyDataInfo.gameState >= GameState.GameStart);
        beRefreshView.ForEach(LayoutRebuilder.ForceRebuildLayoutImmediate);
        currentMapLogic?.OnUpdate();
        routeDecorateGo.transform.SetAsLastSibling();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //这里是检测点击区域是否在地图内部
        Vector2 newPos = resolutionRatioNormalized(eventData.position);

        Vector2 point = mousePos2UI(newPos) + new Vector2(mapView.sizeDelta.x / 2, mapView.sizeDelta.y / 2);
        if (point.x < 0 || point.y < 0 || point.x > mapView.sizeDelta.x || point.y > mapView.sizeDelta.y) return;
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                currentMapLogic?.OnLeftClickMap(newPos);
                break;
            case PointerEventData.InputButton.Right:
                currentMapLogic?.OnRightClickMap(newPos);
                break;
        }
    }

    private void setAirCellMaxLayer()
    {
        foreach (var iconCell in allIconCells)
        {
            var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, iconCell.Key));
            if (itemEquip && string.Equals(itemEquip.BeLongToCommanderId, MyDataInfo.leadId))
            {
                iconCell.Value.transform.SetAsLastSibling();
            }
        }
    }

    private void OnChangeZyShow(List<string> showZys)
    {
        foreach (var iconCell in allIconCells)
        {
            if (iconCell.Value is ZiYuanIconCell)
                (iconCell.Value as ZiYuanIconCell)?.OnSetShow(showZys.Find(x => string.Equals(x, iconCell.Key)) != null);
        }
    }

    private void OnHideZyShow(string id)
    {
        foreach (var iconCell in allIconCells)
        {
            if (iconCell.Value is AirIconCell) continue;
            if (string.IsNullOrEmpty(id)) iconCell.Value.gameObject.SetActive(true);
            else iconCell.Value.gameObject.SetActive(!string.Equals(id, iconCell.Key));
        }
    }

    private void OnGetPersonData(string pd)
    {
        personData = pd;
    }

    private void OnGetmisDescription(string mis)
    {
        misDescriptionData = mis;
    }

    private void OnGetKongGuanData(string kg)
    {
        kongguanData = kg;
    }

    private void OnGetTianqiData(string tq)
    {
        tianqiData = tq;
    }

    private void OnOpenMap()
    {
        GetControl<Toggle>("tog_Map").isOn = true;
    }

    private void OnClickZqxx(int info)
    {
        // EventManager.Instance.EventTrigger(EventType.ShowMisDescription.ToString());
        string itemShowStr = "";
        switch (info)
        {
            case 1:
                UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowStrInputData((int)ShowZyDataType.zqxxShow, misDescriptionData));
                itemShowStr = "灾情信息查看";
                break;
            case 2:
                UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowNoInputData((int)ShowZyDataType.zbxxShow));
                itemShowStr = "装备信息查看";
                break;
            case 3:
                UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowStrInputData((int)ShowZyDataType.ryxxShow, personData));
                itemShowStr = "人员信息查看";
                break;
            case 4:
                UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowStrInputData((int)ShowZyDataType.kgxxShow, kongguanData));
                itemShowStr = "空管信息查看";
                break;
            case 5:
                UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowStrInputData((int)ShowZyDataType.rwxxShow, tianqiData));
                itemShowStr = "任务信息查看";
                break;
            case 6:
                UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowNoInputData((int)ShowZyDataType.rwqzbShow));
                itemShowStr = "任务前准备界面，输入油量和装载量";
                break;
            case 7:
                UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowNoInputData((int)ShowZyDataType.dmzbShow));
                itemShowStr = "地面前准备，输入飞机载油量，装载量";
                break;
        }
    }

    private void OnClickHxsb()
    {
        UIManager.Instance.ShowPanel<UIAirLineInfoShow>(UIName.UIAirLineInfoShow, null);
    }

    private void OnClickZcfx()
    {
        ConfirmatonInfo infoa = new ConfirmatonInfo
        {
            type = showType.secondConfirm, showStrInfo = "是否开始转场飞行?",
            sureCallBack = (a) => { EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendFerryFlights, ""); }
        };
        UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
    }

    private void OnClickSqrwzx()
    {
        ConfirmatonInfo infoa = new ConfirmatonInfo
        {
            type = showType.secondConfirm, showStrInfo = "是否向机长下达任务?",
            sureCallBack = (a) => { EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendAgreeTaskExecute, ""); }
        };
        UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
    }

    private void OnClickRwghwc()
    {
        // UIManager.Instance.ShowPanel<>();

        if (MyDataInfo.gameState == GameState.AgreeTaskExecute)
        {
            ConfirmatonInfo infoa = new ConfirmatonInfo
            {
                type = showType.secondConfirm, showStrInfo = "是否确认任务规划完成?", sureCallBack = (a) =>
                {
                    //这里发出消息，我完成了任务规划
                    var myEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BeLongToCommanderId, MyDataInfo.leadId));
                    if (myEquip == null)
                    {
                        Debug.LogError("身份错了，找不到我的飞机");
                        return;
                    }

                    EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendTaskPlanningCompleted, myEquip.BObjectId);
                    EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendRwghData, PathPointManager.Instance.PackedData());
                    EventManager.Instance.EventTrigger(EventType.CloseEditorModel.ToString(), new Vector2());
                    EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendTrainPointSucInfo, TrainsPintType.JZCompletePlan.ToString());
                }
            };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
            return;
        }

        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendRwghData, PathPointManager.Instance.PackedData());
        EventManager.Instance.EventTrigger(EventType.CloseEditorModel.ToString(), new Vector2());
        SwitchMapLogic(OperatorState.DqNormal);
    }

    private void OnImportAndExportData(bool isEnter)
    {
        if (isEnter)
        {
            //导入逻辑
            string data = FileOperator.LoadData_Txt(Application.dataPath + "/MapLib/Scheme");

            if (string.IsNullOrEmpty(data)) return;
            SwitchMapLogic(OperatorState.PlanningPath);
            EventManager.Instance.EventTrigger(EventType.LoadPathPlanningData.ToString(), data);
        }
        else
        {
            //导出逻辑
            FileOperator.SaveAsData_Txt(PathPointManager.Instance.PackedData(), Application.dataPath + "/MapLib/Scheme");
        }
    }

    private void OnAskForReturn(int state)
    {
        EventManager.Instance.EventTrigger(EventType.AskForReturnTrigger.ToString(), state);
    }

    private void OnSendNewDisaster()
    {
        //发送新发现灾情，并且提示
        var myEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BeLongToCommanderId, MyDataInfo.leadId));
        if (myEquip == null)
        {
            Debug.LogError("身份错了，找不到我的飞机");
            return;
        }

        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendDiscoverNewDisaster, myEquip.BObjectId);
        EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "已申报新灾情");
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendTrainPointSucInfo, TrainsPintType.JZSendTqInfo.ToString());
    }

    private void OnSendReport(int type)
    {
        var myEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BeLongToCommanderId, MyDataInfo.leadId));
        if (myEquip == null)
        {
            Debug.LogError("身份错了，找不到我的飞机");
            return;
        }

        EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "上报成功");
        if (type == 1) EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendReportTianQi, myEquip.BObjectId);
        if (type == 2) EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendReportZbgz, myEquip.BObjectId);
    }

    private void OnSetData()
    {
        SwitchMapLogic(OperatorState.PlanningPath);
    }

    private Vector3 localYAxis;

    public void OnShowRouteArrow(bool isShow, Vector2 startPos, Vector2 endPos)
    {
        routeDecorateGo.SetActive(isShow);
        startPoint.anchoredPosition = startPos;
        endPoint.anchoredPosition = endPos;
        middlePoint.anchoredPosition = startPos + (endPos - startPos).normalized * (endPos - startPos).magnitude / 2;

        Vector3 targetDirection = (endPos - startPos).normalized;

        // 计算旋转轴
        Vector3 rotationAxis = Vector3.Cross(localYAxis, targetDirection);

        // 计算旋转角度
        float angle = Mathf.Acos(Vector3.Dot(localYAxis, targetDirection));

        // 创建四元数表示绕rotationAxis旋转angle弧度
        Quaternion rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, rotationAxis);
        middlePoint.rotation = rotation;
        endPoint.rotation = rotation;
    }

    //针对屏幕分辨率对应鼠标位置进行归一化
    public Vector2 resolutionRatioNormalized(Vector2 nowPos)
    {
        float xbl = Screen.width / uiCameraSize.x;
        float ybl = Screen.height / uiCameraSize.y;
        return new Vector2(nowPos.x / xbl, nowPos.y / ybl);
    }

    public Vector2 resolutionRatioNormalized_size(Vector2 nowSize)
    {
        float xbl = Screen.width / uiCameraSize.x;
        float ybl = Screen.height / uiCameraSize.y;
        return new Vector2(nowSize.x * xbl, nowSize.y * ybl);
    }

    /// <summary>
    /// 鼠标位置转UI点
    /// </summary>
    /// <returns></returns>
    public Vector2 mousePos2UI(Vector2 pos)
    {
        Vector2 point = new Vector2(pos.x, pos.y) - new Vector2(uiCameraSize.x / 2, uiCameraSize.y / 2);
        return point;
    }

    private void OnCaptureMap()
    {
        StartCoroutine(getScreenTexture(mapView));
    }

    private IEnumerator getScreenTexture(RectTransform rectT)
    {
        yield return 1;
        UIManager.Instance.GetUIPanel<UITopMenuView>(UIName.UITopMenuView).gameObject.SetActive(false);
        if (MyDataInfo.MyLevel != -1)
            UIManager.Instance.GetUIPanel<UICommanderView>(UIName.UICommanderView).gameObject.SetActive(false);
        else
            UIManager.Instance.GetUIPanel<UIDirectorView>(UIName.UIDirectorView).gameObject.SetActive(false);
        leftPart.SetActive(false);
        rightPart.SetActive(false);
        yield return new WaitForEndOfFrame();

        Texture2D texture2ds = new Texture2D((int)rectT.rect.width, (int)rectT.rect.height, TextureFormat.RGB24, true);
        float x = rectT.localPosition.x + (Screen.width - rectT.rect.width) / 2;
        float y = rectT.localPosition.y + (Screen.height - rectT.rect.height) / 2;
        Rect position = new Rect(x, y, rectT.rect.width, rectT.rect.height);
        texture2ds.ReadPixels(position, 0, 0, true); //按照设定区域读取像素；注意是以左下角为原点读取
        texture2ds.Apply();
        //保存到streamingAssets
        byte[] bytes = texture2ds.EncodeToJPG();
        if (!Directory.Exists(Path.Combine(Application.dataPath, "MapLib", "Images")))
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "MapLib", "Images"));
        string filename = Path.Combine(Application.dataPath, "MapLib", "Images", "Screenshot.png");
        File.WriteAllBytes(filename, bytes);

        yield return 1;
        leftPart.SetActive(true);
        rightPart.SetActive(true);
        rightPart.SetActive(!MyDataInfo.isPlayBack);
        if (MyDataInfo.MyLevel == -1) GetControl<Toggle>("tog_Scene").isOn = true;
        UIManager.Instance.GetUIPanel<UITopMenuView>(UIName.UITopMenuView).gameObject.SetActive(true);
        if (MyDataInfo.MyLevel != -1)
            UIManager.Instance.GetUIPanel<UICommanderView>(UIName.UICommanderView).gameObject.SetActive(true);
        else
            UIManager.Instance.GetUIPanel<UIDirectorView>(UIName.UIDirectorView).gameObject.SetActive(true);
    }
}

public abstract class MapOperateLogicBase
{
    protected UIMap mainLogic;
    protected Vector2 canvanceSize;

    public void setCanvanceSize(Vector2 size)
    {
        canvanceSize = size;
    }

    public MapOperateLogicBase(UIMap mainLogic)
    {
        this.mainLogic = mainLogic;
    }

    public abstract void OnEnter();
    public abstract void OnLeftClickIcon(IconCellBase clickIcon);
    public abstract void OnRightClickIcon(IconCellBase clickIcon);
    public abstract void OnUpdate();
    public abstract void OnLeftClickMap(Vector2 pos);
    public abstract void OnRightClickMap(Vector2 pos);
    public abstract void OnExit();

    protected Vector2 worldPos2UiPos(Vector3 pos)
    {
        return new Vector2(pos.x / mainLogic.mapBLx /* - mainLogic.mapView.sizeDelta.x / 2*/, pos.z / mainLogic.mapBLz /*- mainLogic.mapView.sizeDelta.y / 2*/);
    }

    protected Vector3 uiPos2WorldPos(Vector2 pos)
    {
        return new Vector3((pos.x - canvanceSize.x / 2 /*+ mainLogic.mapView.sizeDelta.x / 2*/) * mainLogic.mapBLx, 0,
            (pos.y - canvanceSize.y / 2 /*+ mainLogic.mapView.sizeDelta.y / 2*/) * mainLogic.mapBLz);
    }
}