using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

// Database script for all relevant control actions
// This is for prototype, if for real game, should be seperate

public class UserControlManager : MonoBehaviour
{
    public GameObject selectedCharacter;
    public GameObject targetedTile;
    public GameObject hoveredEntity;

    public UserControlState currentState;
    public UserControlState IDLE = new UserControlState_Idle();
    public UserControlState UNITSELECTED = new UserControlState_UnitSelected();

    // Control Components for Toggle Enable Functionality
    public CA_HoverTileSelection CA_HoverTileSelection;
    public CA_MoveCharacter CA_MoveCharacter;


    //*** Build this later
    //public CommandQueue commandQueue;

    private void Start()
    {
        currentState = IDLE;

        input = GameObject.Find("InputSystem").GetComponent<InputSystem>().input;

        CA_HoverTileSelection = gameObject.AddComponent<CA_HoverTileSelection>();
        CA_HoverTileSelection.userControlManager = this;

        CA_MoveCharacter = gameObject.AddComponent<CA_MoveCharacter>();
        CA_MoveCharacter.userControlManager = this;
        CA_MoveCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
        

        if (camera == null)
            camera = Camera.main;

        if (hoverLayer == 0)
            hoverLayer = tileLayer;
    }

    public void Update()
    {
        currentState.Update(this);
        ScriptUpdate();

    }

    public void FixedUpdate()
    {
        currentState.FixedUpdate(this);
        ScriptFixedUpdate();
    }

    public void EnterState(UserControlState state)
    {
        currentState.Enter(this);
    }

    public void ExitState(UserControlState state)
    {
        currentState.Exit(this);
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
    public Ray _lastRay;
    public bool _hasRay;
    public bool _lastRayHit;
    public Vector3 _lastHitPoint;
    public float _lastRayLength = 100f;

    // Hover ray debug
    public Ray _hoverRay;
    public bool _hasHoverRay;
    public bool _hoverHit;
    public Vector3 _hoverHitPoint;
    public float _hoverRayLength = 100f;

    // Tile Management

    public GameObject selectedTile;

    [Header("Character Movement")]
    public bool enableCharacterMovement = true;


    public void ScriptUpdate()
    {
        if (input.Player.LeftClick.IsPressed())
        {
            HandleClick();
        }

        // manage control action (CA) toggle here

        if (enableHover)
        {
            CA_HoverTileSelection.HandleHover();
        }
        
    }

    public void ScriptFixedUpdate()
    {
        if (enableCharacterMovement)
        {
            CA_MoveCharacter.Move();
        }
    }

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

            // get game object hit
            selectedTile = hit.collider.gameObject;

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
