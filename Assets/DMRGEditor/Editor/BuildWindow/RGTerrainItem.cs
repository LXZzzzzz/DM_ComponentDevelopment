using DM.Entity;
using Sirenix.OdinInspector;
using System;

namespace DM.RGEditor
{
   [Serializable]
    public class RGTerrainItem
    {
        private bool Hide = true;

        [LabelText("地形名称")]
        public string Name;
        [FolderPath(AbsolutePath = true)]
        [LabelText("地形路径")]
        public string Path;
        [LabelText("版本号")]
        public string Version;     
    }
}
