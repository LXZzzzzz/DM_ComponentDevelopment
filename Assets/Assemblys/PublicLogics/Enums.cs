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
        SendSkillInfoForControler, //向指挥角色发送技能请求
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
        ClearMsgBox, //清除操作记录数据
        GameStop, //通知游戏结束
        ChangeCurrentCom, //更改当前选择的指挥端
        ReceiveTask, //总指挥接收任务
        ChangeObjController, //更改某个对象的控制者
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

        TriggerOnlyShow = 1200, //触发只做显示的文本
        TriggerReport = 1201, //触发报备指令
    }
}