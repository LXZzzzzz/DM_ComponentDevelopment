using System;
using System.Collections.Generic;
using DM.Core.Map;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.PathPart;
using UiManager;
using UnityEngine;
using UnityEngine.EventSystems;
using PathPoint = ToolsLibrary.PathPart.PathPoint;
using Vectrosity;
using EventType = Enums.EventType;

namespace OldLogic
{
    public enum OperatorState
    {
        Click,
        SetPathPoint,
        NoPlanningRoute, //不规划路径模式
        CreatEquip //根据模板创建飞机模式
    }

    public class UIMap_OldLogic : BasePanel, IPointerClickHandler
    {
        [HideInInspector] public RectTransform mapView;
        [HideInInspector] public Transform iconCellParent;
        [HideInInspector] public IconCellBase airIconPrefab, pointIconPrefab;

        [HideInInspector] public Vector2 uiCameraSize;
        private List<EquipBase> allObjModels;
        private OperatorState currentState;
        private EquipBase currentChooseEquip;
        public Dictionary<string, IconCellBase> allIconCells; //存储地图上的所有点
        private Dictionary<string, VectorLine> equipPathLines; //装备ID：路径线
        private Dictionary<string, List<Vector2>> equipPathDatas; //装备id：路径点
        [HideInInspector] public float mapBL;

        public override void Init()
        {
            base.Init();
            mapView = transform.Find("maxMap/map").GetComponent<RectTransform>();
            iconCellParent = transform.Find("maxMap/objects").GetComponent<RectTransform>();
            airIconPrefab = transform.Find("prefabs/airCell").GetComponent<AirIconCell>();
            pointIconPrefab = transform.Find("prefabs/pointCell").GetComponent<PointIconCell>();
            //todo:后续要改成实际地图比例
            ////当前测试地图长宽是100，地图数据转换比例是
            mapBL = 100 / mapView.sizeDelta.x;
        }

        public override void ShowMe(object userData)
        {
            base.ShowMe(userData);
            //在场景中初始化装备图标、任务图标、地域图标（有共性，都包含自身属性和附加点属性，可以抽象一个类，每个类型在自己具体类中，实现具体逻辑）
            equipPathLines = new Dictionary<string, VectorLine>();
            equipPathDatas = new Dictionary<string, List<Vector2>>();
            allObjModels = (List<EquipBase>)userData;

            allIconCells = new Dictionary<string, IconCellBase>();
            for (int i = 0; i < allObjModels?.Count; i++)
            {
                if (allObjModels[i].GetComponent<EquipBase>() == null) continue;
                creatAirCell(allObjModels[i]);
            }

            uiCameraSize = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
            EventManager.Instance.AddEventListener<string>(EventType.SwitchMapModel.ToString(), switchCreatModel);
            EventManager.Instance.AddEventListener<EquipBase>(EventType.CreatEquipCorrespondingIcon.ToString(), creatAirCell);
            currentState = OperatorState.NoPlanningRoute;
        }

        public override void HideMe()
        {
            base.HideMe();

            EventManager.Instance.RemoveEventListener<string>(EventType.SwitchMapModel.ToString(), switchCreatModel);
            EventManager.Instance.RemoveEventListener<EquipBase>(EventType.CreatEquipCorrespondingIcon.ToString(), creatAirCell);
        }

        private EquipBase[] sceneAllObjs;

        private void Start()
        {
#if UNITY_EDITOR
            mapBL = 100f / mapView.sizeDelta.x;
            equipPathLines = new Dictionary<string, VectorLine>();
            equipPathDatas = new Dictionary<string, List<Vector2>>();
            allObjModels = new List<EquipBase>();
            uiCameraSize = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
            Debug.Log("uiCameraSize：" + uiCameraSize);
            sceneAllObjs = FindObjectsOfType<EquipBase>();
            for (int i = 0; i < sceneAllObjs?.Length; i++)
            {
                var item = sceneAllObjs[i];
                item.BObjectId = ((i + 1) * 11111111).ToString();
                allObjModels.Add(item);
            }

            allIconCells = new Dictionary<string, IconCellBase>();
            for (int i = 0; i < allObjModels.Count; i++)
            {
                var itemCell = Instantiate(airIconPrefab, iconCellParent);
                itemCell.gameObject.SetActive(true);
                //传入这个组件的基本信息，和选择后的回调
                itemCell.transform.position = worldPos2UiPos(allObjModels[i].gameObject.transform.position);
                // (itemCell as AirIconCell).Init(allObjModels[i], allObjModels[i].BObjectId, OnChooseObj, worldPos2UiPos);
                allIconCells.Add(allObjModels[i].BObjectId, itemCell);
            }

            currentState = OperatorState.NoPlanningRoute;
#endif
        }

