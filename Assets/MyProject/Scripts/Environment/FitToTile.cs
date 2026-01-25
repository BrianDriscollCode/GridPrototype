using UnityEngine;

[ExecuteAlways]
public class FitToTile : MonoBehaviour
{
    [Tooltip("Target width in world units (matches your 1x1 plane tile).")]
    public float targetSize = 4f;

    [Tooltip("Fit using X (width) and Z (depth). Typical for floor-aligned props.")]
    public bool fitXZ = true;

    void Update()
    {
        var rend = GetComponentInChildren<Renderer>();
        if (rend == null) return;

        var bounds = rend.bounds;

        float sizeX = bounds.size.x;
        float sizeZ = bounds.size.z;

        // Prevent division by zero
        if (sizeX <= 0.0001f || sizeZ <= 0.0001f) return;

        float scaleFactor;

        if (fitXZ)
        {
            // Make BOTH X and Z fit inside 1x1.
            // Uses the largest dimension so it never spills over.
            float max = Mathf.Max(sizeX, sizeZ);
            scaleFactor = targetSize / max;
        }
        else
        {
            // Just fit X to 1 unit
            scaleFactor = targetSize / sizeX;
        }

        transform.localScale *= scaleFactor;

        // stop it from applying every frame in editor
        enabled = false;
    }
}
