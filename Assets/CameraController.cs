using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float zoomSpeed = 5f;

    void Update()
    {
        if (Input.GetMouseButton(1))  // Right-click to rotate
        {
            float h = Input.GetAxis("Mouse X") * rotationSpeed;
            float v = -Input.GetAxis("Mouse Y") * rotationSpeed;
            transform.RotateAround(Vector3.zero, Vector3.up, h);
            transform.RotateAround(Vector3.zero, transform.right, v);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        transform.Translate(0, 0, scroll, Space.Self);
    }
}
