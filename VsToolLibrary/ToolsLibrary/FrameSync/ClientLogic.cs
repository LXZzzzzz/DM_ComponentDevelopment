using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ToolsLibrary.FrameSync
{
    public class ClientLogic : FrameSyncLogicBase
    {
        private List<SyncDataBase> timeDatas;
        private float timer;
        private bool isRun;
        public ClientLogic()
        {
            timeDatas = new List<SyncDataBase>();
            sender.RunSend(DM.IFS.SendType.SubToMain, MyDataInfo.leadId, (int)MessageID.C2S_InitClient, "");
            currentFrame = 0;
            timer = 0;
            isRun = false;
        }
        public override void OnUpdate()
        {
            if (!isRun) return;
            if (Time.time > timer)
            {
                //走过了一帧
                timer += 1 / SynFrameRate;
                SendTimeDatas();
                timeDatas.Clear();
            }
        }
        private void SendTimeDatas()
        {
            //把当前帧时间内的数据发给服务器

        }
        //接收操作指令
        public override void OnRecordOperation(SyncDataBase recoed)
        {
            if (!isRun) return;
            timeDatas.Add(recoed);
        }
        //处理操作指令
        public void ProcessingOperations(List<SyncDataBase> datas)
        {
            //对服务端传过来的数据进行拆解，响应
            //从数据中解析出来操作的对象
            //找到指定对象，调用接口，传过去数据
        }
    }
}
