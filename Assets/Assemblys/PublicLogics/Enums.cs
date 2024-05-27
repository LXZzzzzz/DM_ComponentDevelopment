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
    }

    public enum MessageID
    {
        SendProgramme = 1001, //一级指挥端发送方案
        MoveToTarget = 1002, //发送移动指令
        TriggerWaterIntaking = 1003, //触发取水操作
    }
}