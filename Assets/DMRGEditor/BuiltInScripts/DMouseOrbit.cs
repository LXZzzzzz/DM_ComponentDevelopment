using UnityEngine;
public class DMouseOrbit : MonoBehaviour
{
    private Camera m_camera;
  
    public float XSpeed = 5.0f;
    public float YSpeed = 5.0f;

    public float YMinLimit = -360f;
    public float YMaxLimit = 360f;

    private float m_x = 5.0f;
    private float m_y = 5.0f;

    private void Awake()
    {
        m_camera = GetComponent<Camera>();
    }

    private void Start()
    {
        SyncAngles();
    }

    public void SyncAngles()
    {
        Vector3 angles = transform.eulerAngles;
        m_x = angles.y;
        m_y = angles.x;
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            float deltaX = Input.GetAxis("Mouse X");
            float deltaY = Input.GetAxis("Mouse Y");

            deltaX = deltaX * XSpeed;
            deltaY = deltaY * YSpeed;

            m_x += deltaX;
            m_y -= deltaY;
            m_y = ClampAngle(m_y, YMinLimit, YMaxLimit);
            Zoom();
        }         
    }

    public void Zoom()
    {
        Quaternion rotation = Quaternion.Euler(m_y, m_x, 0);
        transform.rotation = rotation; 
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
        {
            angle += -360F;
        }
        if (angle > 360F)
        {
            angle -= 360F;
        }
        return Mathf.Clamp(angle, min, max);
    }
}
