/*********************************************
 * Author:Leo
 * Create:2018.05.01
 * Modify:2023.04.01
 * Func:
 * *******************************************/
using DM.Entity;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DM.RGEditor
{
    [CustomEditor(typeof(AffectSettings))]
    public class AffectInspector:Editor
    {
        private AffectSettings affect;

        private void OnEnable()
        {
            affect = (AffectSettings)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("名称:"+affect.name,EditorStyles.boldLabel);
            EditorGUILayout.LabelField("路径:"+affect.Path);
            EditorGUILayout.LabelField("触发类型");
            for (int i = 0; i < affect.Triggers.Count; i++)
            {
                DrawSetItem(affect.Triggers,i);
            }
            DrawAddButton(affect.Triggers,1000);
            EditorGUILayout.LabelField("效果类型");
            for (int i = 0; i < affect.Affects.Count; i++)
            {
                DrawSetItem(affect.Affects,i);
            }
            DrawAddButton(affect.Affects,2000);
        }
        void DrawSetItem(List<RGAffectTriggerItem> list, int index)
        {
            if (index < 0 || index >= list.Count)
                return;
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(list[index].BuiltIn);
            string lblName = EditorGUILayout.TextField("", list[index].Name, GUILayout.Width(100));
            int lblId = EditorGUILayout.IntField("", list[index].Id, GUILayout.Width(50));
            EditorGUI.EndDisabledGroup();
            bool isOn = EditorGUILayout.ToggleLeft("默认", list[index].IsOn, GUILayout.Width(50));

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(affect, "Modify State");
                list[index].Name = lblName;
                list[index].Id = lblId;
                list[index].IsOn = isOn;

                //已经修改
                EditorUtility.SetDirty(affect);
            }
            EditorGUI.BeginDisabledGroup(list[index].BuiltIn);
            if (GUILayout.Button("删除"))
            {
                EditorApplication.Beep();
                if (EditorUtility.DisplayDialog("确认?", "您要删除这一行数据吗?", "是", "不") == true)
                {
                    Undo.RecordObject(affect, "Delete State");
                    list.RemoveAt(index);
                    EditorUtility.SetDirty(affect);
                }
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            //名称和Id检测
            for (int i = 0; i < index; i++)
            {
                if (index != i && list.Count > index && list[index].Name == list[i].Name)
                {
                    EditorGUILayout.HelpBox("子项名称重复！", MessageType.Warning);
                    break;
                }
            }
            for (int i = 0; i < index; i++)
            {
                if (index != i && list.Count > index && list[index].Id == list[i].Id)
                {
                    EditorGUILayout.HelpBox("Id冲突,请检查后修改！", MessageType.Error);
                    break;
                }
            }
        }

        void DrawAddButton(List<RGAffectTriggerItem> list, int def)
        {
            if (GUILayout.Button("添加", GUILayout.Height(20)))
            {
                Undo.RecordObject(affect, "Add new State");
                list.Add(new RGAffectTriggerItem() {
                   Id=list.Count == 0 ? def : list[list.Count - 1].Id + 1,
                   Name="新建类型",
                   BuiltIn=false,
                   IsOn=false,
                });
                EditorUtility.SetDirty(affect);
            }
        }
    }
}
