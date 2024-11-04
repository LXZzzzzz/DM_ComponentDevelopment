using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public partial class CommanderController
{
    private void OnRunInstructionUpdate()
    {
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