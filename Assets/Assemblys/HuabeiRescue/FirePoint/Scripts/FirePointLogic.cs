using ToolsLibrary.EquipPart;
using UnityEngine;

public class FirePointLogic : ZiYuanBase, ISourceOfAFire
{
    private FireManage fm;
    private float fs, pd, csrsmj;

    public void Init(float fs, float pd, float csrsmj, string id)
    {
        base.Init(id, 50);
        ZiYuanType = ZiYuanType.SourceOfAFire;
        fm = gameObject.AddComponent<FireManage>();

        fm.Init(fs, pd, csrsmj);

        this.fs = fs;
        this.pd = pd;
        this.csrsmj = csrsmj;
    }

    public void waterPour(float time, float squareMeasure)
    {
        Debug.LogError($"火源点在{time}时刻受到{squareMeasure}kg的水");
        fm.SetDrowning(squareMeasure, time);
    }

    public bool getFireExtinguishingProgress()
    {
        return fm.IsFire;
    }

    protected override void OnReset()
    {
        fm.Init(fs, pd, csrsmj);
    }
}