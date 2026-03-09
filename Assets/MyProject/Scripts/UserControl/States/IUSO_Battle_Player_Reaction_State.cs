using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class IUSO_Battle_Player_Reaction_State : IUSO_State
{
    private UserControlOrchestrator userControlOrchestrator;
    private InputSystem_Actions input;
    private EnemyAI enemyAI;
    private InterfaceRaycastSelection interfaceRaycastSelection;
    private ECharacterPhase characterPhase;

    private CA_IdleCharacter CA_IdleCharacter;
    private CA_MoveCharacter CA_MoveCharacter;
    private CA_HoverTileSelection CA_HoverTileSelection;
    private CA_HoverCharacter CA_HoverCharacter;
    private CA_SelectCharacterWithClick CA_SelectCharacterWithClick;
    private CA_SelectTileWithClick CA_SelectTileWithClick;
    private CA_BasicMeeleAttack CA_BasicMeeleAttack;
    private List<MonoBehaviour> allControlActions;

    private PlayerStateHelper playerStateHelper;


    public void EnterState(UserControlOrchestrator USO)
    {
        characterPhase = ECharacterPhase.IDLE;
        allControlActions = new List<MonoBehaviour>();
        playerStateHelper = new PlayerStateHelper();
        userControlOrchestrator = USO;
        input = userControlOrchestrator.input;
        enemyAI = userControlOrchestrator.enemyAI;
        interfaceRaycastSelection = userControlOrchestrator.interfaceRaycastSelection;
        UIEventManager.MoveButtonClicked += HandleMoveButtonClicked;

        CreateCA(E_CA_Type.IDLE_CHARACTER);
        CreateCA(E_CA_Type.MOVE_CHARACTER);
        CreateCA(E_CA_Type.HOVER_TILE_SELECTION);
        CreateCA(E_CA_Type.HOVER_CHARACTER);
        CreateCA(E_CA_Type.MOVE_CHARACTER);
        enemyAI.currentTarget.GetComponent<PlayerAnim>().IdleAnimation();

        CreateCA(E_CA_Type.SELECT_TILE_WITH_CLICK);
        CreateCA(E_CA_Type.IDLE_CHARACTER);
        CreateCA(E_CA_Type.SELECT_CHARACTER_WITH_CLICK);
        CreateCA(E_CA_Type.BASIC_MEELE_ATTACK);

        // TODO: Initialize reaction-specific control actions
        // e.g., CA_ReactionChoice, CA_CounterAttack, etc.

        playerStateHelper.CheckAvailableTilesHelper(enemyAI.currentTarget, USO.gridManager, true);

        
    }

    public void SuspendState() 
    {
    }
    public void ResumeState() 
    { 

    }

    public void ExitState()
    {
        UIEventManager.MoveButtonClicked -= HandleMoveButtonClicked;
 
        if (playerStateHelper != null)
        {
            playerStateHelper = null;
        }
        
        // Clear references
        input = null;
        enemyAI = null;
        userControlOrchestrator = null;
    }

    public void Update()
    {
        CA_HoverTileSelection.Action();
        CA_HoverCharacter.Action();
        CA_SelectCharacterWithClick.Action();
        if (!CA_HoverCharacter.isHittingCharacter)
        {
            CA_SelectTileWithClick.Action();
        }
        CA_BasicMeeleAttack.ActionHandler();

        // Guard against null input
        if (input == null)
        {
            input = userControlOrchestrator.input;
            Debug.LogError("Input is null in Reaction State Update!");
        }
            
        if (input.Player.Exit.WasPressedThisFrame())
        {
            userControlOrchestrator.PopState();
        }
    }

    public void FixedUpdate()
    {
        // TODO: Handle reaction physics/state updates
        //Debug.Log("RUNNING REACTION");
        if (characterPhase == ECharacterPhase.IDLE && CA_IdleCharacter != null)
        {
            CA_IdleCharacter.Action();
        }
        else if (characterPhase == ECharacterPhase.MOVE && CA_MoveCharacter != null)
        {
            CA_MoveCharacter.Action();
        }
        else if (characterPhase == ECharacterPhase.ATTACK && CA_BasicMeeleAttack != null)
        {
            CA_BasicMeeleAttack.Action();
        }

    }

    public void DeleteCA(E_CA_Type type)
    {
        // TODO: Implement control action deletion if needed
        // Similar pattern to IUSO_Battle_PlayerTurn_State.DeleteCA()
    }

    private void DestroyComponent(Component component)
    {
        if (component != null)
            UnityEngine.Object.Destroy(component);
    }

    public InfoObject GetStateInfo()
    {
        return new InfoObject { characterPhase = ECharacterPhase.REACT };
    }

    public ECharacterPhase GetCharacterPhase()
    {
        return ECharacterPhase.REACT;
    }

    public void SetCharacterPhase(ECharacterPhase phase)
    {
        // Reaction state typically stays in REACT phase
        // Can add validation or state switching logic if needed
    }

    public void HandleMoveButtonClicked()
    {

    }

    private void CreateCA(E_CA_Type type)
    {
        GameObject GO = userControlOrchestrator.gameObject;
        GameObject selectedCharacter = userControlOrchestrator.selectedCharacter;


        if (type == E_CA_Type.BASIC_MEELE_ATTACK)
        {
            CA_BasicMeeleAttack = GO.AddComponent<CA_BasicMeeleAttack>();
            CA_BasicMeeleAttack.userControlOrchestrator = userControlOrchestrator;
            CA_BasicMeeleAttack.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
            CA_BasicMeeleAttack.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
            CA_BasicMeeleAttack.input = input;
            allControlActions.Add(CA_BasicMeeleAttack);
        }
        else if (type == E_CA_Type.SELECT_TILE_WITH_CLICK)
        {
            CA_SelectTileWithClick = GO.AddComponent<CA_SelectTileWithClick>();
            CA_SelectTileWithClick.userControlOrchestrator = userControlOrchestrator;
            CA_SelectTileWithClick.input = input;
            allControlActions.Add(CA_SelectTileWithClick);
        }
        else if (type == E_CA_Type.HOVER_TILE_SELECTION)
        {
            CA_HoverTileSelection = GO.AddComponent<CA_HoverTileSelection>();
            CA_HoverTileSelection.userControlOrchestrator = userControlOrchestrator;
            CA_HoverTileSelection.interfaceRaycastSelection = interfaceRaycastSelection;
            allControlActions.Add(CA_HoverTileSelection);
        }
        else if (type == E_CA_Type.HOVER_CHARACTER)
        {
            CA_HoverCharacter = GO.AddComponent<CA_HoverCharacter>();
            CA_HoverCharacter.userControlOrchestrator = userControlOrchestrator;
            CA_HoverCharacter.interfaceRaycastSelection = interfaceRaycastSelection;
            allControlActions.Add(CA_HoverCharacter);
        }
        else if (type == E_CA_Type.SELECT_CHARACTER_WITH_CLICK)
        {
            CA_SelectCharacterWithClick = GO.AddComponent<CA_SelectCharacterWithClick>();
            CA_SelectCharacterWithClick.userControlOrchestrator = userControlOrchestrator;
            CA_SelectCharacterWithClick.interfaceRaycastSelection = interfaceRaycastSelection;
            CA_SelectCharacterWithClick.input = input;
            allControlActions.Add(CA_SelectCharacterWithClick);
        }
    }
}