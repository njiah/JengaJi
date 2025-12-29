using UnityEngine;

public class TouchOrbitCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 targetOffset = new(0f, 1.2f, 0f);

    [Header("Orbit")]
    public float rotationSpeed = 180f;
    public float minPitch = 10f;
    public float maxPitch = 75f;

    [Header("Zoom")]
    public float distance = 6f;
    public float minDistance = 2.5f;
    public float maxDistance = 12f;
    public float zoomSpeed = 1.5f;
    public float pinchZoomSpeed = 0.01f;

    [Header("External Gate (set by TouchInputController)")]
    public bool allowOrbit = true;

    float yaw;
    float pitch = 25f;

    void Start()
    {
        var angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = Mathf.Clamp(angles.x, minPitch, maxPitch);
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (allowOrbit)
        {
            HandleTouchOrbit();
        }

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 focus = target.position + targetOffset;
        Vector3 desiredPos = focus - rot * Vector3.forward * distance;

        transform.position = desiredPos;
        transform.rotation = rot;
    }

    void HandleTouchOrbit()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved)
            {
                Vector2 delta = t.deltaPosition;
                float dx = delta.x / Screen.width;
                float dy = delta.y / Screen.height;

                yaw += dx * rotationSpeed;
                pitch -= dy * rotationSpeed;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }
        } else if (Input.GetMouseButton(0))
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed * 0.02f;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * 0.02f;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);
        }

        if (Input.touchCount >= 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 p0Prev = t0.position - t0.deltaPosition;
            Vector2 p1Prev = t1.position - t1.deltaPosition;

            float prevDist = Vector2.Distance(p0Prev, p1Prev);
            float currDist = Vector2.Distance(t0.position, t1.position);

            float pinchDelta = currDist - prevDist;
            distance = Mathf.Clamp(distance - pinchDelta * pinchZoomSpeed, minDistance, maxDistance);
        }
    }
}
