using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolsLibrary.PathPart
{
    //作为装备的抽象类，共性是都有路径逻辑,移动逻辑
    public abstract class EquipBase_Logic : MonoBehaviour
    {
        //由一级指挥官分配ID
        [HideInInspector] public string BObjectId;
        //记录最后一个路径点Id
        [HideInInspector] public string lastPointId;

        private Vector3 targetPos; // 目标对象的 Transform 组件
        private float speed = 5.0f; // 移动速度
        private float threshold = 0.1f; // 到达目标的距离阈值

        public virtual void Init()
        {
            //初始化飞机基本属性
            
        }

        public void MoveToTarget(Vector3 targetPos)
        {
            this.targetPos = targetPos;
        }

        private void MoveLogic()
        {
            if (targetPos != Vector3.zero)
            {
                // 计算物体到目标的方向
                Vector3 direction = (targetPos - transform.position).normalized;
                // 移动物体
                transform.Translate(direction * speed * Time.deltaTime);

                // 如果物体已经到达目标位置，则可以执行到达目标后的行为
                if (Vector3.Distance(transform.position, targetPos) < threshold)
                {
                    // 到达目标
                    targetPos = Vector3.zero;
                }
            }
        }

        void Update()
        {
            MoveLogic();
        }
    }
}