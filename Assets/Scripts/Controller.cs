using Unity.VisualScripting;
using UnityEngine;

public class TouchInputController : MonoBehaviour
{
    [Header("References")]
    public Camera cam;
    public TouchOrbitCamera orbitCamera;

    [Header("Raycast")]
    public LayerMask blockLayerMask;

    [Header("Drag Plane")]
    public float dragPlaneHeight = 0f;  

    [Header("Tap vs Drag")]
    public float dragThresholdPixels = 30f;

    [Header("Drag Physics")]
    public float spring = 60f;
    public float damper = 30f;
    public float maxDistance = 0.22f;

    Block selected;
    bool draggingBlock;

    Vector2 pointerStart;
    bool pointerDown;

    // Drag handle + joint
    Rigidbody dragHandleRb;
    GameObject dragHandle;
    SpringJoint joint;

    Vector3 localGrabAnchor;

    public bool constrainPullToLongAxis = true;
    public float maxPullDistance = 1.0f;
    Vector3 dragStartHandlePosition;
    Vector3 dragStartBlockPosition;

    void Awake()
    {
        if (!cam) cam = Camera.main;

        dragHandle = new GameObject("DragHandle");
        dragHandleRb = dragHandle.AddComponent<Rigidbody>();
        dragHandleRb.isKinematic = true;
    }

    void Update()
    {
        if (!cam || !orbitCamera) return;

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            HandlePointer(t.phase, t.position, t.deltaPosition);

            if (Input.touchCount >= 2)
            {
                Touch a = Input.GetTouch(0);
                Touch b = Input.GetTouch(1);

                Vector2 aPrev = a.position - a.deltaPosition;
                Vector2 bPrev = b.position - b.deltaPosition;

                float prevDist = Vector2.Distance(aPrev, bPrev);
                float currDist = Vector2.Distance(a.position, b.position);

                float pinchDelta = currDist - prevDist; // pixels
                orbitCamera.ApplyZoomDelta(pinchDelta * 0.01f);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                pointerDown = true;
                Vector2 pos = Input.mousePosition;
                pointerStart = pos;
                HandlePointer(TouchPhase.Began, pos, Vector2.zero);
            }
            else if (Input.GetMouseButton(0) && pointerDown)
            {
                Vector2 pos = Input.mousePosition;
                Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 12f;
                HandlePointer(TouchPhase.Moved, pos, delta);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                pointerDown = false;
                HandlePointer(TouchPhase.Ended, (Vector2)Input.mousePosition, Vector2.zero);
            }

            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                orbitCamera.ApplyZoomDelta(scroll * 0.6f);
            }
        }

        orbitCamera.allowOrbit = (selected == null && !draggingBlock);
    }

    void HandlePointer(TouchPhase phase, Vector2 screenPos, Vector2 delta)
    {
        if (phase == TouchPhase.Began)
        {
            pointerStart = screenPos;

            if (!TrySelect(screenPos))
            {
                Deselect();
            }

            return;
        }

        if (phase == TouchPhase.Moved || phase == TouchPhase.Stationary)
        {
            if (selected != null)
            {
                float moved = (screenPos - pointerStart).magnitude;

                if (!draggingBlock && moved >= dragThresholdPixels)
                {
                    BeginDrag(screenPos);
                }

                if (draggingBlock)
                {
                    UpdateDrag(screenPos);
                }
            }
            else
            {
                // Orbit camera if no block selected
                orbitCamera.ApplyOrbitDelta(delta);
            }

            return;
        }

        if (phase == TouchPhase.Ended || phase == TouchPhase.Canceled)
        {
            EndDrag();
        }
    }

    bool TrySelect(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, blockLayerMask))
        {
            var block = hit.collider.GetComponentInParent<Block>();
            if (block == null) return false;

            Select(block);

            localGrabAnchor = block.transform.InverseTransformPoint(hit.point);
            return true;
        }

        return false;
    }

    void Select(Block block)
    {
        if (selected != null) selected.SetSelected(false);
        selected = block;
        selected.SetSelected(true);
    }

    void Deselect()
    {
        EndDrag();
        if (selected != null)
        {
            selected.SetSelected(false);
            selected = null;
        }
    }

    void BeginDrag(Vector2 screenPos)
    {
        if (selected == null) return;
        EndDrag();

        Vector3 grabWorld = selected.transform.TransformPoint(localGrabAnchor);
        Vector3 handleStart = new Vector3(grabWorld.x, dragPlaneHeight, grabWorld.z);

        dragHandle.transform.position = handleStart;
        dragHandleRb.position = handleStart;

        dragStartHandlePosition = handleStart;
        dragStartBlockPosition = selected.transform.position;

        joint = selected.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedBody = dragHandleRb;

        joint.anchor = localGrabAnchor;      
        joint.connectedAnchor = Vector3.zero;

        joint.spring = spring;              
        joint.damper = damper;               
        joint.maxDistance = maxDistance;    
        joint.enablePreprocessing = false;

        draggingBlock = true;
    }

    void UpdateDrag(Vector2 screenPos)
    {
        Vector3 worldPointOnPlane = ScreenToPlanePoint(screenPos);

        if (selected == null || !constrainPullToLongAxis)
        {
            Vector3 nextFree = Vector3.Lerp(dragHandleRb.position, worldPointOnPlane,
                1f - Mathf.Exp(-12f * Time.deltaTime));
            dragHandleRb.MovePosition(nextFree);
            return;
        }

        Vector3 axis = selected.transform.right;
        axis.y = 0f;
        axis = axis.normalized;

        Vector3 fromStart = worldPointOnPlane - dragStartHandlePosition;
        float along = Vector3.Dot(fromStart, axis);

        along = Mathf.Clamp(along, 0f, maxPullDistance);

        Vector3 constrained = dragStartHandlePosition + axis * along;
        constrained.y = dragPlaneHeight;

        Vector3 next = Vector3.Lerp(dragHandleRb.position, constrained,
            1f - Mathf.Exp(-10f * Time.deltaTime));
        dragHandleRb.MovePosition(next);
    }

    Vector3 ScreenToPlanePoint(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        Plane plane = new Plane(Vector3.up, new Vector3(0f, dragPlaneHeight, 0f));

        if (plane.Raycast(ray, out float enter))
            return ray.GetPoint(enter);

        return dragHandle.transform.position;
    }

    void EndDrag()
    {
        draggingBlock = false;
        if (joint != null)
        {
            Destroy(joint);
            joint = null;
        }
    }
}
