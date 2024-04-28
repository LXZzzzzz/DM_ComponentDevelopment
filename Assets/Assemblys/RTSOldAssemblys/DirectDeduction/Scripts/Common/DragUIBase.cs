using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace 指挥端
{
    /// <summary>
    /// UGUI/拖拽UI对象在画布(Canvas)内部移动
    /// </summary>
    public class DragUIBase : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Vector2 originalLocalPointerPosition;   //记录开始拖拽时鼠标指针的本地坐标
        private Vector3 originalPanelLocalPosition;     //记录开始拖拽时目标对象本地坐标
        private RectTransform panelRectTransform;       //目标实例(RectTransform类型)
        private RectTransform canvasRectTransform;      //画布实例(RectTransform类型)

        void Start()
        {
            panelRectTransform = transform as RectTransform;
            canvasRectTransform = panelRectTransform.parent.parent.transform as RectTransform;
            OnStart();
        }
        public void OnBeginDrag(PointerEventData eventData)//开始拖拽时发生事件
        {
            if (panelRectTransform == null || canvasRectTransform == null)
                return;
            originalPanelLocalPosition = panelRectTransform.localPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);
            transform.SetAsLastSibling();
            BaseOnBeginDrag(eventData);
        }
        public void OnDrag(PointerEventData eventData)//拖拽中发生事件
        {
            if (panelRectTransform == null || canvasRectTransform == null)
                return;

            Vector2 localPointerPosition;//拖拽时鼠标指针的本地坐标
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
            {
                Vector3 offsetToOriginal = localPointerPosition - originalLocalPointerPosition;     //鼠标指针相对位移
                panelRectTransform.localPosition = originalPanelLocalPosition + offsetToOriginal;   //目标对象位移补偿
            }
            BaseOnDrag(eventData);

        }
        public void OnEndDrag(PointerEventData eventData)//结束拖拽时发生事件
        {
            BaseOnEndDrag(eventData);
        }

        //限制窗口于画布内部
        private void ClampToWindow()
        {
            Vector3 pos = panelRectTransform.localPosition;

            Vector3 minPosition = canvasRectTransform.rect.min - panelRectTransform.rect.min;   //矩形(2D)左下角坐标相对差
            Vector3 maxPosition = canvasRectTransform.rect.max - panelRectTransform.rect.max;   //矩形(2D)右上角坐标相对差
                                                                                                //在min与max之间取值给panelRectTransform.localPosition
            pos.x = Mathf.Clamp(panelRectTransform.localPosition.x, minPosition.x, maxPosition.x);
            pos.y = Mathf.Clamp(panelRectTransform.localPosition.y, minPosition.y, maxPosition.y);

            panelRectTransform.localPosition = pos;//最终赋值
        }

        protected virtual void OnStart() { }
        protected virtual void BaseOnBeginDrag(PointerEventData eventData) { }
        protected virtual void BaseOnDrag(PointerEventData eventData, bool isClampToWindow = true)
        {
            if (isClampToWindow) ClampToWindow();
        }
        protected virtual void BaseOnEndDrag(PointerEventData eventData) { }
    }
}
