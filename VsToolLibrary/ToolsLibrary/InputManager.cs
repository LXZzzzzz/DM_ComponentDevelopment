using DM.IFS;
using UnityEngine;
using UnityEngine.Events;
using Lockstep.Math;

namespace ToolsLibrary
{
    public static class InputManager
    {
        public static Vector3 previousMousePos;
        private static Vector3 nowMousePos;
        public static UnityAction<Vector2, bool> cursorControl;
        private static InputKeyCodeData ikcd;
        public static LFloat GetAxis(string instruct)
        {
            if (ikcd == null) return 0;
            LFloat currentValue = 0;
            switch (instruct)
            {
                case "Horizontal":
                    currentValue = ikcd.Horizontal;
                    break;

                case "Vertical":
                    currentValue = -ikcd.Vertical;
                    break;

                case "Rotate":
                    if (Input.GetKey(KeyCode.Q))
                    {
                        currentValue = -1;
                    }
                    else if (Input.GetKey(KeyCode.E))
                    {
                        currentValue = 1;
                    }

                    break;

                case "Jump":
                    if (Input.GetKey(KeyCode.Space))
                    {
                        currentValue = 1;
                    }

                    break;

                case "Mouse X":
                    //判断当前鼠标位置和上一帧位置的偏差
                    if (previousMousePos == Vector3.zero)
                        previousMousePos = Input.mousePosition;
                    currentValue = (LFloat)Input.mousePosition.x - (LFloat)previousMousePos.x;

                    //记录鼠标这一帧的位置信息
                    previousMousePos.x = Input.mousePosition.x;
                    break;
                case "Mouse Y":
                    if (previousMousePos == Vector3.zero)
                        previousMousePos = Input.mousePosition;
                    currentValue = (LFloat)Input.mousePosition.y - (LFloat)previousMousePos.y;

                    //记录鼠标这一帧的位置信息
                    previousMousePos.y = Input.mousePosition.y;
                    break;
                default:
                    currentValue = 0;
                    break;
            }
            //为什么会这样
            return currentValue;
        }

        public static void SystemKeyInput(string parm)
        {
            //系统输入过来后的操作，类似于把input.get... 在这里检测了
            //这样在getAxis中不用get了，直接获取这里存起来的键入值，去修改值
            if (ikcd == null) ikcd = new InputKeyCodeData();
            DMKeyCode keyCode = new DMKeyCode(parm);
            switch (keyCode.Key)
            {
                case "WSAxis":
                    ikcd.Vertical = (LFloat)keyCode.Value;
                    break;
                case "ADAxis":
                    ikcd.Horizontal = (LFloat)keyCode.Value;
                    break;
                default:
                    break;
            }
        }
    }

    class InputKeyCodeData
    {
        public LFloat Horizontal;
        public LFloat Vertical;
    }
}
