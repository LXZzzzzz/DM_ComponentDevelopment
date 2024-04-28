using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LGroup
{
    public Transform Group;
    public bool IsOn = true;
    public string BeginTime = "18:00";
    public string EndTime = "06:00";
}
[System.Serializable]
public class LGroup2
{
    public Transform Group;
    public string Selected;
    public bool IsOn = true;
    public string BeginTime = "18:00";
    public string EndTime = "06:00";
    [HideInInspector]
    public Transform[] SelectdObjs;
}
[System.Serializable]
public class EGroup
{
    public Transform Group;
    public bool IsOn = true;
    public string BeginTime = "18:00";
    public string EndTime = "06:00";
    public Color Color;
    public float Intensity;
    public Color OutTimeColor;
    public float OutTimeIntensity;
}
[System.Serializable]
public class RGroup
{
    public Transform Group;
    public RGroupItem[] TimeSpans;
}
[System.Serializable]
public class RGroupItem
{
    public string BeginTime = "18:00";
    public string EndTime = "06:00";
    public float Interval = 60f;
    [HideInInspector]
    public float Timer = 0;
}
[System.Serializable]
public class PostGroup
{
    public Transform PostProcess;
    //public PostProcessProfile Profile;
    public string BeginTime = "18:00";
    public string EndTime = "06:00";
}
[System.Serializable]
public class VolumeCloud
{
    public Transform Cloud;
    public string Weather;
    public VCloudItem[] TimeSpans;
}
[System.Serializable]
public class VCloudItem
{
    public string BeginTime = "18:00";
    public string EndTime = "20:00";
    public Color Color = Color.white;
}
[System.Serializable]
public class ModeObject
{
    public GameObject ShowObj;
    [Tooltip("运行模式")]
    public bool RunMode;
    [Tooltip("编辑模式")]
    public bool EditorMode;
    [Tooltip("火源编辑模式")]
    public bool FireMode;
    [Tooltip("路径编辑模式")]
    public bool PathFindMode;
}
public enum ObjMode
{
    RunMode,
    EditorMode,
    FireMode,
    PathFindMode,
}
[System.Serializable]
public class Wall
{
    public GameObject ShowWall;
    public GameObject Trigger;
}

public class DMSceneConfig : MonoBehaviour
{
    public static DMSceneConfig instance;
    public Transform MainCameraPos;
    public float MaxCamHeight = 5000;
    public LGroup[] Light;
    public LGroup2[] Light2;
    public EGroup[] Emission;
    [Tooltip("反射球")]
    public RGroup ReflectionProbe;
    [Tooltip("体积云")]
    public VolumeCloud VolumeCloud;
    [Tooltip("后期文件")]
    public PostGroup[] PostProcess;
    [Tooltip("物体显示模式")]
    public ModeObject[] ShowObjects;
    [Tooltip("墙")]
    public Wall Wall;
    
    [HideInInspector]
    public bool OnRender;

