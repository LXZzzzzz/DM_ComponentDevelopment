using System.Collections.Generic;
using UnityEngine;

namespace ToolsLibrary.EquipPart
{
    public interface IGroundReady
    {
        /// <summary>
        /// 起飞前准备
        /// </summary>
        void GroundReady(IAirPort airPort);

        /// <summary>
        /// 入库
        /// </summary>
        void BePutInStorage();
    }

    public interface ITakeOffAndLand
    {
        void TakeOff();
        void Landing();
    }

    public interface ISupply
    {
        void Supply();
    }

    public interface IWatersOperation
    {
        // //取水参数回传
        // void WaterIntaking(Vector3 pos, float range, float amount, bool isExecuteImmediately);
        //
        // //取水前检查最大取水量
        // float CheckCapacity();

        //新版取水操作
        void WaterIntaking_New();

        void WaterPour(Vector3 pos);
    }

    public interface IGoodsOperation
    {
        void LadeGoods();
        void UnLadeGoods();
        void AirdropGoods(Vector3 pos);
    }

    //救援人员的操作
    public interface IRescuePersonnelOperation
    {
        void Manned();
        void PlacementOfPersonnel();
        void CableDescentRescue();
    }

    public class SkillData
    {
        public SkillType SkillType;
        public string skillName;
        public bool isUsable;
    }

    public enum SkillType
    {
        None,
        GroundReady, //起飞前准备
        BePutInStorage, //入库
        TakeOff, //起飞
        Supply, //补给
        Landing, //降落
        WaterIntaking, //取水
        WaterPour, //投水
        LadeGoods, //装载物资
        UnLadeGoods, //卸载物资
        AirdropGoods, //空投物资
        Manned, //装载人员
        PlacementOfPersonnel, //安置人员
        CableDescentRescue, //索降救援
        ReturnFlight, //返航
        EndTask //结束任务
    }
}