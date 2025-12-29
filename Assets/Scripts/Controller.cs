using UnityEngine;

public class TouchInputController : MonoBehaviour
{
    public Camera cam;
    public TouchOrbitCamera orbitCamera;
    public LayerMask blockLayerMask;

    public float dragPlaneHeight = 0f;
    public float dragThreshold = 10f;

    Block selected;
    Rigidbody dragHandleRb;
    SpringJoint joint;
    GameObject dragHandle;

    Vector2 startPos;
    bool draggingBlock;

    void Awake()
    {
        if (!cam) cam = Camera.main;

        dragHandle = new GameObject("DragHandle");
        dragHandleRb = dragHandle.AddComponent<Rigidbody>();
        dragHandleRb.isKinematic = true;
    }

    void Update()
    {
        // TOUCH
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            HandleInput(t.phase, t.position, t.deltaPosition.magnitude);
        }
        // MOUSE
        else if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            Vector2 pos = Input.mousePosition;
            float delta = (pos - startPos).magnitude;

            if (Input.GetMouseButtonDown(0))
                HandleInput(TouchPhase.Began, pos, 0);
            else if (Input.GetMouseButton(0))
                HandleInput(TouchPhase.Moved, pos, delta);
            else
                HandleInput(TouchPhase.Ended, pos, delta);
        }

        orbitCamera.allowOrbit = (selected == null && !draggingBlock);
    }

    void HandleInput(TouchPhase phase, Vector2 screenPos, float delta)
    {
        if (phase == TouchPhase.Began)
        {
            startPos = screenPos;
            TrySelect(screenPos);
        }
        else if (phase == TouchPhase.Moved && selected && delta > dragThreshold)
        {
            if (!draggingBlock)
                BeginDrag(screenPos);

            UpdateDrag(screenPos);
        }
        else if (phase == TouchPhase.Ended)
        {
            EndDrag();
        }
    }

    void TrySelect(Vector2 pos)
    {
        Ray ray = cam.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, blockLayerMask))
        {
            var block = hit.collider.GetComponentInParent<Block>();
            if (block)
            {
                Select(block);
                return;
            }
        }

        Deselect();
    }

    void Select(Block block)
    {
        if (selected) selected.SetSelected(false);
        selected = block;
        selected.SetSelected(true);
    }

    void Deselect()
    {
        EndDrag();
        if (selected)
        {
            selected.SetSelected(false);
            selected = null;
        }
    }

    void BeginDrag(Vector2 pos)
    {
        Ray ray = cam.ScreenPointToRay(pos);
        Plane plane = new Plane(Vector3.up, Vector3.up * dragPlaneHeight);

        if (!plane.Raycast(ray, out float enter)) return;

        dragHandle.transform.position = ray.GetPoint(enter);

        joint = selected.gameObject.AddComponent<SpringJoint>();
        joint.connectedBody = dragHandleRb;
        joint.spring = 120f;
        joint.damper = 25f;
        joint.maxDistance = 0.1f;

        draggingBlock = true;
    }

    void UpdateDrag(Vector2 pos)
    {
        Ray ray = cam.ScreenPointToRay(pos);
        Plane plane = new Plane(Vector3.up, Vector3.up * dragPlaneHeight);

        if (plane.Raycast(ray, out float enter))
        {
            dragHandleRb.MovePosition(ray.GetPoint(enter));
        }
    }

    void EndDrag()
    {
        draggingBlock = false;
        if (joint) Destroy(joint);
    }
}
