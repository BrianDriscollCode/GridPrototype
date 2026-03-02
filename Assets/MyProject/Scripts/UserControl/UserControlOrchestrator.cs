using System.Collections.Generic;
using UnityEngine;

// must rename to ControlOrchestrator, this has been relegated to
// running the whole battle state. Manages user and AI turns
// by managing IUSO_State.

public class UserControlOrchestrator : MonoBehaviour
{
    [Header("AI Component")]
    public EnemyAI enemyAI;

    [Header("Selection State")]
    public GameObject selectedCharacter;
    public GameObject selectedTile;
    public GameObject target;

    [Header("State Management")]
    public IUSO_State userControlState;
    public IUSO_State battle_PlayerTurn_State;
    public IUSO_State battle_EnemyTurn_State;

    [Header("System References")]
    public Camera camera;
    public InputSystem_Actions input;
    public GridManager gridManager;
    public ManagerRegistry managerRegistry;
    public string stateString;

    // IUSO_State grab this to do raycasts (on enter state)
    public InterfaceRaycastSelection interfaceRaycastSelection;
    public LayerMask characterHoverLayer;

    // Manager references shared with AI and states
    private MovementPointsManager movementPointsManager;
    private CharacterRegisterManager characterRegisterManager;

    private Stack<IUSO_State> stateStack = new Stack<IUSO_State>();

    public void PushState(IUSO_State state)
    {
        userControlState?.ExitState();
        stateStack.Push(userControlState);
        userControlState = state;
        userControlState?.EnterState(this);
    }

    public void PopState()
    {
        userControlState?.ExitState();
        userControlState = stateStack.Count > 0 ? stateStack.Pop() : null;
        userControlState?.EnterState(this); // re-enter so it resets CAs cleanly
    }

    public void Start()
    {
        InitializeManagers();
        InitializeRaycastSystem();
        InitializeStates();
        InitializeAI();
        
        
        // Start first turn
        SwitchState(battle_PlayerTurn_State);
    }

    private void InitializeManagers()
    {
        // Find input system
        input = GameObject.Find("InputSystem").GetComponent<InputSystem>().input;
        
        // Find or get manager registry
        if (managerRegistry == null)
        {
            managerRegistry = FindObjectOfType<ManagerRegistry>();
        }

        // Cache manager references
        movementPointsManager = FindManager<MovementPointsManager>();
        characterRegisterManager = FindManager<CharacterRegisterManager>();
        
        // GridManager should already be assigned in inspector
        if (gridManager == null)
        {
            gridManager = FindManager<GridManager>();
        }
    }

    private void InitializeRaycastSystem()
    {
        interfaceRaycastSelection = gameObject.AddComponent<InterfaceRaycastSelection>();
        interfaceRaycastSelection.camera = camera;
        interfaceRaycastSelection.gridManager = gridManager;
        interfaceRaycastSelection.input = input;
    }

    private void InitializeAI()
    {
        // Add EnemyAI component
        enemyAI = gameObject.AddComponent<EnemyAI>();
        
        // Hydrate AI with dependencies
        enemyAI.Initialize(
            gridManager,
            movementPointsManager,
            characterRegisterManager,
            battle_EnemyTurn_State as IUSO_Battle_EnemyTurn_State
        );
    }

    private void InitializeStates()
    {
        battle_PlayerTurn_State = new IUSO_Battle_PlayerTurn_State();
        battle_EnemyTurn_State = new IUSO_Battle_EnemyTurn_State();
    }

    private T FindManager<T>() where T : MonoBehaviour
    {
        if (managerRegistry == null) return null;
        
        GameObject managerObj = managerRegistry.managerList.Find(obj => obj.GetComponent<T>() != null);
        return managerObj?.GetComponent<T>();
    }

    public void Update()
    {
        stateString = userControlState?.ToString() ?? "None";
        userControlState?.Update();
    }

    public void FixedUpdate()
    {
        userControlState?.FixedUpdate();
    }

    public void SwitchState(IUSO_State state)
    {
        userControlState?.ExitState();
        userControlState = state;
        userControlState?.EnterState(this);
    }
}
