using DM.Entity;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace DM.RGEditor
{
    public class ModelAssetTemplate : ScriptableObject
    {
        protected const string LEFT_VERTICAL_GROUP1 = "基本信息";
        protected const string LEFT_VERTICAL_GROUP2 = "属性设置";
        protected const string LEFT_VERTICAL_GROUP21 = "属性设置/标签";
        protected const string LEFT_VERTICAL_GROUP3 = "生成设置";


        [HideLabel, PreviewField(60)]
        [HorizontalGroup(LEFT_VERTICAL_GROUP1 + "/Split", 60, LabelWidth = 200)]
        [Required]public Sprite Icon;

        [FoldoutGroup(LEFT_VERTICAL_GROUP1, Expanded = true)]
        [VerticalGroup(LEFT_VERTICAL_GROUP1 + "/Split/Right")]
        [Required]
        [DelayedProperty][OnValueChanged("NameChanged")]
        [LabelText("组件名称")]public string AssetName;

        [VerticalGroup(LEFT_VERTICAL_GROUP1 + "/Split/Right")]
        [LabelText("描述信息")]public string Description;

        [AssetsOnly]
        [VerticalGroup(LEFT_VERTICAL_GROUP1 + "/Split/Right")]
        [Required][LabelText("组件模型")]public GameObject Prefab;

        /*********************************************************/
        [FoldoutGroup(LEFT_VERTICAL_GROUP2)]
        [Button]
        public void 更新RGSettings配置()
        {
            if (UpdateSettingsEvent != null)
                UpdateSettingsEvent();
        }

        [TabGroup(LEFT_VERTICAL_GROUP21, "类型属性")]
        [LabelText("自身尺寸")]
        public float Size;

        [TabGroup(LEFT_VERTICAL_GROUP21, "类型属性")]
        [LabelText("组件类型")]
        public List<RGTagItem> listComponent;

        [TabGroup(LEFT_VERTICAL_GROUP21, "类型属性")]
        [LabelText("容器类型")]
        public List<RGTagItem> listContainer;


        /*********************************************************/

        [TabGroup(LEFT_VERTICAL_GROUP21, "放置属性")]
        [LabelText("可放置地形上")]
        public List<RGTagItem> listLayoutTerrain;

        [TabGroup(LEFT_VERTICAL_GROUP21, "放置属性")]
        [LabelText("可放置容器内")]
        public List<RGTagItem> listLayoutContainer;

        [TabGroup(LEFT_VERTICAL_GROUP21, "放置属性")]
        [LabelText("可放置组件上")]
        public List<RGTagItem> listLayoutComponent;

        /*********************************************************/

        [TabGroup(LEFT_VERTICAL_GROUP21, "标签属性")]
        [LabelText("标签数据")]
        public List<RGTagItem> listTags;

        /*********************************************************/

        [TabGroup(LEFT_VERTICAL_GROUP21, "其他属性")]
        [LabelText("触发类型")]
        public List<RGAffectTriggerItem> listTriggers;
        [TabGroup(LEFT_VERTICAL_GROUP21, "其他属性")]
        [LabelText("效果类型")]
        public List<RGAffectTriggerItem> listAffects;

        /*********************************************************/

        [FoldoutGroup(LEFT_VERTICAL_GROUP3, Expanded = true)]
        [HorizontalGroup(LEFT_VERTICAL_GROUP3+"/Group1")]
        [LabelText("生成标签")]public bool IsCreateTag = true;

        [HorizontalGroup(LEFT_VERTICAL_GROUP3 + "/Group1")]
        [LabelText("生成脚本")]public bool IsCreateScript=true;

        [HorizontalGroup(LEFT_VERTICAL_GROUP3 + "/Group2")]
        [LabelText("生成模型")] public bool IsCreateModel = true;

        [HorizontalGroup(LEFT_VERTICAL_GROUP3 + "/Group2")]
        [LabelText("模型压缩")]public bool IsCompressModel = false;

        [FoldoutGroup(LEFT_VERTICAL_GROUP3, Expanded = true)]
        [LabelText("主入口自定义(组件只能有一个入口(继承ScriptManager)),如已挂载入口脚本，该功能失效")]
        public bool isAssignMain;
        [FoldoutGroup(LEFT_VERTICAL_GROUP3, Expanded = true)]
        [ShowIf("$isAssignMain")]
        [LabelText("主入口Main(命名空间.MainClass,若没有命名空间则不填)")]
        public string MainCalss;

        [FoldoutGroup(LEFT_VERTICAL_GROUP3, Expanded = true)]
        [FolderPath(AbsolutePath = true)]
        [LabelText("打包额外脚本路径")]
        public string[] OtherScriptPaths;

        [FoldoutGroup(LEFT_VERTICAL_GROUP3, Expanded = true)]
        [LabelText("输出路径")]
        [FolderPath(AbsolutePath =true)]
        public string[] OutputPaths;

        [FoldoutGroup(LEFT_VERTICAL_GROUP3, Expanded = true)]
        [LabelText("引用类库")]
        [FilePath(AbsolutePath = true)]
        public string[] ReferencePaths;

        [HorizontalGroup(LEFT_VERTICAL_GROUP3+"group")]
        [FolderPath(AbsolutePath = true)]
        [LabelText("批量添加引用类库")]
        public string BatchAddRefsPath;

        [HorizontalGroup(LEFT_VERTICAL_GROUP3 + "group")]
        [Button]
        public void 全部添加()
        {
            string[] batchs = Directory.GetFiles(BatchAddRefsPath, "*.dll", SearchOption.AllDirectories);
            List<string> paths=new List<string>(ReferencePaths);
            paths.AddRange(batchs);
            ReferencePaths = paths.ToArray();
        }
        [HorizontalGroup(LEFT_VERTICAL_GROUP3 + "group")]
        [Button]
        public void 清空引用()
        {
            ReferencePaths = null;
        }


        public Action UpdateSettingsEvent;
        public Action<string> NameChangedEvent;
        public void NameChanged()
        {
            if (NameChangedEvent != null)
                NameChangedEvent(AssetName);
        }
    }
}
