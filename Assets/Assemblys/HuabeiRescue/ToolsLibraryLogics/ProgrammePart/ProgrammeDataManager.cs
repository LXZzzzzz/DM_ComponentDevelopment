using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using ToolsLibrary;
using UnityEngine;

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
}