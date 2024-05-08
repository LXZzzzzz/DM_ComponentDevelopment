using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolsLibrary.PathPart
{
    //作为装备的抽象类，共性是都有路径逻辑
    public abstract class EquipBase : ScriptManager
    {
        //记录最后一个路径点Id
        public string lastPointId;
        

    }
}
