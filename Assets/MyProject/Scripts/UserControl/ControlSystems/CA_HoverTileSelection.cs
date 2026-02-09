using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CA_HoverTileSelection : MonoBehaviour
{
    // new User Control Manager
    public UserControlOrchestrator userControlOrchestrator;

    // will be deprecated
    public UserControlManager userControlManager;

    // Access Raycast Functionality
    public InterfaceRaycastSelection interfaceRaycastSelection;

    // Managing material and color swaps
    public Material _hoveredMaterialInstance;
    public Material _hoveredNoAccessMaterialInstance;
    private Color _originalColor;
    private Dictionary<GameObject, Material> _originalMaterials = new Dictionary<GameObject, Material>();


    private void Start()
    {
        // access materials from files
        _hoveredMaterialInstance = Resources.Load<Material>("Materials/grid_tile_top_512_thicklines_lightouterPurp");
        _hoveredNoAccessMaterialInstance = Resources.Load<Material>("Materials/grid_tile_top_512_thicklines_Red");

        if (_hoveredMaterialInstance == null)
        {
            Debug.LogError("Failed to load hover material! Check path and Resources folder.");
        }
        else
        {
            Debug.LogError("Succes to load hover material! Not error!");
        }
    }

    public void Action()
    {
        HandleHover();
    }
    public void HandleHover()
    {
        // Raycast from camera center (like a crosshair)
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = userControlOrchestrator.camera.ScreenPointToRay(mousePos);
        interfaceRaycastSelection._hoverRay = ray;
        interfaceRaycastSelection._hasHoverRay = true;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interfaceRaycastSelection.hoverLayer))
        {
            Debug.Log("entered");
            interfaceRaycastSelection._hoverHit = true;
            interfaceRaycastSelection._hoverHitPoint = hit.point;
            interfaceRaycastSelection._hoverRayLength = hit.distance;

            GameObject hitObject = hit.collider.gameObject;
            Vector2Int gridPos = interfaceRaycastSelection.gridManager.WorldToGridPosition(hit.point);

            // Check if we started hovering a new object
            if (interfaceRaycastSelection._currentHoveredObject != hitObject)
            {
                Debug.Log("entered more");
                // Exit previous hover
                if (interfaceRaycastSelection._currentHoveredObject != null)
                {
                    OnHoverExit(interfaceRaycastSelection._currentHoveredObject, interfaceRaycastSelection._currentHoveredGridPos);
                    
                }

                // Enter new hover
                interfaceRaycastSelection._currentHoveredObject = hitObject;
                interfaceRaycastSelection._currentHoveredGridPos = gridPos;
                OnHoverEnter(hitObject, gridPos);
            }
        }
        else
        {
            Debug.Log("elsed");
            interfaceRaycastSelection._hoverHit = false;
            interfaceRaycastSelection._hoverRayLength = 100f;

            // Clear hover if we had one
            if (interfaceRaycastSelection._currentHoveredObject != null)
            {
                OnHoverExit(interfaceRaycastSelection._currentHoveredObject, interfaceRaycastSelection._currentHoveredGridPos);
                interfaceRaycastSelection._currentHoveredObject = null;
                interfaceRaycastSelection._currentHoveredGridPos = new Vector2Int(-1, -1);
            }
        }


    }
    private void OnDestroy()
    {
        // Clean up hover state when component is destroyed
        ClearHoverState();
    }

    public void ClearHoverState()
    {
        // Exit hover on current object if any
        if (interfaceRaycastSelection != null && interfaceRaycastSelection._currentHoveredObject != null)
        {
            OnHoverExit(interfaceRaycastSelection._currentHoveredObject, interfaceRaycastSelection._currentHoveredGridPos);
            interfaceRaycastSelection._currentHoveredObject = null;
            interfaceRaycastSelection._currentHoveredGridPos = new Vector2Int(-1, -1);
        }
    }

    protected virtual void OnHoverEnter(GameObject obj, Vector2Int gridPos)
    {
        //Debug.Log($"Hover Enter: {obj.name} at grid ({gridPos.x}, {gridPos.y})");


        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Store original if not already stored
            if (!_originalMaterials.ContainsKey(obj))
                _originalMaterials[obj] = renderer.material;

            if(userControlOrchestrator.gridManager.levelData.IsAccessible(gridPos.x, gridPos.y))
            {
                // Switch to correct material based on tile accessibility
                renderer.material = _hoveredMaterialInstance;
            }
            else
            {
                // Switch to correct material based on tile accessibility
                renderer.material = _hoveredNoAccessMaterialInstance;
            }
        }
    }


    protected virtual void OnHoverExit(GameObject obj, Vector2Int gridPos)
    {
        //Debug.Log($"Hover Exit: {obj.name} at grid ({gridPos.x}, {gridPos.y})");

        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null && _originalMaterials.ContainsKey(obj))
        {
            renderer.material = _originalMaterials[obj];
        }
    }
}
