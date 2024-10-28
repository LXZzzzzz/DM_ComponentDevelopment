using System;
using System.Collections.Generic;
using DM.Core.Map;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.PathPart;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using Vectrosity;
using Object = UnityEngine.Object;

public class MapOperate_PlanningPath : MapOperateLogicBase
{
    enum CreatModel
    {
        AddPoint,
        InsertPoint
    }

    private Dictionary<string, VectorLine> equipPathLines; //装备ID：路径线
    private Dictionary<string, List<Vector2>> equipPathDatas; //装备id：路径点
    private bool isCreatPathPoint;
    private bool isWaitCreat;
    private string attachedObjectId;
    private EquipBase currentChooseEquip;

    private CreatModel currentCreatModel;
    private string beInsertPointId;
    private int insertIndex;

    public override void OnEnter()
    {
        isCreatPathPoint = false;
        isWaitCreat = false;
        attachedObjectId = String.Empty;
        if (equipPathLines == null) equipPathLines = new Dictionary<string, VectorLine>();
        if (equipPathDatas == null) equipPathDatas = new Dictionary<string, List<Vector2>>();
    }

    private void InitLineByObjId(string objId)
    {
        equipPathDatas.Add(objId, new List<Vector2>() { mainLogic.allIconCells[objId].GetComponent<RectTransform>().anchoredPosition });
        var itemLine = new VectorLine("Line" + objId, equipPathDatas[objId], 10, LineType.Continuous);
#if UNITY_EDITOR
        itemLine.SetCanvas(mainLogic.gameObject.GetComponentInParent<Canvas>());
#else
        itemLine.SetCanvas(UIManager.Instance.CurrentCanvans);
#endif
        itemLine.rectTransform.SetParent(mainLogic.iconCellParent);
        itemLine.rectTransform.localPosition = Vector3.zero;
        itemLine.rectTransform.localScale = Vector3.one;
        itemLine.active = true;
        itemLine.color = Color.cyan;
        equipPathLines.Add(objId, itemLine);
    }

    public override void OnLeftClickIcon(IconCellBase clickIcon)
    {
        if (!isCreatPathPoint)
        {
            if (clickIcon is AirIconCell)
            {
                var itemObj = MyDataInfo.sceneAllEquips.Find(x => string.Equals(clickIcon.belongToId, x.BObjectId));
#if UNITY_EDITOR
                var airObj = itemObj.gameObject.tag == "Plane" ? itemObj : null;
#else
                    //todo:这里的Id==3只是测试逻辑，最后要通过组件的实际Id对这里进行修改
                    // var airObj = itemObj.BObject.Info.Tags.Find(x => x.Id == 3) != null ? itemObj : null;
#endif
                //证明选中的是可移动装备
                currentChooseEquip = itemObj;
                // currentChooseEquip.BObjectId = currentChooseEquip.GetComponent<BObjectModel>().BObject.Id;//这是以前的测试代码，现在Id统一分配不需要了
                //获取选中装备的路径轨迹线，进行操作
                if (!equipPathLines.ContainsKey(currentChooseEquip.BObjectId))
                    InitLineByObjId(currentChooseEquip.BObjectId);
                //给数据末尾添加一个随鼠标移动的点
                int itemCount = equipPathDatas[currentChooseEquip.BObjectId].Count - 1;
                Vector2 lastPoint = equipPathDatas[currentChooseEquip.BObjectId][itemCount];
                equipPathDatas[currentChooseEquip.BObjectId].Add(lastPoint);
                equipPathLines[currentChooseEquip.BObjectId].Draw();

                currentCreatModel = CreatModel.AddPoint;
                isCreatPathPoint = true;
            }

            if (clickIcon is PointIconCell)
            {
                //选中的是标点
#if UNITY_EDITOR
                Debug.Log("选中的标点是" + clickIcon.belongToId + "的点；" + "名字是：" + clickIcon.name);
                Debug.Log($"经过了{(clickIcon as PointIconCell).allViaPointIds?.Count}个点");
#else
                    mainLogic.sender.LogError("选中的标点是" + clickIcon.belongToId + "的点；" + "名字是：" + clickIcon.name);
#endif
                ShowPathPointsData sppd = new ShowPathPointsData() { allViaPointData = (clickIcon as PointIconCell).allViaPointIds, RemoveAction = RemovePoint, InsertAction = InsertAPoint };
                UIManager.Instance.ShowPanel<UIPathPointsShow>(UIName.UIPathPointsShow, sppd);
            }
        }
        else
        {
            //当前是设置路径模式，这时选择了已存在的点，证明要以该点为目标点
            if (clickIcon is AirIconCell) return;
            if (isWaitCreat) return;
            isWaitCreat = true;

            var toBeCreatPoint = uiPos2WorldPos(clickIcon.gameObject.transform.position);
            attachedObjectId = clickIcon.belongToId;
            switch (currentCreatModel)
            {
                case CreatModel.AddPoint:
                    //添加一个点（具体逻辑交给他去处理，我只关注应用层的逻辑处理）
                    PathPointManager.Instance.AddPoint(currentChooseEquip, attachedObjectId, toBeCreatPoint, OnAddPointSuc);
                    break;
                case CreatModel.InsertPoint:
                    PathPointManager.Instance.InsertPoint(currentChooseEquip, attachedObjectId, toBeCreatPoint, beInsertPointId, OnInsertPointSuc);
                    break;
            }
        }
    }

