using System;
using System.Collections.Generic;

namespace DM.RGEditor
{
    [Serializable]
    public class RGDataConfig
    {
        public string MainClass;
        public List<RGScript> RGScripts;

        public RGDataConfig()
        {
            RGScripts = new List<RGScript>();
        }
    }
    [Serializable]
    public class RGScript
    {
        public string SubPath;
        public string ClassName;
        public List<RGField> RGFields;
        public RGScript()
        {
            RGFields = new List<RGField>();
        }
    }
    [Serializable]
    public class RGField
    {
        public string Name;
        public string Type;
        public string Value;
    }
}
