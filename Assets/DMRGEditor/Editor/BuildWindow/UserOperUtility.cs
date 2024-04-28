using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEditor.EditorTools.TLM.ThumbnailGenerator;

namespace DM.RGEditor
{
    public class UserOperUtility
    {
        [MenuItem("Assets/DMRGEditor/添加选中场景 &1", false, 1)]
        private static void AddSelectSceneToRGTerrain()
        {
            RGTerrainEditorWindow window =EditorWindow.GetWindow<RGTerrainEditorWindow>();
            if (window != null)
            {
                window.Show();
                string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                window.AddSceneFromMenu(assetPath);
            }          
        }

        [MenuItem("Assets/DMRGEditor/添加当前场景 &2", false, 2)]
        private static void AddCurrentSceneToRGTerrain()
        {
            RGTerrainEditorWindow window = EditorWindow.GetWindow<RGTerrainEditorWindow>();
            if (window != null)
            {
                window.Show();
                //string path = EditorSceneManager.GetActiveScene().path;
                //string abPath=Application.dataPath.Substring(0, Application.dataPath.Length - 6) + path;
                window.AddSceneFromMenu(EditorSceneManager.GetActiveScene().path);
            } 
        }

        [MenuItem("Assets/DMRGEditor/添加选中模型 &3", false, 11)]
        private static void AddSelectModelToRGModel()
        {
            RGModelEditorWindow window = EditorWindow.GetWindow<RGModelEditorWindow>();
            if (window != null)
            {
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    string prefabName = Selection.objects[i].name;
                    ModelAssetTemplate modelAsset = window.AddModelInstance(prefabName);
                    modelAsset.Prefab = Selection.objects[i] as GameObject;
                    string assetPath=AssetDatabase.GetAssetPath(Selection.objects[i]);
                    string abPath=Application.dataPath.Substring(0, Application.dataPath.Length - 6) + assetPath;
                    modelAsset.Icon = GetSpriteOnPrefab(abPath);
                }
            }
        }
        [MenuItem("Assets/DMRGEditor/添加全部模型 &4", false, 12)]
        private static void AddSelectFolderToRGModel()
        {
            RGModelEditorWindow window = EditorWindow.GetWindow<RGModelEditorWindow>();
            if (window != null)
            {
                string[] prefabs = Directory.GetFiles(GetSelectionPath(), "*.prefab", SearchOption.AllDirectories);
                for (int i = 0; i < prefabs.Length; i++)
                {
                    string prefabName=Path.GetFileNameWithoutExtension(prefabs[i]);
                    ModelAssetTemplate modelAsset=window.AddModelInstance(prefabName);
                    modelAsset.Prefab=AssetDatabase.LoadAssetAtPath<GameObject>(GetAssetPath(prefabs[i]));
                    modelAsset.Icon = GetSpriteOnPrefab(prefabs[i]);  
                }
            }                
        }

        #region 验证
        [MenuItem("Assets/DMRGEditor/添加选中场景 &1", true, 1)]
        private static bool ValidatateAddSelectSceneToRGTerrain()
        {
            FileInfo file = new FileInfo(GetSelectionPath());
            if (file.Extension == ".unity")
                return true;
            else return false;
        }
        [MenuItem("Assets/DMRGEditor/添加当前场景 &2", true, 2)]
        private static bool ValidatateAddCurrentSceneToRGTerrain()
        {
            string path = EditorSceneManager.GetActiveScene().path;
            return !string.IsNullOrEmpty(path);
        }     
        [MenuItem("Assets/DMRGEditor/添加选中模型 &3", true, 11)]
        private static bool ValidatateAddSelectModelToRGModel()
        {
           return Selection.activeGameObject != null;
        }
        [MenuItem("Assets/DMRGEditor/添加全部模型 &4", true, 12)]
        private static bool ValidatateAddSelectFolderToRGModel()
        { 
            return Directory.Exists(GetSelectionPath());
        }
        #endregion

        #region private Method
        private static string GetSelectionPath()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            return Application.dataPath.Substring(0, Application.dataPath.Length - 6) + path;
        }
        public static string GetAssetPath(string abPath)
        {
            string[] strs = abPath.Split("Assets");
            if (strs.Length != 2)
            {
                Debug.LogError("GetAssetPath路径错误");
                return null;
            }
            return "Assets" + strs[1];
        }
        private static string GetSpritePathOnPrefab(string abPath)
        {
            string prefabName = Path.GetFileNameWithoutExtension(abPath);
            string[] spritePaths = Directory.GetFiles(Application.dataPath,"*.*", SearchOption.AllDirectories)
                                   .Where(s => s.EndsWith(".jpg") || s.EndsWith(".jpeg") || s.EndsWith(".png")).ToArray();
            for (int i = 0; i < spritePaths.Length; i++)
            {
                string spriteName = Path.GetFileNameWithoutExtension(spritePaths[i]);
                if (spriteName.Contains(prefabName))
                    return spritePaths[i];
            }
            return null;
        }
        //预制体同名的Sprite存在，则获取；若不存在，则获取预览图片
        private static Sprite GetSpriteOnPrefab(string abPath)
        {
            string spritePath = GetSpritePathOnPrefab(abPath);
            if (!string.IsNullOrEmpty(spritePath))
            {
               return AssetDatabase.LoadAssetAtPath<Sprite>(GetAssetPath(spritePath));
            }
            else
            {
                GameObject gameObject=AssetDatabase.LoadAssetAtPath<GameObject>(GetAssetPath(abPath));
                //Sprite没有精灵图集，代码修改没有Apply,加入图集？？？
                //预览图没有透明，放弃该方法，采用ThumbnailGenerator
                //Texture2D tex=AssetPreview.GetAssetPreview(gameObject) as Texture2D;
                //byte[] bytes = tex.EncodeToPNG();
                //string savePath= abPath.Replace(".prefab", ".png");
                //File.WriteAllBytes(savePath, bytes);
                //SettingsAsset.PreViewPicture = GetAssetPath(savePath);
                //AssetDatabase.Refresh();
                //SettingsAsset.PreViewPicture = null;
                //return AssetDatabase.LoadAssetAtPath<Sprite>(GetAssetPath(savePath));

                string assetPath=ThumbnailGenerator.QuickGeneratePrefabThumbnail(gameObject);
                return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
            }
        }
        #endregion
    }
}
