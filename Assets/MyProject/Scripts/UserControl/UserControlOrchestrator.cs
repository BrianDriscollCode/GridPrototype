using UnityEngine;

public class UserControlOrchestrator : MonoBehaviour
{
    // Enabling Raycast Selection
    public GameObject selectedCharacter;
    public GameObject selectedTile;

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


    public void Start()
    {
        input = GameObject.Find("InputSystem").GetComponent<InputSystem>().input;

        interfaceRaycastSelection = gameObject.AddComponent<InterfaceRaycastSelection>();
        interfaceRaycastSelection.camera = camera;
        interfaceRaycastSelection.gridManager = gridManager;
        interfaceRaycastSelection.input = input;

        battle_PlayerTurn_State = new IUSO_Battle_PlayerTurn_State();
        battle_EnemyTurn_State = new IUSO_Battle_EnemyTurn_State();
        SwitchState(battle_PlayerTurn_State);
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
        userControlState.EnterState(this);
    }

}
