using Sirenix.OdinInspector;
using UnityEngine;
using System;
using System.IO;

namespace DM.RGEditor
{
    public enum SceneBundleType
    {
        BuildPlayer,
        BuildAssetBundles
    }
    public class TerrainAsssetTemplate : ScriptableObject
    {
        protected const string LEFT_VERTICAL_GROUP1 = "通用设置";
        protected const string LEFT_VERTICAL_GROUP2 = "地形列表";

        [FoldoutGroup(LEFT_VERTICAL_GROUP1, Expanded = true)]
        [DelayedProperty]
        [LabelText("生成设置名")]
        [OnValueChanged("NameChanged")]
        public string AssetName;
      
        [HorizontalGroup(LEFT_VERTICAL_GROUP1+"/Group1")]
        [LabelText("自动填充版本号")]
        public bool AutoCreateNum = true;

        [HorizontalGroup(LEFT_VERTICAL_GROUP1 + "/Group1")]
        [LabelText("隐藏场景中全部相机")]
        public bool HideCams = true;

        [HorizontalGroup(LEFT_VERTICAL_GROUP1 + "/Group2")]
        [LabelText("场景压缩")]
        public bool IsCompressTerrain = true;

        [HorizontalGroup(LEFT_VERTICAL_GROUP1 + "/Group4")]
        [LabelText("生成方式")]
        public SceneBundleType BundleType = SceneBundleType.BuildPlayer;

        [HorizontalGroup(LEFT_VERTICAL_GROUP1 + "/Group5")]
        [LabelText("生成平台")]
        public UnityEditor.BuildTarget BundleTarget = UnityEditor.BuildTarget.StandaloneWindows;

        [HorizontalGroup(LEFT_VERTICAL_GROUP1 + "/Group6")]
        [LabelText("参数设置")]
        public UnityEditor.BuildAssetBundleOptions BundleOption = UnityEditor.BuildAssetBundleOptions.UncompressedAssetBundle;

        [HorizontalGroup(LEFT_VERTICAL_GROUP1 + "/Group3")]
        [FolderPath(AbsolutePath = false)]
        [LabelText("Scene文件根路径")]
        public string RootPath;

        [HorizontalGroup(LEFT_VERTICAL_GROUP1 + "/Group3")]
        [Button]
        public void 获取地形()
        {
            string absolutePath=Application.dataPath + RootPath.Substring(6);
            string[] scenes = Directory.GetFiles(absolutePath, "*.unity", SearchOption.AllDirectories);
            Array.Clear(terrains,0,terrains.Length);
            terrains = new RGTerrainItem[scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
            {
                terrains[i] = new RGTerrainItem() {
                    Name =new FileInfo(scenes[i]).Name.Split('.')[0]+".dmScene",
                    Path = scenes[i],
                    Version = "2.1.0",
               };
            }
        }

        [FoldoutGroup(LEFT_VERTICAL_GROUP1)]
        [LabelText("输出路径")]
        [FolderPath(AbsolutePath = true)]
        public string[] TerrainOutputPaths;

        [FoldoutGroup(LEFT_VERTICAL_GROUP1)]
        [LabelText("引用类库")]
        [FilePath(AbsolutePath = true)]
        public string[] TerrainReferencePaths;

        [FoldoutGroup(LEFT_VERTICAL_GROUP2, Expanded = true)]
        [LabelText("添加地形")]
        public RGTerrainItem[] terrains;

        public Action<string> NameChangedEvent;
        public void NameChanged()
        {
            if (NameChangedEvent != null)
                NameChangedEvent(AssetName);
        }
    }
}
