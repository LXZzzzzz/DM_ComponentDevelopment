using UnityEngine;

namespace ToolsLibrary
{
    public interface IDrive
    {
        //进入驾驶舱
        void OnEnter(GameObject player,bool isMe);
        //实时控制
        void OnDrive(float ver,float hor,float stop);
        //退出驾驶
        void OnExit();
    }
}
