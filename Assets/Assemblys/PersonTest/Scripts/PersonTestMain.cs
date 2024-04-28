using System;
using DM.Entity;
using DM.IFS;
using System.Collections.Generic;
using UnityEngine;
using ToolsLibrary;
using System.Collections;
using TestBuild;
using ToolsLibrary.FrameSync;

public class PersonTestMain : ScriptManager, IControl, IContainer, IMesRec
{
    [HideInInspector]
    public bool isLead;//是否是以自己为主角运行的
    private bool isMain;//当前是不是主机
    private PlayerController player;
    //编辑模式下两种初始化的方法
    #region 通过重写父类的初始化方法实现
    public override void EditorModeInitialized()
    {
        base.EditorModeInitialized();
        sender.DebugMode = true;
        sender.LogError("通过重写父类的初始化方法实现--编辑模式初始化");
        Debug.LogError("通过unity的Log打印错误信息");
        sender.Log("通过组件的Log打印普通信息");
        Debug.Log("通过unity的Log打印普通信息");
        EventEnums = new DM.Entity.EnumDescription[2]
        {
                new DM.Entity.EnumDescription(1,"运行模式事件1"),
                new DM.Entity.EnumDescription(2,"运行模式事件2")
        };
        sender.Log("打印初始化的参数值：" + Properties[2]);
        sender.Log("打印初始化的参数值Name：" + Properties[2].Name);
        sender.Log("打印初始化的参数值Description：" + Properties[2].Description);
    }
    #endregion
    #region 通过为自定义方法添加特性来实现
    //特性中的参数代表组件初始化的顺序，不参数为最先初始化
    [EditorModeInitialized]
    public void EditorInitial1()
    {

    }
    [EditorModeInitialized(2)]
    public void EditorInitial2()
    {

    }
    #endregion

    //运行模式下的两种初始化方法
    #region 通过重写父类的初始化方法来实现
    private GameObject cameraObject;
    public override void RunModeInitialized(bool isMain, SceneInfo info)
    {
        base.RunModeInitialized(isMain, info);
        CurStatus = 1;
        isLead = false;

        this.isMain = isMain;
        Debug.LogError(gameObject.name + gameObject.transform.position.ToString());

        // sender.LogError("运行模式下打印初始化的参数值：" + Properties[2]);

        player = gameObject.AddComponent<PlayerController>();
        player.Init(this);
    }
    #endregion
    #region 通过给自定义函数添加特性来实现
    [RunModeInitialized]
    public void RunInit1()
    {
        sender.Log("通过特性的初始化方法实现--运行模式初始化");
        //运行模式下的初始化
    }
    [RunModeInitialized(2)]
    public void RunInit2()
    {
        //运行模式下的初始化
    }
    #endregion

    //这个是初始化界面的显示 ，这个Awake等同于unity中的，在编辑模式和运行模式都会执行
    public void Awake()
    {
        Debug.LogError("打印Sender是否为空"+sender);
#if !UNITY_EDITOR
        sender.LogError("测试一下Awake中的sender会不会报错");
#endif
        List<EnumDescription> list = new List<EnumDescription>();
        list.Add(new EnumDescription(1, "Item1"));
        list.Add(new EnumDescription(2, "Item2"));
        list.Add(new EnumDescription(3, "Item3"));
        Properties = new DynamicProperty[13] {
                new InputIntProperty("血量",100),
                new IntSliderProperty("强度",5,2,10),
                new InputFloatProperty("移动速度",10),
                new InputFloatProperty("相机偏移量X",0),
                new InputFloatProperty("相机偏移量Y",8),
                new InputFloatProperty("相机偏移量Z",-8),
                new InputFloatProperty("缓冲时间",5),
                new InputFloatProperty("旋转速度",30),
                new InputStringProperty("InputString","Test"),
                new LabelProperty("Label"),
                new ToggleProperty("Toggle",true),
                new SliderProperty("SliderFloat",45.3f,12.5f,78.3f),
                new DropDownProperty("DropDown",list,1)};
        Debug.LogError("Aweak走完了，看看数据"+Properties[2]);
    }

