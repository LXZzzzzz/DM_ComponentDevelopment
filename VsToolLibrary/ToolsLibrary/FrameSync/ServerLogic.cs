using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ToolsLibrary.FrameSync
{
    public class ServerLogic : FrameSyncLogicBase
    {
        private int clientNums;
        private float timer;
        private List<ClientData> clientDatas;
        private Dictionary<string, SyncDataBase> ClientDataPerFrame;//每帧所有客户端数据
        public ServerLogic()
        {
            //服务端初始化
            clientDatas = new List<ClientData>();
            ClientDataPerFrame = new Dictionary<string, SyncDataBase>();
            //获取当前房间有多少人
            clientNums = 2;
            ////初始化所有客户端数据结构
            //for (int i = 0; i < clientNums; i++)
            //{
            //    //客户端初始化的过程中就给客户端发去开始的消息
            //    clientDatas.Add(new ClientData());
            //}

            timer = Time.time;
            currentFrame = 0;
            isRun = false;
            itemClientNums = 0;
        }

        bool isRun;
        int itemClientNums;
        //TODO:当前是因为获取不到房间内的客户端，暂时让客户端进入后主动告知服务器，之后可以换成构造函数中的逻辑
        public void ClientInit(string playerId)
        {
            clientDatas.Add(new ClientData() { clientPlayerId = playerId });
            itemClientNums++;
            if (itemClientNums >= clientNums)
            {
                //所有玩家都发来了，给所有玩家一个开始的消息
                for (int i = 0; i < clientDatas.Count; i++)
                {
                    sender.RunSend(DM.IFS.SendType.MainToAll, clientDatas[i].clientPlayerId, (int)MessageID.S2C_GameStart, MyDataInfo.leadId);
                }
            }
        }

        public override void OnUpdate()
        {
            if (!isRun) return;
            if (Time.time > timer && IsRunNextFrame())
            {
                //走过了一帧
                timer += 1 / SynFrameRate;
                ClientDataPerFrame.Clear();
            }
        }
        //是否进入下一帧
        private bool IsRunNextFrame()
        {
            if (ClientDataPerFrame.Count == clientNums)
            {
                //将当前帧中所有数据发给所有客户端

                currentFrame++;
                return true;
            }
            return false;
        }

        public override void OnRecordOperation(SyncDataBase recoed)
        {
            var itemData = recoed as PlayerControlData;
            ClientDataPerFrame[itemData.objectId] = itemData;
        }
    }
}


public struct ClientData
{
    public string clientPlayerId;
}