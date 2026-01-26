using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CA_HoverTileSelection : MonoBehaviour
{
    public UserControlManager userControlManager;

    // Managing material and color swaps
    public Material _hoveredMaterialInstance;
    private Color _originalColor;
    private Dictionary<GameObject, Material> _originalMaterials = new Dictionary<GameObject, Material>();


    private void Start()
    {
        // access materials from files
        _hoveredMaterialInstance = Resources.Load<Material>("Materials/grid_tile_top_512_thicklines_lightouterPurp");

        if (_hoveredMaterialInstance == null)
        {
            Debug.LogError("Failed to load hover material! Check path and Resources folder.");
        }
        else
        {
            Debug.LogError("Succes to load hover material! Not error!");
        }
    }
    public void HandleHover()
    {
        // Raycast from camera center (like a crosshair)
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = userControlManager.camera.ScreenPointToRay(mousePos);
        userControlManager._hoverRay = ray;
        userControlManager._hasHoverRay = true;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, userControlManager.hoverLayer))
        {
            userControlManager._hoverHit = true;
            userControlManager._hoverHitPoint = hit.point;
            userControlManager._hoverRayLength = hit.distance;

            GameObject hitObject = hit.collider.gameObject;
            Vector2Int gridPos = userControlManager.gridSystem.WorldToGridPosition(hit.point);

            // Check if we started hovering a new object
            if (userControlManager._currentHoveredObject != hitObject)
            {
                // Exit previous hover
                if (userControlManager._currentHoveredObject != null)
                {
                    OnHoverExit(userControlManager._currentHoveredObject, userControlManager._currentHoveredGridPos);
                }

                // Enter new hover
                userControlManager._currentHoveredObject = hitObject;
                userControlManager._currentHoveredGridPos = gridPos;
                OnHoverEnter(hitObject, gridPos);
            }
        }
        else
        {
            userControlManager._hoverHit = false;
            userControlManager._hoverRayLength = 100f;

            // Clear hover if we had one
            if (userControlManager._currentHoveredObject != null)
            {
                OnHoverExit(userControlManager._currentHoveredObject, userControlManager._currentHoveredGridPos);
                userControlManager._currentHoveredObject = null;
                userControlManager._currentHoveredGridPos = new Vector2Int(-1, -1);
            }
        }


    }

    protected virtual void OnHoverEnter(GameObject obj, Vector2Int gridPos)
    {
        Debug.Log($"Hover Enter: {obj.name} at grid ({gridPos.x}, {gridPos.y})");


        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Store original if not already stored
            if (!_originalMaterials.ContainsKey(obj))
                _originalMaterials[obj] = renderer.material;

            renderer.material = _hoveredMaterialInstance;
        }
    }


    protected virtual void OnHoverExit(GameObject obj, Vector2Int gridPos)
    {
        Debug.Log($"Hover Exit: {obj.name} at grid ({gridPos.x}, {gridPos.y})");

        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null && _originalMaterials.ContainsKey(obj))
        {
            renderer.material = _originalMaterials[obj];
        }
    }
}
