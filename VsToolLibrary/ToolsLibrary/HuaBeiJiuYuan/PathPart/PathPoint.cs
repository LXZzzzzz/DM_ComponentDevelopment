using System.Collections.Generic;
using ToolsLibrary.EquipPart;
using UnityEngine;

namespace ToolsLibrary.PathPart
{
    public class PathPoint
    {
        public string pointId;
        public string PreviousPointId;
        public string NextPointId;

        public Vector3 currentPoint;

        //该点属于哪个飞机的
        public string belongToEquipId;

        //该点附加于哪个地图标点（包括资源Icon和pointIcon）
        public string belongToIconId;

        //这个点绑定的任务
        public List<TaskBase> tasks;
    }

    //限定任务的抽象行为
    public abstract class TaskBase
    {
        public int orderNumber;
        public SkillType runSkillType;
    }
}