using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolsLibrary.FrameSync
{
    public abstract class FrameSyncLogicBase
    {
        //初始化的时候把主角的sender传过来
        public SendManager sender;
        //一秒同步几帧
        protected int SynFrameRate = 10;
        protected int currentFrame;
        public abstract void OnUpdate();
        //接收其他组件传来的操作数据
        public abstract void OnRecordOperation(SyncDataBase recoed);
    }

    public enum MessageID
    {
        C2S_MouseInput = 900,//鼠标输入的记录
        S2C_MouseInput = 901,//客户端收到主机通知某个玩家鼠标移动了
        S2C_GameStart = 1000,//服务端告知客户端游戏可以开始了
        C2S_InitClient,//客户端告知服务器初始化自己
    }
}