    //这个是在编辑器里修改了动态属性，点击保存产生的回调
    public override void PropertiesChanged(DynamicProperty[] pros)
    {
        base.PropertiesChanged(pros);
        //外部修改信息传回，修改属性后的逻辑处理

        sender.Log("外部修改了自身属性" + pros.Length);
        for (int i = 0; i < Properties.Length; i++)
        {
            Properties[i].Description = pros[i].Description;
        }
    }

    //逻辑状态
    public override void CurStatusChanged(int status)
    {
        base.CurStatusChanged(status);
        //ToDo:StatusEnums字段记录了当前组件有哪些状态
    }

    //姿态状态
    public override void AnimStatusChanged(int anim)
    {
        base.AnimStatusChanged(anim);
        if (anim == 1)
        {
            sender.Log("动画切换为状态1");
        }
    }

    //皮肤状态
    public override void SkinStatusChanged(int skin)
    {
        base.SkinStatusChanged(skin);
        if (skin == 1)
        {
            sender.Log("组件切换为皮肤1");
        }
    }

    //调试模式
    //DebugMode设为true

    //其余特性：
    //ToDo:这个组件的路径就是在unity中添加的组件的预制体的路径； 一般不使用这种方式去添加脚本，都是在逻辑中使用AddComponent方式添加，可控性更好
    //AddScriptInEditorMode:编辑器模式下，为组件子物体添加脚本  
    //AddScriptInRunMode:运行模式下，为组件子物体添加脚本
    //参数childPath：子物体路径； Index：添加顺序
    [AddScriptInEditorMode("子物体路径", 0)]
    [AddScriptInRunMode("子物体路径", 1)]
    [AttachedModel]//ToDo: 这个特性当前用不到
    public class ClassTest : MonoBehaviour { }
    //CatchError: 捕获错误异常  ToDo:类似于try catch 
    [CatchError]
    private void Start()
    {
        int i = 1;
#if UNITY_EDITOR
        Active( DevType.PC,false);
#endif
    }

    private void OnDestroy()
    {
        // sender.LogError("删除了自己");
        if (ThirdPersonCamera.Instance != null)
            Destroy(ThirdPersonCamera.Instance.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            sender.LogError(gameObject.name+"是我的角色吗"+isLead);
        }
    }

    //角色接口
    //ToDo：这个是以此角色运行时会产生回调，第二个参数是是否以回放模式运行
    public void Active(DevType type, bool playback)
    {
        //以此角色运行时，会回调
        Debug.LogError("进入运行模式,是主机吗：" + isMain + "是复盘吗" + playback);
        isLead = true;
        MyDataInfo.isHost = isMain;
        MyDataInfo.leadId = main.BObjectId;
        MyDataInfo.isPlayBack = playback;
        if (!playback)
        {
            gameObject.AddComponent<MouseInput>();
            Debug.LogError("非回放模式，打开鼠标记录");
        }

        StartCoroutine(showUI());
    }
    IEnumerator showUI()
    {
        yield return 1;

        cameraObject = new GameObject("Main Camera");
        cameraObject.transform.parent = transform.parent;
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = transform.position;
        cameraObject.AddComponent<Camera>();
        ThirdPersonCamera tpc = cameraObject.AddComponent<ThirdPersonCamera>();
        Vector3 offset = new Vector3((Properties[3] as InputFloatProperty).Value, (Properties[4] as InputFloatProperty).Value, (Properties[5] as InputFloatProperty).Value);
        tpc.Init(transform, offset, (Properties[6] as InputFloatProperty).Value, false);

        yield return 1;
        EventManager.Instance.EventTrigger(ToolsLibrary.EventType.ShowUI, "IconShow");
        if (MyDataInfo.isPlayBack)
            EventManager.Instance.EventTrigger(ToolsLibrary.EventType.ShowUI, "CursorShow");
    }
    public void DeActive(DevType type, bool playback)
    {
        //这里不会执行，只有自己去调用
        sender.LogError("退出运行");
        Destroy(GetComponent<PlayerController>());
        Destroy(cameraObject);
    }

    //容器接口
    //ToDo：选择内视图和外视图会产生回调
    public void ExternalView()
    {
        //切换到外视图调用（例如：将双层容器中的上层显示出来）
    }
    public void InternalView()
    {
        //...隐藏起来
    }


