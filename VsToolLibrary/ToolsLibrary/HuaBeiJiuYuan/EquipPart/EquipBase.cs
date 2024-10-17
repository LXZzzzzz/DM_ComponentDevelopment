using System.Collections.Generic;
using UnityEngine;
using ToolsLibrary.EffectivenessEvaluation;

namespace ToolsLibrary.EquipPart
{
    //作为装备的抽象类，共性是都有路径逻辑,移动逻辑
    public abstract class EquipBase : MonoBehaviour
    {
        //由一级指挥官分配ID
        [HideInInspector] public string BObjectId;

        //当前飞机归属于哪个指挥者
        [HideInInspector] public string BeLongToCommanderId;

        //记录最后一个路径点Id
        [HideInInspector] public string lastPointId;
        [HideInInspector] public bool isChooseMe;

        //当前是否停靠在机场
        public bool isDockingAtTheAirport;

        public Sprite EquipIcon;

        //记录这个飞机所有属性参数（只用来显示）
        public List<string> AttributeInfos;

        //当前选择的技能
        public SkillType CurrentChooseSkillType;

        //当前进行的技能
        public SkillType currentSkill;

        //技能进度
        public float skillProgress;

        private Vector3 targetPos; // 目标对象的 Transform 组件
        protected float speed = 20.0f; // 移动速度
        private float threshold = 1f; // 到达目标的距离阈值

        private bool _isArrive; //是否到达目的地

        private bool _isCrash; //是否坠毁

        public bool isTS, isYSWZ, isYSRY, isSJJY; //直升机是否包含这些装备技能

        protected bool isArrive => _isArrive;

        public Vector3 TargetPos => targetPos;

        public bool isCrash
        {
            get => _isCrash;
            protected set => _isCrash = value;
        }

        public virtual void Init(EquipBase baseData, List<ZiYuanBase> sceneAllZiyuan)
        {
            //初始化飞机基本属性
            _isArrive = true;
            isCrash = false;
            EquipIcon = baseData.EquipIcon;
            AttributeInfos = new List<string>();
            for (int i = 0; i < baseData.AttributeInfos.Count; i++)
            {
                AttributeInfos.Add(baseData.AttributeInfos[i]);
            }
        }

        public abstract List<SkillData> GetSkillsData();

        public abstract RecordedData GetRecordedData();

        public abstract void OnSelectSkill(SkillType st);

        public abstract bool OnCheckIsMove();

        public abstract void OnNullCommand(int type);

        public abstract void GetCurrentAllMass(out float currentOil, out float totalOil, out float water, out float goods, out float person, out int personType);

        public void MoveToTarget(Vector3 targetPos)
        {
            // if (currentSkill != SkillType.None) return;
            _isArrive = false;
            this.targetPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
            transform.LookAt(this.targetPos);
        }

        private void MoveLogic()
        {
            if (targetPos != Vector3.zero)
            {
                // 计算物体到目标的方向
                Vector3 direction = (targetPos - transform.position).normalized;
                // 移动物体
                transform.Translate(direction * speed * Time.deltaTime * MyDataInfo.speedMultiplier, Space.World);

                // 如果物体已经到达目标位置，则可以执行到达目标后的行为
                if (Vector3.Distance(transform.position, targetPos) < threshold * MyDataInfo.speedMultiplier)
                {
                    // 到达目标
                    transform.position = targetPos;
                    targetPos = Vector3.zero;
                    _isArrive = true;
                }
            }
        }

        void Update()
        {
            if (MyDataInfo.gameState == GameState.GamePause || MyDataInfo.gameState == GameState.GameStop) return;
            MoveLogic();
        }

        public virtual void OnCrash()
        {
            isCrash = true;
        }

        public void Destroy()
        {
            OnClose();
            Destroy(gameObject);
        }

        protected abstract void OnClose();
    }
}