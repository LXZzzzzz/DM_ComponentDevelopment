using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//[CustomEditor(typeof(EnvLights))]
public class EnvLightsInspector : Editor
{
    private List<LightGameObject> lights;
    public class LightSerialParam
    {
        public bool Name;
        public bool Transform;
        public bool Light;
    }
    private List<LightSerialParam> toggles;
    private int size = 0;
    private void OnEnable()
    {
        lights = ((EnvLights)target).lightSettings;
        toggles = new List<LightSerialParam>();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.indentLevel = 1;
        size=EditorGUILayout.IntField("Size",size);
        if (size > lights.Count)
        {
            for (int i = 0; i < lights.Count - size; i++)
            {
                lights.Add(new LightGameObject());
                toggles.Add(new LightSerialParam());
            }
        }
        else if (size < lights.Count)
        {
            for (int i = 0; i < size-lights.Count; i++)
            {
                lights.RemoveAt(lights.Count-i-1);
                lights.RemoveAt(lights.Count-i-1);
            }
        }
        //Todo 序列化List<LightGameObject>
#if UNITY_EDITOR
        float t = (size - 0.125f) * 8;
#endif
    }
}
