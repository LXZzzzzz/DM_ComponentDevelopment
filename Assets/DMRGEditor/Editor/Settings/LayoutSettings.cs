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
    //ScriptableObject本地序列化后重启Unity后报错的坑
    //http://www.cnblogs.com/Huil1993/p/6255620.html
    [System.Serializable]
    public class LayoutSettings : ScriptableObject
    {
        public string Name;
        public string Path;
        public List<RGTagItem> Terrain;
        public List<RGTagItem> Container;
        public List<RGTagItem> Component;
       
        public List<RGTagItem> GetTerrain()
        {
            return CopyList(Terrain);
        }
        public List<RGTagItem> GetContainer()
        {
            return CopyList(Container);
        }
        public List<RGTagItem> GetComponent()
        {
            return CopyList(Component);
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
