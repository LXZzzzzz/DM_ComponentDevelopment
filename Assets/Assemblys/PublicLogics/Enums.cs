namespace Enums
{
    public enum EventType
    {
        Test,
        ShowUI,
        ChooseEquip,
        MoveToTarget,
        SwitchCreatModel, //切换为创建模式
        CreatEquipEntity, //创建飞机实体
        CreatEquipCorrespondingIcon, //创建飞机对应的图标
        LoadProgrammeDataSuc, //读取方案成功通知场景响应
        MapChooseIcon, //地图上选择icon后通知出去（主要代表选择资源：取水点、补给点。。。）
        SendWaterInfoToControler, //向指挥角色发送取水技能数据
        AddCommanderForZiYuan, //为资源添加控制者
        InitZiYuanBeUsed, //初始化资源可被谁使用
        ConfirmationCbSure, //确认窗口返回确认消息
        CameraSwitch,//相机切换是否开启控制脚本
        ShowProgrammeName,//显示方案名
    }

    public enum MessageID
    {
        SendProgramme = 1001, //一级指挥端发送方案
        SendGameStart = 1002, //一级指挥端发送开始推演
        MoveToTarget = 1003, //发送移动指令
        TriggerWaterIntaking = 1004, //触发取水操作
    }
}