        public void creatAirCell(EquipBase equip)
        {
            var itemCell = Instantiate(airIconPrefab, iconCellParent);
            itemCell.gameObject.SetActive(true);
            //传入这个组件的基本信息，和选择后的回调
            itemCell.transform.position = worldPos2UiPos(equip.gameObject.transform.position);
            // (itemCell as AirIconCell).Init(equip, equip.BObjectId, OnChooseObj, worldPos2UiPos);
            allIconCells.Add(equip.BObjectId, itemCell);
        }

        private string creatTargetTemplate;

        private void switchCreatModel(string creatTemplateId)
        {
            currentState = OperatorState.CreatEquip;
            //todo:显示要创建的飞机模板标志，并跟随鼠标移动
            creatTargetTemplate = creatTemplateId;
        }

        private void InitLineByObjId(string objId)
        {
            equipPathDatas.Add(objId, new List<Vector2>() { uiPos2LinePos(allIconCells[objId].transform.position) });
            var itemLine = new VectorLine("Line" + objId, equipPathDatas[objId], 10, LineType.Continuous);
#if UNITY_EDITOR
            itemLine.SetCanvas(GetComponentInParent<Canvas>());
#else
        itemLine.SetCanvas(UIManager.Instance.CurrentCanvans);
#endif
            itemLine.rectTransform.SetParent(iconCellParent);
            itemLine.rectTransform.localPosition = Vector3.zero;
            itemLine.rectTransform.localScale = Vector3.one;
            itemLine.active = true;
            itemLine.color = Color.cyan;
            equipPathLines.Add(objId, itemLine);
        }

        //该段逻辑是点击了地图上任意图标的回调，包含两部分：1.点击装备图表 2.点击非装备图标(包含系统实体、本地实体)
        private void OnChooseObj(string objId,PointerEventData.InputButton button)
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
            switch (currentState)
            {
                case OperatorState.Click:
                    //查看他的类型

                    if (targetIconCell is AirIconCell)
                    {
                        var itemObj = allObjModels.Find(x => string.Equals(targetIconCell.belongToId, x.BObjectId));
#if UNITY_EDITOR
                        var airObj = itemObj.gameObject.tag == "Plane" ? itemObj : null;
#else
                    //todo:这里的Id==3只是测试逻辑，最后要通过组件的实际Id对这里进行修改
                    // var airObj = itemObj.BObject.Info.Tags.Find(x => x.Id == 3) != null ? itemObj : null;
#endif
                        //证明选中的是可移动装备
                        currentChooseEquip = itemObj.gameObject.GetComponent<EquipBase>();
                        currentChooseEquip.BObjectId = currentChooseEquip.GetComponent<BObjectModel>().BObject.Id;
                        //获取选中装备的路径轨迹线，进行操作
                        if (!equipPathLines.ContainsKey(currentChooseEquip.BObjectId))
                            InitLineByObjId(currentChooseEquip.BObjectId);
                        //给数据末尾添加一个随鼠标移动的点
                        int itemCount = equipPathDatas[currentChooseEquip.BObjectId].Count - 1;
                        Vector2 lastPoint = equipPathDatas[currentChooseEquip.BObjectId][itemCount];
                        equipPathDatas[currentChooseEquip.BObjectId].Add(lastPoint);
                        equipPathLines[currentChooseEquip.BObjectId].Draw();

                        currentState = OperatorState.SetPathPoint;
                    }

                    if (targetIconCell is PointIconCell)
                    {
                        //选中的是标点
#if UNITY_EDITOR
                        Debug.Log("选中的标点是" + targetIconCell.belongToId + "的点；" + "名字是：" + targetIconCell.name);
                        Debug.Log($"经过了{(targetIconCell as PointIconCell).allViaPointIds?.Count}个点");
#else
                    sender.LogError("选中的标点是" + targetIconCell.belongToId + "的点；" + "名字是：" + targetIconCell.name);
#endif
                    }

                    break;
                case OperatorState.SetPathPoint:
                    //当前是设置路径模式，这时选择了已存在的点，证明要以该点为目标点
                    if (targetIconCell is AirIconCell) return;
                    if (isWaitCreat) return;
                    isWaitCreat = true;
                    //todo:判断如果是实体，就用实体位置，否则用currentPos
                    var toBeCreatPoint = (targetIconCell.gameObject.transform.position);
                    attachedObjectId = targetIconCell.belongToId;
                    //添加一个点（具体逻辑交给他去处理，我只关注应用层的逻辑处理）
                    PathPointManager.Instance.AddPoint(currentChooseEquip, toBeCreatPoint, AddPointSuccess);
                    break;
                case OperatorState.NoPlanningRoute:
#if !UNITY_EDITOR
                sender.LogError("选中了一个对象"+targetIconCell.belongToId);
#endif
                    EventManager.Instance.EventTrigger(EventType.ChooseEquip.ToString(), targetIconCell.belongToId);
                    break;
            }
        }

        //点击地图返回标点位置
        private void OnChooseObj(Vector2 mapPoint)
        {
            if (isWaitCreat) return;
            isWaitCreat = true;
            attachedObjectId = String.Empty;
            PathPointManager.Instance.AddPoint(currentChooseEquip, mapPoint, AddPointSuccess);
        }

