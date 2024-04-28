using UnityEngine;
using UnityEditor;
using System.IO;

namespace DM.RGEditor
{
    public static class ScriptableObjectUtility
    {
        /// <summary>
        /// 生成配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        /// <summary>
        /// 生成.asset配置文件
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="outputPath">输出路径(例如：Assets/SerializeScriptParameter/AutoSo/)</param>
        /// <returns></returns>
        public static ScriptableObject CreateAsset(string className, string outputPath)
        {
            ScriptableObject asset = ScriptableObject.CreateInstance(className);

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(outputPath + "/" + className + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }
    }
}
