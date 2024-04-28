using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnvLights : MonoBehaviour
{
    public List<LightGameObject> lightSettings = new List<LightGameObject>();
	void Awake ()
    {
        Apply();
	}
    [ContextMenu("Apply")]
    void Apply()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(transform.childCount-i-1));
        }
        for (int i = 0; i < lightSettings.Count; i++)
        {
            GameObject obj = new GameObject();
            obj.name = lightSettings[i].lightName;
            obj.transform.SetParent(transform);
            obj.name = lightSettings[i].lightName;
            Light mLight = obj.AddComponent<Light>();
            LightGameObject settings = lightSettings[i];
            mLight.transform.position = settings.transform.position;
            mLight.transform.rotation = Quaternion.Euler(settings.transform.rotation);
            mLight.transform.localScale = settings.transform.scale;
            mLight.type = settings.light.type;
            mLight.range = settings.light.range;
            mLight.spotAngle = settings.light.spotAngle;
           // mLight.areaSize = new Vector2(settings.light.width, settings.light.height);
            mLight.color = settings.light.color;
           // mLight.lightmapBakeType = settings.light.mode;
            mLight.intensity = settings.light.intensity;
            mLight.bounceIntensity = settings.light.indirectMultiplier;
            mLight.shadows = settings.light.shadowType;
            //bakedShadowRadius bakedShadowRadius
            mLight.shadowStrength = settings.light.realtimeShadows.strength;
            mLight.shadowResolution = settings.light.realtimeShadows.resolution;
            mLight.shadowBias = settings.light.realtimeShadows.bias;
            mLight.shadowNormalBias = settings.light.realtimeShadows.normalBias;
            mLight.shadowNearPlane = settings.light.realtimeShadows.nearPlane;
            mLight.cookieSize = settings.light.cookieSize;
            mLight.renderMode = settings.light.renderMode;
        }
    }
}

[System.Serializable]
public class LightGameObject
{
    [HideInInspector]
    public string lightName="Light";
    public TransformComponent transform;
    public LightComponent light;
}

[System.Serializable]
public class TransformComponent
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale=Vector3.one;
}

[System.Serializable]
public class LightComponent
{
    public LightType type = LightType.Directional;
    public float range = 10f;
    [Range(1,179)]
    public float spotAngle = 30f;
    public float width = 1f;
    public float height = 1f;
    [ColorUsage(true)]
    public Color color = Color.white;
    public LightmapBakeType mode = LightmapBakeType.Baked;
    public float intensity=1f;
    public float indirectMultiplier=1f;
    public LightShadows shadowType = LightShadows.None;
    [Range(0,90)]
    public float bakedShadowAngle;
    public float bakedShadowRadius;
    public RealtimeShadows realtimeShadows;
    [Tooltip("属性暂不支持设置")]
    public string cookie = "None(Texture)";
    public float cookieSize = 10f;
    public bool drawHalo = false;
    [Tooltip("属性暂不支持设置")]
    public string flare = "None(Flare)";
    public LightRenderMode renderMode = LightRenderMode.Auto;
    [Tooltip("属性暂不支持设置")]
    public string cullingMask = "Everything";
}

[System.Serializable]
public class RealtimeShadows
{
    [Range(0,1)]
    public float strength = 1f;
    public UnityEngine.Rendering.LightShadowResolution resolution;
    [Range(0, 2)]
    public float bias;
    [Range(0, 3)]
    public float normalBias;
    [Range(0, 10)]
    public float nearPlane;
}

