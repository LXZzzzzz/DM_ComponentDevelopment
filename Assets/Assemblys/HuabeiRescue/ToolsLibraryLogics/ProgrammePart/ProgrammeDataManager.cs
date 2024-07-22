using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using System.Windows.Forms; //Editor\Data\MonoBleedingEdge\lib\mono\2.0-api
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace ToolsLibrary.ProgrammePart_Logic
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
            currentData.CommanderControlList = new Dictionary<string, List<string>>();
            currentData.ZiYuanControlledList = new Dictionary<string, List<string>>();
            currentData.TaskControlledList = new Dictionary<string, List<string>>();
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

        public string AddEquip(string templateId, Vector3 initPos)
        {
            JsonVector3 itemPos = new JsonVector3() { x = initPos.x, y = initPos.y, z = initPos.z };
            AEquipData itemData = new AEquipData()
                { templateId = templateId, pos = itemPos, myId = templateId + (equipIdNum += 1) };
            currentData.AllEquipDatas.Add(itemData);
            return itemData.myId;
        }

        public bool DeleEquip(string equipId)
        {
            if (currentData != null && currentData.AllEquipDatas != null)
            {
                for (int i = 0; i < currentData.AllEquipDatas.Count; i++)
                {
                    if (string.Equals(equipId, currentData.AllEquipDatas[i].myId))
                    {
                        currentData.AllEquipDatas.RemoveAt(i);
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        public AEquipData GetEquipDataById(string targetId)
        {
            AEquipData itemData = currentData.AllEquipDatas.Find(x => string.Equals(x.myId, targetId));

            return itemData;
        }

        public bool ChangeZiYuanData(string ziYuanId, string commanderId, bool isAdd)
        {
            if (!currentData.ZiYuanControlledList.ContainsKey(ziYuanId))
                currentData.ZiYuanControlledList.Add(ziYuanId, new List<string>());
            if (isAdd)
            {
                if (currentData.ZiYuanControlledList[ziYuanId].Find(x => string.Equals(x, commanderId)) == null)
                    currentData.ZiYuanControlledList[ziYuanId].Add(commanderId);
                else return false;
            }
            else
            {
                int removeIndex = -1;
                for (int i = 0; i < currentData.ZiYuanControlledList[ziYuanId].Count; i++)
                {
                    if (string.Equals(commanderId, currentData.ZiYuanControlledList[ziYuanId][i]))
                    {
                        removeIndex = i;
                        break;
                    }
                }

                if (removeIndex != -1)
                    currentData.ZiYuanControlledList[ziYuanId].RemoveAt(removeIndex);
                else return false;
            }

            return true;
        }


        public List<string> GetZiYuanData(string ziYuanId)
        {
            if (currentData.ZiYuanControlledList.ContainsKey(ziYuanId))
                return currentData.ZiYuanControlledList[ziYuanId];
            else
                return null;
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

        public static bool LoadData<T>(string path, out T outData)
        {
            outData = LoadData<T>(path);

            return outData != null;
        }

        private static void SaveDataFunc<T>(T data, string folderPath)
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
            Debug.LogError($"打开的路径：{folderPath}");
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                InitialDirectory = folderPath, RestoreDirectory = true, FilterIndex = 2
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                lastPath = filePath;
                string jsonData = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(jsonData);
            }

            Debug.LogError($"检查路径：{openFileDialog.InitialDirectory}");

            return default(T);
        }
    }
}