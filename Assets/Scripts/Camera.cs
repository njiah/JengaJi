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

    [Header("External Gate (set by controller)")]
    public bool allowOrbit = true;

    float yaw;
    float pitch = 25f;

    void Start()
    {
        var angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = Mathf.Clamp(angles.x, minPitch, maxPitch);
    }

    public void ApplyOrbitDelta(Vector2 screenDeltaPixels)
    {
        if (!allowOrbit) return;

        float dx = screenDeltaPixels.x / Screen.width;
        float dy = screenDeltaPixels.y / Screen.height;

        yaw += dx * rotationSpeed;
        pitch -= dy * rotationSpeed;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    public void ApplyZoomDelta(float zoomDelta)
    {
        distance = Mathf.Clamp(distance - zoomDelta, minDistance, maxDistance);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 focus = target.position + targetOffset;
        Vector3 pos = focus - rot * Vector3.forward * distance;

        transform.position = pos;
        transform.rotation = rot;
    }
}
