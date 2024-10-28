using System.Collections.Generic;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.Events;

namespace ToolsLibrary.PathPart
{
    public class PathPointManager : MonoSingleTon<PathPointManager>
    {
        private int testID = 666888;

        //新增点，删除点，通过Id获取点数据，同步点（将自己的数据传给指挥官，所有客户端进行同步数据）
        private List<PathPoint> allPathPoints;
        private EquipBase targetEquip;
        private string currentPointBelongtoIconId;
        private Vector3 currentPointPos;
        private UnityAction<PathPoint> addPointCb;

        public void AddPoint(EquipBase targetEquip, string belongtoIconId, Vector3 pointPos, UnityAction<PathPoint> callBack)
        {
            this.targetEquip = targetEquip;
            currentPointBelongtoIconId = belongtoIconId;
            currentPointPos = pointPos;
            addPointCb = callBack;
            if (allPathPoints == null)
                allPathPoints = new List<PathPoint>();
            //todo:向主机申请一个点ID

            _CreatAPoint(testID++.ToString());
        }

        private void _CreatAPoint(string pointId)
        {
            PathPoint item = new PathPoint()
            {
                pointId = pointId, belongToEquipId = targetEquip.BObjectId, PreviousPointId = targetEquip.lastPointId,
                currentPoint = currentPointPos, tasks = new List<TaskBase>(), NextPointId = null,
                belongToIconId = string.IsNullOrEmpty(currentPointBelongtoIconId) ? pointId : currentPointBelongtoIconId
            };
            //创建新点，把上一个点的下一个点设为自己 ; 如果是第一个点，把飞机的下一个点设为自己
            if (!string.IsNullOrEmpty(item.PreviousPointId)) GetPointDataById(item.PreviousPointId).NextPointId = pointId;
            else targetEquip.nextPointId = pointId;
            //把飞机的最后一个点设为自己
            targetEquip.lastPointId = pointId;

            allPathPoints.Add(item);
            addPointCb?.Invoke(item);
        }

        public int RemovePoint(string pointId)
        {
            for (int i = 0; i < allPathPoints.Count; i++)
            {
                if (allPathPoints[i].pointId == pointId)
                {
                    PathPoint delePoint = allPathPoints[i];
                    PathPoint itemPoint = delePoint;
                    EquipBase itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, delePoint.belongToEquipId));
                    //算出是链路第几个点
                    int pointIndex = 1;
                    while (!string.IsNullOrEmpty(itemPoint.PreviousPointId))
                    {
                        pointIndex++;
                        itemPoint = GetPointDataById(itemPoint.PreviousPointId);
                    }


                    if (!string.IsNullOrEmpty(delePoint.PreviousPointId) && !string.IsNullOrEmpty(delePoint.NextPointId))
                    {
                        //中间点情况
                        GetPointDataById(delePoint.PreviousPointId).NextPointId = delePoint.NextPointId;
                        GetPointDataById(delePoint.NextPointId).PreviousPointId = delePoint.PreviousPointId;
                    }
                    else if (string.IsNullOrEmpty(delePoint.PreviousPointId) && !string.IsNullOrEmpty(delePoint.NextPointId))
                    {
                        //上一个点是空，下一个点不空，证明我是第一个点
                        GetPointDataById(delePoint.NextPointId).PreviousPointId = null;
                        itemEquip.nextPointId = delePoint.NextPointId;
                    }
                    else if (string.IsNullOrEmpty(delePoint.NextPointId) && !string.IsNullOrEmpty(delePoint.PreviousPointId))
                    {
                        //下一个点是空，上一个点不空，证明我是最后一个点
                        GetPointDataById(delePoint.PreviousPointId).NextPointId = null;
                        itemEquip.lastPointId = delePoint.PreviousPointId;
                    }
                    else if (string.IsNullOrEmpty(delePoint.PreviousPointId) && string.IsNullOrEmpty(delePoint.NextPointId))
                    {
                        //孤立的点，证明只有一个点
                        itemEquip.nextPointId = itemEquip.lastPointId = null;
                    }

                    allPathPoints.RemoveAt(i);
                    return pointIndex;
                }
            }

            return -1;
        }

        //插入一个点
        public void InsertPoint(EquipBase targetEquip, string belongtoIconId, Vector3 pointPos, string beInsertPointId, UnityAction<PathPoint> callBack)
        {
            this.targetEquip = targetEquip;
            currentPointBelongtoIconId = belongtoIconId;
            currentPointPos = pointPos;
            addPointCb = callBack;

            _InsertAPoint(testID++.ToString(), beInsertPointId);
        }

        private void _InsertAPoint(string pointId, string beInsertPointId)
        {
            PathPoint item = new PathPoint()
            {
                pointId = pointId, belongToEquipId = targetEquip.BObjectId, PreviousPointId = GetPointDataById(beInsertPointId).PreviousPointId,
                currentPoint = currentPointPos, tasks = new List<TaskBase>(), NextPointId = beInsertPointId,
                belongToIconId = string.IsNullOrEmpty(currentPointBelongtoIconId) ? pointId : currentPointBelongtoIconId
            };
            GetPointDataById(beInsertPointId).PreviousPointId = pointId;

            allPathPoints.Add(item);
            addPointCb?.Invoke(item);
        }

        public PathPoint GetPointDataById(string pointId)
        {
            for (int i = 0; i < allPathPoints.Count; i++)
            {
                if (allPathPoints[i].pointId == pointId)
                {
                    PathPoint itemPoint = allPathPoints[i];
                    return itemPoint;
                }
            }

            return null;
        }

        //修改路径点上的数据
        public void ChangePointDataInfo(string pointId, List<TaskBase> tasks)
        {
            var changePointData = allPathPoints.Find(x => string.Equals(x.pointId, pointId));
            if (changePointData != null)
                changePointData.tasks = tasks;
        }

        //装备组件通过自己的Id获取自己的路径起点
        public PathPoint GetPointDataByBObjectId(string bObjectId)
        {
            return null;
        }

        public void GetAllPointDatas()
        {
        }

        public void SetSyncPointDatas(List<PathPoint> otherPoints)
        {
            //其他人的数据传来，通知场景做
        }
    }
}