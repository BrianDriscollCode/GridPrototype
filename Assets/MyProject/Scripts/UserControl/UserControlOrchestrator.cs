using UnityEngine;

// must rename to ControlOrchestrator, this has been relegated to
// running the whole battle state. Manages user and AI turns
// by managin IUSO_State.

public class UserControlOrchestrator : MonoBehaviour
{
    public EnemyAI enemyAI;

    // Enabling Raycast Selection
    public GameObject selectedCharacter;
    public GameObject selectedTile;

    // Target
    public GameObject target;

    // State
    public IUSO_State userControlState;
    public IUSO_State battle_PlayerTurn_State;
    public IUSO_State battle_EnemyTurn_State;

    // Enable Raycast Selection
    [Header("System References")]
    public Camera camera;
    public InputSystem_Actions input;
    public GridManager gridManager;
    public string stateString;

    // IUSO_State grab this to do raycasts (on enter state)
    public InterfaceRaycastSelection interfaceRaycastSelection;

    public LayerMask characterHoverLayer;


    public void Start()
    {
        input = GameObject.Find("InputSystem").GetComponent<InputSystem>().input;

        // Manage Raycast Func
        interfaceRaycastSelection = gameObject.AddComponent<InterfaceRaycastSelection>();
        interfaceRaycastSelection.camera = camera;
        interfaceRaycastSelection.gridManager = gridManager;
        interfaceRaycastSelection.input = input;

        // Establishing AI controls
        enemyAI = gameObject.AddComponent<EnemyAI>();
        
        // Manage State
        battle_PlayerTurn_State = new IUSO_Battle_PlayerTurn_State();
        battle_EnemyTurn_State = new IUSO_Battle_EnemyTurn_State();
        SwitchState(battle_EnemyTurn_State);
    }

    public void Update()
    {
        stateString = userControlState.ToString();
        userControlState.Update();
    }

    public void FixedUpdate()
    {
        userControlState.FixedUpdate();
    }

    public void SwitchState(IUSO_State state)
    {
        if (userControlState != null)
            userControlState.ExitState();      
        userControlState = state;
        
        // Set enemyAI BEFORE calling EnterState
        if (userControlState is IUSO_Battle_EnemyTurn_State enemyState)
        {
            enemyState.enemyAI = enemyAI;
        }
        
        userControlState.EnterState(this);
    }

}
