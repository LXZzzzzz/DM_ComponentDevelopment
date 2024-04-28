using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DM.RGEditor
{
    public class RGGlobalEditorWindow: OdinMenuEditorWindow
    {
        [MenuItem("DMRGEditor/全局设置", priority = 202)]
        private static void Open()
        {
            var window = GetWindow<RGGlobalEditorWindow>();
            window.titleContent = new GUIContent("全局设置-DMRGEditorGlobal");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
            if (!Directory.Exists(SettingsAsset.GlobalAssetsPath))
                Directory.CreateDirectory(SettingsAsset.GlobalAssetsPath);
            if (!Directory.Exists(SettingsAsset.OutputPath))
                Directory.CreateDirectory(SettingsAsset.OutputPath);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree(false);
            tree.DefaultMenuStyle.IconSize = 28.00f;
            tree.Config.DrawSearchToolbar = true;
          
            tree.AddAllAssetsAtPath("",SettingsAsset.GlobalAssetsPath , typeof(GlobalAsssetTemplate), true).ForEach(this.AddDragHandles);
            tree.EnumerateTree().Where(x => x.Value as GlobalAsssetTemplate).ForEach(AddDragHandles);
            tree.EnumerateTree().Where(x => x.Value as GlobalAsssetTemplate).ForEach(delegate (OdinMenuItem item){
                GlobalAsssetTemplate asset = (GlobalAsssetTemplate)item.Value;
                asset.NameChangedEvent += delegate (string newName){
                      string oldAsset = SettingsAsset.GlobalAssetsPath + "/"+item.Name+".asset";
                      string newAsset = SettingsAsset.GlobalAssetsPath + "/" + newName + ".asset";
                      if (File.Exists(oldAsset))
                      {
                          File.Move(oldAsset, newAsset);
                          File.Move(oldAsset+".meta",newAsset+".meta");
                          asset.AssetName = newName;
                          string oldModelPath = SettingsAsset.ModelAssetsPath + "/" + item.Name;
                          string newModelPath= SettingsAsset.ModelAssetsPath + "/" + newName;
                          Directory.Move(oldModelPath,newModelPath);
                          if (GetApply() == item.Name)
                             SetApply(newName);
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
        private OdinMenuItem mCurrent;
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

                //显示当前选择配置
                string current = SettingsAsset.GlobalAssetsPath + "/current.config";
                if (File.Exists(current))
                {
                    string curApply = File.ReadAllLines(current)[0];
                    mCurrent=this.MenuTree.MenuItems.Find(k => k.Name == curApply);
                    if (mCurrent != null)
                    {
                        //item.AddThumbnailIcon(true);  取消不掉图标
                        mCurrent.Icon = AssetDatabase.LoadAssetAtPath<Texture>(SettingsAsset.ConfigSelectPath);
                    }                 
                }
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("创建")))
                {
                    GlobalAsssetTemplate asset = ScriptableObject.CreateInstance<GlobalAsssetTemplate>();
                    string path = SettingsAsset.GlobalAssetsPath+"/我的配置";
                    string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + ".asset");
                    AssetDatabase.CreateAsset(asset, assetPathAndName);
                    AssetDatabase.SaveAssets();
                    asset.OutputPaths = new string[1];
                    asset.OutputPaths[0] = SettingsAsset.OutputPath;
                    asset.TerrainOutputPaths = new string[1];
                    asset.TerrainOutputPaths[0] = SettingsAsset.OutputPath;
                    asset.AssetName = asset.name;
                    string[] dllPaths=Directory.GetFiles(SettingsAsset.RGEditorPath+"/Plugins", "*.dll");
                    asset.ReferencePaths = dllPaths;
                    asset.TerrainReferencePaths = dllPaths;
                    string modelPath = SettingsAsset.ModelAssetsPath + "/"+asset.name;
                    if(!Directory.Exists(modelPath))
                       Directory.CreateDirectory(modelPath);
                    AssetDatabase.Refresh();
                }
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("删除")))
                {
                    string delPath=SettingsAsset.GlobalAssetsPath+"/"+MenuTree.Selection[0].Name+".asset";
                    if (File.Exists(delPath))
                    {
                        if (MenuTree.Selection[0].Name == GetApply())
                            File.Delete(current);
                        File.Delete(delPath);
                        File.Delete(delPath+".meta");
                        // todo:提示 是否删除组件   Directory.Delete(oldModelPath);
 
                    }
                    AssetDatabase.Refresh();
                }
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("应用")))
                {
                    if (mCurrent != null)
                        mCurrent.Icon = null;
                    SetApply(MenuTree.Selection[0].Name);
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
        private void SetApply(string name)
        {
            string current = SettingsAsset.GlobalAssetsPath + "/current.config";
            File.WriteAllLines(current, new string[] { name });
        }
        private string GetApply()
        {
            string current = SettingsAsset.GlobalAssetsPath + "/current.config";
            if (!File.Exists(current)) return null;
            return File.ReadAllLines(current)[0];
        }
    }
}
