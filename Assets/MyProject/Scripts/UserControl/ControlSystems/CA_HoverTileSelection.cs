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
    public UnityEngine.Material _hoveredMaterialInstance;
    public UnityEngine.Material _hoveredNoAccessMaterialInstance;
    private Color _originalColor;
    private Dictionary<GameObject, UnityEngine.Material> _originalMaterials = new Dictionary<GameObject, Material>();

    private ManagerRegistry managerRegistry;
    private GridManager gridManager;



    private void Start()
    {
        if (managerRegistry == null)
        {
            managerRegistry = FindObjectOfType<ManagerRegistry>();
        }

        gridManager = FindManager<GridManager>();
        // access materials from files
        _hoveredMaterialInstance = Resources.Load<UnityEngine.Material>("Materials/grid_tile_top_512_thicklines_lightouterPurp");
        _hoveredNoAccessMaterialInstance = Resources.Load<UnityEngine.Material>("Materials/grid_tile_top_512_thicklines_Red");

        if (_hoveredMaterialInstance == null)
        {
            Debug.LogError("Failed to load hover material! Check path and Resources folder.");
        }
        else
        {
            //Debug.Log"Succes to load hover material! Not error!");
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
            ////Debug.Log"entered");
            interfaceRaycastSelection._hoverHit = true;
            interfaceRaycastSelection._hoverHitPoint = hit.point;
            interfaceRaycastSelection._hoverRayLength = hit.distance;

            GameObject hitObject = hit.collider.gameObject;
            Vector2Int gridPos = interfaceRaycastSelection.gridManager.WorldToGridPosition(hit.point);

            // Check if we started hovering a new object
            if (interfaceRaycastSelection._currentHoveredObject != hitObject)
            {
                ////Debug.Log"entered more");
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
            ////Debug.Log"elsed");
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
        if (gridManager == null)
        {
            if (managerRegistry == null)
                managerRegistry = FindObjectOfType<ManagerRegistry>();

            gridManager = FindManager<GridManager>();

            if (gridManager == null)
            {
                Debug.LogError("CA_HoverTileSelection: Failed to find GridManager!");
                return;
            }
        }

        Color highlightColor = new Color(0.2f, 0.2f, 0.2f, 1f); // Dark gray
        Color rimLightColor = new Color(0.3f, 0.3f, 0.3f, 1f); // Ligh
        float rimPower = 3f;

        gridManager.GetComponent<HighlightGridTile>()
            .HighlightTileWithRim(obj, highlightColor, rimLightColor, rimPower, HighlightGridTile.HighlightType.Hover);
    }

    protected virtual void OnHoverExit(GameObject obj, Vector2Int gridPos)
    {
        // Only remove hover highlight - movement range highlights remain
        gridManager.GetComponent<HighlightGridTile>()
            .RemoveHighlight(obj, HighlightGridTile.HighlightType.Hover);
    }

    // Need to make a helper for all scripts
    private T FindManager<T>() where T : MonoBehaviour
    {
        if (managerRegistry == null) return null;

        GameObject managerObj = managerRegistry.managerList.Find(obj => obj.GetComponent<T>() != null);
        return managerObj?.GetComponent<T>();
    }
}