    public override void OnRightClickIcon(IconCellBase clickIcon)
    {
        //todo:这里可以判断一下，如果该点依附路径点为空，就显示删除
    }

    public override void OnUpdate()
    {
        if (!isCreatPathPoint) return;

        switch (currentCreatModel)
        {
            case CreatModel.AddPoint:
                int itemCount = equipPathDatas[currentChooseEquip.BObjectId].Count - 1;
                //实时设置鼠标位置为线段终点，并刷新线段显示
                equipPathDatas[currentChooseEquip.BObjectId][itemCount] = mainLogic.mousePos2UI(Input.mousePosition);
                equipPathLines[currentChooseEquip.BObjectId].Draw();
                break;
            case CreatModel.InsertPoint:
                equipPathDatas[currentChooseEquip.BObjectId][insertIndex] = mainLogic.mousePos2UI(Input.mousePosition);
                equipPathLines[currentChooseEquip.BObjectId].Draw();
                break;
        }
    }

    public override void OnLeftClickMap(Vector2 pos)
    {
        if (!isCreatPathPoint) return;

        if (isWaitCreat) return;
        isWaitCreat = true;
        attachedObjectId = String.Empty;
        switch (currentCreatModel)
        {
            case CreatModel.AddPoint:
                PathPointManager.Instance.AddPoint(currentChooseEquip, String.Empty, uiPos2WorldPos(pos), OnAddPointSuc);
                break;
            case CreatModel.InsertPoint:
                PathPointManager.Instance.InsertPoint(currentChooseEquip, String.Empty, uiPos2WorldPos(pos), beInsertPointId, OnInsertPointSuc);
                break;
        }
    }

    public override void OnRightClickMap(Vector2 pos)
    {
        if (!isCreatPathPoint) return;
        isCreatPathPoint = false;
        //取消线段跟随鼠标
        switch (currentCreatModel)
        {
            case CreatModel.AddPoint:
                int itemCount = equipPathDatas[currentChooseEquip.BObjectId].Count - 1;
                equipPathDatas[currentChooseEquip.BObjectId].RemoveAt(itemCount);
                equipPathLines[currentChooseEquip.BObjectId].Draw();
                break;
            case CreatModel.InsertPoint:
                equipPathDatas[currentChooseEquip.BObjectId].RemoveAt(insertIndex);
                equipPathLines[currentChooseEquip.BObjectId].Draw();
                break;
        }

        currentChooseEquip = null;
    }

    public override void OnExit()
    {
        isCreatPathPoint = false;
    }

    private void OnAddPointSuc(PathPoint pointData)
    {
        if (isWaitCreat)
        {
            //暂定当没有归属点，就将点ID设为归属点Id，
            creatPathPoint(string.IsNullOrEmpty(attachedObjectId) ? pointData.belongToIconId : attachedObjectId, pointData);
            isWaitCreat = false;
            attachedObjectId = String.Empty;
            //数据上加完点后，把点插入到线段倒数第二个位置
            int itemCount = equipPathDatas[currentChooseEquip.BObjectId].Count - 1;
            equipPathDatas[currentChooseEquip.BObjectId].Insert(itemCount, worldPos2UiPos(pointData.currentPoint));
        }
    }

