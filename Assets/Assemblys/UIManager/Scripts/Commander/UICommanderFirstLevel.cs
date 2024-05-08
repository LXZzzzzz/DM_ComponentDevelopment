using System.Collections;
using System.Collections.Generic;
using UiManager;
using UnityEngine;

public class UICommanderFirstLevel : BasePanel
{
    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        //刷新界面上的数据显示 
        //操控者会把自身关联的下级指挥官传过来
        //获取场景中全部可用装备
        //通过接口获取全部任务数据
    }
    
    
    
}
