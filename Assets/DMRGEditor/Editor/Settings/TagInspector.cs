using DM.Entity;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DM.RGEditor
{
    [CustomEditor(typeof(TagSettings))]
    public class TagInspector:Editor
    {
        private TagSettings tags;

        private void OnEnable()
        {
            tags = (TagSettings)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("名称:" + tags.Name, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("路径:" + tags.Path);
            EditorGUILayout.LabelField("分类标签");
            for (int i = 0; i < tags.Tags.Count; i++)
            {
                SettingsTool.DrawTagItem(tags.Tags,i,tags);
            }
            SettingsTool.DrawAddButton(tags.Tags, 0,tags);
        }

       
    }
}
