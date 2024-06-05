using UnityEngine;

namespace DMCameraControl
{
    public class DMCameraViewMove : DMonoBehaviour
    {
        [SerializeField]
        private float Pow = 1.02f;
        [SerializeField]
        private float Radix = 3f;
        public float moveSpeed = 5f;
        private float mwheelSpeed = 5;
        private float minHeight = -100f;
        private float maxHeight = 10000f;
        [SerializeField]
        private float PlanformHeight = 1500;
        private DMouseOrbit m_mouseOrbit;
        void Start()
        {
            if (null == m_mouseOrbit)
                m_mouseOrbit = GetComponent<DMouseOrbit>();
        }

        private Vector3 Direction
        {
            get
            {
                Vector3 direction = new Vector3(transform.forward.x, 0, transform.forward.z);
                return direction.normalized;
            }
        }
        private float MoveSpeed(float radix)
        {
            float speed = Mathf.Pow(radix, Pow);
            return speed;
        }
        private float MoveDistance
        {
            get
            {
                Ray ray = new Ray(transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    float transHeight = transform.position.y - hit.point.y;
                    moveSpeed = MoveSpeed(transHeight);
                }
                return moveSpeed * Radix * Time.deltaTime;
            }
        }
        private float MoveDistance_Mwheel
        {
            get
            {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    float dis = Vector3.Distance(transform.position, hit.point);
                    mwheelSpeed = MoveSpeed(dis);
                }
                return mwheelSpeed;
            }
        }
        private void MoveFunction()
        {
            float mwheel = Input.GetAxis("Mouse ScrollWheel");
            if (mwheel != 0)
            {
                if (transform.position.y >= minHeight && transform.position.y <= maxHeight)//限制高度
                {
                    transform.Translate(new Vector3(0, 0, MoveDistance_Mwheel * mwheel));
                }
                else
                {
                    bool isTrue = (transform.position.y < minHeight && mwheel < 0);
                    bool isTrue1 = (transform.position.y > maxHeight && mwheel > 0);
                    bool isTrue2 = (transform.position.y < minHeight && mwheel > 0);
                    if (isTrue || isTrue1 || (transform.eulerAngles.x > 90 && isTrue2))
                    {
                        transform.Translate(new Vector3(0, 0, MoveDistance_Mwheel * mwheel));
                    }
                }
            }
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Direction * MoveDistance, Space.World);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Direction * -MoveDistance, Space.World);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(new Vector3(-MoveDistance, 0, 0));
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(new Vector3(MoveDistance, 0, 0));
            }
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) ||
               Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                m_mouseOrbit.SyncAngles();
            }
        }
        public void MoveWS(float ws)
        {
            int isW = 0;
            if (ws > 0.2f) isW = 1;
            else if (ws < -0.2f) isW = -1;
            transform.Translate(Direction * MoveDistance * isW, Space.World);
        }
        public void MoveAD(float ad)
        {

            int isA = 0;
            if (ad > 0.2f) isA = 1;
            else if (ad < -0.2f) isA = -1;
            transform.Translate(new Vector3(MoveDistance * isA, 0, 0));
        }
        void LateUpdate()
        {
            MoveFunction();
        }
    }
}
