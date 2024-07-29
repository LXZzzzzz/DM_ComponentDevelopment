using System.Collections.Generic;
using System.IO;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EventType = Enums.EventType;

public enum OperatorState
{
    Normal,
    CreatAndEditor,
    PlanningPath
}

public class UIMap : BasePanel, IPointerClickHandler
{
    [HideInInspector] public RectTransform mapView;
    [HideInInspector] public Transform iconCellParent;
    [HideInInspector] public IconCellBase airIconPrefab, pointIconPrefab, ziYuanIconPrefab;

    private Vector2 uiCameraSize;
    public List<EquipBase> allObjModels;
    public Dictionary<string, IconCellBase> allIconCells; //存储地图上的所有点
    [HideInInspector] public float mapBLx, mapBLz;

    private Dictionary<OperatorState, MapOperateLogicBase> mapLogics;
    private MapOperateLogicBase currentMapLogic;

    public RectTransform TempIcon;
    private GameObject routeDecorateGo;
    private RectTransform startPoint, middlePoint, endPoint;

    public override void Init()
    {
        base.Init();
        mapView = transform.Find("maxMap/map").GetComponent<RectTransform>();
        iconCellParent = transform.Find("maxMap/objects").GetComponent<RectTransform>();
        airIconPrefab = transform.Find("prefabs/airCell").GetComponent<AirIconCell>();
        pointIconPrefab = transform.Find("prefabs/pointCell").GetComponent<PointIconCell>();
        ziYuanIconPrefab = transform.Find("prefabs/ziyuanCell").GetComponent<ZiYuanIconCell>();
        TempIcon = transform.Find("maxMap/TempIcon").GetComponent<RectTransform>();
        GetControl<Toggle>("tog_Map").onValueChanged.AddListener(OnCloseMap);
        routeDecorateGo = transform.Find("maxMap/objects/routeDecorate").gameObject;
        startPoint = transform.Find("maxMap/objects/routeDecorate/startPoint").GetComponent<RectTransform>();
        middlePoint = transform.Find("maxMap/objects/routeDecorate/middlePoint").GetComponent<RectTransform>();
        endPoint = transform.Find("maxMap/objects/routeDecorate/endPoint").GetComponent<RectTransform>();

        mapLogics = new Dictionary<OperatorState, MapOperateLogicBase>();
        allObjModels = new List<EquipBase>();
        allIconCells = new Dictionary<string, IconCellBase>();
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        //刚开始都要显示地图的，都为普通模式
        //当一级点击创建某个装备时，切换为创建模式，并传过来要创建对象的ID
        //当一级发布方案后，把自己状态切换为普通，二级收到消息后切换为创建，创建完立刻切回普通

        LoadMap(Application.dataPath + $"/LibRes/TerrainLib/{UIManager.Instance.terrainName}/{UIManager.Instance.terrainName}.png");

        mapBLx = ((Vector2)userData).x / mapView.sizeDelta.x;
        mapBLz = ((Vector2)userData).y / mapView.sizeDelta.y;

        uiCameraSize = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
        EventManager.Instance.AddEventListener<int>(EventType.SwitchMapModel.ToString(), SwithMode);

        //刚开始切换为编辑模式，并通知初始化场景
        SwitchMapLogic(OperatorState.CreatAndEditor);
        EventManager.Instance.EventTrigger<object>(EventType.TransferEditingInfo.ToString(), allBObjects);
        EventManager.Instance.AddEventListener(EventType.SetMyEquipIconLayer.ToString(), setAirCellMaxLayer);
        // 当前UI对象的局部Y轴
        localYAxis = middlePoint.transform.up;
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
        }
    }

    public void SwitchMapLogic(OperatorState targetState)
    {
#if !UNITY_EDITOR
     sender.LogError("当前地图模式："+targetState);
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
        }

        mapLogics[targetState].setCanvanceSize(uiCameraSize);

        currentMapLogic?.OnExit();
        currentMapLogic = mapLogics[targetState];
        currentMapLogic?.OnEnter();
    }

    private void OnCloseMap(bool isShowMap)
    {
        //点击了切换三维地图或二维地图
        EventManager.Instance.EventTrigger(EventType.CameraSwitch.ToString(), !isShowMap);
        UIManager.Instance.GetUIPanel<UIAttributeView>(UIName.UIAttributeView).gameObject.SetActive(isShowMap);
    }

    private EquipBase[] sceneAllObjs;

    private void Start()
    {
        return;
#if UNITY_EDITOR
        mapBLx = 3600f / mapView.sizeDelta.x;
        allObjModels = new List<EquipBase>();
        // uiCameraSize = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
        Debug.Log("uiCameraSize：" + uiCameraSize);
        sceneAllObjs = GameObject.FindObjectsOfType<EquipBase>();
        for (int i = 0; i < sceneAllObjs?.Length; i++)
        {
            var item = sceneAllObjs[i];
            item.BObjectId = ((i + 1) * 11111111).ToString();
            allObjModels.Add(item);
        }

        allIconCells = new Dictionary<string, IconCellBase>();
        mapLogics = new Dictionary<OperatorState, MapOperateLogicBase>();

        SwitchMapLogic(OperatorState.CreatAndEditor);
        EventManager.Instance.EventTrigger<object>(EventType.TransferEditingInfo.ToString(), allObjModels);
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
        currentMapLogic?.OnUpdate();
        routeDecorateGo.transform.SetAsLastSibling();
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.LogError(Screen.width + "=" + Screen.height);
        }
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

    /// <summary>
    /// 鼠标位置转UI点
    /// </summary>
    /// <returns></returns>
    public Vector2 mousePos2UI(Vector2 pos)
    {
        Vector2 point = new Vector2(pos.x, pos.y) - new Vector2(uiCameraSize.x / 2, uiCameraSize.y / 2);
        return point;
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