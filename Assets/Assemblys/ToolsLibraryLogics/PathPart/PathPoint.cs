using System.Collections.Generic;
using UnityEngine;

namespace ToolsLibrary.PathPart
{
    public class PathPoint
    {
        public string pointId;
        public string PreviousPointId;
        public string NextPointId;
        public Vector3 currentPoint;
        public string belongTo;
        //这个点绑定的任务
        public List<TaskBase> tasks;
    }

    public abstract class TaskBase
    {
        //限定任务的抽象行为
    }
}