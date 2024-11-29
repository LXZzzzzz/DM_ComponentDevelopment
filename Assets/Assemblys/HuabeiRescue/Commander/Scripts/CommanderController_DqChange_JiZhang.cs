using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public partial class CommanderController
{
    private EquipBase myEquip;
    public void Init(string myId)
    {
        //这里应该是主角数据初始化已经走过了，直接去直升机列表中找自己的id
        myEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BeLongToCommanderId, myId));
    }
}