    //通信接口
    //ToDo:sender.Send()是网络通信相关的  。。。。网络通信模块是在组件中实现的，当前一部分逻辑也被封装在了类库中
    //发送消息
    //  sender来实现组件和物体，组件和组件之间的相互调用
    //接收消息
    //  组件如果需要接受消息，则需要实现IMessageRec接口（运行模式）或IEditorRec接口(编辑模式)

    public void RecMessage(SendType type, GameObject senderObj, int eventType, string param)
    {
        // sender.LogError(string.Format("组件[{0}]收到一条[{1}]类型的消息，发送者名称为[{2}]，事件类型ID是[{3}]，携带了参数：{4}",
        //     gameObject.name,
        //     type.ToString(),
        //     senderObj?.ToString(),
        //     eventType,
        //     param == null ? "" : param));

        if (type == SendType.SubToMain && eventType == -1)
        {
            sender.RunSend(SendType.MainToAll,senderObj.GetComponent<PersonTestMain>().BObjectId,-1,param);
        }

        if (type== SendType.MainToAll&&eventType==-1)
        {
            //如果收到了输入信息，并且我是操作者，说明这个输入是我发出的
            DMKeyCode keyCode = new DMKeyCode(param);
            sender.LogError("输入信息："+keyCode.Value);
            if(isLead) InputManager.SystemKeyInput(param);
        }

        switch (eventType)
        {
            case 11:
                Debug.LogError("收到上车指令");
                //上车，获取到载具
                for (int i = 0; i < allBObjects.Length; i++)
                {
                    if (string.Equals(param, allBObjects[i].BObject.Id))
                    {
                        sender.LogError("main找到了汽车组件" + allBObjects[i].gameObject.name);
                        player?.EnterVehicle(allBObjects[i].gameObject, string.Equals(BObjectId, MyDataInfo.leadId));
                        break;
                    }
                }
                break;
            case 12:
                player?.EnterVehicle(null, string.Equals(BObjectId, MyDataInfo.leadId));
                sender.LogError("收到下车指令");
                break;
            case 100:
                //角色上下左右控制
                if (MyDataInfo.isHost)
                    sender.RunSend(SendType.MainToAll, BObjectId, 101, param);
                break;
            case 101:
                player?.InputInfo(param);
                break;
            case (int)MessageID.C2S_InitClient:
                //只有主机上的角色才能收到；收到了该角色控制者的消息，需要将此角色保存为数据通信对象
                Debug.LogError($"是不是主机：{MyDataInfo.isHost},收到了客户端初始化的消息");
                break;
            case (int)MessageID.S2C_GameStart:
                //客户端收到服务端的初始化完成的消息

                break;
            case (int)MessageID.C2S_MouseInput:
                //非回放模式下,主机收到某个客户端发送的鼠标操作，分发下去
                if (MyDataInfo.isHost && !MyDataInfo.isPlayBack)
                    sender.RunSend(SendType.MainToAll, BObjectId, (int)MessageID.S2C_MouseInput, param);
                break;
            case (int)MessageID.S2C_MouseInput:
                if (MyDataInfo.isPlayBack && string.Equals(MyDataInfo.leadId, BObjectId))
                {
                    MouseInfo itemData = Split(param);
                    InputManager.cursorControl?.Invoke(itemData.mousePos, itemData.isClick);
                }
                break;
            default:
                break;
        }

    }
    public MouseInfo Split(string dataStr)
    {
        var strs = dataStr.Split("_");
        MouseInfo iid = new MouseInfo()
        {
            mousePos = new Vector2(float.Parse(strs[0]),float.Parse(strs[1])), isClick = Boolean.Parse(strs[2])
        };
        return iid;
    }

    //主机和从机同步方案：    其他所有客户端将指令发送给主机，主机操作后，将状态实时同步给所有从机（同步逻辑也可以更改）

    //全局接口： IGlobal 

    //MainSystem 跟平台的交互

    //Param工具的意义，也可以不用

    //生命周期：
    /*
        Awake 默认初始化状态
        RecoveryData 还原数据(transform,皮肤，状态，动画等) //ToDo:暂时无需关心
        EditorInitialize 编辑器初始化
        Start Unity Start函数
        RunInitialize 运行模式初始化
     */
}
