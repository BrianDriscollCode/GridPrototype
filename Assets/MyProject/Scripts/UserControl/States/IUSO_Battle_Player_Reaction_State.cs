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
    private GameObject targetCharacter;

    private CA_IdleCharacter CA_IdleCharacter;
    private CA_MoveCharacter CA_MoveCharacter;
    private CA_HoverTileSelection CA_HoverTileSelection;
    private CA_HoverCharacter CA_HoverCharacter;
    private CA_SelectCharacterWithClick CA_SelectCharacterWithClick;
    private CA_SelectTileWithClick CA_SelectTileWithClick;
    private CA_BasicMeeleAttack CA_BasicMeeleAttack;
    private List<MonoBehaviour> allControlActions;

    private PlayerStateHelper playerStateHelper;

    private MovementPointsManager movementPointsManager;
    private GridManager gridManager;
    private TurnManager turnManager;
    private PartyTracker partyTracker;


    public void EnterState(UserControlOrchestrator USO)
    {
        characterPhase = ECharacterPhase.IDLE;
        allControlActions = new List<MonoBehaviour>();
        playerStateHelper = new PlayerStateHelper();
        userControlOrchestrator = USO;
        input = userControlOrchestrator.input;
        enemyAI = userControlOrchestrator.enemyAI;
        targetCharacter = enemyAI.currentTarget;
        interfaceRaycastSelection = userControlOrchestrator.interfaceRaycastSelection;
        UIEventManager.MoveButtonClicked += HandleMoveButtonClicked;

        ManagerRegistry managerRegistry = GameObject.FindAnyObjectByType<ManagerRegistry>();

        if (managerRegistry != null)
        {
            GameObject managerObj = managerRegistry.managerList.Find(obj => obj.GetComponent<MovementPointsManager>() != null);
            if (managerObj != null)
            {
                movementPointsManager = managerObj.GetComponent<MovementPointsManager>();
            }
            managerObj = null;


            managerObj = managerRegistry.managerList.Find(obj => obj.GetComponent<GridManager>() != null);
            if (managerObj != null)
            {
                gridManager = managerObj.GetComponent<GridManager>();
            }

            managerObj = managerRegistry.managerList.Find(obj => obj.GetComponent<TurnManager>() != null);
            if (managerObj != null)
            {
                turnManager = managerObj.GetComponent<TurnManager>();
                partyTracker = turnManager.GetPartyTracker();
                partyTracker.SetCurrentParty(PartyTracker.EWhosParty.PLAYER);
            }


        }

        CreateCA(E_CA_Type.IDLE_CHARACTER);
        CreateCA(E_CA_Type.MOVE_CHARACTER);
        CreateCA(E_CA_Type.HOVER_TILE_SELECTION);
        CreateCA(E_CA_Type.HOVER_CHARACTER);
        enemyAI.currentTarget.GetComponent<PlayerAnim>().IdleAnimation();

        CreateCA(E_CA_Type.SELECT_TILE_WITH_CLICK);
        CreateCA(E_CA_Type.IDLE_CHARACTER);
        CreateCA(E_CA_Type.SELECT_CHARACTER_WITH_CLICK);
        CreateCA(E_CA_Type.BASIC_MEELE_ATTACK);

        // TODO: Initialize reaction-specific control actions
        // e.g., CA_ReactionChoice, CA_CounterAttack, etc.

        playerStateHelper.CheckAvailableTilesHelper(enemyAI.currentTarget, USO.gridManager, true);
        RegisterEventHandlers();
        
    }

    public void SuspendState() 
    {
    }
    public void ResumeState() 
    { 

    }

    public void ExitState()
    {
        // Unsubscribe from all events
        UIEventManager.MoveButtonClicked -= HandleMoveButtonClicked;
        EventManager.ClickedTile -= HandleTileClicked;
        EventManager.MovingComplete -= HandleMovingComplete;
        
        // Clean up helpers
        if (playerStateHelper != null)
        {
            playerStateHelper = null;
        }
        
        // Destroy control actions
        DestroyComponent(CA_HoverTileSelection);
        DestroyComponent(CA_HoverCharacter);
        DestroyComponent(CA_MoveCharacter);
        DestroyComponent(CA_SelectTileWithClick);
        DestroyComponent(CA_IdleCharacter);
        DestroyComponent(CA_SelectCharacterWithClick);
        DestroyComponent(CA_BasicMeeleAttack);
        
        // Clear references to prevent stale access
        input = null;
        enemyAI = null;
        userControlOrchestrator = null;
        gridManager = null;
        movementPointsManager = null;
        
        Logger.LogCategory("Turn", "Reaction State - Exited and cleaned up");
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
        GameObject selectedCharacter = enemyAI.currentTarget;


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
        else if (type == E_CA_Type.MOVE_CHARACTER)
        {
            CA_MoveCharacter = GO.AddComponent<CA_MoveCharacter>();
            CA_MoveCharacter.userControlOrchestrator = userControlOrchestrator;
            CA_MoveCharacter.playerControls = enemyAI.currentTarget.GetComponent<PlayerClickControls>();
            CA_MoveCharacter.playerAnim = enemyAI.currentTarget.GetComponent<PlayerAnim>();
            allControlActions.Add(CA_MoveCharacter);
        }
        else if (type == E_CA_Type.IDLE_CHARACTER)
        {
            CA_IdleCharacter = GO.AddComponent<CA_IdleCharacter>();
            CA_IdleCharacter.userControlOrchestrator = userControlOrchestrator;
            CA_IdleCharacter.playerControls = enemyAI.currentTarget.GetComponent<PlayerClickControls>();
            CA_IdleCharacter.playerAnim = enemyAI.currentTarget.GetComponent<PlayerAnim>();
            allControlActions.Add(CA_IdleCharacter);
        }
    }

    private void RegisterEventHandlers()
    {
        EventManager.ClickedTile += HandleTileClicked;
        EventManager.MovingComplete += HandleMovingComplete;
        
        Logger.LogCategory("Turn", "Reaction State - Registered event handlers");
    }

    private void HandleTileClicked(Vector2Int clickedGridPos)
    {
        Logger.LogCategory("Turn", "HandleTileClicked Heard");

        if (characterPhase != ECharacterPhase.IDLE)
            return;

        List<GameObject> characterList = movementPointsManager.characters;
        GameObject matchingCharacter = characterList.Find(obj => obj == targetCharacter);
        
        if (matchingCharacter == null)
        {
            Logger.LogCategory("Turn", "ERROR: No matching character found!");
            return;
        }

        Vector2Int characterOriginalPos = gridManager.WorldToGridPosition(matchingCharacter.transform.position);
        PlayerStatSheet playerStatSheet = matchingCharacter.GetComponent<PlayerStatSheet>();

        if (playerStatSheet == null)
        {
            Logger.LogCategory("Turn", "ERROR: PlayerStatSheet is null!");
            return;
        }

        int distance = gridManager.GetTileDistance(characterOriginalPos, clickedGridPos);
        int availableMovement = Mathf.RoundToInt(playerStatSheet.movementPoints / 2f);

        // Validate movement
        if (distance <= availableMovement && distance > 0)
        {
            // PlayerClickControls will automatically handle fromPos/toPos via its own listener
            // Just update game state
            playerStatSheet.movementPoints -= distance;
            characterPhase = ECharacterPhase.MOVE;
            
            Logger.LogCategory("Turn", "Movement phase activated!");
        }
        else
        {
            Logger.LogCategory("Turn", $"Movement blocked: distance {distance}, available {availableMovement}");
        }
    }

    private void HandleMovingComplete()
    {
        Debug.Log("HandleMovingcomplete RUNNING!! **************");

        List<GameObject> characterList = movementPointsManager.characters;
        GameObject matchingCharacter = characterList.Find(obj => obj == targetCharacter);

        if (matchingCharacter != null)
        {
            Logger.LogCategory("Grid", "HandleMovingComplete - Character Match");
            PlayerStatSheet playerStatSheet = matchingCharacter.GetComponent<PlayerStatSheet>();
            //characterPhase = ECharacterPhase.IDLE;

            //playerStateHelper.CheckAvailableTilesHelper(targetCharacter, gridManager);
            gridManager.ClearAvailableTiles();
            //Ending turn
            userControlOrchestrator.PopState();
            // *** State May Switch
            //turnManager.CheckIfTurnComplete(playerStatSheet, userControlOrchestrator);

        }
        else
        {
            Logger.LogCategory("Grid", "HandleMovingComplete - No Match");
        }
    }
}