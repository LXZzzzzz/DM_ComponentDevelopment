using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class HelicopterController : EquipBase
{
    public override void Init()
    {
        base.Init();
        //初始化飞机的基本属性
    }

    private void Start()
    {
        Debug.LogError("直升机控制脚本挂上了");
    }

    // public void WaterIntaking(Vector3 pos, float range, float amount)
    // {
    //     //判断自己是否处于取水地的范围内，不在的话调move机动到目的地，然后，执行取水逻辑
    //     if (Vector3.Distance(transform.position, pos) > range)
    //     {
    //         MoveToTarget(pos);
    //     }
    // }
    //
    // public float CheckCapacity()
    // {
    //     return 10;
    // }
}