    private void OnInsertPointSuc(PathPoint pointData)
    {
        if (isWaitCreat)
        {
            //暂定当没有归属点，就将点ID设为归属点Id，
            creatPathPoint(string.IsNullOrEmpty(attachedObjectId) ? pointData.belongToIconId : attachedObjectId, pointData);
            isWaitCreat = false;
            attachedObjectId = String.Empty;
            //数据上加完点后，把点插入到记录的插入下标位置，并将下标后移
            equipPathDatas[currentChooseEquip.BObjectId].Insert(insertIndex, worldPos2UiPos(pointData.currentPoint));
            insertIndex++;
        }
    }

    private void RemovePoint(string pointId)
    {
        //去数据管理器中算出这个点所在下标，回来通过下标改折线数据
        string equipId = PathPointManager.Instance.GetPointDataById(pointId).belongToEquipId;
        string iconId = PathPointManager.Instance.GetPointDataById(pointId).belongToIconId;
        int pointIndex = PathPointManager.Instance.RemovePoint(pointId);
        if (equipPathDatas.ContainsKey(equipId) && equipPathDatas[equipId].Count > pointIndex)
            equipPathDatas[equipId].RemoveAt(pointIndex);
        (mainLogic.allIconCells[iconId] as PointIconCell).RemoveAttachedPoint(pointId);
        mainLogic.allIconCells[iconId].RefreshView();
        equipPathLines[equipId].Draw();
    }

    private void InsertAPoint(string pointId, bool isInFront)
    {
        PathPoint itemPoint = PathPointManager.Instance.GetPointDataById(pointId);
        if (!isInFront && string.IsNullOrEmpty(itemPoint.NextPointId))
        {
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, new ConfirmatonInfo()
            {
                showStrInfo = "该点后方无数据，不可执行插入操作", type = showType.tipView
            });
            return; //后面没有点，没必要做插入逻辑
        }

        UIManager.Instance.HidePanel(UIName.UIPathPointsShow.ToString());

        beInsertPointId = pointId;
        currentChooseEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, itemPoint.belongToEquipId));

        //找到这个点的下标记录起来，
        insertIndex = 1;
        while (!string.IsNullOrEmpty(itemPoint.PreviousPointId))
        {
            insertIndex++;
            itemPoint = PathPointManager.Instance.GetPointDataById(itemPoint.PreviousPointId);
        }

        insertIndex = isInFront ? insertIndex : insertIndex + 1;

        equipPathDatas[currentChooseEquip.BObjectId].Insert(insertIndex, equipPathDatas[itemPoint.belongToEquipId][insertIndex]);
        equipPathLines[currentChooseEquip.BObjectId].Draw();

        //改为插入模式，然后打开创建路径开关
        currentCreatModel = CreatModel.InsertPoint;
        isCreatPathPoint = true;
    }

    private void creatPathPoint(string belongToPointCellId, PathPoint pointData)
    {
        //执行创建点逻辑
        //找ID对应的实体，没找到创建，找到了附加
        if (!mainLogic.allIconCells.ContainsKey(belongToPointCellId))
        {
            //创建一个点
            var itemPoint = Object.Instantiate(mainLogic.pointIconPrefab, mainLogic.iconCellParent);
            itemPoint.gameObject.SetActive(true);
            //传入这个组件的基本信息，和选择后的回调
            itemPoint.GetComponent<RectTransform>().anchoredPosition = worldPos2UiPos(pointData.currentPoint);
            itemPoint.Init(belongToPointCellId, mainLogic.OnChooseObj);
            mainLogic.allIconCells.Add(belongToPointCellId, itemPoint);
        }

        //给已存在的点附加
        (mainLogic.allIconCells[belongToPointCellId] as PointIconCell).AddAttachedPoint(pointData.pointId);
        mainLogic.allIconCells[belongToPointCellId].RefreshView();

        //打开编辑页面

        UIManager.Instance.ShowPanel<UIChangePointDataInfo>(UIName.UIChangePointDataInfo, pointData);
    }


    public MapOperate_PlanningPath(UIMap mainLogic) : base(mainLogic)
    {
    }
}

public class ShowPathPointsData
{
    public List<string> allViaPointData;
    public UnityAction<string> RemoveAction;
    public UnityAction<string, bool> InsertAction;
}