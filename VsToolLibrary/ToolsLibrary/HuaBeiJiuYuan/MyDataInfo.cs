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
        public static int MyLevel; //我的角色级别
        public static GameState gameState;
        public static List<ClientInfo> playerInfos;
        public static float speedMultiplier; //场景运行速度
        public static float gameStartTime; //记录本局游戏开始时刻
    }

    public enum GameState
    {
        /// <summary>
        /// 初始默认阶段
        /// </summary>
        None,

        /// <summary>
        /// 一级指挥端编辑阶段
        /// </summary>
        FirstLevelCommanderEditor,

        /// <summary>
        /// 收到方案后的准备阶段
        /// </summary>
        Preparation,

        /// <summary>
        /// 游戏开始阶段
        /// </summary>
        GameStart,

        /// <summary>
        /// 游戏暂停
        /// </summary>
        GamePause,

        /// <summary>
        /// 游戏停止
        /// </summary>
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