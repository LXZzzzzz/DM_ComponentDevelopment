using UnityEngine;

namespace ToolsLibrary
{

    enum CameraModel
    {
        follow,
        rotate
    }
    public class ThirdPersonCamera : MonoSingleTon<ThirdPersonCamera>
    {
        private Transform player;  // 对象用于跟随的玩家角色

        private Vector3 cameraOffset;  // 相机偏移量

        private float damping = 1f;  // 相机移动的缓冲时间
        private CameraModel cm;

        private bool isFixedFollow;//选择是否使用fixed更新，来控制相机跟随。。跟随对象如果是通过fixedUpdate中执行的移动逻辑，在lateUpdate中跟随就会抖动

        public void Init(Transform player, Vector3 cameraOffset, float damping, bool isFixedFollow = false)
        {
            this.player = player;
            this.cameraOffset = cameraOffset;
            this.damping = damping;
            this.isFixedFollow = isFixedFollow;
            cm = CameraModel.follow;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                cm = CameraModel.rotate;
                InputManager.previousMousePos = Vector3.zero;
                distance = Vector3.Distance(transform.position, player.position);
                Vector3 angles = transform.eulerAngles;
                x = angles.y;
                y = angles.x;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                cm = CameraModel.follow;
            }
        }

        private void LateUpdate()
        {
            if (isFixedFollow) return;
            switch (cm)
            {
                case CameraModel.follow:
                    followMode();
                    break;
                case CameraModel.rotate:
                    Rotate();
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (!isFixedFollow) return;
            switch (cm)
            {
                case CameraModel.follow:
                    followMode();
                    break;
                case CameraModel.rotate:
                    Rotate();
                    break;
            }
        }

        private void followMode()
        {
            if (player == null) return;
            // 计算目标位置
            Vector3 targetPosition = player.position + player.forward * cameraOffset.z + player.up * cameraOffset.y;

            // 平滑相机移动
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * damping);

            // 相机看向玩家角色
            transform.LookAt(player);
        }


        private float distance = 10.0f; // 相机到物体的距离
        private float xSpeed = 20.0f; // 水平旋转速度
        private float ySpeed = 15.0f; // 垂直旋转速度

        private float x = 0.0f;
        private float y = 0.0f;

        // 限定相机角度的范围
        private float yMinLimit = -20;
        private float yMaxLimit = 80;

        // 鼠标滚动缩放的最大、最小距离
        private float distanceMin = 0.5f;
        private float distanceMax = 15f;

        private void Rotate()
        {
            //隐藏鼠标的显示，锁定到屏幕中间

            //获取鼠标横向位移
            x += InputManager.GetAxis("Mouse X") * xSpeed * distance * Time.fixedDeltaTime;
            y -= InputManager.GetAxis("Mouse Y") * ySpeed * Time.fixedDeltaTime;
            //计算旋转目标位置
            // 限定角度范围
            y = ClampAngle(y, yMinLimit, yMaxLimit);

            // 计算相机位置
            Quaternion rotation = Quaternion.Euler(y, x, 0);
            Vector3 position = rotation * new Vector3(0, 0, -distance) + player.position;

            // 滚轮缩放
            // distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            // 更新相机位置和角度
            transform.rotation = rotation;
            transform.position = position;
        }


        // 将角度限定在指定范围内
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
