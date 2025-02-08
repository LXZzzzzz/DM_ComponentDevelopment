using System;
using System.Collections;
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

    // private bool isAutoRunEnd;
    private string skillConfirmationStr;
    private TaskBase currentRunTask;
    private string stopQjdtId; //临时起降点Id
    private string stopAirPortId; //停靠机场Id
    private Queue<object> runQueue;
    private bool isHaveReturnForRepair; //是否需要返修
    private bool isAtAirport; //是否在机场

    public void InitData(string id, string qjdId, string ctrlId, string airPortId)
    {
        BObjectId = id;
        stopQjdtId = qjdId;
        BeLongToCommanderId = ctrlId;
        stopAirPortId = airPortId;
        isAtAirport = true;
        // isAutoRunEnd = false;
    }

    public string GetStopAtAirPort()
    {
        if (isAtAirport) return stopAirPortId;
        else return stopQjdtId;
    }

    public void GoReturnBack()
    {
        //去除后续执行计划，并取消当前所执行内容，直接回机场
        EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 2);
        EventManager.Instance.EventTrigger(EventType.ClearPathPlanningData.ToString(), BObjectId);
        StartCoroutine(WaitAndPrint());

        if (!string.IsNullOrEmpty(skillConfirmationStr))
            MyDataInfo.SkillsToBeConfirmed.Remove(skillConfirmationStr);
        skillConfirmationStr = String.Empty;

