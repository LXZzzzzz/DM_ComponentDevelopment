namespace Enums
{
    public enum EventType
    {
        Test,
        ShowUI,
        ChooseEquip,
        MoveToTarget,
        SwitchMapModel, //切换地图模式
        TransferEditingInfo, //传递地图编辑信息
        CreatEquipEntity, //创建飞机实体
        CreatEquipCorrespondingIcon, //创建飞机对应的图标
        LoadProgrammeDataSuc, //读取方案成功通知场景响应
        MapChooseIcon, //地图上选择icon后通知出去（主要代表选择资源：取水点、补给点。。。）
        SendSkillInfoForControler, //向指挥角色发送技能请求,消息发送入口，所有的消息都通过这里发出
        AddCommanderForZiYuan, //为资源添加控制者
        InitZiYuanBeUsed, //初始化资源可被谁使用
        CameraSwitch, //相机切换是否开启控制脚本
        CameraControl, //控制相机追踪或定位
        ShowProgrammeName, //显示方案名
        ChooseZiyuan, //选中某个资源
        DestoryEquip, //删除某个飞机
        ClearProgramme, //清除方案数据
        CloseCreatTarget, //关闭创建的模板对象
        SetMyEquipIconLayer, //设置自己飞机icon的最高层级
        GeneratePDF, //生成pdf
        ShowTipUI, //非UI层调用弹窗页面
        ShowTipUIAndCb, //非UI层调用弹窗页面带回调
        ShowConfirmUI, //非UI曾调用二次确认窗口
        ChooseEquipToZiYuanType, //控制飞机飞往某个资源点
        ShowAMsgInfo, //展示一个指令信息
        ShowAMsgInfoWithData, //展示一个指令信息,携带展示数据
        ClearMsgBox, //清除操作记录数据
        GameStop, //通知游戏结束
        ChangeCurrentCom, //更改当前选择的指挥端
        ReceiveTask, //总指挥接收任务
        ChangeObjController, //更改某个对象的控制者
        CreatZaiQuZy, //创建灾区资源
        CreatZaiQuZyRun, //创建灾区资源执行
        DestoryZaiQuzy, //删除一个灾区资源
        CreatAZiyuanIcon, //创建指定资源的UI标识
        DestoryZiyuanIcon, //删除指定资源的UI标识
        CreatATaskIcon, //创建指定任务的UI标识
        MarkMapPoints, //标记地图点请求
        ShowMarkMapPoint, //显示标记点
        crashIcon, //坠毁后通知Ui
        LoadPathPlanningData, //加载规划点数据
        ClearPathPlanningData, //清空规划点
        CloseEditorModel, //关闭路径编辑模式
        MoveToTarget_AutoRun, //自动执行移动到目标点

        InitEquipData, //加载方案时初始化直升机数据
        ChangeJiZhangView, //机长端通知页面修改,控制哪些资源显示
        DqChooseGo, //选中某个对象
        changeJizuShow, //更改机组可选信息
        AskForReturnTrigger, //界面发送给管理器申请返回
        HideGoIcon, //隐藏某个对象的图标，主要用来创建灾区不立即显示
        TransferPersonData, //传递人员数据给UI
        TransferMisDescription, //传递灾情信息给UI
        TransferKongguanData, //传递空管信息给UI
        TransferTianqiData, //传递天气信息给UI
        CompleteATrainPoint, //完成了一个训练点
    }

    public enum MessageID
    {
        SendProgramme = 1001, //一级指挥端发送方案
        SendGameStart = 1002, //一级指挥端发送开始推演
        MoveToTarget = 1003, //发送移动指令
        SendGamePause = 1005, //一级指挥端发送暂停操作
        SendGameStop = 1006, //一级指挥端发送停止操作
        SendChangeSpeed = 1007, //一级指挥端更改运行速度
        SendReceiveTask = 1008, //一级指挥端接收任务，开始计时
        SendChangeController = 1009, //游戏进行中更改控制者
        SendChangeZaiqu = 1010, //更改场景中的灾区
        SendMarkMapPoint = 1011, //发送标记场景点的请求
        SendGetChangeZQPower = 1012, //请求获取修改灾区权限
        SendLoseChangeZQPower = 1013, //请求释放修改灾区权限
        SendPathPlanningData = 1014, //发送路径规划数据
        SendSkillConfirmation = 1015, //发送技能确认，随技能指令发送，收到服务器反馈标志技能使用成功
        SendEquipBindingZiyuan = 1016, //发送直升机绑定资源数据
        SendEquipState = 1017, //发送所有直升机当前状态
        SendTianQi = 1018, //发送天气情况
        SendAskForAirLine = 1019, //总指发送申请航线消息
        SendAgreeAirLine = 1020, //导教端发送航线信息的反馈
        SendAskForTaskExecute = 1021, //前指发送任务执行请求
        SendAgreeTaskExecute = 1022, //总指发送任务执行的反馈--->修改为前指发送任务下达指令
        SendTaskPlanningCompleted = 1023, //机长发送任务规划完成
        SendTurnBack = 1024, //发送立即返航指令
        SendChangeTaskPlanning = 1025, //可能要加 ：导教端修改了灾区数据后，前指要发送这个指令，告诉机长要根据最新灾情调整任务规划数据
        SendTaskBgInfo = 1026, //导教端发送任务背景信息
        SendZySetData = 1027, //导教端发送地面保障资源的修改数据
        SendEquipsInfo = 1028, //导教端发送各个直升机信息
        SendPersonsInfo = 1029, //导教端发送人员信息
        SendAskForReturn = 1030, //机长请求返航或返修
        SendAgreeReturn = 1031, //前指发送请求反馈
        SendCompleteTaskBgSet = 1032, //导教端发送完成任务背景设置
        SendRwghData = 1033, //机长发送任务规划数据
        SendDiscoverNewDisaster = 1034, //上报发现新灾情
        SendAgreeDiscoverNewDisaster = 1035, //确认发现了新灾情
        SendChangeEquipOilAndLoad = 1036, //修改直升机的载油量和装载量
        SendChangeZiyuanData = 1037, //一级修改资源点的数据
        SendFerryFlights = 1038, //触发直升机转场飞行
        SendEquipUsedInfo = 1039, //一级指挥发送哪些直升机可用
        SendPersonUsedInfo = 1040, //一级发送哪些机组人员可用
        SendTrainPointSucInfo = 1041, //发送训练点完成的信息
        SendShowAMsgWithData = 1042, //发送某端带数据的实时信息


        TriggerGroundReady = 1101, //触发起飞前准备操作
        TriggerBePutInStorage = 1102, //触发入库操作

        TriggerTakeOff = 1103, //触发起飞操作
        TriggerLanding = 1104, //触发降落操作
        TriggerSupply = 1105, //触发补给操作

        TriggerWaterIntaking = 1106, //触发取水操作
        TriggerWaterPour = 1107, //触发投水操作

        TriggerLadeGoods = 1108, //触发装载物资操作
        TriggerUnLadeGoods = 1109, //触发卸载物资操作
        TriggerAirDropGoods = 1110, //触发空投物资操作

        TriggerManned = 1111, //触发装载人员操作
        TriggerPlacementOfPersonnel = 1112, //触发安置人员操作
        TriggerCableDescentRescue = 1113, //触发索降救援操作

        TriggerReturnFlight = 1114, //触发返航指令
        TriggerEndTask = 1115, //触发结束任务操作

        TriggerEquipCrash = 1116, //触发飞机坠毁


        //这俩消息应该放到Send部分
        TriggerOnlyShow = 1200, //触发只做显示的文本
        TriggerReport = 1201, //触发报备指令
    }

    public enum ShowZyDataType
    {
        TaskBgShow, //导教端展示任务背景界面
        GroundSupport, //导教端地面保障
        GroundDisaster, //地面灾区
        EquipsShow, //导教端直升机信息展示
        PersonShow, //导教端人员信息展示
        TqChange, //导教端天气变化设置界面
        zbgzChange, //导教端装备故障设置界面

        zqxxShow, //一级展示灾情信息
        zbxxShow, //展示装备信息
        ryxxShow, //展示人员信息
        kgxxShow, //展示空管信息
        rwxxShow, //展示任务信息 
        rwqzbShow, //展示任务前准备
        dmzbShow, //展示地面准备
    }

    public enum TrainsPintType
    {
        ZBLDSureDisasterInfo, //确认灾情信息
        ZBLDSureEquipInfo, //确认装备信息
        ZBLDSurePersonInfo, //确认人员信息
        ZBLDRouteDeclaration, //航线申报
        ZBLDSendTask, //值班领导下达任务
        XCZHGetTask, //现场指挥领受任务
        XCZHInspectEquipInfo, //检查装备装载设备、载油量、状态信息
        XCZHSendTask, //前线指挥完成任务分配并下达任务
        XCZHSureTqInfo, //前线指挥确认机长的特情信息
        JZSureTaskInfo, //机长确认任务分配信息
        JZSureOilAndLoad, //机长确认油量和装载量
        JZCompletePlan, //机长完成航线规划
        JZOilInsufficient, //机长发生燃油不足告警
        JZLandingError, //机长着陆区域错误
        JZSendTqInfo, //机长发送特情信息
    }
}