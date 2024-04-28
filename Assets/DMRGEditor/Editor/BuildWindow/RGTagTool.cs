using DM.Entity;
using System;
using System.Collections.Generic;

namespace DM.RGEditor
{
    public static class RGTagTool
    {
        public static List<RGTagItem> ConvertToRG(this List<TagItem> list)
        {
            List<RGTagItem> items = new List<RGTagItem>();
            for (int i = 0; i < list.Count; i++)
            {
                items.Add(new RGTagItem(list[i]));
            }
            return items;
        }
        public static List<RGAffectTriggerItem> ConvertToRG(this List<AffectTriggerItem> list)
        {
            List<RGAffectTriggerItem> items = new List<RGAffectTriggerItem>();
            for (int i = 0; i < list.Count; i++)
            {
                items.Add(new RGAffectTriggerItem(list[i]));
            }
            return items;
        }
    }
}
