using System;
using System.Collections.Generic;
using Enums;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.PathPart;
using UnityEngine;
using UnityEngine.Events;
using EventType = Enums.EventType;

public partial class HelicopterController
{
    private PathPoint currentPathPoint;
    private bool isAutoRunEnd;
    private string skillConfirmationStr;
    private TaskBase currentRunTask;
    private string stopAtAirPortId; //停靠机场Id
    private Queue<object> runQueue;
    private bool isHaveReturnForRepair; //是否需要返修

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

    public void GoReturnBack()
    {
        //去除后续执行计划，并取消当前所执行内容，直接回机场

        while (!string.IsNullOrEmpty(lastPointId))
        {
            PathPointManager.Instance.RemovePoint(lastPointId);
        }
        //⭐⭐这里还得通知界面把规划路径点删掉

        skillConfirmationStr = String.Empty;

        CancelCurrentSkill();
//⭐后面要把移动到目的地归为技能一类
        //起飞->飞往机场->降落->入库
        if (runQueue == null) runQueue = new Queue<object>();
        runQueue.Enqueue(SkillType.TakeOff);
        runQueue.Enqueue(MessageID.MoveToTarget);
        runQueue.Enqueue(SkillType.Landing);
        runQueue.Enqueue(SkillType.BePutInStorage);
    }

    public void ChangeCurrentState(int state)
    {
        CurrentState = state;
        //这里要提示一个窗口，直升机故障，是否返航或返修
        if (MyDataInfo.MyLevel == 3)
        {
            switch (state)
            {
                case 0: //正常情况
                    break;
                case 1: //返修
                    //标记返修，在直升机执行完当前动作，再规划返修操作
                    EventManager.Instance.EventTrigger<string, UnityAction>(EventType.ShowTipUIAndCb.ToString(), "直升机发生小故障，是否待执行完当前动作后返修？",
                        () =>
                        {
                            isHaveReturnForRepair = true;
                            Debug.LogError("需要返修");
                        });
                    break;
                case 2:
                    EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "直升机严重故障，将立刻返航！");
                    GoReturnBack();
                    break;
            }
        }
    }

    // 强制结束当前进度
    private void CancelCurrentSkill()
    {
        skillProgress = 1;
        timer = 0; // 重置计时器
        isRunTimer = false;
        currentSkill = SkillType.None;
    }

    private void GoReturnForRepair()
    {
        isHaveReturnForRepair = false;
        if (runQueue == null) runQueue = new Queue<object>();
        runQueue.Enqueue(SkillType.TakeOff);
        runQueue.Enqueue(MessageID.MoveToTarget);
        runQueue.Enqueue(SkillType.Landing);
        runQueue.Enqueue(SkillType.BePutInStorage);
        runQueue.Enqueue(SkillType.GroundReady);
        runQueue.Enqueue(SkillType.TakeOff);
    }

    private void OnRunInstructionUpdate()
    {
        //坠毁就不执行了，可能还需要做标记，任务未完成
        if (isCrash || isAutoRunEnd) return;

        //如果有发出未执行指令，就跳出
        if (!string.IsNullOrEmpty(skillConfirmationStr))
        {
            if (MyDataInfo.SkillsToBeConfirmed.Find(a => string.Equals(a, skillConfirmationStr)) != null)
            {
                if (runQueue != null && runQueue.Count > 0)
                {
                    runQueue.Dequeue();
                    skillConfirmationStr = String.Empty;
                    return;
                }

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

        //如果有插入类待执行，就先走这个
        if (runQueue != null && runQueue.Count > 0)
        {
            //要执行临时队列事件
            if (runQueue.Peek() is MessageID)
            {
                //执行机动到机场的指令
                Vector3 targetPos = sceneAllZiyuan.Find(x => string.Equals(x.BobjectId, stopAtAirPortId)).transform.position;
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.MoveToTarget, MsgSend_Move(BObjectId, targetPos, stopAtAirPortId));

                skillConfirmationStr = BObjectId + MessageID.MoveToTarget;
            }
            else if (runQueue.Peek() is SkillType)
            {
                //执行正常指令
                OnSelectSkill((SkillType)runQueue.Peek());
                skillConfirmationStr = BObjectId + (SkillType)runQueue.Peek();
            }

            EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendSkillConfirmation, skillConfirmationStr);
            return;
        }

        if (PathPointManager.Instance.GetPointDataByBObjectId(BObjectId) == null) return;

        //以下逻辑是正常走规划数据
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
            if (isHaveReturnForRepair)
            {
                Debug.LogError("执行返修");
                GoReturnForRepair();
                return;
            }

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