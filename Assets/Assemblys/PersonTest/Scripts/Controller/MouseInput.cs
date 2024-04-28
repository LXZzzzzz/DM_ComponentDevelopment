using System;
using DM.IFS;
using TestBuild;
using ToolsLibrary.FrameSync;
using UnityEngine;


public class MouseInput : DMonoBehaviour
{
    private void Update()
    {
        MouseInfo mi = new MouseInfo()
        {
            mousePos = Input.mousePosition,
            isClick = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)
        };

        string data = Montage(mi);
#if !UNITY_EDITOR
        sender.RunSend(SendType.SubToMain, main.BObjectId, (int)MessageID.C2S_MouseInput, data);
#endif
    }

    public string Montage(MouseInfo data)
    {
        return String.Format($"{data.mousePos.x}_{data.mousePos.y}_{data.isClick}");
    }

    public MouseInfo Split(string dataStr)
    {
        var strs = dataStr.Split("_");
        MouseInfo iid = new MouseInfo()
        {
            mousePos = new Vector2(float.Parse(strs[0]), float.Parse(strs[1])), isClick = Boolean.Parse(strs[2])
        };
        return iid;
    }
}

public struct MouseInfo
{
    public Vector2 mousePos;
    public bool isClick;
}