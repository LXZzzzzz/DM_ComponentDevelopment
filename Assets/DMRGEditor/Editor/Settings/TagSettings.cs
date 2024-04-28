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
    public class TagSettings : ScriptableObject
    {
        public string Name;
        public string Path;
        public List<RGTagItem> Tags;

        public List<RGTagItem> GetTags()
        {
            return CopyList(Tags);
        }
        private List<RGTagItem> CopyList(List<RGTagItem> source)
        {
            List<RGTagItem> list = new List<RGTagItem>();
            for (int i = 0; i < source.Count; i++)
            {
                RGTagItem item = new RGTagItem();
                item.Id = source[i].Id;
                item.Name = source[i].Name;
                item.IsOn = source[i].IsOn;
                item.BuiltIn = source[i].BuiltIn;
                item.SubFoldOut = source[i].BuiltIn;
                for (int j = 0; j < source[i].SubTags.Count; j++)
                {
                    RGSubTagItem subItem = new RGSubTagItem();
                    subItem.Id = source[i].SubTags[j].Id;
                    subItem.SubName = source[i].SubTags[j].SubName;
                    subItem.IsOn = source[i].SubTags[j].IsOn;
                    subItem.IsModify = source[i].SubTags[j].IsOn;
                    item.SubTags.Add(subItem);
                }
                list.Add(item);
            }
            return list;
        }
    }
}