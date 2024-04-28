using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(EnvSettings))]
public class EnvSettingsInspector : Editor
{
    private EnvSettings mSetting;
    private bool showLightProbe = true;

    private void OnEnable()
    {
        mSetting = (EnvSettings)target;
    }

    public override void OnInspectorGUI()
    {
        #region Environment
        DrawLine();
        Environment env = mSetting.environment;
        env.isShow = EditorGUILayout.Foldout(env.isShow, "Environment");
        if (env.isShow)
        {         
            env.skyBoxMaterial = EditorGUILayout.TextField(Space(2)+"Skybox Material",env.skyBoxMaterial);
            env.skyShader = (SkyMaterialShader)EditorGUILayout.EnumPopup(Space(2)+"Material Shader",env.skyShader);
            if (env.skyShader == SkyMaterialShader.Skybox_6Sided)
            {
                env.sixSided.tintColor = EditorGUILayout.ColorField(Space(6)+"TintColor",env.sixSided.tintColor);
                env.sixSided.exposure = EditorGUILayout.Slider(Space(6)+"Exposure",env.sixSided.exposure,0,8);
                env.sixSided.rotation = EditorGUILayout.Slider(Space(6)+"Rotation",env.sixSided.rotation,0,360);
            }
            env.sunSource = EditorGUILayout.TextField(Space(2)+"Sun Source",env.sunSource);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(Space(2)+"Environment Lighting");
            env.source = (EnvSource)EditorGUILayout.EnumPopup(Space(6)+"Source",env.source);
            if (env.source == EnvSource.Skybox)
                env.intensityMultiplier = EditorGUILayout.Slider(Space(6)+"Intensity Multiplier",env.intensityMultiplier,0,8);
            if (env.source == EnvSource.Gradient)
            {
                env.ambientColor = EditorGUILayout.ColorField(Space(6)+"Sky Color",env.ambientColor);
                env.equatorColor = EditorGUILayout.ColorField(Space(6) + "Equator Color", env.equatorColor);
                env.groundColor = EditorGUILayout.ColorField(Space(6) + "Ground Color", env.groundColor);
            }
            if (env.source == EnvSource.Color)
                env.ambientColor = EditorGUILayout.ColorField(Space(6)+"Ambient Color",env.ambientColor);
            if (mSetting.realtimeLighting.realtimeGlobalIllumination || mSetting.mixLighting.bakedGlobalIllumination)
            {
                EditorGUI.BeginDisabledGroup(true);
                env.ambientMode = EditorGUILayout.TextField(Space(6) + "Ambient Mode", env.ambientMode);
                EditorGUI.EndDisabledGroup();
            }
              
            EditorGUILayout.Space();

            EditorGUILayout.LabelField(Space(2)+"Environment Reflections");
            env.reflectionMode = (DefaultReflectionMode)EditorGUILayout.EnumPopup(Space(6)+"Source",env.reflectionMode);
            if (env.reflectionMode == DefaultReflectionMode.Skybox)
                env.resolution = (EnvResolution)EditorGUILayout.EnumPopup(Space(6) + "Resolution", env.resolution);
            else
                env.cubemap = EditorGUILayout.TextField(Space(6)+"Cubemap",env.cubemap);
            env.compression = (EnvCompression)EditorGUILayout.EnumPopup(Space(6)+"Compression",env.compression);
            env.intensityMultiplier2 = EditorGUILayout.Slider(Space(6)+"Intensty Multiplier",env.intensityMultiplier2,0,1);
            env.bounces = EditorGUILayout.IntSlider(Space(6)+"Bounces",env.bounces,1,5);
            EditorGUILayout.Space();
        }
        #endregion

        #region Realtime Lighting
        DrawLine();
        mSetting.realtimeLighting.isShow = EditorGUILayout.Foldout(mSetting.realtimeLighting.isShow, "Realtime Lighting");
        if (mSetting.realtimeLighting.isShow)
        {
            mSetting.realtimeLighting.realtimeGlobalIllumination = EditorGUILayout.Toggle(
            Space(2)+"Realtime Global Illumination" ,mSetting.realtimeLighting.realtimeGlobalIllumination);
        }
        #endregion

        #region Mixed Lighting
        DrawLine();
        MixedLighting m = mSetting.mixLighting;
        m.isShow = EditorGUILayout.Foldout(m.isShow, "Mixed Lighting");
        if (m.isShow)
        {        
            m.bakedGlobalIllumination = EditorGUILayout.Toggle(Space(2)+"Baked Global Illumination",m.bakedGlobalIllumination);
            if (m.bakedGlobalIllumination)
            {
                m.lightingMode = (LightingMode)EditorGUILayout.EnumPopup(Space(2) + "Lighting Mode", m.lightingMode);
                if (m.lightingMode == LightingMode.BakedIndirect)
                {
                    EditorGUILayout.HelpBox("Mixed lights provide realtime direct lighting while indirect light is baked into lightmaps and light probes.", MessageType.Info);
                }
                else if (m.lightingMode == LightingMode.DistanceShadowmask)
                {
                    EditorGUILayout.HelpBox("Mixed lights provide realtime direct lighting while indirect lights is baked into lightmaps and light probes.Shadow are handled with realtime shadow maps up to the shadow distance quality setting.", MessageType.Info);
                }
                else if (m.lightingMode == LightingMode.Shadowmask)
                {
                    EditorGUILayout.HelpBox("Mixed lights provide realtime direct lighting while indirect light is baked into lightmaps and light probes.Shadowmasks are used for static objects while dynamic objects are realtime up to the shadow distance quality setting.",MessageType.Info);
                }
                else if (m.lightingMode == LightingMode.Subtractive)
                {
                    EditorGUILayout.HelpBox("Mixed lights provide baked direct and indirect lighting for static objects.Dynamic objects receive realtime direct lighting and cast shadows on static objects using the main directional light in the scene.",MessageType.Info);
                    m.realtimeShadowColor = EditorGUILayout.ColorField(Space(2) + "Realtime Shadow Color", m.realtimeShadowColor);
                }
            }       
        }
        #endregion

        #region Lightmapping Settings
        DrawLine();
        LightmappingSettings s = mSetting.lightmappingSettings;
        s.isShow = EditorGUILayout.Foldout(s.isShow, "Lightmapping Settings");
        if (s.isShow)
        {          
            s.lightmapper = (SLgihtMapper)EditorGUILayout.EnumPopup(Space(2)+"Lightmapper",s.lightmapper);
            if (s.lightmapper == SLgihtMapper.Progressive)
            {
                s.prioritizeView = EditorGUILayout.Toggle(Space(6)+"Prioritize View",s.prioritizeView);
                s.directSamples = EditorGUILayout.IntField(Space(6)+"Direct Sample",s.directSamples);
                s.indirectSamples = EditorGUILayout.IntField(Space(6)+"Indirect Samples",s.indirectSamples);
                s.bounces = (SBounces)EditorGUILayout.EnumPopup(Space(6)+"Bounces",s.bounces);
                s.filtering = (SFiltering)EditorGUILayout.EnumPopup(Space(6)+"Filtering",s.filtering);
                if (s.filtering == SFiltering.Advanced)
                {
                    s.directRadius = EditorGUILayout.IntSlider(Space(10)+"Direct Radius",s.directRadius,0,5);
                    s.indirectRadius = EditorGUILayout.IntSlider(Space(10)+"Indirect Radius",s.indirectRadius,0,5);
                    s.ambienOcclusionRadius = EditorGUILayout.IntSlider(Space(10)+"Ambient Occlusion Radius",s.ambienOcclusionRadius,0,5);
                }
                EditorGUILayout.Space();
            }
            EditorGUI.BeginDisabledGroup(s.lightmapper==SLgihtMapper.Progressive);
            s.indirectResolution = EditorGUILayout.FloatField(Space(2)+"Indirect Resolution",s.indirectResolution);
            EditorGUI.EndDisabledGroup();
            s.lightmapResolution = EditorGUILayout.FloatField(Space(2)+"Lightmap Resolution",s.lightmapResolution);
            s.lightmapPadding = EditorGUILayout.FloatField(Space(2)+"Lightmap Padding",s.lightmapPadding);
            s.lightmapSize = (SLightmapSize)EditorGUILayout.EnumPopup(Space(2)+"Lightmap Size",s.lightmapSize);
            s.compressLightmaps = EditorGUILayout.Toggle(Space(2)+"Compress Lightmaps",s.compressLightmaps);
            s.ambientOcclusion = EditorGUILayout.Toggle(Space(2)+"Ambient Occlusion",s.ambientOcclusion);
            if (s.ambientOcclusion)
            {
                s.maxDistance = EditorGUILayout.FloatField(Space(6)+"Max Distance",s.maxDistance);
                s.indirectContribution = EditorGUILayout.Slider(Space(6)+"Indirect Contribution",s.indirectContribution,0,5);
                s.directContribution = EditorGUILayout.Slider(Space(6)+"Indirect Contribution",s.directContribution,0,5);
            }
            if (s.lightmapper == SLgihtMapper.Enlighten)
            {
                s.finalGather = EditorGUILayout.Toggle(Space(2) + "Final Gather", s.finalGather);
                if (s.finalGather)
                {
                    s.rayCount = EditorGUILayout.IntField(Space(6) + "Ray Count", s.rayCount);
                    s.denoising = EditorGUILayout.Toggle(Space(6) + "Denoising", s.denoising);
                }
            }          
            s.directionalMode = (SDirectionalMode)EditorGUILayout.EnumPopup(Space(2)+"Directional Mode",s.directionalMode);
            if(s.directionalMode==SDirectionalMode.Directional)
                EditorGUILayout.HelpBox("Directional lightmaps cannot be decoded on SM2.0 hardware nor when using GLES2.0.They will fallback to Non-Directional lightmaps.", MessageType.Warning);
            s.indirectIntensity = EditorGUILayout.Slider(Space(2)+"Indirect Intensity",s.indirectIntensity,0,5);
            s.albedoBoost = EditorGUILayout.Slider(Space(2)+"Albedo Boost",s.albedoBoost,1,10);
            EditorGUILayout.BeginHorizontal();
            s.lightmapParameters = (SLightmapParameters)EditorGUILayout.EnumPopup(Space(2) + "Lightmap Parameters", s.lightmapParameters);
            //Button
            EditorGUILayout.EndHorizontal();         
        }
        #endregion

        #region Other Setting
        DrawLine();
        OtherSettings o = mSetting.otherSettings;
        o.isShow = EditorGUILayout.Foldout(o.isShow, "Other Settings");
        if (o.isShow)
        {         
            o.fog = EditorGUILayout.Toggle(Space(2)+"Fog",o.fog);
            if (o.fog)
            {
                o.color = EditorGUILayout.ColorField(Space(6)+"Color",o.color);
                o.mode = (FogMode)EditorGUILayout.EnumPopup(Space(6)+"Mode",o.mode);
                if (o.mode == FogMode.Linear)
                {
                    o.start = EditorGUILayout.FloatField(Space(6)+"Start",o.start);
                    o.end = EditorGUILayout.FloatField(Space(6)+"End",o.end);
                }
                else
                {
                    o.density = EditorGUILayout.FloatField(Space(6)+"Density",o.density);
                }
                EditorGUILayout.Space();
            }
            EditorGUI.BeginDisabledGroup(true);
            o.haloTexture = EditorGUILayout.TextField(Space(2)+"Halo Texture",o.haloTexture);
            EditorGUI.EndDisabledGroup();
            o.haloStrength = EditorGUILayout.Slider(Space(2)+"Halo Strength",o.haloStrength,0,1);
            o.flareFadeSpeed = EditorGUILayout.FloatField(Space(2)+"Flare Fade Speed",o.flareFadeSpeed);
            o.flareStrength = EditorGUILayout.Slider(Space(2)+"Flare Strength",o.flareStrength,0,1);
            EditorGUI.BeginDisabledGroup(true);
            o.spotCookie = EditorGUILayout.TextField(Space(2)+"Spot Cookie",o.spotCookie);
            EditorGUI.EndDisabledGroup();
        }
        #endregion

        #region Debug Settings
        DrawLine();
        DebugSettings debug = mSetting.debugSettings;
        debug.isShow = EditorGUILayout.Foldout(debug.isShow, "Debug Settings");
        if (debug.isShow)
        {
            EditorGUI.indentLevel = 1;
            showLightProbe = EditorGUILayout.Foldout(showLightProbe, Space(1) + "Light Probe Visualization");
            if (showLightProbe)
            {
                EditorGUI.indentLevel = 2;              
                debug.probe = (LightProbeVisalization)EditorGUILayout.EnumPopup(debug.probe);
                debug.displayWeights = EditorGUILayout.Toggle("Display Weights", debug.displayWeights);
                debug.displayOcclusion = EditorGUILayout.Toggle("Display Occlusion", debug.displayOcclusion);
            }
        }
        #endregion
    }

    private string Space(int count)
    {
        string str= "";
        for (int i = 0; i < count; i++)
            str += " ";
        return str;
    }
    private void DrawLine()
    {
        EditorGUILayout.LabelField("_____________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________");
    }
}
