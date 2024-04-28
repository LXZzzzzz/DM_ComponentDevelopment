using UnityEngine;

namespace DM.RGEditor
{
    public class DMScriptableObject : ScriptableObject
    {
        /// <summary>
        /// 脚本挂载物体相对路径
        /// </summary>
        [HideInInspector]
        public string ObjectPath;
        /// <summary>
        /// 脚本类名
        /// </summary>
        [HideInInspector]
        public string ClassName;
    }
}
