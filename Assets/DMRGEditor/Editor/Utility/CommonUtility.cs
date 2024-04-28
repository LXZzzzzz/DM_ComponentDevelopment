using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DM.RGEditor
{
    public class CommonUtility
    {
        /// <summary>
        /// 复制文件夹及文件
        /// </summary>
        /// <param name="sourceFolder">原文件路径</param>
        /// <param name="destFolder">目标文件路径</param>
        /// <returns></returns>
        public static int CopyFolder(string sourceFolder, string destFolder)
        {
            try
            {
                //如果目标路径不存在,则创建目标路径
                if (!Directory.Exists(destFolder))
                {
                    Directory.CreateDirectory(destFolder);
                }
                //得到原文件根目录下的所有文件
                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(destFolder, name);
                    File.Copy(file, dest,true);//复制文件
                }
                //得到原文件根目录下的所有文件夹
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);//构建目标路径,递归复制文件
                }
                return 1;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return -1;
            }
        }


        public static string GetPreviewTex(string astPath)
        {
            var guid = AssetDatabase.AssetPathToGUID(astPath);
            string rootPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            string source = rootPath+"/Library/metadata/" + guid.Substring(0, 2) + "/" + guid + ".info";
            Debug.LogError("source:"+source);
            string dest = rootPath+astPath.Replace(".prefab",".png");
            Debug.LogError("dest:" + dest);
            File.Copy(source, dest);
            return dest;
        }
    }
}
