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
    public class SettingsTool
    {
        public static void DrawLine()
        {
            EditorGUILayout.LabelField("_____________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________");
        }
        public static void DrawTagItem(List<RGTagItem> list, int index, UnityEngine.Object data)
        {
            if (index < 0 || index >= list.Count)
                return;
            EditorGUI.indentLevel = 0;
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            string lblName = EditorGUILayout.TextField("", list[index].Name, GUILayout.Width(100));
            int lblId = EditorGUILayout.IntField("", list[index].Id, GUILayout.Width(50));
            bool isOn = EditorGUILayout.ToggleLeft("默认", list[index].IsOn, GUILayout.Width(50));

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(data, "Modify State");
                list[index].Name = lblName;
                list[index].Id = lblId;
                list[index].IsOn = isOn;

                //已经修改
                EditorUtility.SetDirty(data);
            }

            if (GUILayout.Button("删除"))
            {
                EditorApplication.Beep();
                if (EditorUtility.DisplayDialog("确认?", "您要删除这个标签吗" + "?", "是", "不") == true)
                {
                    Undo.RecordObject(data, "Delete State");
                    list.RemoveAt(index);
                    EditorUtility.SetDirty(data);
                }
            }
            if (GUILayout.Button("添加子标签"))
            {
                Undo.RecordObject(data, "Add new State");
                list[index].SubTags.Add(new RGSubTagItem() {
                    Id= list[index].SubTags.Count == 0 ? 0 : list[index].SubTags[list[index].SubTags.Count - 1].Id + 1,
                    SubName="新建子标签",
                    IsOn=false,
                    IsModify=true
                });
                EditorUtility.SetDirty(data);
            }
            GUILayout.EndHorizontal();
            //名称和Id检测
            for (int i = 0; i < index; i++)
            {
                if (index != i && list.Count > index && list[index].Name == list[i].Name)
                {
                    EditorGUILayout.HelpBox("标签名称重复！", MessageType.Warning);
                    break;
                }
            }
            for (int i = 0; i < index; i++)
            {
                if (index != i && list.Count > index && list[index].Id == list[i].Id)
                {
                    EditorGUILayout.HelpBox("标签Id冲突,请检查后修改！", MessageType.Error);
                    break;
                }
            }
            if (list.Count > index)
            {
                EditorGUI.indentLevel = 1;
                list[index].SubFoldOut = EditorGUILayout.Foldout(list[index].SubFoldOut, "子标签(" + list[index].SubTags.Count + ")");
                if (list[index].SubFoldOut)
                {
                    EditorGUI.indentLevel = 2;
                    for (int i = 0; i < list[index].SubTags.Count; i++)
                    {
                        DrawSubTagItem(list[index].SubTags, i, data);
                    }
                }
            }
            EditorGUILayout.Space();
        }
        private static void DrawSubTagItem(List<RGSubTagItem> subList, int subIndex, UnityEngine.Object data)
        {
            if (subIndex < 0 || subIndex >= subList.Count)
                return;
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            string lblName = EditorGUILayout.TextField("", subList[subIndex].SubName, GUILayout.Width(100));
            int lblId = EditorGUILayout.IntField("", subList[subIndex].Id, GUILayout.Width(50));
            bool isOn = EditorGUILayout.ToggleLeft("默认", subList[subIndex].IsOn, GUILayout.Width(100));

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(data, "Modify State");
                subList[subIndex].SubName = lblName;
                subList[subIndex].Id = lblId;
                subList[subIndex].IsOn = isOn;

                //已经修改
                EditorUtility.SetDirty(data);
            }

            if (GUILayout.Button("删除子标签", GUILayout.Width(80)))
            {
                EditorApplication.Beep();
                if (EditorUtility.DisplayDialog("确认?", "您要删除这个子标签吗?", "是", "不") == true)
                {
                    Undo.RecordObject(data, "Delete State");
                    subList.RemoveAt(subIndex);
                    EditorUtility.SetDirty(data);
                }
            }
            GUILayout.EndHorizontal();

            //名称和Id检测
            for (int i = 0; i < subIndex; i++)
            {
                if (subIndex != i && subList.Count > subIndex && subList[subIndex].SubName == subList[i].SubName)
                {
                    EditorGUILayout.HelpBox("子标签名称重复！", MessageType.Warning);
                    break;
                }
            }
            for (int i = 0; i < subIndex; i++)
            {
                if (subIndex != i && subList.Count > subIndex && subList[subIndex].Id == subList[i].Id)
                {
                    EditorGUILayout.HelpBox("子标签Id冲突,请检查后修改！", MessageType.Error);
                    break;
                }
            }
        }
        public static void DrawAddButton(List<RGTagItem> list, int def, UnityEngine.Object data)
        {
            if (GUILayout.Button("添加", GUILayout.Height(20)))
            {
                Undo.RecordObject(data, "Add new State");
                list.Add(new RGTagItem(list.Count == 0 ? def : list[list.Count - 1].Id + 1,"新建标签",false,true));
                EditorUtility.SetDirty(data);
            }
        }
    }
}
