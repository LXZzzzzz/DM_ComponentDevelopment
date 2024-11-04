using System;
using Enums;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.PathPart;
using UnityEngine;
using EventType = Enums.EventType;

public partial class HelicopterController
{
    private PathPoint currentPathPoint;
    private bool isRunEnd;

    private void OnRunInstructionUpdate()
    {
        //坠毁就不执行了，可能还需要做标记，任务未完成
        if (isCrash || isRunEnd) return;
        if (currentPathPoint == null)
            currentPathPoint = PathPointManager.Instance.GetPointDataByBObjectId(BObjectId);

        //如果正在执行指令或正在飞行，就等待
        if (currentSkill != SkillType.None || !isArrive) return;

        TaskBase nextRunTask = getNextRunTask();
        if (nextRunTask != null)
        {
            //执行下一个任务项
            OnSelectSkill(nextRunTask.runSkillType);
        }
        else
        {
            //该路点任务集执行完毕，走向下一个点
            currentPathPoint = PathPointManager.Instance.GetPointDataById(currentPathPoint.NextPointId);
            if (currentPathPoint == null) isRunEnd = true;
            else
            {
                currentPathPoint.tasks.Sort((a, b) => a.orderNumber < b.orderNumber ? -1 : 1);

                //执行让直升机飞往该路点
                Vector3 targetPos = new Vector3(currentPathPoint.currentPoint.x, currentPathPoint.currentPoint.y, currentPathPoint.currentPoint.z);
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.MoveToTarget, MsgSend_Move(BObjectId, targetPos, String.Empty));
            }
        }
    }

    //获取下一个要执行的操作
    private TaskBase getNextRunTask()
    {
        for (int i = 0; i < currentPathPoint.tasks?.Count; i++)
        {
            if (!currentPathPoint.tasks[i].isRuned) return currentPathPoint.tasks[i];
        }

        return null;
    }

    private string MsgSend_Move(string id, Vector3 pos, string targetId)
    {
        return string.Format($"{id}_{pos.x}_{pos.y}_{pos.z}_{targetId}");
    }
}