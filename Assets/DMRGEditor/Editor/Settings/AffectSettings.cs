/*********************************************
 * Author:Leo
 * Create:2018.05.01
 * Modify:2023.04.01
 * Func:
 * *******************************************/
using UnityEngine;
using System.Collections.Generic;

namespace DM.RGEditor
{
    [System.Serializable]
    public class AffectSettings:ScriptableObject
    {
        public string Name;
        public string Path;
        public List<RGAffectTriggerItem> Triggers;
        public List<RGAffectTriggerItem> Affects;

        public List<RGAffectTriggerItem> GetTriggers()
        {
            return CopyList(Triggers);
        }
        public List<RGAffectTriggerItem> GetAffects()
        {
            return CopyList(Affects);
        }
        private List<RGAffectTriggerItem> CopyList(List<RGAffectTriggerItem> source)
        {
            List<RGAffectTriggerItem> list = new List<RGAffectTriggerItem>();
            for (int i = 0; i < source.Count; i++)
            {
                RGAffectTriggerItem item = new RGAffectTriggerItem();
                item.Id = source[i].Id;
                item.Name = source[i].Name;
                item.IsOn = source[i].IsOn;
                item.BuiltIn = source[i].BuiltIn;
                list.Add(item);
            }
            return list;
        }
    }
}
