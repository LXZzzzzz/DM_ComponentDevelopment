using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;
using UnityEngine;

namespace 指挥端
{
    public class UIHover : DMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public System.Action<string> OnEnterAction;
        public GameObject hoverObj;
        public GameObject onlyShowObj;
        public GameObject onlyHiddleObj;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverObj != null) hoverObj.SetActive(true);
            if (onlyShowObj != null) onlyShowObj.SetActive(true);
            OnEnterAction?.Invoke(gameObject.name);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (hoverObj != null) hoverObj.SetActive(false);
            if (onlyHiddleObj != null) onlyHiddleObj.SetActive(false);
        }
    }
}
