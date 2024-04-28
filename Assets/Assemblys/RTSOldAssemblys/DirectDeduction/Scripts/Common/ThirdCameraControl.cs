using UnityEngine;

namespace 指挥端
{
    public class ThirdCameraControl : DMonoBehaviour
    {
        public bool EnableControls = true;
        public GameObject Target;
        public float MaxDistance = 50f;
        public float MinDistance = 10f;
        public float StartDistance = 200f;
        private float currentDistance;
        public float RotationSpeed = 90f;
        public float ScrollSpeed = 50f;
        [Range(-89.0f, 89.0f)]
        public float MaxAngle = 70f;
        [Range(-89.0f, 89.0f)]
        public float MinAngle = 10f;
        private float xMovement = 0.0f;
        private float yMovement = 0.0f;
        public int MouseButton = 1;
        public bool ReverseScrollDirection = false;
        public bool AlwaysRotateWithMouseMovement = false;
        private Quaternion rotation;
        private float timeSinceLastWarningAboutMissingTarget = 10.0f;
        private Camera localCamera;

        void Awake()
        {
            localCamera = GetComponent<Camera>();
        }

        void Start()
        {
            currentDistance = StartDistance;
            yMovement = 30;
        }

        void Update()
        {
            if (Target == null)
            {
                timeSinceLastWarningAboutMissingTarget = timeSinceLastWarningAboutMissingTarget + Time.deltaTime;
                if (timeSinceLastWarningAboutMissingTarget >= 10.0f)
                {
                    timeSinceLastWarningAboutMissingTarget = 0f;
                }
                return;
            }

            if (EnableControls)
            {
                zoom();
                if (Input.GetMouseButton(MouseButton) || AlwaysRotateWithMouseMovement)
                    doRotation();
            }
            rotation = Quaternion.Euler(yMovement, xMovement, 0);
            Vector3 distanceVector = new Vector3(0.0f, 0.0f, -currentDistance);
            Vector3 position = rotation * distanceVector + Target.transform.position;
            transform.position = position;
            transform.LookAt(Target.transform, Vector3.up);
            localCamera.farClipPlane = Mathf.Max(50000.0f, 1000.0f + transform.position.y * 20);
        }

        private void doRotation()
        {
            xMovement = xMovement + Input.GetAxis("Mouse X") * RotationSpeed * 0.02f;
            yMovement = yMovement - Input.GetAxis("Mouse Y") * RotationSpeed * 0.02f;
            if (yMovement > MaxAngle)
                yMovement = MaxAngle;
            if (yMovement < MinAngle)
                yMovement = MinAngle;
        }

        private void zoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (ReverseScrollDirection)
                currentDistance = currentDistance + (scroll * ScrollSpeed);
            else
                currentDistance = currentDistance + (-scroll * ScrollSpeed);
            if (currentDistance > MaxDistance)
                currentDistance = MaxDistance;
            if (currentDistance < MinDistance)
                currentDistance = MinDistance;
        }
    }
}

