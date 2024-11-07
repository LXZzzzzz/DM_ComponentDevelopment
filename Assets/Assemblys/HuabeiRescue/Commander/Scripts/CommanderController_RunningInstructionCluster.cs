using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public partial class CommanderController
{
    private void OnRunInstructionUpdate()
    {
        //Todo:这里得到指令并开始运行后，要对指令（指令是模糊指令，要针对当前状态做对应具体决策）中对应的飞机下具体操作指令
        
        
        
        
        
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            if (string.Equals(MyDataInfo.sceneAllEquips[i].BeLongToCommanderId, MyDataInfo.leadId))
            {
                //归属于自己支队的直升机才运行
                RunInstruction(MyDataInfo.sceneAllEquips[i]);
            }
        }
    }

    private void RunInstruction(EquipBase equipId)
    {
        
    }
}