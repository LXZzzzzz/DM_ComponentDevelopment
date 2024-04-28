using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using DM.Entity;

namespace DM.RGEditor
{
    public class SceneAsset
    {
        [MenuItem("GameObject/DM Object/Environment", priority = 0)]
        private static void CreateEnvironment()
        {
            CreateDMObject<EnvSettings>("Environment");
        }

        [MenuItem("GameObject/DM Object/Post-Processing Profile", priority = 0)]
        private static void CreatePostProcessingProfile()
        {
            //CreateDMObject<UnityEngine.PostProcessing.PostProcessingProfile>("PostProcessingProfile");
        }

        [MenuItem("GameObject/DM Object/PostProcessing(New)")]
        private static void CreatePostProcessingNew()
        {
            //GameObject obj = CreateDMObject<PostProcessLayer>("PostProcessing(New)");
            //obj.GetComponent<PostProcessLayer>().volumeLayer = LayerMask.NameToLayer("PostProcess");
            //obj.AddComponent<PostProcessVolume>();
            //obj.GetComponent<Camera>().enabled = false;
        }

        [MenuItem("GameObject/DM Object/MainLight")]
        private static void CreateLight()
        {
            CreateDMObject<EnvMainLight>("MainLight");
        }

        [MenuItem("GameObject/DM Object/SceneConfig")]
        private static void CreateSceneConfig()
        {
            CreateDMObject<DMSceneConfig>("SceneConfig");
        }
        private static GameObject CreateDMObject<T>(string name) where T : MonoBehaviour
        {
            GameObject obj = new GameObject();
            obj.transform.position = Vector3.zero;
            obj.name = name;
            obj.AddComponent<T>();
            return obj;
        }

    }
}
