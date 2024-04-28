using Sirenix.OdinInspector;
using UnityEngine;
using System;

namespace DM.RGEditor
{
    public class GlobalAsssetTemplate: ScriptableObject
    {
        protected const string LEFT_VERTICAL_GROUP1 = "组件生成设置";
        protected const string LEFT_VERTICAL_GROUP2 = "地形生成设置";
        protected const string LEFT_VERTICAL_GROUP3 = "组件调试设置";

        //[FoldoutGroup(LEFT_VERTICAL_GROUP1,Expanded =true)]
        [DelayedProperty]
        [LabelText("配置文件名")]
        [OnValueChanged("NameChanged")]
        public string AssetName;

        [HorizontalGroup(LEFT_VERTICAL_GROUP1 + "/Group1")]
        [LabelText("生成标签")]
        public bool IsCreateTag = true;

        [HorizontalGroup(LEFT_VERTICAL_GROUP1 + "/Group1")]
        [LabelText("生成脚本")]
        public bool IsCreateScript = true;

        [HorizontalGroup(LEFT_VERTICAL_GROUP1 + "/Group2")]
        [LabelText("生成模型")]
        public bool IsCreateModel = true;

        [HorizontalGroup(LEFT_VERTICAL_GROUP1 + "/Group2")]
        [LabelText("模型压缩")]
        public bool IsCompressModel = false;

        [FoldoutGroup(LEFT_VERTICAL_GROUP1)]
        [LabelText("输出路径")]
        [FolderPath(AbsolutePath = true)]
        public string[] OutputPaths;

        [FoldoutGroup(LEFT_VERTICAL_GROUP1)]
        [LabelText("引用类库")]
        [FilePath(AbsolutePath = true)]
        public string[] ReferencePaths;

        [FoldoutGroup(LEFT_VERTICAL_GROUP1)]
        [Button]
        public void 更新已有组件生成设置()
        {
        }


        [HorizontalGroup(LEFT_VERTICAL_GROUP2 + "/Group1")]
        [LabelText("自动填充版本号")]
        public bool AutoCreateNum = true;

        [HorizontalGroup(LEFT_VERTICAL_GROUP2 + "/Group1")]
        [LabelText("隐藏场景中全部相机")]
        public bool HideCams = true;

        [HorizontalGroup(LEFT_VERTICAL_GROUP2 + "/Group2")]
        [LabelText("场景压缩")]
        public bool IsCompressTerrain = false;

        [FoldoutGroup(LEFT_VERTICAL_GROUP2)]
        [FolderPath(AbsolutePath = false)]
        [LabelText("Scene文件根路径")]
        public string RootPath;

        [FoldoutGroup(LEFT_VERTICAL_GROUP2)]
        [LabelText("输出路径")]
        [FolderPath(AbsolutePath = true)]
        public string[] TerrainOutputPaths;

        [FoldoutGroup(LEFT_VERTICAL_GROUP2)]
        [LabelText("引用类库")]
        [FilePath(AbsolutePath = true)]
        public string[] TerrainReferencePaths;

        [FoldoutGroup(LEFT_VERTICAL_GROUP2)]
        [Button]
        public void 更新已有地形生成设置()
        {
        }

        [FoldoutGroup(LEFT_VERTICAL_GROUP3, Expanded = true)]
        [LabelText("开启调试功能")]
        public bool IsDebug = true;

        public Action<string> NameChangedEvent;
        public void NameChanged()
        {
            if (NameChangedEvent != null)
                NameChangedEvent(AssetName);
        }
    }
}
