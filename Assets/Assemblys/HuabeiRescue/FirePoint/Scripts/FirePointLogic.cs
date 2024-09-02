using System;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class FirePointLogic : ZiYuanBase, ISourceOfAFire
{
    private FireManage fm;
    private float fs, pd, csrsmj;
    private float allWeight;
    private bool isStart;

    public void Init(float fs, float pd, float csrsmj, string id, string colorCode, string chooseColoeCode)
    {
        base.Init(id, 50, colorCode, chooseColoeCode);
        ZiYuanType = ZiYuanType.SourceOfAFire;
        fm = gameObject.AddComponent<FireManage>();

        fm.Init(fs, pd, csrsmj);

        this.fs = fs;
        this.pd = pd;
        this.csrsmj = csrsmj;
        allWeight = 0;
        isStart = false;
    }

    public void waterPour(float time, float squareMeasure, float weight)
    {
        fm.SetDrowning(weight, time);
        allWeight += weight;
    }

    public bool getTaskProgress()
    {
        return fm.IsFire;
    }

    public void getFireData(out float ghmj, out float rsmj, out float csghmj, out float csrsmj, out float tszl)
    {
        // if (isStart)
        //     fm.UpdateBurnArea(MyDataInfo.gameStartTime);
        ghmj = (float)fm.burnedArea;
        rsmj = (float)fm.burnArea;
        csghmj = (float)fm.csBurnedArea;
        csrsmj = this.csrsmj;
        tszl = allWeight;
    }

    public void updateBA()
    {
        fm.UpdateBurnArea();
    }

    public override void OnStart()
    {
        fm.OnStart();
        isStart = true;
    }

    protected override void OnReset()
    {
        fm.Init(fs, pd, csrsmj);
        allWeight = 0;
        isStart = false;
    }
}