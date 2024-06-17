using System;
using System.Collections.Generic;
using DM.Core.Map;
using ToolsLibrary.EquipPart;
using ToolsLibrary.PathPart;
using UiManager;
using UnityEngine;
using Vectrosity;
using Object = UnityEngine.Object;

public class MapOperate_PlanningPath : MapOperateLogicBase
{
    private Dictionary<string, VectorLine> equipPathLines; //装备ID：路径线
    private Dictionary<string, List<Vector2>> equipPathDatas; //装备id：路径点
    private bool isCreatPathPoint;
    private bool isWaitCreat;
    private string attachedObjectId;
    private EquipBase currentChooseEquip;

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
                var itemObj = mainLogic.allObjModels.Find(x => string.Equals(clickIcon.belongToId, x.BObjectId));
#if UNITY_EDITOR
                var airObj = itemObj.gameObject.tag == "Plane" ? itemObj : null;
#else
                    //todo:这里的Id==3只是测试逻辑，最后要通过组件的实际Id对这里进行修改
                    // var airObj = itemObj.BObject.Info.Tags.Find(x => x.Id == 3) != null ? itemObj : null;
#endif
                //证明选中的是可移动装备
                currentChooseEquip = itemObj.gameObject.GetComponent<EquipBase>();
                // currentChooseEquip.BObjectId = currentChooseEquip.GetComponent<BObjectModel>().BObject.Id;//这是以前的测试代码，现在Id统一分配不需要了
                //获取选中装备的路径轨迹线，进行操作
                if (!equipPathLines.ContainsKey(currentChooseEquip.BObjectId))
                    InitLineByObjId(currentChooseEquip.BObjectId);
                //给数据末尾添加一个随鼠标移动的点
                int itemCount = equipPathDatas[currentChooseEquip.BObjectId].Count - 1;
                Vector2 lastPoint = equipPathDatas[currentChooseEquip.BObjectId][itemCount];
                equipPathDatas[currentChooseEquip.BObjectId].Add(lastPoint);
                equipPathLines[currentChooseEquip.BObjectId].Draw();

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
            }
        }
        else
        {
            //当前是设置路径模式，这时选择了已存在的点，证明要以该点为目标点
            if (clickIcon is AirIconCell) return;
            if (isWaitCreat) return;
            isWaitCreat = true;
            //todo:判断如果是实体，就用实体位置，否则用currentPos
            var toBeCreatPoint = uiPos2WorldPos(clickIcon.gameObject.transform.position);
            attachedObjectId = clickIcon.belongToId;
            //添加一个点（具体逻辑交给他去处理，我只关注应用层的逻辑处理）
            PathPointManager.Instance.AddPoint(currentChooseEquip, toBeCreatPoint, AddPointSuccess);
        }
    }

    public override void OnRightClickIcon(IconCellBase clickIcon)
    {
    }

    public override void OnUpdate()
    {
        if (!isCreatPathPoint) return;

        int itemCount = equipPathDatas[currentChooseEquip.BObjectId].Count - 1;
        //实时设置鼠标位置为线段终点，并刷新线段显示
        equipPathDatas[currentChooseEquip.BObjectId][itemCount] = mainLogic.mousePos2UI(Input.mousePosition);
        equipPathLines[currentChooseEquip.BObjectId].Draw();
    }

    public override void OnLeftClickMap(Vector2 pos)
    {
        if (isCreatPathPoint)
            OnChooseObj(uiPos2WorldPos(pos));
    }

    public override void OnRightClickMap(Vector2 pos)
    {
        if (!isCreatPathPoint) return;
        isCreatPathPoint = false;
        //取消线段跟随鼠标
        int itemCount = equipPathDatas[currentChooseEquip.BObjectId].Count - 1;
        equipPathDatas[currentChooseEquip.BObjectId].RemoveAt(itemCount);
        equipPathLines[currentChooseEquip.BObjectId].Draw();
        currentChooseEquip = null;

        mainLogic.SwitchMapLogic(OperatorState.Normal);
    }

    public override void OnExit()
    {
    }

    //点击地图返回标点位置
    private void OnChooseObj(Vector3 mapPoint)
    {
        if (isWaitCreat) return;
        isWaitCreat = true;
        attachedObjectId = String.Empty;
        PathPointManager.Instance.AddPoint(currentChooseEquip, mapPoint, AddPointSuccess);
    }

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
            equipPathDatas[currentChooseEquip.BObjectId].Insert(itemCount, worldPos2UiPos(pointData.currentPoint));
        }
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
    }


    public MapOperate_PlanningPath(UIMap mainLogic) : base(mainLogic)
    {
    }
}