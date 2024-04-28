using DM.Entity;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace DM.RGEditor
{
    [Serializable]
    public class RGTagItem
    {
        private bool Hide = true;

        [HideIf("Hide")]
        public bool BuiltIn;
        [HideIf("Hide")]
        public int Id;
        [LabelText("$Name")]
        public bool IsOn;
        [HideIf("Hide")]
        public string Name;
        [HideIf("Hide")]
        public bool SubFoldOut;
        [LabelText("子标签")]
        public List<RGSubTagItem> SubTags;

        public RGTagItem()
        {
            SubTags = new List<RGSubTagItem>();
        }
        public RGTagItem(TagItem item)
        {
            BuiltIn = item.BuiltIn;
            Id = item.Id;
            IsOn = item.IsOn;
            Name = item.Name;
            SubFoldOut = item.SubFoldOut;
            SubTags = new List<RGSubTagItem>();
            for (int i = 0; i < item.SubTags.Count; i++)
            {
                SubTags.Add(new RGSubTagItem(item.SubTags[i]));
            }
        }
        public RGTagItem(int id, string name, bool isOn = false, bool builtIn = false)
        {
            Id = id;
            Name = name;
            IsOn = isOn;
            BuiltIn = builtIn;
            SubTags = new List<RGSubTagItem>();
        }
    }
    [Serializable]
    public class RGSubTagItem
    {
        private bool Hide = true;

        [HideIf("Hide")]
        public int Id;
        [HideIf("Hide")]
        public bool IsModify;
        [LabelText("$SubName")]
        public bool IsOn;
        [HideIf("Hide")]
        public string SubName;

        public RGSubTagItem() { }
        public RGSubTagItem(SubTagItem item)
        {
            Id = item.Id;
            IsModify = item.IsModify;
            IsOn = item.IsOn;
            SubName = item.SubName;
        }
    }

    [Serializable]
    public class RGAffectTriggerItem
    {
        private bool Hide = true;

        [HideIf("Hide")]
        public bool BuiltIn;
        [HideIf("Hide")]
        public int Id;
        [LabelText("$Name")]
        public bool IsOn;
        [HideIf("Hide")]
        public string Name;

        public RGAffectTriggerItem() { }
        public RGAffectTriggerItem(AffectTriggerItem item)
        {
            BuiltIn = item.BuiltIn;
            Id = item.Id;
            IsOn = item.IsOn;
            Name = item.Name;
        }
    }
}
