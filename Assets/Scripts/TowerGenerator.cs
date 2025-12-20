using UnityEngine;

public class TowerGenerator : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject blockPrefab;

    [Header("Tower Defaults")]
    public int layers = 18;
    public float blocksPerLayer = 3;

    [Header("Block Dimensions")]
    public float blockLength = 1.5f;
    public float blockHeight = 0.28f;
    public float blockWidth = 0.48f;

    [Header("Spacing")]
    public float verticalGap = 0.005f;
    public float horizontalGap = 0.01f;

    [Header("Placement")]
    public Vector3 baseCenter = Vector3.zero;
    public bool freezeBottomLayer = true;

    [ContextMenu("Generate Tower")]
    public void GenerateTower()
    {
        if (blockPrefab == null)
        {
            Debug.LogError("whoopsies");
            return;
        }

        for (int i = transform.childCount -1; i >= 0; i--)
        {
            #if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(i).gameObject);
            #else
            Destroy(transform.GetChild(i).gameObject);
            #endif
        }

        float layerStepY = blockHeight + verticalGap;
        float startY = (blockHeight / 2f) + baseCenter.y;

        for (int layer = 0; layer < layers; layer++)
        {
            bool rotateLayer = layer % 2 == 0;
            float y = startY + layer * layerStepY;

            float step = blockWidth + horizontalGap;

            for (int i=0; i < blocksPerLayer; i++)
            {
                float offset = (i - 1) * step;

                Vector3 position;
                Quaternion rotation;

                if (!rotateLayer)
                {
                    position = new Vector3(baseCenter.x + offset, y, baseCenter.z);
                    rotation = Quaternion.Euler(0f, 90f, 0f);
                }
                else
                {
                    position = new Vector3(baseCenter.x, y, baseCenter.z + offset);
                    rotation = Quaternion.identity;
                }

                var block = Instantiate(blockPrefab, position, rotation, transform);
                block.name = $"Block_Layer{layer}_{i}";

                if (freezeBottomLayer && layer == 0)
                {
                    var rb = block.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                        rb.constraints = RigidbodyConstraints.FreezePositionX |
                                        RigidbodyConstraints.FreezePositionY |
                                        RigidbodyConstraints.FreezePositionZ |
                                        RigidbodyConstraints.FreezeRotationX |
                                        RigidbodyConstraints.FreezeRotationY |
                                        RigidbodyConstraints.FreezeRotationZ;
                    }
                }
            }
        }
    }
}