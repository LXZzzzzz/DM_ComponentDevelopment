using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace 指挥端
{
    public class SelectBox : DMonoBehaviour
    {

        #region Fields & Attribute

        //画线的材质
        private Material drawMaterial;

        //画笔颜色
        private Color brushColor = Color.white;

        //设置选择区域的Color
        private Color selectionAreaColor = new Color(1, 1, 1, 0.1f);

        //开始和结束绘制点
        private Vector3 mouseStartPos, mouseEndPos;

        //开始绘制标志
        private bool isStartDraw = false;

        //获取绘制状态（开始绘制标志）
        public bool IsStartDraw { get => isStartDraw; set => isStartDraw = value; }

        //绘制开始坐标
        public Vector3 MouseStartPos { get => mouseStartPos; set => mouseStartPos = value; }

        //绘制结束坐标
        public Vector3 MouseEndPos { get => mouseEndPos; set => mouseEndPos = value; }

        //设置画笔颜色
        public Color BrushColor { get => brushColor; set => brushColor = value; }

        //设置选择区域的Color
        public Color SelectionAreaColor { get => selectionAreaColor; set => selectionAreaColor = value; }

        #endregion

        private void Awake()
        {
            drawMaterial = new Material(Shader.Find("UI/Default"));
            this.drawMaterial.hideFlags = HideFlags.HideAndDontSave;
            this.drawMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
        }

        private void Start()
        {
            IsStartDraw = true;
        }

        #region Private Methods

        private Vector2 GetMouseScreenPos(Vector3 mousePosition)
        {
            float X = mousePosition.x - Screen.width / 2f;
            float Y = mousePosition.y - Screen.height / 2f;
            return new Vector2(X, Y);
        }

        /// <summary>
        /// 绘制逻辑
        /// </summary>
        private void OnGUI()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0))
            {
                IsStartDraw = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (IsStartDraw)
                {
                    IsStartDraw = false;
                    
                    Vector2 mouse1 = GetMouseScreenPos(mouseStartPos);
                    Vector2 mouse2 = GetMouseScreenPos(mouseEndPos);
                    float minX = mouse2.x > mouse1.x ? mouse1.x : mouse2.x;
                    float minY = mouse2.y > mouse1.y ? mouse1.y : mouse2.y;
                    float maxX = mouse2.x > mouse1.x ? mouse2.x : mouse1.x;
                    float maxY = mouse2.y > mouse1.y ? mouse2.y : mouse1.y;
                    Vector2 minPos = new Vector2(minX, minY);
                    Vector2 maxPos = new Vector2(maxX, maxY);
                    UIDirectDeduction.Instance.missionElementPanel.SelectedComs(minPos, maxPos);
                }
            }
            if (IsStartDraw && !EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    MouseStartPos = Input.mousePosition;
                }
                else if (Input.GetMouseButton(0))
                {
                    MouseEndPos = Input.mousePosition;
                }
                //材质通道，0为默认。 
                drawMaterial.SetPass(0);
                GL.LoadOrtho();
                //设置用屏幕坐标绘图
                GL.LoadPixelMatrix();
                DrawRect();
                DrawRectLine();
                //复制
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
                {
                    Debug.Log("000000000000000000000000000000000000");
                }
            }
        }

        /// <summary>
        /// 绘制框选区
        /// </summary>
        private void DrawRect()
        {
            GL.Begin(GL.QUADS);
            //设置颜色和透明度
            GL.Color(SelectionAreaColor);
            if (MouseStartPos != default && MouseEndPos != default)
            {
                if ((MouseStartPos.x > MouseEndPos.x && MouseStartPos.y > MouseEndPos.y) || (MouseStartPos.x < MouseEndPos.x && MouseStartPos.y < MouseEndPos.y))
                {
                    GL.Vertex3(MouseStartPos.x, MouseStartPos.y, 0);
                    GL.Vertex3(MouseStartPos.x, MouseEndPos.y, 0);
                    GL.Vertex3(MouseEndPos.x, MouseEndPos.y, 0);
                    GL.Vertex3(MouseEndPos.x, MouseStartPos.y, 0);

                }
                else
                {
                    GL.Vertex3(MouseStartPos.x, MouseStartPos.y, 0);
                    GL.Vertex3(MouseEndPos.x, MouseStartPos.y, 0);
                    GL.Vertex3(MouseEndPos.x, MouseEndPos.y, 0);
                    GL.Vertex3(MouseStartPos.x, MouseEndPos.y, 0);
                }
            }
            GL.End();
        }

        /// <summary>
        /// 绘制框选边框
        /// </summary>
        private void DrawRectLine()
        {
            GL.Begin(GL.LINES);
            //设置方框的边框颜色 边框不透明
            GL.Color(BrushColor);
            GL.Vertex3(MouseStartPos.x, MouseStartPos.y, 0);
            GL.Vertex3(MouseEndPos.x, MouseStartPos.y, 0);
            GL.Vertex3(MouseEndPos.x, MouseStartPos.y, 0);
            GL.Vertex3(MouseEndPos.x, MouseEndPos.y, 0);
            GL.Vertex3(MouseEndPos.x, MouseEndPos.y, 0);
            GL.Vertex3(MouseStartPos.x, MouseEndPos.y, 0);
            GL.Vertex3(MouseStartPos.x, MouseEndPos.y, 0);
            GL.Vertex3(MouseStartPos.x, MouseStartPos.y, 0);
            GL.End();
        }

        #endregion
    }
}
