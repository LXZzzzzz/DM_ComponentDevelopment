using ToolsLibrary.EquipPart;
using UnityEngine;

public class FirePointLogic : ZiYuanBase, ISourceOfAFire
{
    private FireManage fm;
    private float fs, pd, csrsmj;
    private float allWeight;

    public void Init(float fs, float pd, float csrsmj, string id)
    {
        base.Init(id, 50);
        ZiYuanType = ZiYuanType.SourceOfAFire;
        fm = gameObject.AddComponent<FireManage>();

        fm.Init(fs, pd, csrsmj);

        this.fs = fs;
        this.pd = pd;
        this.csrsmj = csrsmj;
        allWeight = 0;
    }

    public void waterPour(float time, float squareMeasure, float weight)
    {
        Debug.LogError($"火源点在{time}时刻受到{weight}面积对应{weight}kg重的水");
        fm.SetDrowning(weight, time);
        allWeight += weight;
    }

    public bool getTaskProgress()
    {
        return fm.IsFire;
    }

    public void getFireData(out float ghmj, out float rsmj,out float csghmj, out float csrsmj, out float tszl)
    {
        Debug.LogError("收到的投水量"+allWeight);
        ghmj = (float)fm.burnedArea;
        rsmj = (float)fm.burnArea;
        csghmj = (float)fm.csBurnedArea;
        csrsmj = this.csrsmj;
        tszl = allWeight;
    }

    protected override void OnReset()
    {
        fm.Init(fs, pd, csrsmj);
        allWeight = 0;
    }
}