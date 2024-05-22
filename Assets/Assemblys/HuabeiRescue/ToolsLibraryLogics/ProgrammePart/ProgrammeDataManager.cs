using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System.Windows.Forms;  //Editor\Data\MonoBleedingEdge\lib\mono\2.0-api
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace ToolsLibrary.ProgrammePart_Logic
{
    public class ProgrammeDataManager : MonoSingleTon<ProgrammeDataManager>
    {
        private ProgrammeData currentData;
        private int serialNumberId;

        public void CreatProgramme(string name)
        {
            currentData = new ProgrammeData();
            currentData.programmeName = name;
            currentData.AllEquipDatas = new List<AEquipData>();
            currentData.CommanderControlList = new Dictionary<string, List<string>>();
            serialNumberId = 0;
        }

        public ProgrammeData LoadProgramme()
        {
            if (FileOperator.LoadData(out ProgrammeData data))
            {
                currentData = data;
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
                FileOperator.SaveData(currentData);
        }

        public string AddEquip(string templateId, Vector3 initPos)
        {
            JsonVector3 itemPos = new JsonVector3() { x = initPos.x, y = initPos.y, z = initPos.z };
            AEquipData itemData = new AEquipData()
                { templateId = templateId, pos = itemPos, myId = templateId + serialNumberId++ };
            currentData.AllEquipDatas.Add(itemData);
            return itemData.myId;
        }

        public AEquipData GetEquipDataById(string targetId)
        {
            AEquipData itemData = currentData.AllEquipDatas.Find(x => string.Equals(x.myId, targetId));

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

        public static void SaveData<T>(T data)
        {
            if (string.IsNullOrEmpty(lastPath))
                lastPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            SaveData(data, lastPath);
        }

        public static bool LoadData<T>(out T outData)
        {
            if (string.IsNullOrEmpty(lastPath))
                lastPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            outData = LoadData<T>(lastPath);

            return outData != null;
        }

        private static void SaveData<T>(T data, string folderPath)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                DefaultExt = "json",
                AddExtension = true,
                FileName = "NewFile", // 可以提供一个默认文件名
                InitialDirectory = folderPath
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                lastPath = filePath;
                string jsonData = JsonConvert.SerializeObject(data);
                File.WriteAllText(filePath, jsonData);
            }
        }

        private static T LoadData<T>(string folderPath)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                InitialDirectory = folderPath
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
    }
}