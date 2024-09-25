using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class ZiYuan_Task : ZiYuanBase, ITaskProgress
{
    private ZiYuanBase associationGo;

    public void Init(string id, string associationId)
    {
        base.Init(id, 0, "", "");
        ZiYuanType = ZiYuanType.TaskPoint;
        StartCoroutine(getZy(associationId));
    }

    IEnumerator getZy(string associationId)
    {
        yield return 1;
        //找到关联ID对应的GO
        for (var i = 0; i < allBObjects.Length; i++)
        {
            yield return 1;
            if (string.Equals(allBObjects[i].BObject.Id, associationId))
            {
                Debug.LogError("找到了关联资源");
                associationGo = allBObjects[i].GetComponent<ZiYuanBase>();
                break;
            }
        }
    }

    public override void OnStart()
    {
    }

    protected override void OnReset()
    {
        //清除任务状态
    }

    public string getAssociationAssemblyId()
    {
        if (associationGo == null)
            return String.Empty;
        return associationGo.BobjectId;
    }

    public bool getTaskProgress(out string progressInfo, out float progressNum)
    {
        progressInfo = "";
        progressNum = 0;
        bool isComplete = false;
        if (associationGo == null)
            return false;
        switch (associationGo.ZiYuanType)
        {
            case ZiYuanType.SourceOfAFire:
                isComplete = (associationGo as ISourceOfAFire).getTaskProgress();
                (associationGo as ISourceOfAFire).getFireData(out float ghmj, out float rsmj, out float csghmj, out float csrsmj, out float tszl);
                progressInfo = $"需求水量：{(int)(rsmj < 0 ? 0 : rsmj)}kg";
                progressNum = Mathf.Clamp((csrsmj - rsmj) / csrsmj, 0, 1);
                break;
            case ZiYuanType.RescueStation:
                isComplete = (associationGo as IRescueStation).getTaskProgress(out int currentPersonNum, out int maxPersonNum, out float currentGoodsNum, out float maxGoodsNum);
                progressInfo = $"安置受灾群众:{currentPersonNum}人/{maxPersonNum}人\n所需物资:{currentGoodsNum}kg/{maxGoodsNum}kg";
                progressNum = maxGoodsNum == 0 ? 0 : Mathf.Clamp(currentGoodsNum / maxGoodsNum, 0, 1);
                break;
            case ZiYuanType.Hospital:
                isComplete = (associationGo as IRescueStation).getTaskProgress(out int currentPersonNum1,
                    out int maxPersonNum1, out float currentGoodsNum1, out float maxGoodsNum1);
                progressInfo = $"救治伤员:{currentPersonNum1}人";
                break;
            case ZiYuanType.DisasterArea:
                isComplete = (associationGo as IDisasterArea).getTaskProgress(out int currentNum, out int maxNum);
                progressInfo = $"转运人员:{currentNum}人/{maxNum}人";
                progressNum = Mathf.Clamp((maxNum - currentNum) / (float)maxNum, 0, 1);
                break;
        }

        return isComplete;
    }
}