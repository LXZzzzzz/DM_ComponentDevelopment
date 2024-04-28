namespace DefaultRole
{
    public enum RoleEvent
    {
        UIOperInput=100,
        UIToolInput=101,
        UIPanelInput=105,
        RoleMouseInput=106,
        UIMapTargetInput=107,
        MouseInput=108,
        ConfirmInput=109,
        READInput=110,

        SyncData =102,

        DialogEvent=103,
        DialogMes=104,

        Board = 901,
        Leave = 902,
        GetOperCtrl = 903,   //获取操控
        LostOperCtrl = 904,   //释放操控
        GetAutoDrive = 905,  //获取自动驾驶控制
        LostAutoDrive = 906, //释放自动驾驶控制
        BoardMove=907,   
        Board_Method2=908,
        FailAnimRollBack=909,  //操作失败回滚动画
        GetOperUI=910,    //获取操作UI
        LostOperUI=911,   //释放操作UI
        ConfirmUI=912,    //显示确认消息面板
    }
    public enum RoleSystemType
    {
        RoleOper=9,
    }
}