    private string gameTime;
    private RGroupItem tempRGItem;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        //刷新ReflectionProbe
        if (OnRender)
        {
            for (int i = 0; i < ReflectionProbe.TimeSpans.Length; i++)
            {
                tempRGItem = ReflectionProbe.TimeSpans[i];
                if (CheckTime(tempRGItem.BeginTime, tempRGItem.EndTime, gameTime))
                {
                   tempRGItem.Timer += Time.deltaTime;
                    if (tempRGItem.Timer >= tempRGItem.Interval)
                    {
                        ReflectionProbe[] rps = ReflectionProbe.Group.GetComponentsInChildren<ReflectionProbe>(true);
                        for (int j = 0; j < rps.Length; j++)
                        {
                            rps[j].RenderProbe();
                        }
                        tempRGItem.Timer = 0f;
                    }
                }
            }
        }
    }
    public void SetMode(ObjMode mode)
    {
        if (ShowObjects != null)
        {
            for (int i = 0; i < ShowObjects.Length; i++)
            {
                if (ShowObjects[i].ShowObj != null)
                {
                    switch (mode)
                    {
                        case ObjMode.RunMode:
                            ShowObjects[i].ShowObj.SetActive(ShowObjects[i].RunMode);
                            break;
                        case ObjMode.EditorMode:
                            ShowObjects[i].ShowObj.SetActive(ShowObjects[i].EditorMode);
                            break;
                        case ObjMode.FireMode:
                            ShowObjects[i].ShowObj.SetActive(ShowObjects[i].FireMode);
                            break;
                        case ObjMode.PathFindMode:
                            ShowObjects[i].ShowObj.SetActive(ShowObjects[i].PathFindMode);
                            break;
                    }
                }
            }
        }

        if (Light2 != null)
        {
            for (int i = 0; i < Light2.Length; i++)
            {
                List<Transform> list = new List<Transform>();
                Transform[] children = Light2[i].Group.GetComponentsInChildren<Transform>(true);
                for (int j = 0; j < children.Length; j++)
                {
                    if (children[j].name == Light2[i].Selected)
                        list.Add(children[j]);
                }
                Light2[i].SelectdObjs = list.ToArray();
            }
        }
    }
    public void RefreshRender()
    {
        for (int i = 0; i < ReflectionProbe.TimeSpans.Length; i++)
        {
            RGroupItem tempRGItem = ReflectionProbe.TimeSpans[i];
            if (CheckTime(tempRGItem.BeginTime, tempRGItem.EndTime, gameTime))
            {
                ReflectionProbe[] rps = ReflectionProbe.Group.GetComponentsInChildren<ReflectionProbe>(true);
                for (int j = 0; j < rps.Length; j++)
                {
                    rps[j].RenderProbe();
                }
            }
        }
    }
    public void SetTime(string time)
    {
        gameTime = time;
        //Lights
        if (Light != null)
        {
            for (int i = 0; i < Light.Length; i++)
            {
                if (Light[i].Group != null)
                {
                    Light[i].Group.gameObject.SetActive(
                        CheckTime(Light[i].BeginTime, Light[i].EndTime, time) ? Light[i].IsOn : !Light[i].IsOn);
                }
            }
        }

        //Lights2
        if (Light2 != null)
        {
            for (int i = 0; i < Light2.Length; i++)
            {
                if (Light2[i].Group != null)
                {
                    bool isShow = CheckTime(Light2[i].BeginTime, Light2[i].EndTime, time);
                    for (int j = 0; j < Light2[i].SelectdObjs.Length; j++)
                    {
                        Light2[i].SelectdObjs[j].gameObject.SetActive(isShow ? Light2[i].IsOn : !Light2[i].IsOn);
                    }
                }
            }
        }

        //Emissions
        if (Emission != null)
        {
            for (int i = 0; i < Emission.Length; i++)
            {
                if (Emission[i].Group != null && Emission[i].IsOn)
                {
                    MeshRenderer[] mrs = Emission[i].Group.GetComponentsInChildren<MeshRenderer>(true);
                    for (int j = 0; j < mrs.Length; j++)
                    {
                        bool include = CheckTime(Emission[i].BeginTime, Emission[i].EndTime, time);
                        if (include)
                            SetEmission(mrs[j], Emission[i].Color, Emission[i].Intensity);
                        else
                            SetEmission(mrs[j],Emission[i].OutTimeColor,Emission[i].OutTimeIntensity);
                    }
                }
            }
        }
       
        //Volume Cloud
        if (VolumeCloud.Cloud != null&&VolumeCloud.Weather.Contains("curValue"))
        {
            for (int i = 0; i < VolumeCloud.TimeSpans.Length; i++)
            {
                if (CheckTime(VolumeCloud.TimeSpans[i].BeginTime, VolumeCloud.TimeSpans[i].EndTime, time))
                {
                    SetCloudColor(VolumeCloud.TimeSpans[i].Color);
                }
            }
        }
    }
    public PostGroup GetCurPostProcess()
    {
        if (PostProcess == null) return null;
        for (int i = 0; i < PostProcess.Length; i++)
        {
            if (PostProcess[i].PostProcess != null)
            {
                if (CheckTime(PostProcess[i].BeginTime, PostProcess[i].EndTime, gameTime))
                    return PostProcess[i];
            }
        }
        return null;
    }
    private void SetEmission(MeshRenderer mr, Color col,float intensity)
    {
        Material[] mats = mr.materials;
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i].SetColor("_EmissionColor", col);
            mats[i].SetVector("_EmissionColor",col*1);  //intensity置为0
            Vector4 ec = mats[i].GetVector("_EmissionColor");  //获取intensity=0时的标准值
            //GV(intensity0) = ec; GV(intensity1)=new Vector4(ec.x*2^1,ec.y*2^1,ec.z*2^1,1);
            //GV(intensity2)=new Vector4(ec.x*2^2,ec.y*2^2,ec.z*2^2,1);
            mats[i].SetVector("_EmissionColor", new Vector4(ec.x * Mathf.Pow(2, intensity), ec.y * Mathf.Pow(2, intensity), ec.z * Mathf.Pow(2, intensity), 1));
        }    
    }
    private void SetCloudColor(Color col)
    {
    }
    private bool CheckTime(string begin, string end,string value)
    {
        float fBegin = TimeToFloat(begin);
        float fEnd = TimeToFloat(end);
        float fValue = TimeToFloat(value);
        if (fBegin < fEnd) //没有跨天
        {
            if (fValue >= fBegin && fValue < fEnd)
                return true;
            return false;
        }
        else  //跨天，需拆分为两块
        {
            if (fValue >= fBegin || fValue < fEnd)
                return true;
            return false;
        }
    }
    private float TimeToFloat(string time)
    {
        if (string.IsNullOrEmpty(time)) return 0;
        string[] strs = time.Split(':');
        if (strs.Length != 2) return 0;
        return int.Parse(strs[0]) + float.Parse(strs[1])/60;
    }
}
