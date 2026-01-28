using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

// Database script for all relevant control actions
// This is for prototype, if for real game, should be seperate

// ********
// *** TODO - Add Scriptable Object for toggling/untoggling CA_Actions
// ********

public class UserControlManager : MonoBehaviour
{
    public GameObject selectedCharacter;
    public GameObject targetedTile;
    public GameObject hoveredEntity;

    public UserControlState currentState;
    [SerializeField] public string currentStateString;
    // rename to SELECTION
    public UserControlState SELECT = new UserControlState_Select();
    // rename to MOVE
    public UserControlState CHARACTERACTION = new UserControlState_CharacterMove();

    // Control Components for Toggle Enable Functionality
    public CA_HoverTileSelection CA_HoverTileSelection;
    public CA_MoveCharacter CA_MoveCharacter;
    public CA_SelectTileWithClick CA_SelectTileWithClick;
    public CA_IdleCharacter CA_IdleCharacter;

    public ControlActionsTogglerScriptableObject SelectionCA;
    public ControlActionsTogglerScriptableObject MoveCA;

    public ControlActionsTogglerScriptableObject currentControlMode;

    private void OnEnable()
    {
        EventManager.ClickedTile += HandleTileClicked;
        //Debug.Log("Listener: subscribed to ClickedTile");
    }

    private void OnDisable()
    {
        EventManager.ClickedTile -= HandleTileClicked;
        //Debug.Log("Listener: unsubscribed from ClickedTile");
    }

    private void HandleTileClicked(Vector2Int gridPos)
    {
        ExitState(currentState);
        EnterState(CHARACTERACTION);

    }

    //*** Build this later
    //public CommandQueue commandQueue;

    private void Start()
    {
        currentControlMode = SelectionCA;
        currentState = SELECT;
        UpdateStateString();

        input = GameObject.Find("InputSystem").GetComponent<InputSystem>().input;

        CA_HoverTileSelection = gameObject.AddComponent<CA_HoverTileSelection>();
        CA_HoverTileSelection.userControlManager = this;

        CA_MoveCharacter = gameObject.AddComponent<CA_MoveCharacter>();
        CA_MoveCharacter.userControlManager = this;
        CA_MoveCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
        CA_MoveCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
        selectedCharacter.GetComponent<PlayerAnim>().IdleAnimation();

        CA_SelectTileWithClick = gameObject.AddComponent<CA_SelectTileWithClick>();
        CA_SelectTileWithClick.userControlManager = this;

        CA_IdleCharacter = gameObject.AddComponent<CA_IdleCharacter>();
        CA_IdleCharacter.userControlManager = this;
        CA_IdleCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
        CA_IdleCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
        

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
        currentState = state;
        currentState.Enter(this);
        UpdateStateString();
    }

    public void ExitState(UserControlState state)
    {
        currentState.Exit(this);
    }

    private void UpdateStateString()
    {
        currentStateString = currentState?.GetType().Name ?? "None";
    }

    // UserControlState Parent Functionality

    [Header("Systems")]
    public Camera camera;
    public InputSystem_Actions input;
    public GridManager gridManager;

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
        if (input.Player.LeftClick.IsPressed() && currentControlMode.enableSelectTileWithClick)
        {
            CA_SelectTileWithClick.Action();
        }

        // manage control action (CA) toggle here

        if (enableHover && currentControlMode.enableHoverTileSelection)
        {
            CA_HoverTileSelection.Action();
        }
        
    }

    public void ScriptFixedUpdate()
    {
        if (enableCharacterMovement && currentControlMode.enableMoveCharacter)
        {
            CA_MoveCharacter.Action();
        }
        
        if (currentControlMode.enableIdleCharacter)
        {
            CA_IdleCharacter.Action();
        }
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
