using System.Collections;
using System.Collections.Generic;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class ZiYuan_Task : ZiYuanBase, ITaskProgress
{
    private ZiYuanBase associationGo;

    public void Init(string id, string associationId)
    {
        base.Init(id, 0);
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

    protected override void OnReset()
    {
        //清除任务状态
    }

    public bool getTaskProgress()
    {
        if (associationGo == null)
            return false;
        switch (associationGo.ZiYuanType)
        {
            case ZiYuanType.SourceOfAFire:
                return (associationGo as ISourceOfAFire).getTaskProgress();
            case ZiYuanType.RescueStation:
                return (associationGo as IRescueStation).getTaskProgress();
            case ZiYuanType.DisasterArea:
                return (associationGo as IDisasterArea).getTaskProgress();
        }

        return false;
    }
}