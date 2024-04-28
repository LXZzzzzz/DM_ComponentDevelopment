using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DM.RGEditor
{
    public class RGTerrainEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("DMRGEditor/地形生成器")]
        private static void Open()
        {
            var window = GetWindow<RGTerrainEditorWindow>();
            window.titleContent = new GUIContent("地形生成器-DMRGEditorTerrain");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 700);
            if (!Directory.Exists(SettingsAsset.TerrianAssetsPath))
                Directory.CreateDirectory(SettingsAsset.TerrianAssetsPath);
        }
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;

            tree.AddAllAssetsAtPath("", SettingsAsset.TerrianAssetsPath, typeof(TerrainAsssetTemplate), true).ForEach(this.AddDragHandles);
            tree.EnumerateTree().Where(x => x.Value as TerrainAsssetTemplate).ForEach(AddDragHandles);
            tree.EnumerateTree().Where(x => x.Value as TerrainAsssetTemplate).ForEach(delegate (OdinMenuItem item) {
                TerrainAsssetTemplate asset = (TerrainAsssetTemplate)item.Value;
                asset.NameChangedEvent += delegate (string newName) {
                    string oldAsset = SettingsAsset.TerrianAssetsPath + "/" + item.Name + ".asset";
                    string newAsset = SettingsAsset.TerrianAssetsPath + "/" + newName + ".asset";
                    if (File.Exists(oldAsset))
                    {
                        File.Move(oldAsset, newAsset);
                        File.Move(oldAsset + ".meta", newAsset + ".meta");
                        asset.AssetName = newName;
                        AssetDatabase.Refresh();
                    }
                };
            });

            return tree;
        }
        private void AddDragHandles(OdinMenuItem menuItem)
        {
            menuItem.OnDrawItem += x => DragAndDropUtilities.DragZone(menuItem.Rect, menuItem.Value, false, false);
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
                    GUILayout.Label("未选择");

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("创建")))
                {
                    TerrainAsssetTemplate asset = ScriptableObject.CreateInstance<TerrainAsssetTemplate>();
                    string path = SettingsAsset.TerrianAssetsPath + "/NewTerrain";
                    string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + ".asset");

                    string current = SettingsAsset.GlobalAssetsPath + "/current.config";
                    if (File.Exists(current))
                    {
                        string curApply = File.ReadAllLines(current)[0];
                        string globalPath = SettingsAsset.GlobalAssetsPath + "/" + curApply + ".asset";
                        GlobalAsssetTemplate global = AssetDatabase.LoadAssetAtPath<GlobalAsssetTemplate>(globalPath);
                        asset.AutoCreateNum = global.AutoCreateNum;
                        asset.HideCams = global.HideCams;
                        asset.IsCompressTerrain = global.IsCompressTerrain;
                        asset.RootPath = global.RootPath;
                        asset.TerrainOutputPaths = global.TerrainOutputPaths;
                        asset.TerrainReferencePaths = global.TerrainReferencePaths;
                    }
                    AssetDatabase.CreateAsset(asset, assetPathAndName);
                    AssetDatabase.SaveAssets();
                    asset.AssetName = asset.name;
                    AssetDatabase.Refresh();
                }
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("删除")))
                {
                    for (int i = 0; i < MenuTree.Selection.Count; i++)
                    {
                        string delPath = SettingsAsset.TerrianAssetsPath + "/" + MenuTree.Selection[i].Name + ".asset";
                        if (File.Exists(delPath))
                        {
                            File.Delete(delPath);
                            File.Delete(delPath + ".meta");
                        }
                    }                 
                    AssetDatabase.Refresh();
                }
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("生成")))
                {
                    for (int k = 0; k < MenuTree.Selection.Count; k++)
                    {
                        TerrainAsssetTemplate asset = (TerrainAsssetTemplate)MenuTree.Selection[k].Value;
                        if (asset.TerrainOutputPaths.Length <= 0)
                        {
                            Debug.LogError(asset.AssetName + "输出路径不能为空！");
                        }
                        else
                        {
                            for (int i = 0; i < asset.terrains.Length; i++)
                            {
                                if (string.IsNullOrEmpty(asset.terrains[i].Name) ||
                                    !File.Exists(asset.terrains[i].Path))
                                {
                                    Debug.LogError("地形列表第" + i + "个场景名为空或者路径不正确！");
                                    return;
                                }
                            }
                            BundleFromRGTerrainAsset(asset);
                        }
                    }                   
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
        private void BundleFromRGTerrainAsset(TerrainAsssetTemplate asset)
        {
                    
            for (int i = 0; i < asset.terrains.Length; i++)
            {
                //自动隐藏全部场景相机   问题：只能获取loaded scene？
                //string assetPath = UserOperUtility.GetAssetPath(asset.terrains[i].Path);
                //Scene scene=EditorSceneManager.GetSceneByPath(assetPath);
                //EditorSceneManager.sceneOpened += SceneHandleMethod;
                Scene scene = EditorSceneManager.GetActiveScene();
                GameObject[] rootObjects = scene.GetRootGameObjects();
                for (int j = 0; j < rootObjects.Length; j++)
                {
                    Camera[] cams = rootObjects[j].GetComponentsInChildren<Camera>(true);
                    for (int m = 0; m < cams.Length; m++)
                        cams[m].enabled = false;
                }

                //生成地形
                string[] outputPaths = asset.TerrainOutputPaths.Distinct().ToArray();
                string folderPath = outputPaths[0] + "/" + asset.terrains[i].Name;
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                //Method1:BuildAdditionalStreamedScenes
                if (asset.BundleType == SceneBundleType.BuildPlayer)
                {
                    string[] levels = { asset.terrains[i].Path };
                    string outputScenePath = folderPath + "/" + asset.terrains[i].Name + ".dmScene";
                    BuildPipeline.BuildPlayer(levels, outputScenePath, BuildTarget.StandaloneWindows, BuildOptions.BuildAdditionalStreamedScenes);
                }
                //Method2:BuildAssetBundles
                if (asset.BundleType == SceneBundleType.BuildAssetBundles)
                {
                    //保存组件模型贴图资源
                    string absPath = asset.terrains[i].Path;
                    string assetPath = absPath.Substring(absPath.IndexOf("Assets"));
                    assetPath = assetPath.Replace('\\', '/');
                    string[] assetPaths = new string[] { assetPath };
                    AssetBundleBuild[] abbs = new AssetBundleBuild[1];
                    abbs[0].assetBundleName = asset.terrains[i].Name + ".dmScene";
                    abbs[0].assetNames = assetPaths;
                    for (int k = 0; k < assetPaths.Length; k++)
                    {
                        Debug.Log("Path:"+assetPaths[i]);
                    }
                    BuildAssetBundleOptions options = asset.IsCompressTerrain ?
                        BuildAssetBundleOptions.ChunkBasedCompression : BuildAssetBundleOptions.UncompressedAssetBundle;
                    BuildPipeline.BuildAssetBundles(folderPath, abbs, options, BuildTarget.StandaloneWindows);
                }

                string outputConfigPath = folderPath + "/" + asset.terrains[i].Name + ".config";
                string abPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + asset.terrains[i].Path;
                string sceneName = Path.GetFileNameWithoutExtension(abPath);
                string[] configContext = new string[] {sceneName, asset.terrains[i].Version, asset.BundleType.ToString()};
                File.WriteAllLines(outputConfigPath,configContext);

                //复制到其他路径
                //for (int k = 1; k < outputPaths.Length; k++)
                //{
                //    string destFolder = outputPaths[k] + "/" + asset.terrains[i].Name;
                //    CommonUtility.CopyFolder(folderPath, destFolder);
                //}
                                  
                //版本号自增
                string[] ver=asset.terrains[i].Version.Split('.');
                if (ver.Length != 3)
                {
                    Debug.LogError("地形列表第" + i + "个版本号错误,生成失败！");
                    return;
                }
                int oldVer = int.Parse(ver[2]);
                asset.terrains[i].Version = "2.1." + (oldVer + 1);
            }
            Debug.Log("地形场景生成完成[" + asset.AssetName + "]");
            ShowNotification(new GUIContent("地形场景生成完成[" + asset.AssetName + "]"));
        }
        public void AddSceneFromMenu(string sceneAssetPath)
        {
            if (MenuTree==null) return;
            TerrainAsssetTemplate asset = (TerrainAsssetTemplate)MenuTree.Selection[0].Value;
            RGTerrainItem item = new RGTerrainItem() {
                Name = new FileInfo(sceneAssetPath).Name.Split('.')[0],
                Path = sceneAssetPath,
                Version = "2.1.0",
            };
            List<RGTerrainItem> list = new List<RGTerrainItem>(asset.terrains);
            if (list.Find(k => k.Name == item.Name) != null)
            {
                Debug.LogError("该场景["+item.Name+"]已存在！");
                return;
            } 
            list.Add(item);
            asset.terrains = list.ToArray();
        }
    }
}
