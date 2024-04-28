using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefaultRole;
using DM.Core.Map;
using DM.Entity;
using DM.IFS;
using UnityEngine;
using 指挥端;

public class ZhiHuiDuanMain : ScriptManager, IControl, IMesRec
{
    public UIDirectDeduction uiDirectDeduction;

    public InputManager Input;   //输入
    public DataManager Data;

    public SchemeManager schemeMgr;
    public Scheme runScheme;

    public Camera m_Camera;
    public DMCameraViewMove cameraViewMove;
    public ThirdCameraControl thirdCameraControl;
    
    private SceneInfo info;
    private DMouseOrbit mouseOrbit;
    private GameObject mCamera;
    private bool loadFinished;

    public void Awake()
    {
        List<EnumDescription> mapData = new List<EnumDescription>() 
        {
            new EnumDescription(0, "中立"),
            new EnumDescription(1, "红方"),
            new EnumDescription(2, "蓝方"),
        };
        List<EnumDescription> groups = new List<EnumDescription>()
        {
            new EnumDescription(0, "按照分组列表"),
            new EnumDescription(1, "按照自定义类型"),
        };
        List<EnumDescription> showDirs = new List<EnumDescription>()
        {
            new EnumDescription(0, "不显示"),
            new EnumDescription(1, "显示选中组件"),
            new EnumDescription(2, "显示全部组件"),
        };
        List<EnumDescription> timeType = new List<EnumDescription>()
        {
            new EnumDescription(0, "系统时间"),
            new EnumDescription(1, "任务时间"),
        };
        List<EnumDescription> groupData = new List<EnumDescription>()
        {
            new EnumDescription(0, "中立"),
            new EnumDescription(1, "红方"),
            new EnumDescription(2, "蓝方"),
        };
        List<EnumDescription> groups2 = new List<EnumDescription>()
        {
            new EnumDescription(0, "按照类型分组"),
            new EnumDescription(1, "按照场景分组"),
        };

        List<DynamicProperty> dProperties = new List<DynamicProperty>()
        {
            //全局属性[0]
            new LabelProperty("全局属性—————————————————————————————————————————————————"),
            new InputStringProperty("任务名", "测试任务"),
            new ToggleProperty("显示全局环境", false),
            new DropDownSceneSelectProperty("关联环境组件"),
            new OpenFileDialogProperty("评估报告路径","路径"),
            new ToggleProperty("启用经济系统", false),
            //小地图[6]
            new LabelProperty("小地图—————————————————————————————————————————————————"),
            new DropDownProperty("数据源",mapData, 0, true),
            new DropDownProperty("分组依据",groups, 0, true),
            new InputStringProperty("分组列表配置", "救援力量,Red,救援力量;蓝方,Blue,遇险事件;"),
            new InputStringProperty("自定义类型配置", "8,1;8,2;"),
            new InputStringProperty("组件图标设置", "8,1;8,2;"),
            new InputStringProperty("通信消息配置", "Add,Color,Yellow; Add,Icon,无;"),
            new ToggleProperty("显示组件名称", false),
            new ToggleProperty("显示选中状态", true),
            new DropDownProperty("是否显示朝向", showDirs, 1, true),
            //消息属性[16]
            new LabelProperty("消息属性—————————————————————————————————————————————————"),
            new InputStringProperty("通信消息配置", "Add,Color,Yellow; Add,Icon,无;"),
            new ToggleProperty("显示通用消息", true),
            new ToggleProperty("显示指令消息", true),
            //任务属性[20]
            new LabelProperty("任务属性—————————————————————————————————————————————————"),
            new OpenFileDialogProperty("数据源XML路径","路径"),
            //控制属性[22]
            new LabelProperty("控制属性—————————————————————————————————————————————————"),
            new InputIntProperty("初始倍速", 1),
            new InputStringProperty("倍速集合", "1;2;4;8;16;32"),
            new DropDownProperty("时间参数",timeType, 0, true),
            //指令属性[26]
            new LabelProperty("指令属性—————————————————————————————————————————————————"),
            //分组属性[27]
            new LabelProperty("分组属性—————————————————————————————————————————————————"),
            new DropDownProperty("数据源", groupData, 0, true),
            new DropDownProperty("分组依据",groups2, 0, true),
            new InputStringProperty("类型分组配置", "救援力量:15_0;遇险目标:15_1;干扰因素:15_2,15_3"),
            new InputStringProperty("场景分组配置", "待定"),
            new ToggleProperty("是否显示隐藏组件", false),
            //组件属性[33]
            new LabelProperty("组件属性—————————————————————————————————————————————————"),
            new ToggleProperty("显示状态信息", true),
            new ToggleProperty("显示选中装备效果", true),
            new ToggleProperty("显示全部装备效果", false),
        };
        Properties = dProperties.ToArray();
        //CurStatus
        mCamera = transform.Find("Camera").gameObject;
        m_Camera = mCamera.GetComponent<Camera>();
        uiDirectDeduction = transform.Find("UIDirectDeduction").gameObject.AddComponent<UIDirectDeduction>();
        uiDirectDeduction.enabled = true;
        
        sender.DebugMode = true;
        //schemeMgr = Manager.Mission.MM.Mission.MapData.SchemeMgr;
        runScheme = schemeMgr.GetSelectScheme();
        if (runScheme == null)
        {
            runScheme = new Scheme();
            runScheme.Name = "实施预案-" + runScheme.Id;
        }
    }

