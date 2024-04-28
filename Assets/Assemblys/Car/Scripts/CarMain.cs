using DM.IFS;
using CarTest;
using UnityEngine;
using ToolsLibrary;
using DM.Entity;
using System;

public class CarMain : ScriptManager, IControl
{
    public DMOperationItem[] myData;

    public override void EditorModeInitialized()
    {
        base.EditorModeInitialized();
        sender.DebugMode = true;
    }
    public void Awake()
    {
        Properties = new DynamicProperty[4] {
                new InputFloatProperty("相机偏移量X",0),
                new InputFloatProperty("相机偏移量Y",8),
                new InputFloatProperty("相机偏移量Z",-8),
                new InputFloatProperty("缓冲速度",5) };
    }
    public override void RunModeInitialized(bool isMain, SceneInfo info)
    {
        sender.LogError("初始化小车，进入运行模式");
        base.RunModeInitialized(isMain, info);

        CarController car = gameObject.AddComponent<CarController>();
        car.Init();

        myData = GetComponentsInChildren<DMOperationItem>();
        for (int i = 0; i < myData.Length; i++)
        {
            sender.LogError(myData[i].Id.ToString());
        }
    }

    public override void PropertiesChanged(DynamicProperty[] pros)
    {
        base.PropertiesChanged(pros);

        for (int i = 0; i < Properties.Length; i++)
        {
            Properties[i] = pros[i];
        }
    }

    public void Active(DevType type, bool playback)
    {
    }

    public void DeActive(DevType type, bool playback)
    {
    }

    public void RecMessage(SendType type, GameObject senderObj, int operCurrentStatus, string param)
    {
        Debug.LogError("收到上下车指令，" + MyDataInfo.isHost);
        sender.Log(string.Format("组件[{0}]收到一条[{1}]类型的消息，发送者名称为[{2}]，事件类型ID是[{3}]，携带了参数：{4}",
            gameObject.name,
            type.ToString(),
            senderObj.ToString(),
            operCurrentStatus,
            param == null ? "" : param));

        string[] datas = param.Split(',');

        string targetId = "";
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(datas[0], allBObjects[i].BObject.Id))
            {
                targetId = allBObjects[i].BObject.Id;
                break;
            }
        }

        for (int i = 0; i < myData.Length; i++)
        {
            if (myData[i].Id == int.Parse(datas[1]))
            {
                //找到了指定操作项，修改操作项的状态，，这里可以根据不同操作项ID决定 操作项数据的改变情况
                for (int j = 0; j < myData[i].OperItems.Length; j++)
                {
                    if (myData[i].OperItems[j].Status == operCurrentStatus)
                    {
                        myData[i].OperStatus = myData[i].OperItems[j].NewStatus;
                        sender.Log("车辆收到了指令，并把自己的状态设为" + myData[i].OperStatus);
                        break;
                    }
                }
                break;
            }
        }


        //这里 自己是被操作项，比如就是汽车本身
        sender.LogError("操作项自己的ID：" + BObjectId);
        sender.LogError("取到操作对象ID：" + targetId);
        //发送给操作对象targetId,当前的指令状态（上车或下车），与哪个对象交互
        if (MyDataInfo.isHost)
        {
            sender.RunSend(SendType.MainToAll, targetId, operCurrentStatus, BObjectId);
            Debug.LogError("主机收到上下车指令，告知所有");
        }
    }
}