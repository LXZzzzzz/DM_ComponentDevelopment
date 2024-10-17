using UnityEngine;

namespace ToolsLibrary
{
    public class MonoSingleTon<T> : DMonoBehaviour where T : class
    {
        //定义一个静态变量保存类的实例
        private static T m_Instance = null;
        private static GameObject instanceGo;
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    //全局查找
                    m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;
                }
                if (m_Instance == null)
                {
                    //生成实例 获取组件
                    instanceGo = new GameObject("SingleTon of " + typeof(T).ToString(), typeof(T));
                    m_Instance = instanceGo.GetComponent<T>();
                }
                return m_Instance;
            }
        }

        private void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this as T;
            }
        }
        private void OnDestroy()
        {
            Destroy(instanceGo);
            m_Instance = null;
        }
    }

}
