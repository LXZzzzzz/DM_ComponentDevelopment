using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using DM.Entity;

namespace DM.RGEditor
{
    public partial class SettingsAsset
    {
        public static string LayoutAssetPath = "Assets/DMRGEditor/RGAsset/TagSettings/LayoutSettings.asset";
        public static string TagAssetPath = "Assets/DMRGEditor/RGAsset/TagSettings/TagSettings.asset";
        public static string AffectAssetPath = "Assets/DMRGEditor/RGAsset/TagSettings/AffectSettings.asset";

        public static string ModelAssetsPath= "Assets/DMRGEditor/RGAsset/ModelAssets";
        public static string TerrianAssetsPath = "Assets/DMRGEditor/RGAsset/TerrainAssets";
        public static string GlobalAssetsPath = "Assets/DMRGEditor/RGAsset/GlobalAssets";

        public static string OutputPath =Application.dataPath+"/DMRGEditor/RGOutput";
        public static string RGEditorPath = Application.dataPath + "/DMRGEditor";

        public static string ConfigSelectPath = "Assets/DMRGEditor/Editor/Resource/ConfigSelect.png";
        public static string PreViewPicture;
    }
    public partial class SettingsAsset
    {
        [MenuItem("DMRGEditor/RGSettings/Layout Settings", priority = 201)]
        static void CreateLayoutAsset()
        {
            LayoutSettings layoutAssets = GetAssetFromPath<LayoutSettings>(LayoutAssetPath);
            if (layoutAssets == null)
            {
                //生成放置配置文件
                layoutAssets = ScriptableObject.CreateInstance<LayoutSettings>();
                layoutAssets.Name = "LayoutSettings";
                layoutAssets.Path = LayoutAssetPath;
                layoutAssets.Terrain = new List<RGTagItem>();
                layoutAssets.Terrain.Add(new RGTagItem(1, "陆地", false, true));
                layoutAssets.Terrain.Add(new RGTagItem(2, "山脉", false, true));
                layoutAssets.Terrain.Add(new RGTagItem(3, "海洋", false, true));
                layoutAssets.Terrain.Add(new RGTagItem(4, "平原", false, true));
                layoutAssets.Terrain.Add(new RGTagItem(5, "湖泊", false, true));
                layoutAssets.Container = new List<RGTagItem>();
                layoutAssets.Component = new List<RGTagItem>();
                AssetDatabase.CreateAsset(layoutAssets, layoutAssets.Path);
            }
            else
            {
                Debug.LogError("LayoutSettings.asset配置文件已存在，若要重新生成，请删除原文件！");
            }
        }

        [MenuItem("DMRGEditor/RGSettings/Tag Settings", priority = 201)]
        static void CreateTagAsset()
        {
            TagSettings tagAssets = GetAssetFromPath<TagSettings>(TagAssetPath);
            if (tagAssets == null)
            {
                //生成标签配置文件
                tagAssets = ScriptableObject.CreateInstance<TagSettings>();
                tagAssets.Name = "TagSettings";
                tagAssets.Path = TagAssetPath;
                tagAssets.Tags = new List<RGTagItem>();
                tagAssets.Tags.Add(new RGTagItem(1, "客户端不显示", false, true));
                tagAssets.Tags.Add(new RGTagItem(2, "客户端显示名字", false, true));
                tagAssets.Tags.Add(new RGTagItem(3, "载具", false, true));
                tagAssets.Tags.Add(new RGTagItem(4, "触发器", false, true));
                tagAssets.Tags.Add(new RGTagItem(5, "AI", false, true));
                tagAssets.Tags.Add(new RGTagItem(6, "刷新器", false, true));
                tagAssets.Tags.Add(new RGTagItem(7, "电线杆", false, true));
                tagAssets.Tags.Add(new RGTagItem(8, "角色", false, true));
                tagAssets.Tags.Add(new RGTagItem(9, "小地图显示", false, true));
                tagAssets.Tags.Add(new RGTagItem(10, "大地图显示", false, true));
                AssetDatabase.CreateAsset(tagAssets, tagAssets.Path);
            }
            else
            {
                Debug.LogError("TagSettings.asset配置文件已存在，若要重新生成，请删除原文件！");
            }
        }

        [MenuItem("DMRGEditor/RGSettings/Affect Settings", priority = 201)]
        static void CreateAffectAsset()
        {
            AffectSettings affectAssets = GetAssetFromPath<AffectSettings>(AffectAssetPath);
            if (affectAssets == null)
            {
                affectAssets = ScriptableObject.CreateInstance<AffectSettings>();
                affectAssets.name = "AffectSettings";
                affectAssets.Path = AffectAssetPath;
                affectAssets.Triggers = new List<RGAffectTriggerItem>();
                affectAssets.Affects = new List<RGAffectTriggerItem>();
                AssetDatabase.CreateAsset(affectAssets,affectAssets.Path);
            }
            else
            {
                Debug.LogError("AffectSettings.asset配置文件已存在，若要重新生成，请删除原文件！");
            }
        }

        public static T GetAssetFromPath<T>(string filePath)
        {
            if (!File.Exists(filePath))
                return default(T);
            object assets = AssetDatabase.LoadAssetAtPath(filePath,typeof(T));
            return (T)assets;
        }
    }
}

