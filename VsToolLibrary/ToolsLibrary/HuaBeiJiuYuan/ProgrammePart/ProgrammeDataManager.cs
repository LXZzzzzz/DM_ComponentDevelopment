using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System.Windows.Forms;
using ToolsLibrary.EquipPart;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace ToolsLibrary.ProgrammePart
{
    public class ProgrammeDataManager : MonoSingleTon<ProgrammeDataManager>
    {
        private ProgrammeData currentData;
        public ProgrammeData GetCurrentData => currentData;
        private int equipIdNum;

        public void CreatProgramme(string name)
        {
            currentData = new ProgrammeData();
            currentData.programmeName = name;
            currentData.AllEquipDatas = new List<AEquipData>();
            currentData.AllZiYuanDatas = new List<AZiYuanData>();
            equipIdNum = 0;
        }

        public ProgrammeData LoadProgramme(string path)
        {
            ProgrammeData data;
            Debug.LogError($"传来的路径：{path}");
            if (FileOperator.LoadData(path, out data))
            {
                currentData = data;
                equipIdNum = currentData.AllEquipDatas.Count;
                return currentData;
            }
            else
            {
                return null;
            }
        }

        public void SaveProgramme()
        {
            if (currentData != null)
                FileOperator.SaveAsData(currentData);
        }

        public void SaveProgramme(string path)
        {
            if (currentData != null)
                FileOperator.SaveData(currentData, path);
        }

        public void ChangeEquipData(AEquipData edata)
        {
            var itemData = currentData.AllEquipDatas.Find(x => string.Equals(x.myId, edata.myId));
            if (itemData != null) currentData.AllEquipDatas.Remove(itemData);
            currentData.AllEquipDatas.Add(edata);
        }

        public AEquipData GetEquipDataById(string targetId)
        {
            AEquipData itemData = currentData.AllEquipDatas.Find(x => string.Equals(x.myId, targetId));

            return itemData;
        }

        public void ChangeZiyuanData(AZiYuanData zdata)
        {
            var itemData = currentData.AllZiYuanDatas.Find(x => string.Equals(x.myId, zdata.myId));
            if (itemData != null) currentData.AllZiYuanDatas.Remove(itemData);
            currentData.AllZiYuanDatas.Add(zdata);
        }
        
        public AZiYuanData GetZiyuanDataById(string targetId)
        {
            AZiYuanData itemData = currentData.AllZiYuanDatas.Find(x => string.Equals(x.myId, targetId));

            return itemData;
        }

        public string PackedData()
        {
            //把数据组装成字符串
            string jsonData = JsonConvert.SerializeObject(currentData);
            return AESUtils.Encrypt(jsonData);
        }

        public ProgrammeData UnPackingData(string dataStr)
        {
            //把字符串解析为数据
            string deStr = AESUtils.Decrypt(dataStr);
            currentData = JsonConvert.DeserializeObject<ProgrammeData>(deStr);
            return currentData;
        }
    }


    public class FileOperator
    {
        private static string lastPath;

        public static void SaveAsData<T>(T data)
        {
            if (string.IsNullOrEmpty(lastPath))
                lastPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            SaveDataFunc(data, lastPath);
        }

        public static void SaveData<T>(T data, string path)
        {
            int fileCount = Directory.GetFiles(path).Length;

            string jsonData = JsonConvert.SerializeObject(data);
            File.WriteAllText(path + $"/方案{fileCount + 1}.json", jsonData);
            lastPath = path;
        }
        
        public static void SaveAsData_Txt(string data,string path)
        {
            if (string.IsNullOrEmpty(path))
                lastPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            else lastPath = path;

            SaveDataFunc_Txt(data, lastPath);
        }

        public static bool LoadData<T>(string path, out T outData)
        {
            outData = LoadData<T>(path);

            return outData != null;
        }

        private static void SaveDataFunc<T>(T data, string folderPath)
        {
            string openPath = folderPath.Replace('/', '\\');
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                DefaultExt = "json",
                AddExtension = true,
                FileName = "NewFile", // 可以提供一个默认文件名
                InitialDirectory = openPath
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                lastPath = filePath;
                string jsonData = JsonConvert.SerializeObject(data);
                File.WriteAllText(filePath, jsonData);
            }
        }
        private static void SaveDataFunc_Txt(string data, string folderPath)
        {
            string openPath = folderPath.Replace('/', '\\');
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Txt Files (*.txt)|*.txt",
                DefaultExt = "txt",
                AddExtension = true,
                FileName = "NewFile", // 可以提供一个默认文件名
                InitialDirectory = openPath
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                lastPath = filePath;
                File.WriteAllText(filePath, data);
            }
        }

        private static T LoadData<T>(string folderPath)
        {
            string openPath = folderPath.Replace('/', '\\');

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                InitialDirectory = openPath, RestoreDirectory = true, FilterIndex = 2
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                lastPath = filePath;
                string jsonData = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(jsonData);
            }

            return default(T);
        }

        public static string LoadData_Txt(string folderPath)
        {
            string openPath = folderPath.Replace('/', '\\');

            Debug.LogError($"打开的路径：{openPath}");

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Txt Files (*.txt)|*.txt",
                InitialDirectory = openPath, RestoreDirectory = true, FilterIndex = 2
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                lastPath = filePath;
                string data = File.ReadAllText(filePath);
                return data;
            }
            
            Debug.LogError($"检查路径：{openFileDialog.InitialDirectory}");

            return default;
        }
    }
}