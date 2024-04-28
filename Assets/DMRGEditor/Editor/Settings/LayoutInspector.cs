/*********************************************
 * Author:Leo
 * Create:2018.05.01
 * Modify:2023.04.01
 * Func:
 * *******************************************/
using UnityEditor;

namespace DM.RGEditor
{
    [CustomEditor(typeof(LayoutSettings))]
    public class LayoutInspector : Editor
    {
        private LayoutSettings layout;
        private void OnEnable()
        {
            layout = (LayoutSettings)target;
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("名称:"+layout.Name,EditorStyles.boldLabel);
            EditorGUILayout.LabelField("路径:"+layout.Path);
            EditorGUILayout.LabelField("地形类型");
            EditorGUILayout.LabelField("所属互斥，放置多选，Id：1-1024");
            for (int i = 0; i < layout.Terrain.Count; i++)
            {
                SettingsTool.DrawTagItem(layout.Terrain,i,layout);
            }
            SettingsTool.DrawAddButton(layout.Terrain,1,layout);
            EditorGUILayout.LabelField("容器类型");
            EditorGUILayout.LabelField("所属互斥，放置互斥，Id：1025-2048");
            for (int i = 0; i < layout.Container.Count; i++)
            {
                SettingsTool.DrawTagItem(layout.Container,i,layout);
            }
            SettingsTool.DrawAddButton(layout.Container,1025,layout);
            EditorGUILayout.LabelField("组件类型");
            EditorGUILayout.LabelField("所属多选，放置多选，Id：2048-65535");
            for (int i = 0; i < layout.Component.Count; i++)
            {
                SettingsTool.DrawTagItem(layout.Component,i,layout);
            }
            SettingsTool.DrawAddButton(layout.Component,2049,layout);
        }
    }
}