    public override void EditorModeInitialized()
    {
        base.EditorModeInitialized();
        SetUI(false);
    }

    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        this.info = info;
        mouseOrbit = mCamera.AddComponent<DMouseOrbit>();
        cameraViewMove = mCamera.AddComponent<DMCameraViewMove>();
        thirdCameraControl = mCamera.AddComponent<ThirdCameraControl>();
        thirdCameraControl.EnableControls = false;
        Input = transform.Find("InputSystem").gameObject.AddComponent<InputManager>();
        //Data.isMain = isRoomCreator; //主机开启发送状态

        DirectDeductionMgr.GetInstance.InitProperties(Properties);
        DirectDeductionMgr.GetInstance.InitComIcon(info.PicBObjects);

        SetUI(true);
        loadFinished = true;

        //测试
        BObjectModel roleModel = GetComponent<BObjectModel>();
        uiDirectDeduction.InitData(roleModel, info);

        //全局环境发送消息
        ToggleProperty togglePro = (ToggleProperty)DirectDeductionMgr.GetInstance.GetProperties("全局属性", "显示全局环境");
        if (togglePro.Value)
        {
            DropDownSceneSelectProperty dropDownScenePro = (DropDownSceneSelectProperty)DirectDeductionMgr.GetInstance.GetProperties("全局属性", "关联环境组件");
            //dropDownScenePro.Value
            //sender.RunSend(SendType.MainToMain, "", 1001, null);
        }
    }

    public void SeletedCom(string[] comIds)
    {
        foreach (var item in comIds)
        {
            //选中组件，如果批量选中则需要循环分别执行
            SchemeEquipment selectEquip = runScheme.Equipments.Find(k => k.EquipId == item);
            //RTS实时操作需要记录
            InstructionRTS ins = new InstructionRTS();
            ins.Name = "RTSOper";
            //ins.Position=鼠标点击位置坐标
            //ins.Time=计时器时间
            //ins.Instruction=构建指令信息
            selectEquip.InsRTSs.Add(ins);  //实时操作指令只有增加，没有删除和修改

            //同时需要将指令信息发给组件
            //sender.RunSend(SendType.MainToMain, "指令执行对象id", (int)SystemInputType.RTSInstruction, ins.Instruction.ToString());
            //todoRTS救援指挥端退出时，弹出是否保存预案的操作，是->需要将Scheme添加到编辑器任务中预案里
            //将InsRTSs按照规则转化为InsSets,并保存到任务文件中

            //运行模式3情况有些特殊
            //todo将InsRTS按照某种规则插入到InsSets，规则比较复杂
        }
    }

    public void SetUI(bool active)
    {
        mCamera.SetActive(active);
        uiDirectDeduction.gameObject.SetActive(active);
    }

    public void RecMessage(SendType type, GameObject senderObj, int eventType, string param)
    {
        if (!loadFinished) return;
        //调试输出Log LogError LogWarning用sender代替Debug
        sender.Log("Mes:" + sender.name + " Type:" + type.ToString() + " Event:" + eventType + " Param:" + param);
        switch (type)
        {
            case SendType.MainToMain:
                //roleCtrlHandler.RecOperationMes(senderObj, eventType, param);
                if(eventType == 1001)
                {
                    if (param == null)
                    {
                        string message = senderObj.name + "开始任务";
                        uiDirectDeduction.CreateNormalLabelItem(message);
                    }
                }
                break;
            case SendType.SubToMain:
                //keyCodeHandler.RecKeyCodeMes(eventType, param);
                break;
            case SendType.MainToSubs:
                //syncHandler.RecSyncMes(eventType, param);
                break;
            case SendType.MainSystem:
                if (eventType == (int)MainSystemType.RoleOperMes)
                {
                    //uiDirectDeduction.roleOperPanel.ShowRoleOperInfo(param);
                }
                else if (eventType == (int)MainSystemType.UIMes)
                    uiDirectDeduction.CreateNormalLabelItem(param);
                    //uiDirectDeduction.AddUIMes(eventType.ToString(), param);
                break;
            case SendType.MainToAll:
                break;
            default:
                break;
        }
    }

    #region 角色需实现IControl接口
    public DevType devType = DevType.PC;
    public bool playBack = true;

    public void Active(DevType type, bool playback)
    {
        this.playBack = playback;
        mCamera.SetActive(true);
        //Input.SetActive(true && !playback);

        uiDirectDeduction.SetMask(playback);
        BObjectModel roleModel = GetComponent<BObjectModel>();
        //uiDirectDeduction.InitData(roleModel, info, playback);
    }

    public void DeActive(DevType type, bool playback)
    {
        this.playBack = playback;
        mCamera.SetActive(false);
        //Input.SetActive(false);

        uiDirectDeduction.SetMask(playback);
    }
    #endregion
}
