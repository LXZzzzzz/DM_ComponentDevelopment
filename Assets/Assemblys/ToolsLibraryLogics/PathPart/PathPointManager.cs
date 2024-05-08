using System.Collections.Generic;
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
        private Vector3 currentPointPos;
        private UnityAction<PathPoint> addPointCb;

        public void AddPoint(EquipBase targetEquip, Vector3 pointPos, UnityAction<PathPoint> callBack)
        {
            this.targetEquip = targetEquip;
            currentPointPos = pointPos;
            addPointCb = callBack;
            allPathPoints ??= new List<PathPoint>();
            //todo:向主机申请一个点ID

            creatAPoint(testID++.ToString());
        }

        private void creatAPoint(string pointId)
        {
            PathPoint item = new PathPoint()
            {
                pointId = pointId, belongTo = targetEquip.BObjectId, PreviousPointId = targetEquip.lastPointId, 
                currentPoint = currentPointPos, tasks = new List<TaskBase>(), NextPointId = null
            };
            allPathPoints.Add(item);
            addPointCb?.Invoke(item);
        }

        public void RemovePoint(string pointId)
        {
            for (int i = 0; i < allPathPoints.Count; i++)
            {
                if (allPathPoints[i].pointId == pointId)
                {
                    PathPoint itemPoint = allPathPoints[i];
                    GetPointDataById(itemPoint.PreviousPointId).NextPointId = itemPoint.NextPointId;
                    GetPointDataById(itemPoint.NextPointId).PreviousPointId = itemPoint.PreviousPointId;
                    allPathPoints.RemoveAt(i);
                    return;
                }
            }
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