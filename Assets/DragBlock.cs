using UnityEngine;

public class DragBlock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Camera mainCamera;
    private Vector3 offset;
    private float zPosition;
    private Rigidbody rb;

    void Start()
    {
        mainCamera = Camera.main;
        zPosition = transform.position.z;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void onMouseDown()
    {
        offset = transform.position - GetMouseWorldPosition();
        rb.isKinematic = true;
    }
    void OnMouseDrag()
    {
        Vector3 newPosition = GetMouseWorldPosition() + offset;
        newPosition.z = zPosition;
        transform.position = newPosition;
    }
    void OnMouseUp()
    {
        rb.isKinematic = false;
    }
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }
}
