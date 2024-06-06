using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(EnvMainLight))]
 public class EnvMainLightInspector:Editor
{
    private EnvMainLight mLight;
    private bool showTransform = true;
    private bool showLight = true;

    private void OnEnable()
    {
        mLight = (EnvMainLight)target;
    }

    public override void OnInspectorGUI()
    {     
        showTransform = EditorGUILayout.Foldout(showTransform,"Transform");
        if (showTransform)
        {
            TransformComponent cTran = mLight.mainLightSettings.transform;

            cTran.position = EditorGUILayout.Vector3Field(Space(2)+"Position",cTran.position);
            cTran.rotation = EditorGUILayout.Vector3Field(Space(2)+"Rotation",cTran.rotation);
            cTran.scale = EditorGUILayout.Vector3Field(Space(2)+"Scale",cTran.scale);
        }
        EditorGUILayout.Space();
        showLight = EditorGUILayout.Foldout(showLight, "Light");
        if (showLight)
        {
            LightComponent cLight = mLight.mainLightSettings.light;
            cLight.type = (LightType)EditorGUILayout.EnumPopup(Space(2) + "Type", cLight.type);
            EditorGUILayout.Space();
            #region Spot Directional Point
            if (cLight.type != LightType.Rectangle)
            {
                if (cLight.type == LightType.Spot)
                {
                    cLight.range = EditorGUILayout.FloatField(Space(2) + "Range", cLight.range);
                    cLight.spotAngle = EditorGUILayout.Slider(Space(2) + "Spot Angle", cLight.spotAngle, 1, 179);
                }
                else if (cLight.type == LightType.Point)
                {
                    cLight.range = EditorGUILayout.FloatField(Space(2) + "Range", cLight.range);
                }
                cLight.color = (Color)EditorGUILayout.ColorField(Space(2) + "Color", cLight.color);
                EditorGUILayout.Space();
                cLight.mode = (LightmapBakeType)EditorGUILayout.EnumPopup(Space(2) + "Mode", cLight.mode);
                cLight.intensity = EditorGUILayout.FloatField(Space(2) + "Intensity", cLight.intensity);
                cLight.indirectMultiplier = EditorGUILayout.FloatField(Space(2) + "Indirect Multiplier", cLight.indirectMultiplier);

                if (cLight.type!=LightType.Directional&&cLight.mode == LightmapBakeType.Realtime)
                    EditorGUILayout.HelpBox("Realtime indirect bounce shadowing is not supported for Spot and Point lights.",MessageType.Info);
                EditorGUILayout.Space();

                cLight.shadowType = (LightShadows)EditorGUILayout.EnumPopup(Space(2) + "Shadow Type", cLight.shadowType);
                if (cLight.mode != LightmapBakeType.Realtime)
                {
                    if (cLight.shadowType == LightShadows.Hard)
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        if (cLight.type == LightType.Directional)
                            cLight.bakedShadowAngle = EditorGUILayout.Slider(Space(6) + "Baked Shadow Angle", cLight.bakedShadowAngle, 0, 90);
                        else
                            cLight.bakedShadowRadius = EditorGUILayout.FloatField(Space(6) + "Baked Shadow Radius", cLight.bakedShadowRadius);
                        EditorGUI.EndDisabledGroup();
                    }
                    else if (cLight.shadowType == LightShadows.Soft)
                    {
                        if (cLight.type == LightType.Directional)
                            cLight.bakedShadowAngle = EditorGUILayout.Slider(Space(6) + "Baked Shadow Angle", cLight.bakedShadowAngle, 0, 90);
                        else
                            cLight.bakedShadowRadius = EditorGUILayout.FloatField(Space(6) + "Baked Shadow Radius", cLight.bakedShadowRadius);
                    }                
                }

                if ((cLight.mode == LightmapBakeType.Mixed||cLight.mode==LightmapBakeType.Realtime)&&
                    (cLight.shadowType==LightShadows.Hard||cLight.shadowType==LightShadows.Soft))
                {
                    EditorGUILayout.LabelField(Space(6) + "Realtime Shadows");
                    cLight.realtimeShadows.strength = EditorGUILayout.Slider(Space(10) + "Strength", cLight.realtimeShadows.strength, 0, 1);
                    cLight.realtimeShadows.resolution = (UnityEngine.Rendering.LightShadowResolution)EditorGUILayout.EnumPopup(Space(10) + "Resolution", cLight.realtimeShadows.resolution);
                    cLight.realtimeShadows.bias = EditorGUILayout.Slider(Space(10) + "Bias", cLight.realtimeShadows.bias, 0, 2);
                    cLight.realtimeShadows.normalBias = EditorGUILayout.Slider(Space(10) + "Normal Bias", cLight.realtimeShadows.normalBias, 0, 3);
                    cLight.realtimeShadows.nearPlane = EditorGUILayout.Slider(Space(10) + "Near Plane", cLight.realtimeShadows.nearPlane, 0, 10);
                }
                if (cLight.mode != LightmapBakeType.Baked)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    cLight.cookie = EditorGUILayout.TextField(Space(2) + "Cookie", cLight.cookie);
                    EditorGUI.EndDisabledGroup();
                    if(cLight.type==LightType.Directional)
                        cLight.cookieSize= EditorGUILayout.FloatField(Space(2) + "CookieSize", cLight.cookieSize);
                }
                EditorGUILayout.Space();
            }
            #endregion
            #region Area
            if (cLight.type == LightType.Rectangle)
            {
                EditorGUI.BeginDisabledGroup(true);
                cLight.range = 7.053368f;
                cLight.range = EditorGUILayout.FloatField(Space(2) + "Range", cLight.range);
                EditorGUI.EndDisabledGroup();
                cLight.width = EditorGUILayout.FloatField(Space(2) + "Width", cLight.width);
                cLight.height = EditorGUILayout.FloatField(Space(2) + "Height", cLight.height);
                cLight.color = (Color)EditorGUILayout.ColorField(Space(2) + "Color", cLight.color);
                EditorGUILayout.Space();
                cLight.intensity = EditorGUILayout.FloatField(Space(2) + "Intensity", cLight.intensity);
                cLight.indirectMultiplier = EditorGUILayout.FloatField(Space(2) + "Indirect Multiplier", cLight.indirectMultiplier);
                EditorGUILayout.Space();
            }
            #endregion
            cLight.drawHalo = EditorGUILayout.Toggle(Space(2)+"Draw Halo", cLight.drawHalo);
            EditorGUI.BeginDisabledGroup(true);
            cLight.flare = EditorGUILayout.TextField(Space(2)+"Flare", cLight.flare);
            EditorGUI.EndDisabledGroup();
            cLight.renderMode = (LightRenderMode)EditorGUILayout.EnumPopup(Space(2)+"Render Mode", cLight.renderMode);
            EditorGUI.BeginDisabledGroup(true);
            cLight.cullingMask = EditorGUILayout.TextField(Space(2)+"Culling Mask", cLight.cullingMask);
            EditorGUI.EndDisabledGroup();
        }
    }
    private string Space(int count)
    {
        string str = "";
        for (int i = 0; i < count; i++)
            str += " ";
        return str;
    }
}