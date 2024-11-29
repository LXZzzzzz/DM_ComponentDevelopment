using System;
using System.Collections.Generic;
using Enums;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.PathPart;
using UnityEngine;
using EventType = Enums.EventType;

public partial class HelicopterController
{
    private PathPoint currentPathPoint;
    private bool isAutoRunEnd;
    private string skillConfirmationStr;
    private TaskBase currentRunTask;
    private string stopAtAirPortId; //停靠机场Id

    public void InitData(string id, string airPortId, string ctrlId)
    {
        BObjectId = id;
        stopAtAirPortId = airPortId;
        BeLongToCommanderId = ctrlId;
    }

    public string GetStopAtAirPort()
    {
        return stopAtAirPortId;
    }

    private void OnRunInstructionUpdate()
    {
        //坠毁就不执行了，可能还需要做标记，任务未完成
        if (isCrash || isAutoRunEnd) return;
        if (PathPointManager.Instance.GetPointDataByBObjectId(BObjectId) == null) return;

        //如果有发出未执行指令，就跳出
        if (!string.IsNullOrEmpty(skillConfirmationStr))
        {
            if (MyDataInfo.SkillsToBeConfirmed.Find(a => string.Equals(a, skillConfirmationStr)) != null)
            {
                //代表接收到了技能回调，说明技能发送完成
                MyDataInfo.SkillsToBeConfirmed.Remove(skillConfirmationStr);
                skillConfirmationStr = String.Empty;
                if (currentRunTask != null) currentRunTask.isRuned = true;
            }

            return;
        }

        //如果正在执行指令或正在飞行，就等待
        if (currentSkill != SkillType.None || !isArrive) return;

        //如果直升机还没开始运行，就先起飞
        if (!isStartAutoRun)
        {
            Debug.LogError("当前的状态" + myState);
            //准备操作
            switch (myState)
            {
                case HelicopterState.NotReady:
                    Debug.LogError("发送起飞前准备");
                    OnSelectSkill(SkillType.GroundReady);
                    skillConfirmationStr = BObjectId + SkillType.GroundReady;
                    break;
                case HelicopterState.Landing:
                    Debug.LogError("发送起飞");
                    OnSelectSkill(SkillType.TakeOff);
                    skillConfirmationStr = BObjectId + SkillType.TakeOff;
                    isStartAutoRun = true;
                    break;
            }
            EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendSkillConfirmation, skillConfirmationStr);
            return;
        }

        currentRunTask = getNextRunTask();
        if (currentRunTask != null)
        {
            //执行下一个任务项
            OnSelectSkill(currentRunTask.runSkillType);
            skillConfirmationStr = BObjectId + currentRunTask.runSkillType;
            EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendSkillConfirmation, skillConfirmationStr);
        }
        else
        {
            //该路点任务集执行完毕，走向下一个点
            currentPathPoint = currentPathPoint == null ? PathPointManager.Instance.GetPointDataByBObjectId(BObjectId) : PathPointManager.Instance.GetPointDataById(currentPathPoint.NextPointId);
            if (currentPathPoint == null) isAutoRunEnd = true;
            else
            {
                currentPathPoint.tasks.Sort((a, b) => a.orderNumber < b.orderNumber ? -1 : 1);

                //执行让直升机飞往该路点
                if (!OnCheckIsMove()) return;
                Vector3 targetPos = new Vector3(currentPathPoint.currentPoint.x, currentPathPoint.currentPoint.y, currentPathPoint.currentPoint.z);
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.MoveToTarget, MsgSend_Move(BObjectId, targetPos, String.Empty));

                skillConfirmationStr = BObjectId + MessageID.MoveToTarget;
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendSkillConfirmation, skillConfirmationStr);
            }
        }
    }

    //获取下一个要执行的操作
    private TaskBase getNextRunTask()
    {
        for (int i = 0; i < currentPathPoint?.tasks?.Count; i++)
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