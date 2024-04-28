using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EnvMainLight : MonoBehaviour
{
    public LightGameObject mainLightSettings;
    void Awake()
    {
        Apply();
    }
    [ContextMenu("Apply")]
    void Apply()
    {
        Light mLight = transform.GetComponentInChildren<Light>();
        if (mLight == null)
        {
            GameObject obj =new GameObject();
            obj.transform.SetParent(transform);
            obj.name = "Main Light";
            mLight = obj.AddComponent<Light>();
        }
        mLight.transform.position = mainLightSettings.transform.position;
        mLight.transform.rotation = Quaternion.Euler(mainLightSettings.transform.rotation);
        mLight.transform.localScale = mainLightSettings.transform.scale;
        mLight.type = mainLightSettings.light.type;
        mLight.range = mainLightSettings.light.range;
        mLight.spotAngle = mainLightSettings.light.spotAngle;
        //mLight.areaSize = new Vector2(mainLightSettings.light.width,mainLightSettings.light.height);
        mLight.color = mainLightSettings.light.color;
        //mLight.lightmapBakeType = mainLightSettings.light.mode;
        mLight.intensity = mainLightSettings.light.intensity;
        mLight.bounceIntensity = mainLightSettings.light.indirectMultiplier;
        mLight.shadows = mainLightSettings.light.shadowType;
        //bakedShadowRadius bakedShadowRadius
        mLight.shadowStrength = mainLightSettings.light.realtimeShadows.strength;
        mLight.shadowResolution = mainLightSettings.light.realtimeShadows.resolution;
        mLight.shadowBias = mainLightSettings.light.realtimeShadows.bias;
        mLight.shadowNormalBias = mainLightSettings.light.realtimeShadows.normalBias;
        mLight.shadowNearPlane = mainLightSettings.light.realtimeShadows.nearPlane;
        mLight.cookieSize = mainLightSettings.light.cookieSize;
        mLight.renderMode = mainLightSettings.light.renderMode;
    }
}