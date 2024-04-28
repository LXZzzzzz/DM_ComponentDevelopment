using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnvSettings : MonoBehaviour
{
    public Environment environment;
    public RealtimeLighting realtimeLighting;
    public MixedLighting mixLighting;
    public LightmappingSettings lightmappingSettings;
    public OtherSettings otherSettings;
    public DebugSettings debugSettings;

    private void Awake()
    {
        Apply();
    }

    [ContextMenu("Apply")]
    void Apply()
    {
        RenderSettings.ambientEquatorColor = environment.equatorColor;
        RenderSettings.ambientGroundColor = environment.groundColor;
        RenderSettings.ambientIntensity = environment.intensityMultiplier;
        RenderSettings.ambientLight = environment.ambientColor;
        RenderSettings.ambientMode =(AmbientMode)(int)environment.source;
        //ambientProbe    
        //ambientSkyboxAmount
        RenderSettings.ambientSkyColor = environment.ambientColor;
        //customReflection
        RenderSettings.defaultReflectionMode = environment.reflectionMode;
        //RenderSettings.defaultReflectionResolution = environment.resolution;
        RenderSettings.flareFadeSpeed = otherSettings.flareFadeSpeed;
        RenderSettings.flareStrength = otherSettings.flareStrength;
        RenderSettings.fog = otherSettings.fog;
        RenderSettings.fogColor = otherSettings.color;
        RenderSettings.fogDensity = otherSettings.density;
        RenderSettings.fogEndDistance = otherSettings.end;
        RenderSettings.fogMode = otherSettings.mode;
        RenderSettings.fogStartDistance = otherSettings.start;
        RenderSettings.haloStrength = otherSettings.haloStrength;
        RenderSettings.reflectionBounces = environment.bounces;
        //skybox
        //subtractiveShadowColor
        //sun
        if (environment.skyShader == SkyMaterialShader.Skybox_6Sided)
        {
            Material mat = Resources.Load<Material>(environment.skyBoxMaterial);
            mat.SetColor("_Tint",environment.sixSided.tintColor);
            mat.SetFloat("_Exposure",environment.sixSided.exposure);
            mat.SetFloat("_Rotation",environment.sixSided.rotation);
        }
    }
}

[System.Serializable]
public class Environment
{
    public bool isShow = true;
    public string skyBoxMaterial="Default-Skybox";
    public SkyMaterialShader skyShader = SkyMaterialShader.Default;
    public Skybox_6Sided sixSided;
    public string sunSource = "MainLight";

    [Header("Environment Lighting")]
    public EnvSource source = EnvSource.Skybox;   //Unknown
    [Range(0, 8)]
    public float intensityMultiplier=1f;
    [ColorUsage(true)]
    public Color equatorColor;
    [ColorUsage(true)]
    public Color groundColor;
    [ColorUsage(true)]
    public Color ambientColor;
    [Tooltip("属性暂不支持设置")]
    public string ambientMode = "Baked";

    [Header("EnvironmentReflections")]
    public DefaultReflectionMode reflectionMode=DefaultReflectionMode.Skybox;
    public EnvResolution resolution = EnvResolution._128;
    public string cubemap = "None(Cubemap)";
    public EnvCompression compression= EnvCompression.Auto; //Unknown
    [Range(0,1)]
    public float intensityMultiplier2=1f;
    [Range(1,5)]
    public int bounces=1;
}

[System.Serializable]
public class RealtimeLighting
{
    public bool isShow = true;
    public bool realtimeGlobalIllumination = true;
}

[System.Serializable]
public class MixedLighting
{
    public bool isShow = true;
    public bool bakedGlobalIllumination = true;
    public LightingMode lightingMode = LightingMode.BakedIndirect;  //Unknown
    public Color realtimeShadowColor = Color.blue;
}

[System.Serializable]
public class LightmappingSettings
{
    public bool isShow = true;
    public SLgihtMapper lightmapper= SLgihtMapper.Progressive;  //Unknown
    public bool prioritizeView = true;
    public int directSamples = 32;
    public int indirectSamples = 500;
    public SBounces bounces = SBounces._2;  //Unknown
    public SFiltering filtering = SFiltering.Advanced; //Unknown
    [Range(0,5)]
    public int directRadius = 1;
    [Range(0, 5)]
    public int indirectRadius = 5;
    [Range(0, 5)]
    public int ambienOcclusionRadius = 2;
    public float indirectResolution = 2;
    public float lightmapResolution = 40;
    public float lightmapPadding = 2;
    public SLightmapSize lightmapSize = SLightmapSize._256;  //Unknown
    public bool compressLightmaps=true;
    public bool ambientOcclusion=false;
    public float maxDistance = 1;
    [Range(0,5)]
    public float indirectContribution = 1;
    [Range(0,5)]
    public float directContribution = 0;
    public bool finalGather = false;
    public int rayCount = 256;
    public bool denoising = true;
    public SDirectionalMode directionalMode = SDirectionalMode.Non_Directional; //Unknown
    [Range(0,5)]
    public float indirectIntensity;
    [Range(1, 10)]
    public float albedoBoost;
    public SLightmapParameters lightmapParameters= SLightmapParameters.Default_Medium; //Unknown
}

[System.Serializable]
public class OtherSettings
{
    public bool isShow = true;
    public bool fog=false;
    public Color color = Color.gray;
    public FogMode mode = FogMode.Linear;
    public float start = 0f;
    public float end = 300f;
    public float density = 0.01f;
    public string haloTexture="None(Texture 2D)";
    [Range(0, 1)]
    public float haloStrength=0.5f;
    public float flareFadeSpeed=3f;
    [Range(0, 1)]
    public float flareStrength = 1f;
    public string spotCookie = "Soft";
}

[System.Serializable]
public class DebugSettings
{
    public bool isShow = true;
    [Tooltip("LightProbeVisualization")]
    public LightProbeVisalization probe = LightProbeVisalization.OnlyProbesUsedBySelection; //Unknown
    public bool displayWeights = true;
    public bool displayOcclusion = false;
}
public enum EnvSource
{
    Skybox,
    Gradient, //Trilight
    Flat,    //None
    Color
}
public enum EnvResolution
{
    _128,
    _256,
    _512,
    _1024
}
public enum EnvCompression
{
    Uncompressed,
    Compressed,
    Auto
}
public enum LightingMode
{
    BakedIndirect,
    DistanceShadowmask,
    Shadowmask,
    Subtractive
}
public enum SLgihtMapper
{
    Enlighten,
    Progressive,
}
public enum SBounces
{
    None,
    _1,
    _2,
    _3,
    _4
}
public enum SFiltering
{
    None,
    Auto,
    Advanced,
}
public enum SLightmapSize
{
    _32,
    _64,
    _128,
    _256,
    _512,
    _1024,
    _2048,
    _4096
}
public enum SDirectionalMode
{
    Non_Directional,
    Directional
}
public enum SLightmapParameters
{
    Default_Medium,
    Default_HighResolution,
    Default_LowResolution,
    Default_VeryLowResolution
}
public enum LightProbeVisalization
{
    OnlyProbesUsedBySelection,
    AllProbesNoCells,
    AllProbesWithCells,
    None 
}

public enum SkyMaterialShader
{
    Default,
    Skybox_6Sided
}
[System.Serializable]
public class Skybox_6Sided
{
    [ColorUsage(true)]
    public Color tintColor;
    [Range(0, 8)]
    public float exposure;
    [Range(0, 360)]
    public float rotation;
}