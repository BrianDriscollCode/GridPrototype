using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

public class UserControlManager : MonoBehaviour
{
    public GameObject selectedCharacter;
    public GameObject targetedTile;
    public GameObject hoveredEntity;

    public UserControlState currentState;
    public UserControlState IDLE = new UserControlState_Idle();
    public UserControlState UNITSELECTED = new UserControlState_UnitSelected();


    //*** Build this later
    //public CommandQueue commandQueue;

    private void Start()
    {
        currentState = IDLE;

        input = GameObject.Find("InputSystem").GetComponent<InputSystem>().input;

        if (camera == null)
            camera = Camera.main;

        if (hoverLayer == 0)
            hoverLayer = tileLayer;
    }

    public void Update()
    {
        currentState.Update();
        ScriptUpdate();

    }

    public void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    public void EnterState(UserControlState state)
    {
        currentState.Enter();
    }

    public void ExitState(UserControlState state)
    {
        currentState.Exit();
    }

    // UserControlState Parent Functionality

    [Header("Systems")]
    public Camera camera;
    public InputSystem_Actions input;
    public GridManager gridSystem;

    [Header("Collisions")]
    public LayerMask tileLayer;

    [Header("Hover Detection")]
    public bool enableHover = true;
    public LayerMask hoverLayer; // Set in Inspector (can use same as tileLayer)

    public GameObject _currentHoveredObject;
    public Vector2Int _currentHoveredGridPos = new Vector2Int(-1, -1);

    public Material _originalMaterial;
    public Renderer _hoveredRenderer;

    // --- Gizmo Debug Data ---
    private Ray _lastRay;
    private bool _hasRay;
    private bool _lastRayHit;
    private Vector3 _lastHitPoint;
    private float _lastRayLength = 100f;

    // Hover ray debug
    private Ray _hoverRay;
    private bool _hasHoverRay;
    private bool _hoverHit;
    private Vector3 _hoverHitPoint;
    private float _hoverRayLength = 100f;

    public void ScriptUpdate()
    {
        if (input.Player.LeftClick.IsPressed())
        {
            HandleClick();
        }

        if (enableHover)
        {
            HandleHover();
        }
    }

    void HandleHover()
    {
        // Raycast from camera center (like a crosshair)
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = camera.ScreenPointToRay(mousePos);
        _hoverRay = ray;
        _hasHoverRay = true;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hoverLayer))
        {
            _hoverHit = true;
            _hoverHitPoint = hit.point;
            _hoverRayLength = hit.distance;

            GameObject hitObject = hit.collider.gameObject;
            Vector2Int gridPos = gridSystem.WorldToGridPosition(hit.point);

            // Check if we started hovering a new object
            if (_currentHoveredObject != hitObject)
            {
                // Exit previous hover
                if (_currentHoveredObject != null)
                {
                    OnHoverExit(_currentHoveredObject, _currentHoveredGridPos);
                }

                // Enter new hover
                _currentHoveredObject = hitObject;
                _currentHoveredGridPos = gridPos;
                OnHoverEnter(hitObject, gridPos);
            }
        }
        else
        {
            _hoverHit = false;
            _hoverRayLength = 100f;

            // Clear hover if we had one
            if (_currentHoveredObject != null)
            {
                OnHoverExit(_currentHoveredObject, _currentHoveredGridPos);
                _currentHoveredObject = null;
                _currentHoveredGridPos = new Vector2Int(-1, -1);
            }
        }

    }

    private Color _originalColor;
    public Material _hoveredMaterialInstance;
    private Dictionary<GameObject, Material> _originalMaterials = new Dictionary<GameObject, Material>();

    
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
        //if (renderer != null)
        //{
        //    _hoveredRenderer = renderer;

        //    // Get the material instance ONCE and store it
        //    _hoveredMaterialInstance = renderer.material;

        //    // Now store the original color from this instance
        //    _originalColor = _hoveredMaterialInstance.color;

        //    // Change the color on the same instance
        //    _hoveredMaterialInstance.color = Color.red;
        //}
    }

    // Override these methods to implement your hover behavior
    //protected virtual void OnHoverEnter(GameObject obj, Vector2Int gridPos)
    //{
    //    Debug.Log($"Hover Enter: {obj.name} at grid ({gridPos.x}, {gridPos.y})");
    //    // Add your hover enter logic here (e.g., highlight tile)

    //    // Better approach: Use the Renderer's material (simple and works immediately)
    //    Renderer renderer = obj.GetComponent<Renderer>();
    //    if (renderer != null)
    //    {
    //        _hoveredRenderer = renderer;
    //        // Store the original material (creates an instance automatically)
    //        _originalMaterial = renderer.material;
    //        // Change the material color (easy and effective)
    //        renderer.material.color = Color.red;

    //        // Or enable emission for a glow effect (if using Standard shader)
    //        // renderer.material.EnableKeyword("_EMISSION");
    //        // renderer.material.SetColor("_EmissionColor", Color.red * 2f);
    //    }
    //}

    protected virtual void OnHoverExit(GameObject obj, Vector2Int gridPos)
    {
        Debug.Log($"Hover Exit: {obj.name} at grid ({gridPos.x}, {gridPos.y})");

        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null && _originalMaterials.ContainsKey(obj))
        {
            renderer.material = _originalMaterials[obj];
        }
    }

    //protected virtual void OnHoverExit(GameObject obj, Vector2Int gridPos)
    //{
    //    Debug.Log($"Hover Exit: {obj.name} at grid ({gridPos.x}, {gridPos.y})");

    //    // Restore the original material
    //    if (_hoveredRenderer != null && _originalMaterial != null)
    //    {
    //        _hoveredRenderer.material = _originalMaterial;
    //        _hoveredRenderer.material.color = Color.violet;
    //        _hoveredRenderer = null;
    //        _originalMaterial = null;
    //    }
    //}

    void HandleClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = camera.ScreenPointToRay(mousePos);

        _lastRay = ray;
        _hasRay = true;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileLayer))
        {
            _lastRayHit = true;
            _lastHitPoint = hit.point;
            _lastRayLength = hit.distance;

            Vector2Int gridPos = gridSystem.WorldToGridPosition(hit.point);
            Debug.Log($"UserControlInterface::Clicked grid position: ({gridPos.x}, {gridPos.y})");
            Debug.Log("UserControlInterface::HasTileAt: " + gridSystem.HasTileAt(gridPos.x, gridPos.y));

            EventManager.OnClickedTile(gridPos);
        }
        else
        {
            Debug.Log("No hit");
            _lastRayHit = false;
            _lastRayLength = 100f;
        }

        Debug.DrawRay(ray.origin, ray.direction * _lastRayLength,
            _lastRayHit ? Color.green : Color.red, 0.2f);
    }

    private void OnDrawGizmos()
    {
        // Click ray
        if (_hasRay)
        {
            Gizmos.color = _lastRayHit ? Color.green : Color.red;
            Gizmos.DrawLine(_lastRay.origin, _lastRay.origin + _lastRay.direction * _lastRayLength);

            if (_lastRayHit)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(_lastHitPoint, 0.15f);
            }
        }

        // Hover ray
        if (_hasHoverRay)
        {
            Gizmos.color = _hoverHit ? Color.cyan : Color.magenta;
            Gizmos.DrawLine(_hoverRay.origin, _hoverRay.origin + _hoverRay.direction * _hoverRayLength);

            if (_hoverHit)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_hoverHitPoint, 0.25f);
            }
        }
    }

}
