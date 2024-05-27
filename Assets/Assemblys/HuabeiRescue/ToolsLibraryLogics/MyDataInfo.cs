using System.Collections.Generic;
using ToolsLibrary.EquipPart_Logic;
using UnityEngine;
using ToolsLibrary.FrameSync;


namespace ToolsLibrary_Logic
{
    public class MyDataInfo
    {
        public static GameObject goParent;
        public static string leadId;
        public static bool isHost;
        public static bool isPlayBack;
        public static List<EquipBase> sceneAllEquips;
    }
}