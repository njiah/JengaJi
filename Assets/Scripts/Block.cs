using UnityEngine;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(BoxCollider))]

public class Block : MonoBehaviour
{
    [Header("Selection Visuals")]
    public Renderer[] renderers;
    public Material normalMaterial;
    public Material selectedMaterial;

    Rigidbody rb;
    bool isSelected;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (renderers == null || renderers.Length == 0)
        {
            var r = GetComponentsInChildren<Renderer>();
            if (r != null) renderers = r;
        }
    }

    public Rigidbody Rigidbody => rb;

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (renderers == null) return;
        if (selectedMaterial == null || normalMaterial == null) return;

        foreach (var renderer in renderers)
        {
            if (renderer == null) continue;
            renderer.material = isSelected ? selectedMaterial : normalMaterial;
        }
    }
}