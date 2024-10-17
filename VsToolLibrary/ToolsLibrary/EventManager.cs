using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;
using UnityEngine;

namespace ToolsLibrary
{
    // 事件管理器
    public interface IEventInfo { }
    public class EventInfo<T> : IEventInfo
    {
        public UnityAction<T> actions;
        public EventInfo(UnityAction<T> action)
        {
            actions += action;
        }
    }
    public class EventInfo : IEventInfo
    {
        public UnityAction actions;
        public EventInfo(UnityAction action)
        {
            actions += action;
        }
    }
    public class EventInfo<T, T1> : IEventInfo
    {
        public UnityAction<T, T1> actions;
        public EventInfo(UnityAction<T, T1> action)
        {
            actions += action;
        }
    }
    public class EventManager : MonoSingleTon<EventManager>
    {
        private void Awake()
        {
            eventDic = new Dictionary<string, IEventInfo>();
        }
        private Dictionary<string, IEventInfo> eventDic;
        //添加事件
        public void AddEventListener<T>(string et, UnityAction<T> callBack)
        {
            if (eventDic.ContainsKey(et))
            {
                (eventDic[et] as EventInfo<T>).actions += callBack;
            }
            else
            {
                eventDic.Add(et, new EventInfo<T>(callBack));
            }
        }
        public void AddEventListener(string et, UnityAction callBack)
        {
            if (eventDic.ContainsKey(et))
            {
                (eventDic[et] as EventInfo).actions += callBack;
            }
            else
            {
                eventDic.Add(et, new EventInfo(callBack));
            }
        }
        public void AddEventListener<T, T1>(string et, UnityAction<T, T1> callBack)
        {
            if (eventDic.ContainsKey(et))
            {
                (eventDic[et] as EventInfo<T, T1>).actions += callBack;
            }
            else
            {
                eventDic.Add(et, new EventInfo<T, T1>(callBack));
            }
        }
        //触发事件
        public void EventTrigger<T>(string et, T info)
        {
            if (eventDic == null)
            {
                Debug.LogError("事件库为空");
                return;
            }
            if (eventDic.ContainsKey(et))
            {
                (eventDic[et] as EventInfo<T>).actions?.Invoke(info);
            }
            else
            {
                Debug.LogError("不存在这个事件");
            }
        }
        public void EventTrigger(string et)
        {
            if (eventDic == null)
            {
                Debug.LogError("事件库为空"); return;
            }
            if (eventDic.ContainsKey(et))
            {
                (eventDic[et] as EventInfo).actions?.Invoke();
            }
            else
            {
                Debug.LogError("不存在这个事件");
            }
        }
        public void EventTrigger<T, T1>(string et, T info, T1 info1)
        {
            if (eventDic == null)
            {
                Debug.LogError("事件库为空");
                return;
            }
            if (eventDic.ContainsKey(et))
            {
                (eventDic[et] as EventInfo<T, T1>).actions?.Invoke(info, info1);
            }
            else
            {
                Debug.LogError("不存在这个事件");
            }
        }

        //移除事件
        public void RemoveEventListener<T>(string et, UnityAction<T> action)
        {
            if (eventDic.ContainsKey(et))
            {
                (eventDic[et] as EventInfo<T>).actions -= action;
            }
        }
        public void RemoveEventListener(string et, UnityAction action)
        {
            if (eventDic.ContainsKey(et))
            {
                (eventDic[et] as EventInfo).actions -= action;
            }
        }
        public void RemoveEventListener<T, T1>(string et, UnityAction<T, T1> action)
        {
            if (eventDic.ContainsKey(et))
            {
                (eventDic[et] as EventInfo<T, T1>).actions -= action;
            }
        }
        //清空事件监听
        public void Clear()
        {
            eventDic?.Clear();
        }
        private void OnDestroy()
        {
            Clear();
        }
    }
}
