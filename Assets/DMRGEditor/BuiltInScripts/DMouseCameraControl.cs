﻿using UnityEngine;

[AddComponentMenu("DMBuiltInScript/DMouseCameraControl")]
public class DMouseCameraControl : MonoBehaviour
{

    public bool forceCameraUp = true;

    // Mouse buttons in the same order as Unity
    public enum MouseButton { Left = 0, Right = 1, Middle = 2, None = 3 }

    [System.Serializable]
    // Handles left modifiers keys (Alt, Ctrl, Shift)
    public class Modifiers
    {
        public bool leftAlt;
        public bool leftControl;
        public bool leftShift;

        public bool checkModifiers()
        {
            return (!leftAlt ^ Input.GetKey(KeyCode.LeftAlt)) &&
                (!leftControl ^ Input.GetKey(KeyCode.LeftControl)) &&
                    (!leftShift ^ Input.GetKey(KeyCode.LeftShift));
        }
    }

    [System.Serializable]
    // Handles common parameters for translations and rotations
    public class MouseControlConfiguration
    {
        public bool activate;
        public MouseButton mouseButton;
        public Modifiers modifiers;
        public float sensitivity;

        public bool isActivated()
        {
            return activate && Input.GetMouseButton((int)mouseButton) && modifiers.checkModifiers();
        }
    }

    [System.Serializable]
    // Handles scroll parameters
    public class MouseScrollConfiguration
    {

        public bool activate;
        public Modifiers modifiers;
        public float sensitivity;

        public bool isActivated()
        {
            return activate && modifiers.checkModifiers();
        }
    }

    // Yaw default configuration
    public MouseControlConfiguration yaw = new MouseControlConfiguration { mouseButton = MouseButton.Right, sensitivity = 10F };

    // Pitch default configuration
    public MouseControlConfiguration pitch = new MouseControlConfiguration { mouseButton = MouseButton.Right, modifiers = new Modifiers { leftControl = true }, sensitivity = 10F };

    // Roll default configuration
    public MouseControlConfiguration roll = new MouseControlConfiguration();

    // Vertical translation default configuration
    public MouseControlConfiguration verticalTranslation = new MouseControlConfiguration { mouseButton = MouseButton.Middle, sensitivity = 2F };

    // Horizontal translation default configuration
    public MouseControlConfiguration horizontalTranslation = new MouseControlConfiguration { mouseButton = MouseButton.Middle, sensitivity = 2F };

    // Depth (forward/backward) translation default configuration
    public MouseControlConfiguration depthTranslation = new MouseControlConfiguration { mouseButton = MouseButton.Left, sensitivity = 2F };

    // Scroll default configuration
    public MouseScrollConfiguration scroll = new MouseScrollConfiguration { sensitivity = 2F };

    // Default unity names for mouse axes
    public string mouseHorizontalAxisName = "Mouse X";
    public string mouseVerticalAxisName = "Mouse Y";
    public string scrollAxisName = "Mouse ScrollWheel";

    public bool isOrthographic = false;
    public Camera[] affectedOrtographicCameras;

    public float KeyBoardSpeed = 100f;

    void LateUpdate()
    {
        //UI上屏蔽的功能
        //!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()
        if (true)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(new Vector3(-Time.deltaTime * KeyBoardSpeed, 0, 0),Space.World);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(new Vector3(0, 0, -Time.deltaTime * KeyBoardSpeed), Space.World);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(new Vector3(Time.deltaTime * KeyBoardSpeed, 0, 0), Space.World);
            }
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(new Vector3(0, 0, Time.deltaTime * KeyBoardSpeed), Space.World);
            }
            if (scroll.isActivated())
            {
                //滚轮
                if (isOrthographic)
                {
                    Camera.main.orthographicSize -= Input.GetAxis(scrollAxisName) * scroll.sensitivity;
                    Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 5, 200);
                    foreach (Camera cam in affectedOrtographicCameras)
                    {
                        cam.orthographicSize = Camera.main.orthographicSize;
                    }
                }
                else
                {
                    float translateZ = Input.GetAxis(scrollAxisName) * scroll.sensitivity;
                    transform.Translate(0, 0, translateZ);
                }
            }

        }
        if (yaw.isActivated())
        {
            float rotationX = Input.GetAxis(mouseHorizontalAxisName) * yaw.sensitivity;
            transform.Rotate(0, rotationX, 0);
        }
        if (pitch.isActivated())
        {
            float rotationY = Input.GetAxis(mouseVerticalAxisName) * pitch.sensitivity;
            transform.Rotate(-rotationY, 0, 0);
        }
        if (roll.isActivated())
        {
            float rotationZ = Input.GetAxis(mouseHorizontalAxisName) * roll.sensitivity;
            transform.Rotate(0, 0, rotationZ);
        }
        if (verticalTranslation.isActivated())
        {
            float translateY = Input.GetAxis(mouseVerticalAxisName) * verticalTranslation.sensitivity;
            if (isOrthographic)
            {
                Vector3 move = transform.up;
                //Transform to world coords
                //move = transform.TransformDirection(move);
                move.y = 0;
                move.Normalize();
                move *= translateY;

                transform.Translate(move, Space.World);
            }
            else
            {
                transform.Translate(0, translateY, 0);
            }
        }

        if (horizontalTranslation.isActivated())
        {
            float translateX = Input.GetAxis(mouseHorizontalAxisName) * horizontalTranslation.sensitivity;
            transform.Translate(translateX, 0, 0);
        }

        if (depthTranslation.isActivated())
        {
            float translateZ = Input.GetAxis(mouseVerticalAxisName) * depthTranslation.sensitivity;
            transform.Translate(0, 0, translateZ);
        }
        if (forceCameraUp)
        {
            Vector3 rot = transform.rotation.eulerAngles;
            rot.z = 0;
            transform.rotation = Quaternion.Euler(rot);
        }
    }
}