//⭐后面要把移动到目的地归为技能一类
        //起飞->飞往机场->降落->入库
        if (runQueue == null) runQueue = new Queue<object>();
        runQueue.Clear();
        switch (myState)
        {
            case HelicopterState.flying:
                Vector3 targetPos = sceneAllZiyuan.Find(x => string.Equals(x.BobjectId, stopQjdtId)).transform.position;
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.MoveToTarget, MsgSend_Move(BObjectId, targetPos, stopQjdtId));

                skillConfirmationStr = BObjectId + MessageID.MoveToTarget;
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendSkillConfirmation, skillConfirmationStr);
                //在插入指令的同时执行操作，会在指令结束后移除一个，这里补一个移除替代品
                runQueue.Enqueue(SkillType.None);
                break;
            case HelicopterState.Landing:
                runQueue.Enqueue(SkillType.TakeOff);
                runQueue.Enqueue(MessageID.MoveToTarget);
                break;
            case HelicopterState.hover:
                runQueue.Enqueue(MessageID.MoveToTarget);
                break;
            case HelicopterState.NotReady:
                return;
        }

        runQueue.Enqueue(SkillType.Landing);
        runQueue.Enqueue(SkillType.BePutInStorage);
        currentPathPoint = null;

        Debug.LogError("下面要执行的动作：" + runQueue.Peek());
        Debug.LogError("再看一下数据是空吗" + PathPointManager.Instance.GetPointDataByBObjectId(BObjectId));
        Debug.LogError(PathPointManager.Instance.GetPointDataByBObjectId(BObjectId));
        Debug.LogError(PathPointManager.Instance.GetPointDataByBObjectId(BObjectId)?.NextPointId);
    }

    private IEnumerator WaitAndPrint()
    {
        // 等待1秒
        yield return new WaitForSeconds(1f);
        EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 3);
    }

    public void StopRunTime()
    {
        CancelCurrentSkill();
    }

    public void GoReturnRepair()
    {
        if (MyDataInfo.MyLevel != 3) return;
        isHaveReturnForRepair = true;
        Debug.LogError("返修标记成功");
    }

    public void ChangeCurrentState(int state)
    {
        CurrentState = state;
    }

    public void GoFerryFlights()
    {
        //转场飞行执行
        // GoReturnBack();
        if (runQueue == null) runQueue = new Queue<object>();
        runQueue.Clear();
        runQueue.Enqueue(SkillType.GroundReady);
        runQueue.Enqueue(SkillType.TakeOff);
        runQueue.Enqueue(MessageID.MoveToTarget);
        runQueue.Enqueue(SkillType.Landing);
        runQueue.Enqueue(SkillType.BePutInStorage);
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
        runQueue.Clear();
        runQueue.Enqueue(SkillType.TakeOff);
        runQueue.Enqueue(MessageID.MoveToTarget);
        runQueue.Enqueue(SkillType.Landing);
        runQueue.Enqueue(SkillType.BePutInStorage);
        currentPathPoint.isRuned = true;
        currentPathPoint = null;
    }

    private void OnRunInstructionUpdate()
    {
        //坠毁就不执行了，可能还需要做标记，任务未完成
        if (isCrash) return;

        //如果有发出未执行指令，就跳出
        if (!string.IsNullOrEmpty(skillConfirmationStr))
        {
            if (MyDataInfo.SkillsToBeConfirmed.Find(a => string.Equals(a, skillConfirmationStr)) != null)
            {
                Debug.LogError(currentSkill + "收到了一个技能点执行成功" + skillConfirmationStr);
                if (runQueue != null && runQueue.Count > 0)
                {
                    Debug.LogError("插入数据执行成功");
                    runQueue.Dequeue();
                    if (runQueue.Count == 0) isStartAutoRun = false;
                    MyDataInfo.SkillsToBeConfirmed.Remove(skillConfirmationStr);
                    skillConfirmationStr = String.Empty;
                    return;
                }

                //代表接收到了技能回调，说明技能发送完成
                MyDataInfo.SkillsToBeConfirmed.Remove(skillConfirmationStr);
                skillConfirmationStr = String.Empty;
            }

            return;
        }

        //如果正在执行指令或正在飞行，就等待
        if (currentSkill != SkillType.None || !isArrive) return;

        //如果有插入类待执行，就先走这个
        if (runQueue != null && runQueue.Count > 0)
        {
            //要执行临时队列事件
            if (runQueue.Peek() is MessageID)
            {
                //执行机动到机场的指令
                Vector3 targetPos = sceneAllZiyuan.Find(x => string.Equals(x.BobjectId, stopQjdtId)).transform.position;
                EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.MoveToTarget, MsgSend_Move(BObjectId, targetPos, stopQjdtId));

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

        //如果直升机还没开始运行，就先起飞
        if (!isStartAutoRun)
        {
            //这里判断这个飞机是否有未走的点，如果没有了，就return
            if (PathPointManager.Instance.GetPointDataById(nextPointId) == null)
            {
                Debug.LogError("没有待执行任务" + MyDataInfo.SkillsToBeConfirmed?.Count);
                return;
            }

            Debug.LogError("刚开始起飞");
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

        if (PathPointManager.Instance.GetPointDataByBObjectId(BObjectId) == null) return;

        //以下逻辑是正常走规划数据
        currentRunTask = getNextRunTask();
        if (currentRunTask != null)
        {
            //执行下一个任务项
            Debug.LogError("执行指令：" + currentRunTask.runSkillType);
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
            currentPathPoint = getNextRunPoint();

            if (currentPathPoint == null) Debug.LogError("执行完了所有内容"); //isAutoRunEnd = true;
            else
            {
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
        currentPathPoint?.tasks?.Sort((a, b) => a.orderNumber < b.orderNumber ? -1 : 1);

        if (currentRunTask != null) currentRunTask.isRuned = true;

        for (int i = 0; i < currentPathPoint?.tasks?.Count; i++)
        {
            Debug.LogError("遍历该点任务列表" + currentPathPoint.tasks[i].orderNumber + currentPathPoint.tasks[i].runSkillType);
            if (!currentPathPoint.tasks[i].isRuned)
            {
                Debug.LogError("要执行的任务：" + currentPathPoint.tasks[i].orderNumber + currentPathPoint.tasks[i].runSkillType);
                return currentPathPoint.tasks[i];
            }
        }

        return null;
    }

    private PathPoint getNextRunPoint()
    {
        //如果不是空，直接走下一个点，如果是空，就从数据中找出一个未执行的点返回
        if (currentPathPoint == null)
        {
            Debug.LogError("当前点是空");
            var itemPoint = PathPointManager.Instance.GetPointDataByBObjectId(BObjectId);
            Debug.LogError("路径起点是：" + itemPoint?.pointId);
            while (itemPoint != null && PathPointManager.Instance.GetPointDataById(itemPoint.pointId).isRuned)
            {
                itemPoint = PathPointManager.Instance.GetPointDataById(itemPoint.NextPointId);
            }

            Debug.LogError("要执行的点是：" + itemPoint?.pointId);
            return itemPoint;
        }
        else
        {
            Debug.LogError("当前点走完了，前往下一个");
            currentPathPoint.isRuned = true;
            return PathPointManager.Instance.GetPointDataById(currentPathPoint.NextPointId);
        }
    }

    private string MsgSend_Move(string id, Vector3 pos, string targetId)
    {
        return string.Format($"{id}_{pos.x}_{pos.y}_{pos.z}_{targetId}");
    }
}