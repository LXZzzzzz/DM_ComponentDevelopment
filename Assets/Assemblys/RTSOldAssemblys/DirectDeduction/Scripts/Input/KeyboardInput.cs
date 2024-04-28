/*********************************************
 * Author:fchfghy
 * Create:2019.04.23
 * Modify:
 * Func:键盘输入
 * *******************************************/

using System;
using UnityEngine;
using DM.IFS;

namespace DefaultRole
{
    /// <summary>
    /// 自定义键盘输入
    /// </summary>
    public class KeyboardInput : DMonoBehaviour
    {
        private void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            sender.RunSend(SendType.SubToMain,main.BObjectId,1,horizontal+","+vertical);
        }
    }
}

