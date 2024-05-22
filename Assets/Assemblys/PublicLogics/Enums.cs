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
        LoadProgrammeDataSuc,//读取方案成功通知场景响应
    }

    public enum MessageID
    {
        SendProgramme = 1001, //一级指挥端发送方案
        MoveToTarget = 1002, //发送移动指令
    }
}