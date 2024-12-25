using System.Collections.Generic;
using ToolsLibrary.EquipPart;
using ToolsLibrary.PathPart;
using UnityEngine;

namespace ToolsLibrary
{
    public class MyDataInfo
    {
        public static Transform SceneGoParent;
        public static string leadId;
        public static bool isHost;
        public static bool isPlayBack;
        public static FrameSync.FrameSyncLogicBase netLogic;
        public static List<EquipBase> sceneAllEquips;
        public static List<ZiYuanBase> sceneAllZiYuan;
        public static int MyLevel; //我的角色级别
        public static GameState gameState;
        public static List<ClientInfo> playerInfos;
        public static float speedMultiplier; //场景运行速度
        public static float gameStartTime; //记录本局游戏开始时刻
        public static List<string> SkillsToBeConfirmed; //待确认技能列表
        public static List<string> TaskPlanningCompletedPersons; //记录完成了任务规划的机长
        public static List<string> BeUsedJizus, BeUsedBaozhangs; //可用的机组信息和保障人员信息
    }

    public enum GameState
    {
        // 初始默认阶段(此时是导教端设置任务背景，其他端等着)
        None,

        //导教端完成了任务背景设置（此时总指挥可以创建方案和查看灾情信息）
        CompleteTaskBgSet,

        // 发布了方案（此时前线指挥可以进行资源分配了，总可以申请航线）
        ReleaseProgramme,

        // 航线得到了同意（前指促使可以申请任务执行了）
        AgreeAirLine,

        //申请了任务执行（机长可以规划路径了（机长端改为Plan模式））
        AgreeTaskExecute,

        // 开始推演（导可以特情触发了，机长normal）
        GameStart,

        // 游戏暂停
        GamePause,

        // 游戏停止
        GameStop
    }

    public struct ClientInfo
    {
        /// <summary>用户ID</summary>
        public string UID;

        /// <summary>用户名</summary>
        public string PlayerName;

        /// <summary>占用角色Id</summary>
        public string RoleId;

        /// <summary>
        /// 客户端角色等级
        /// </summary>
        public int ClientLevel;

        /// <summary>
        /// 角色等级名称
        /// </summary>
        public string ClientLevelName;

        /// <summary>
        /// 玩家对应的标志色
        /// </summary>
        public Color MyColor;

        /// <summary>
        /// 颜色色值
        /// </summary>
        public string ColorCode;

        /// <summary>
        /// 常态icon显示的颜色
        /// </summary>
        public Color NormalColor;

        /// <summary>
        /// 选中颜色
        /// </summary>
        public Color ChooseColor;

        /// <summary>
        /// icon底色
        /// </summary>
        public Color IconBgColor;

        /// <summary>
        /// 进度条标识
        /// </summary>
        public string progressId;
    }
}