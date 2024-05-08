using System.Collections;
using System.Collections.Generic;
using UiManager;
using UnityEngine;

public class UICommanderSecondLevel : BasePanel
{
    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
    }
    
    
    //接收一级指挥官发来的具体任务和装备数据，并刷新到界面上
    //收到可操作装备后，要打开小地图上指定装备的可操作属性
    
}
