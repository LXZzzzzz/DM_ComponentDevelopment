using System;
using System.Collections.Generic;
using DM.Core.Map;
using DM.Entity;
using ToolsLibrary.PathPart;
using UiManager;
using UnityEngine;
using UnityEngine.EventSystems;
using PathPoint = ToolsLibrary.PathPart.PathPoint;

public enum OperatorState
{
    Click,
    SetPathPoint
}

public class UIMap : BasePanel, IPointerClickHandler
{
    public Transform iconCellParent;
    public IconCellBase airIconPrefab,pointIconPrefab;

    private List<BObjectModel> allObjModels;
    private OperatorState currentState;
    private Dictionary<string, IconCellBase> allIconCells; //存储地图上的所有点
    private EquipBase currentChooseEquip;

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        //在场景中初始化装备图标、任务图标、地域图标（有共性，都包含自身属性和附加点属性，可以抽象一个类，每个类型在自己具体类中，实现具体逻辑）
        allObjModels = new List<BObjectModel>();
        for (int i = 0; i < ((BObjectModel[])userData).Length; i++)
        {
            allObjModels.Add(((BObjectModel[])userData)[i]);
        }

        allIconCells = new Dictionary<string, IconCellBase>();
        for (int i = 0; i < allObjModels.Count; i++)
        {
            var itemCell = Instantiate(airIconPrefab, iconCellParent);
            itemCell.gameObject.SetActive(true);
            //传入这个组件的基本信息，和选择后的回调
            itemCell.transform.position = worldPos2UiPos(allObjModels[i].gameObject.transform.position);
            itemCell.Init(allObjModels[i].BObject.Id, OnChooseObj);
            allIconCells.Add(allObjModels[i].BObject.Id, itemCell);
        }

        currentState = OperatorState.Click;
    }

    public BObjectModel[] sceneAllObjs;
    private void Start()
    {
        allObjModels = new List<BObjectModel>();

        for (int i = 0; i < sceneAllObjs.Length; i++)
        {
            var item = sceneAllObjs[i];
            item.BObject = new BObject(){Info = new ComInfo(){Name = item.name}};
            item.BObject.Id = ((i + 1) * 11111111).ToString();
            item.GetComponent<EquipBase>().BObjectId = item.BObject.Id;
            allObjModels.Add(item);
        }
        
        allIconCells = new Dictionary<string, IconCellBase>();
        for (int i = 0; i < allObjModels.Count; i++)
        {
            var itemCell = Instantiate(airIconPrefab, iconCellParent);
            itemCell.gameObject.SetActive(true);
            //传入这个组件的基本信息，和选择后的回调
            itemCell.transform.position = worldPos2UiPos(allObjModels[i].gameObject.transform.position);
            itemCell.Init(allObjModels[i].BObject.Id, OnChooseObj);
            allIconCells.Add(allObjModels[i].BObject.Id, itemCell);
        }
    }

    //该段逻辑是点击了地图上任意图标的回调，包含两部分：1.点击装备图表 2.点击非装备图标(包含系统实体、本地实体)
    private void OnChooseObj(string objId)
    {
        IconCellBase targetIconCell=null;
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
#if UNITY_EDITOR
                    var itemObj = allObjModels.Find(x => string.Equals(targetIconCell.belongToId, x.BObject.Id));
                    var airObj = itemObj.gameObject.tag == "Plane" ? itemObj : null;
#else
                BObjectModel itemObj = allObjModels.Find(x => string.Equals(targetIconCell.belongToId, x.BObject.Id));
                //todo:这里的Id==3只是测试逻辑，最后要通过组件的实际Id对这里进行修改
                var airObj = itemObj.BObject.Info.Tags.Find(x => x.Id == 3);
#endif
                    //证明选中的是可移动装备
                    currentState = OperatorState.SetPathPoint;
                    currentChooseEquip = airObj.gameObject.GetComponent<EquipBase>();
                    Debug.Log("选中的是"+airObj.name);
                    //打开线段的显示，设置该装备的最后一个目标点为线段起始点
                }

                if (targetIconCell is PointIconCell)
                {
                    //选中的是标点
                    Debug.Log("选中的标点是"+targetIconCell.belongToId+"的点；"+"名字是："+targetIconCell.name);
                    Debug.Log($"经过了{targetIconCell.allViaPointIds.Count}个点");
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
        }
    }

    //点击地图返回标点位置
    private void OnChooseObj(Vector2 mapPoint)
    {
        if (currentState == OperatorState.Click) return;
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
        allIconCells[belongToPointCellId].AddAttachedPoint(pointData.pointId);
        allIconCells[belongToPointCellId].RefreshView();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && currentState == OperatorState.SetPathPoint)
        {
            currentState = OperatorState.Click;
            //取消线段的显示
        }

        if (currentState == OperatorState.SetPathPoint)
        {
            //实时设置鼠标位置为线段终点，并刷新线段显示
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //todo:这里需要检测点击区域是否在地图内部
        if (eventData.button== PointerEventData.InputButton.Left)
        {
            OnChooseObj(eventData.position);
        }
    }

    #region 2D、3D之间的位置转换 //todo:2D、3D之间的位置转换逻辑后续需要补充

    private Vector2 worldPos2UiPos(Vector3 pos)
    {
        return pos;
    }

    private Vector3 uiPos2WorldPos(Vector2 pos)
    {
        return Vector3.zero;
    }

    #endregion
}