        private bool isWaitCreat;
        private string attachedObjectId;

        private void AddPointSuccess(PathPoint pointData)
        {
            if (isWaitCreat)
            {
                currentChooseEquip.lastPointId = pointData.pointId;
                //暂定当没有归属点，就将点ID设为归属点Id，
                creatPathPoint(string.IsNullOrEmpty(attachedObjectId) ? pointData.pointId : attachedObjectId, pointData);
                isWaitCreat = false;
                attachedObjectId = String.Empty;
                //数据上加完点后，把点插入到线段倒数第二个位置
                int itemCount = equipPathDatas[currentChooseEquip.BObjectId].Count - 1;
                equipPathDatas[currentChooseEquip.BObjectId].Insert(itemCount, uiPos2LinePos(pointData.currentPoint));
            }
        }

        private void creatPathPoint(string belongToPointCellId, PathPoint pointData)
        {
            //执行创建点逻辑
            //找ID对应的实体，没找到创建，找到了附加
            if (!allIconCells.ContainsKey(belongToPointCellId))
            {
                //创建一个点
                var itemPoint = Instantiate(pointIconPrefab, iconCellParent);
                itemPoint.gameObject.SetActive(true);
                //传入这个组件的基本信息，和选择后的回调
                itemPoint.transform.position = worldPos2UiPos(pointData.currentPoint);
                itemPoint.Init(belongToPointCellId, OnChooseObj);
                allIconCells.Add(belongToPointCellId, itemPoint);
            }

            //给已存在的点附加
            (allIconCells[belongToPointCellId] as PointIconCell).AddAttachedPoint(pointData.pointId);
            allIconCells[belongToPointCellId].RefreshView();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1) && currentState == OperatorState.SetPathPoint)
            {
                currentState = OperatorState.Click;
                //取消线段跟随鼠标
                int itemCount = equipPathDatas[currentChooseEquip.BObjectId].Count - 1;
                equipPathDatas[currentChooseEquip.BObjectId].RemoveAt(itemCount);
                equipPathLines[currentChooseEquip.BObjectId].Draw();
                currentChooseEquip = null;
            }

            if (currentState == OperatorState.SetPathPoint)
            {
                //实时设置鼠标位置为线段终点，并刷新线段显示
                int itemCount = equipPathDatas[currentChooseEquip.BObjectId].Count - 1;
                equipPathDatas[currentChooseEquip.BObjectId][itemCount] = uiPos2LinePos((Vector2)Input.mousePosition);
                equipPathLines[currentChooseEquip.BObjectId].Draw();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //这里是检测点击区域是否在地图内部
            Vector2 point = uiPos2LinePos(eventData.position) + new Vector2(mapView.sizeDelta.x / 2, mapView.sizeDelta.y / 2);
            if (point.x < 0 || point.y < 0 || point.x > mapView.sizeDelta.x || point.y > mapView.sizeDelta.y) return;
            if (currentState == OperatorState.Click && eventData.button == PointerEventData.InputButton.Left)
            {
                OnChooseObj(eventData.position);
            }

            if (currentState == OperatorState.NoPlanningRoute && eventData.button == PointerEventData.InputButton.Right)
            {
                //在非规划模式下点击右键
#if !UNITY_EDITOR
            sender.LogError("选择了一个目标点"+eventData.position);
#endif
                EventManager.Instance.EventTrigger(EventType.MoveToTarget.ToString(), uiPos2WorldPos(eventData.position));
            }

            if (currentState == OperatorState.CreatEquip)
            {
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    //通知主角在场景对应位置创建实体
                    EventManager.Instance.EventTrigger(EventType.CreatEquipEntity.ToString(), creatTargetTemplate, uiPos2WorldPos(eventData.position));
                }

                if (eventData.button == PointerEventData.InputButton.Right)
                {
                    currentState = OperatorState.NoPlanningRoute;
                }
            }
        }

        #region 数据转换 //todo:2D、3D之间的位置转换逻辑后续需要补充

        private Vector2 worldPos2UiPos(Vector3 pos)
        {
            return new Vector2(pos.x / mapBL - mapView.sizeDelta.x / 2, pos.z / mapBL - mapView.sizeDelta.y / 2);
        }

        private Vector3 uiPos2WorldPos(Vector2 pos)
        {
            return new Vector3((pos.x - uiCameraSize.x / 2 + mapView.sizeDelta.x / 2) * mapBL, 0, (pos.y - uiCameraSize.y / 2 + mapView.sizeDelta.y / 2) * mapBL);
        }

        /// <summary>
        /// ui点转换为线段相对点
        /// </summary>
        /// <returns></returns>
        private Vector2 uiPos2LinePos(Vector2 pos)
        {
            Vector2 point = new Vector2(uiCameraSize.x * pos.x / Screen.width, uiCameraSize.y * pos.y / Screen.height) -
                            new Vector2(uiCameraSize.x / 2, uiCameraSize.y / 2);
            return point;
        }

        #endregion
    }

}