using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;
using System;

namespace DM.RGEditor
{
    public class RGModelEditorWindow:OdinMenuEditorWindow
    {
        [MenuItem("DMRGEditor/组件生成器")]
        private static void Open()
        {
            string current = SettingsAsset.GlobalAssetsPath + "/current.config";
            if (!File.Exists(current))
            {
                Debug.LogError("全局设置没有应用当前设置");
                return;
            } 
            string projectName = File.ReadAllLines(current)[0];
            curProjectPath = SettingsAsset.ModelAssetsPath + "/" +projectName ;
            var window = GetWindow<RGModelEditorWindow>();
            window.titleContent = new GUIContent("【"+projectName+"】组件生成器-DMRGEditorComponent");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 1000);
            if (!Directory.Exists(SettingsAsset.ModelAssetsPath))
                Directory.CreateDirectory(SettingsAsset.ModelAssetsPath);
        }
        private static string curProjectPath;
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;

            string current = SettingsAsset.GlobalAssetsPath + "/current.config";
            string projectName = File.ReadAllLines(current)[0];
            curProjectPath = SettingsAsset.ModelAssetsPath + "/" + projectName;
            tree.AddAllAssetsAtPath("", curProjectPath, typeof(ModelAssetTemplate), true,false).ForEach(this.AddDragHandles);
            tree.EnumerateTree().Where(x => x.Value as ModelAssetTemplate).ForEach(AddDragHandles);
            tree.EnumerateTree().AddIcons<ModelAssetTemplate>(x => x.Icon);
            tree.EnumerateTree().Where(x => x.Value as ModelAssetTemplate).ForEach(delegate (OdinMenuItem item) {
                ModelAssetTemplate asset = (ModelAssetTemplate)item.Value;
                asset.NameChangedEvent += delegate (string newName) {
                    string oldAsset = curProjectPath + "/" + item.Name + ".asset";
                    string newAsset = curProjectPath + "/" + newName + ".asset";
                    if (File.Exists(oldAsset))
                    {
                        File.Move(oldAsset, newAsset);
                        File.Move(oldAsset + ".meta", newAsset + ".meta");
                        asset.AssetName = newName;
                        AssetDatabase.Refresh();
                    }
                };
                asset.UpdateSettingsEvent += UpdateSettings;
            });
            return tree;
        }

        private void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
        }
        private void UpdateSettings()
        {
            ModelAssetTemplate asset = (ModelAssetTemplate)MenuTree.Selection[0].Value;
            LayoutSettings layout = SettingsAsset.GetAssetFromPath<LayoutSettings>(SettingsAsset.LayoutAssetPath);
            TagSettings tag = SettingsAsset.GetAssetFromPath<TagSettings>(SettingsAsset.TagAssetPath);
            AffectSettings affect = SettingsAsset.GetAssetFromPath<AffectSettings>(SettingsAsset.AffectAssetPath);
            if (layout == null || tag == null || affect == null)
            {
                Debug.LogError("配置文件不存在或者有问题！");
                return;
            }

            asset.listComponent = layout.GetComponent();
            asset.listContainer = layout.GetContainer();
            asset.listLayoutTerrain = layout.GetTerrain();
            asset.listLayoutContainer = layout.GetContainer();
            asset.listLayoutComponent = layout.GetComponent();
            asset.listTags = tag.GetTags();
            asset.listAffects = affect.GetAffects();
            asset.listTriggers = affect.GetTriggers();
        }

        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                    GUILayout.Label(selected.Name);
                else
                    GUILayout.Label("未选中");

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("创建")))
                {
                    AddModelInstance("未命名");
                    AssetDatabase.Refresh();
                }
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("删除")))
                { 
                    bool userConfirmed = EditorUtility.DisplayDialog(
                        "二次确认",
                        "是否确认删除组件?",
                        "确认",
                        "取消"
                    );
                    if (userConfirmed)
                    {
                        for (int i = 0; i < MenuTree.Selection.Count; i++)
                        {
                            string delPath = curProjectPath + "/" + MenuTree.Selection[i].Name + ".asset";
                            if (File.Exists(delPath))
                            {
                                File.Delete(delPath);
                                File.Delete(delPath + ".meta");
                            }
                        }                  
                        AssetDatabase.Refresh();
                    }
                }
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("生成")))
                {
                    for (int i = 0; i < MenuTree.Selection.Count; i++)
                    {
                        ModelAssetTemplate asset = (ModelAssetTemplate)MenuTree.Selection[i].Value;
                        string signal = DateTime.Now.ToFileTime().ToString();
                        if (asset.OutputPaths.Length <= 0)
                        {
                            Debug.LogError(asset.AssetName + "输出路径不能为空！");
                        }
                        else
                        {
                            BundleFromRGModelAsset(asset, signal);
                            Debug.Log(asset.AssetName + "生成完成[" + signal + "]");
                            ShowNotification(new GUIContent(asset.AssetName + "生成完成[" + signal + "]"));
                        }
                    }                        
                }
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("全部生成")))
                {
                    bool userConfirmed = EditorUtility.DisplayDialog(
                        "二次确认",
                        "是否确认全部生成组件?",
                        "确认",
                        "取消"
                    );
                    if (userConfirmed)
                    {
                        string signal = DateTime.Now.ToFileTime().ToString();
                        for (int i = 0; i < MenuTree.MenuItems.Count; i++)
                        {
                            ModelAssetTemplate asset = (ModelAssetTemplate)MenuTree.MenuItems[i].Value;
                            if (asset.OutputPaths.Length <= 0)
                                Debug.LogError(asset.AssetName + "输出路径不能为空！");
                            else
                                BundleFromRGModelAsset(asset,signal);
                        }
                        Debug.Log("全部组件生成完成[" + signal + "]");
                        ShowNotification(new GUIContent("全部组件生成完成[" + signal + "]"));
                    }
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        private void BundleFromRGModelAsset(ModelAssetTemplate asset,string signal)
        {
            if (asset.Prefab == null)
            {
                Debug.LogError("Prefab不能为空！");
                return;
            }
            if (asset.Icon == null)
            {
                Debug.LogError("Icon不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(asset.AssetName))
            {
                Debug.LogError("组件名不能为空！");
                return;
            }

            //输出路径和引用DLL去重
            string[] outputPaths= asset.OutputPaths.Distinct().ToArray();
            string[] references = asset.ReferencePaths.Distinct().ToArray();
        
            //TagConfig
            if (asset.IsCreateTag)
            {
                RGModelCreate.CreateModelTagOnPrefab(asset, outputPaths[0], signal);
            }
            //Script and Data
            if (asset.IsCreateScript)
            {
                RGModelCreate.CreateScriptDLLAndDataOnPrefab(asset, outputPaths[0], signal);
            }

            //Model
            if (asset.IsCreateModel)
            {
                RGModelCreate.CreateModelAssetBundleOnPrefab(asset, outputPaths[0]);
                //生成模型必重新生成TagConfig
                RGModelCreate.CreateModelTagOnPrefab(asset, outputPaths[0], signal);
            }

            //脚本文件没有拷贝 CompileAssemblyFromFile异步执行?
            //int k =0;
            //for (int i = 0; i < 1000; i++){k += i;}
            //Solution: File.Copy(file, dest,true) 

            //Copy to Other OutputPaths
            string sourceFolder = outputPaths[0] + "/" + asset.AssetName;
            for (int i = 1; i < outputPaths.Length; i++)
            {
                string destFolder = outputPaths[i] + "/" + asset.AssetName;
                CommonUtility.CopyFolder(sourceFolder,destFolder);
            }
        }

        public ModelAssetTemplate AddModelInstance(string name)
        {
            ModelAssetTemplate asset = ScriptableObject.CreateInstance<ModelAssetTemplate>();
            LayoutSettings layout = SettingsAsset.GetAssetFromPath<LayoutSettings>(SettingsAsset.LayoutAssetPath);
            TagSettings tag = SettingsAsset.GetAssetFromPath<TagSettings>(SettingsAsset.TagAssetPath);
            AffectSettings affect = SettingsAsset.GetAssetFromPath<AffectSettings>(SettingsAsset.AffectAssetPath);
            if (layout == null || tag == null || affect == null)
            {
                Debug.LogError("配置文件不存在或者有问题！");
                return null;
            }
            string path = curProjectPath + "/"+name;
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + ".asset");
            asset.listComponent = layout.GetComponent();
            asset.listContainer = layout.GetContainer();
            asset.listLayoutTerrain = layout.GetTerrain();
            asset.listLayoutContainer = layout.GetContainer();
            asset.listLayoutComponent = layout.GetComponent();
            asset.listTags = tag.GetTags();
            asset.listAffects = affect.GetAffects();
            asset.listTriggers = affect.GetTriggers();

            string current = SettingsAsset.GlobalAssetsPath + "/current.config";
            if (File.Exists(current))
            {
                string curApply = File.ReadAllLines(current)[0];
                string globalPath = SettingsAsset.GlobalAssetsPath + "/" + curApply + ".asset";
                GlobalAsssetTemplate global = AssetDatabase.LoadAssetAtPath<GlobalAsssetTemplate>(globalPath);
                asset.IsCreateTag = global.IsCreateTag;
                asset.IsCreateScript = global.IsCreateScript;
                asset.IsCreateModel = global.IsCreateModel;
                asset.IsCompressModel = global.IsCompressModel;
                asset.OutputPaths = global.OutputPaths;
                asset.ReferencePaths = global.ReferencePaths;
            }
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            asset.AssetName = asset.name;
            return asset;
        }
    }
}
