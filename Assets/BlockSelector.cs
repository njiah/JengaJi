using UnityEngine;  

public class BlockSelector : MonoBehaviour
{
    public float pushForce = 5f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("JengaBlock"))
                {
                    Rigidbody blockRb = hit.collider.GetComponent<Rigidbody>();
                    Vector3 pushDirection = hit.point - transform.position;

                    blockRb.isKinematic = true;
                    blockRb.AddForce(pushDirection.normalized * pushForce, ForceMode.Impulse);
                }
            }
        }
    }
}