using UnityEngine;
using UnityEngine.UI;
using DM.IFS;
using System.Collections.Generic;

namespace 指挥端
{
    public class MissionKnapItem : MissionItemBase
    {
        private DMKnapsackItem knap;

        public override void Init(string objName, int id, int status, int useCount, string text)
        {
            base.Init(objName, id, status, useCount, text);
            knap = GetDMKnap(objName, id);
            if (knap == null)
                lblOperName.text = "场景中不存在该名字组件";
            else if (!string.IsNullOrEmpty(text))
                lblOperName.text = text;
            else
                lblOperName.text = knap.Name;
        }
        public override void IntervalCheck()
        {
            if (knap != null && knap.Status == finStatus)
            {
                togFinished.isOn = true;
                if (FinishedCall != null)
                    FinishedCall();
                this.enabled = false;
            }
        }

        private DMKnapsackItem GetDMKnap(string objName, int id)
        {
            Transform obj = GetBObjectByName(objName);
            if (obj == null) return null;
            return new List<DMKnapsackItem>(obj.GetComponentsInChildren<DMKnapsackItem>()).Find(k => k.Id == id);
        }
    }
}
