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
    //New ControlManager
    public UserControlOrchestrator userControlOrchestrator;

    public GameStateManager gameStateManager;
    public IGameState currentGameState;

    public GameObject selectedCharacter;
    public GameObject targetedTile;
    public GameObject hoveredCharacter;

    public UserControlState currentState;
    [SerializeField] public string currentStateString;

    // Gates actions for user control
    public UserControlState SELECT = new UserControlState_Select();
    public UserControlState CHARACTERACTION = new UserControlState_CharacterMove();

    // Control Components for Toggle Enable Functionality
    public CA_HoverTileSelection CA_HoverTileSelection;
    public CA_MoveCharacter CA_MoveCharacter;
    public CA_SelectTileWithClick CA_SelectTileWithClick;
    public CA_IdleCharacter CA_IdleCharacter;
    public CA_HoverCharacter CA_HoverCharacter;
    public CA_SelectCharacterWithClick CA_SelectCharacterWithClick;

    // Config for enabling/disabling Control Actions (CA)
    public ControlActionsTogglerScriptableObject SelectionCA;
    public ControlActionsTogglerScriptableObject MoveCA;

    public ControlActionsTogglerScriptableObject currentControlMode;

    private void OnEnable()
    {
        EventManager.ClickedTile += HandleTileClicked;
        ////Debug.Log"Listener: subscribed to ClickedTile");
    }

    private void OnDisable()
    {
        EventManager.ClickedTile -= HandleTileClicked;
        ////Debug.Log"Listener: unsubscribed from ClickedTile");
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
        currentGameState = gameStateManager.currentState;
        currentControlMode = SelectionCA;
        currentState = SELECT;
        UpdateStateString();

        input = GameObject.Find("InputSystem").GetComponent<InputSystem>().input;

        CA_HoverTileSelection = gameObject.AddComponent<CA_HoverTileSelection>();
        //CA_HoverTileSelection.userControlManager = this;

        CA_HoverCharacter = gameObject.AddComponent<CA_HoverCharacter>();
        CA_HoverCharacter.userControlManager = this;

        CA_MoveCharacter = gameObject.AddComponent<CA_MoveCharacter>();
        //CA_MoveCharacter.userControlManager = this;
        CA_MoveCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
        CA_MoveCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
        selectedCharacter.GetComponent<PlayerAnim>().IdleAnimation();

        CA_SelectTileWithClick = gameObject.AddComponent<CA_SelectTileWithClick>();
        // No Longer functional as part of deprecation process
        //CA_SelectTileWithClick.userControlManager = this;

        CA_IdleCharacter = gameObject.AddComponent<CA_IdleCharacter>();
        CA_IdleCharacter.userControlManager = this;
        CA_IdleCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
        CA_IdleCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();

        CA_SelectCharacterWithClick = gameObject.AddComponent<CA_SelectCharacterWithClick>();
        //CA_SelectCharacterWithClick.userControlOrchestrator = this;

        if (camera == null)
            camera = Camera.main;

        if (hoverLayer == 0)
            hoverLayer = tileLayer;
    }

    // Trying to figure out how to state machine this a bit better
    // Need to start getting the logic a little diff based on the config
    // Or at the very least, have decided on a path and follow it.

    public void Update()
    {
        currentState.Update(this);

        if (currentGameState == gameStateManager.Battle)
        {
            BattleControlsUpdate();
        }

    }

    public void FixedUpdate()
    {
        currentState.FixedUpdate(this);

        if (currentGameState == gameStateManager.Battle)
        {
            BattleControlsFixedUpdate();
        }
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

    [Header("Hover Character Detection")]
    public bool enableCharacterHover;
    public LayerMask characterHoverLayer;

    public GameObject _currentCharacterSelected;
    public Vector2Int _currentCharacterGridPos;

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


    public void BattleControlsUpdate()
    {
        bool isRaycastHittingPlayer = CA_HoverCharacter.isHittingCharacter;

        if (input.Player.LeftClick.IsPressed() && currentControlMode.enableSelectTileWithClick
            && !isRaycastHittingPlayer)
        {
            CA_SelectTileWithClick.Action();
        }

        if (input.Player.LeftClick.IsPressed() && currentControlMode.enableCharacterClick)
        {
            CA_SelectCharacterWithClick.Action();
        }

        // manage control action (CA) toggle here

        if (enableHover && currentControlMode.enableHoverTileSelection && !isRaycastHittingPlayer)
        {
            CA_HoverTileSelection.Action();
        }

        if (enableCharacterHover && currentControlMode.enableCharacterHover)
        {
            CA_HoverCharacter.Action();
        }
    }

    public void BattleControlsFixedUpdate()
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
