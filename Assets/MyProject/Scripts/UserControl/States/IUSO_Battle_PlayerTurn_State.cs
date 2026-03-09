using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum ECharacterPhase
{
    IDLE,
    MOVE,
    ATTACK,
    DEFEND,
    REACT
}
public class IUSO_Battle_PlayerTurn_State : IUSO_State
{
    private UserControlOrchestrator userControlOrchestrator;

    private CA_HoverTileSelection CA_HoverTileSelection;
    private CA_HoverCharacter CA_HoverCharacter;
    private CA_IdleCharacter CA_IdleCharacter;
    private CA_MoveCharacter CA_MoveCharacter;
    private CA_SelectCharacterWithClick CA_SelectCharacterWithClick;
    private CA_SelectTileWithClick CA_SelectTileWithClick;
    private CA_BasicMeeleAttack CA_BasicMeeleAttack;
    private List<MonoBehaviour> allControlActions;

    public InterfaceRaycastSelection interfaceRaycastSelection;

    public ECharacterPhase characterPhase;

    public InputSystem_Actions input;

    private MovementPointsManager movementPointsManager;

    private GameObject activeCharacter;

    private GridManager gridManager;

    private TurnManager turnManager;

    private PartyTracker partyTracker;

    private PlayerStateHelper playerStateHelper;

    private bool inReactionState;

    private string pausedAnimationName;
    private float pausedAnimationTime;

    public void EnterState(UserControlOrchestrator USO)
    {
        playerStateHelper = new PlayerStateHelper();
        allControlActions = new List<MonoBehaviour>();
        userControlOrchestrator = USO;

        RegisterEventHandlers();
        RegisterUIEventHandlers();

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

        interfaceRaycastSelection = userControlOrchestrator.interfaceRaycastSelection;
        characterPhase = ECharacterPhase.IDLE;
        input = userControlOrchestrator.input;

        InitialiazeControlActions();

        playerStateHelper.CheckAvailableTilesHelper(userControlOrchestrator.selectedCharacter, gridManager);
    }

    public void ExitState()
    {
        RemoveEventHandlers();
        RemoveUIEventHandlers();

        // Clean up all components
        DestroyComponent(CA_HoverTileSelection);
        DestroyComponent(CA_HoverCharacter);
        DestroyComponent(CA_MoveCharacter);
        DestroyComponent(CA_SelectTileWithClick);
        DestroyComponent(CA_IdleCharacter);
        DestroyComponent(CA_SelectCharacterWithClick);
        DestroyComponent(CA_BasicMeeleAttack);

        // Clean up helpers
        if (playerStateHelper != null)
        {
            playerStateHelper = null;
        }

        // Clear references
        interfaceRaycastSelection = null; // only ref no component - this is good
        CA_HoverTileSelection = null;
        CA_HoverCharacter = null;
        CA_MoveCharacter = null;
        CA_SelectTileWithClick = null;
        CA_IdleCharacter = null;
        CA_SelectCharacterWithClick = null;

        gridManager.GetComponent<HighlightGridTile>().ClearAllHighlights();
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

        if (input.Player.Exit.WasPressedThisFrame())
        {
            userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
        }
    }

    public void FixedUpdate()
    {
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

        if (input.Player.DebugEditorKey.IsPressed())
        {
            gridManager.characterPositionTracker.PrintCharacterPositionList();
        }
    }

    /// <summary>Pauses the animator and disables control actions. Called on PushState.</summary>
    public void SuspendState()
    {
        if (userControlOrchestrator.selectedCharacter != null)
        {
            PlayerAnim playerAnim = userControlOrchestrator.selectedCharacter.GetComponent<PlayerAnim>();
            if (playerAnim != null && playerAnim.playerAnimator != null)
                playerAnim.playerAnimator.speed = 0f;
        }
        SetControlActionsEnabled(false);
    }

    /// <summary>Unpauses the animator and re-enables control actions. Called on PopState.</summary>
    public void ResumeState()
    {
        SetControlActionsEnabled(true);

        if (userControlOrchestrator.selectedCharacter != null)
        {
            PlayerAnim playerAnim = userControlOrchestrator.selectedCharacter.GetComponent<PlayerAnim>();
            if (playerAnim != null && playerAnim.playerAnimator != null)
                playerAnim.playerAnimator.speed = 1f;
        }
    }

    private void SetControlActionsEnabled(bool enabled)
    {
        foreach (MonoBehaviour ca in allControlActions)
        {
            if (ca != null)
                ca.enabled = enabled;
        }
    }


    private void InitialiazeControlActions()
    {
        GameObject GO = userControlOrchestrator.gameObject;
        GameObject selectedCharacter = userControlOrchestrator.selectedCharacter;
        activeCharacter = selectedCharacter;

        CreateCA(E_CA_Type.HOVER_TILE_SELECTION);
        CreateCA(E_CA_Type.HOVER_CHARACTER);
        CreateCA(E_CA_Type.MOVE_CHARACTER);
        selectedCharacter.GetComponent<PlayerAnim>().IdleAnimation();

        CreateCA(E_CA_Type.SELECT_TILE_WITH_CLICK);
        CreateCA(E_CA_Type.IDLE_CHARACTER);
        CreateCA(E_CA_Type.SELECT_CHARACTER_WITH_CLICK);
        CreateCA(E_CA_Type.BASIC_MEELE_ATTACK);
    }

    public void ResetState()
    {
        DestroyMultipleControlActions(allControlActions);
        InitialiazeControlActions();
        playerStateHelper.CheckAvailableTilesHelper(userControlOrchestrator.selectedCharacter, gridManager);
    }

    public List<MonoBehaviour> GetAllControlActions()
    {
        return allControlActions;
    }

    private void DestroyMultipleControlActions(List<MonoBehaviour> actions)
    {
        if (actions == null || allControlActions == null)
            return;

        for (int i = allControlActions.Count - 1; i >= 0; i--)
        {
            MonoBehaviour ca = allControlActions[i];

            if (ca != null && actions.Contains(ca))
            {
                DestroyComponent(ca);
                allControlActions.RemoveAt(i);

                // Clear the field reference if it matches
                if (ca == CA_HoverTileSelection) CA_HoverTileSelection = null;
                else if (ca == CA_HoverCharacter) CA_HoverCharacter = null;
                else if (ca == CA_IdleCharacter) CA_IdleCharacter = null;
                else if (ca == CA_MoveCharacter) CA_MoveCharacter = null;
                else if (ca == CA_SelectCharacterWithClick) CA_SelectCharacterWithClick = null;
                else if (ca == CA_SelectTileWithClick) CA_SelectTileWithClick = null;
                else if (ca == CA_BasicMeeleAttack) CA_BasicMeeleAttack = null;
            }
        }
    }

    // Factory pattern WOULD BE better, but need to focus on prototype
    // CA_MoveCharacter is managed with deletions and readding.
    private void CreateCA(E_CA_Type type)
    {
        GameObject GO = userControlOrchestrator.gameObject;
        GameObject selectedCharacter = userControlOrchestrator.selectedCharacter;

        if (type == E_CA_Type.MOVE_CHARACTER)
        {
            CA_MoveCharacter = GO.AddComponent<CA_MoveCharacter>();
            CA_MoveCharacter.userControlOrchestrator = userControlOrchestrator;
            CA_MoveCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
            CA_MoveCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
            allControlActions.Add(CA_MoveCharacter);
        }
        else if (type == E_CA_Type.IDLE_CHARACTER)
        {
            CA_IdleCharacter = GO.AddComponent<CA_IdleCharacter>();
            CA_IdleCharacter.userControlOrchestrator = userControlOrchestrator;
            CA_IdleCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
            CA_IdleCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
            allControlActions.Add(CA_IdleCharacter);
        }
        else if (type == E_CA_Type.BASIC_MEELE_ATTACK)
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

    public void DeleteCA(E_CA_Type type)
    {
        switch (type)
        {
            case E_CA_Type.HOVER_TILE_SELECTION:
                if (CA_HoverTileSelection != null)
                {
                    allControlActions.Remove(CA_HoverTileSelection);
                    DestroyComponent(CA_HoverTileSelection);
                    CA_HoverTileSelection = null;
                }
                break;

            case E_CA_Type.HOVER_CHARACTER:
                if (CA_HoverCharacter != null)
                {
                    allControlActions.Remove(CA_HoverCharacter);
                    DestroyComponent(CA_HoverCharacter);
                    CA_HoverCharacter = null;
                }
                break;

            case E_CA_Type.IDLE_CHARACTER:
                if (CA_IdleCharacter != null)
                {
                    allControlActions.Remove(CA_IdleCharacter);
                    DestroyComponent(CA_IdleCharacter);
                    CA_IdleCharacter = null;
                }
                break;

            case E_CA_Type.MOVE_CHARACTER:
                if (CA_MoveCharacter != null)
                {
                    allControlActions.Remove(CA_MoveCharacter);
                    DestroyComponent(CA_MoveCharacter);
                    CA_MoveCharacter = null;
                }
                break;

            case E_CA_Type.SELECT_CHARACTER_WITH_CLICK:
                if (CA_SelectCharacterWithClick != null)
                {
                    allControlActions.Remove(CA_SelectCharacterWithClick);
                    DestroyComponent(CA_SelectCharacterWithClick);
                    CA_SelectCharacterWithClick = null;
                }
                break;

            case E_CA_Type.SELECT_TILE_WITH_CLICK:
                if (CA_SelectTileWithClick != null)
                {
                    allControlActions.Remove(CA_SelectTileWithClick);
                    DestroyComponent(CA_SelectTileWithClick);
                    CA_SelectTileWithClick = null;
                }
                break;
        }
    }

    private void DestroyComponent(Component component)
    {
        if (component != null)
            UnityEngine.Object.Destroy(component);
    }

    public InfoObject GetStateInfo()
    {
        return new InfoObject { characterPhase = this.characterPhase };
    }


    public ECharacterPhase GetCharacterPhase()
    {
        return characterPhase;
    }

    public void SetCharacterPhase(ECharacterPhase phase)
    {
        characterPhase = phase;
    }

    private void RegisterEventHandlers()
    {
        EventManager.ClickedTile += HandleTileClicked;
        EventManager.RightClickAttack += HandleBasicAttack;
        EventManager.FinishBasicMeeleAttack += HandleFinishBasicAttack;
        EventManager.MovingComplete += HandleMovingComplete;
        EventManager.AttackDamageGiven += HandleAttackDamageGiven;
        EventManager.ReactionChance += HandleReactionChance;
        EventManager.ReactionEvent += HandleReactionEvent;
    }

    private void RemoveEventHandlers()
    {
        EventManager.ClickedTile -= HandleTileClicked;
        EventManager.RightClickAttack -= HandleBasicAttack;
        EventManager.FinishBasicMeeleAttack -= HandleFinishBasicAttack;
        EventManager.MovingComplete -= HandleMovingComplete;
        EventManager.AttackDamageGiven -= HandleAttackDamageGiven;
        EventManager.ReactionChance -= HandleReactionChance;
        EventManager.ReactionEvent -= HandleReactionEvent;
    }

    private void RegisterUIEventHandlers()
    {
        UIEventManager.EndTurnButtonClicked += HandleEndButtonClicked;
    }

    private void RemoveUIEventHandlers()
    {
        UIEventManager.EndTurnButtonClicked -= HandleEndButtonClicked;
    }


    //private void HandleEndButtonClicked()
    //{
    //    List<GameObject> characterList = movementPointsManager.characters;
    //    GameObject matchingCharacter = characterList.Find(obj => obj == activeCharacter);
    //    PlayerStatSheet playerStatSheet = matchingCharacter.GetComponent<PlayerStatSheet>();

    //    playerStatSheet.movementPoints = 0;
    //    playerStatSheet.attackPoints = 0;
    //    playerStatSheet.turnComplete = true;

    //    turnManager.CheckIfTurnComplete(playerStatSheet, userControlOrchestrator);
    //}
    private void HandleEndButtonClicked()
    {
        if (userControlOrchestrator.CurrentState != this) return;

        PlayerStatSheet playerStatSheet = activeCharacter.GetComponent<PlayerStatSheet>();

        playerStatSheet.movementPoints = 0;
        playerStatSheet.attackPoints = 0;
        playerStatSheet.turnComplete = true;

        turnManager.CheckIfTurnComplete(playerStatSheet, userControlOrchestrator);
    }



    private void HandleTileClicked(Vector2Int clickedGridPos)
    {
        if (characterPhase != ECharacterPhase.IDLE)
            return;

        //if (clickedGridPos)

        List<GameObject> characterList = movementPointsManager.characters;
        GameObject matchingCharacter = characterList.Find(obj => obj == activeCharacter);
        Vector2Int characterOriginalPos = gridManager.WorldToGridPosition(matchingCharacter.transform.position);

        if (matchingCharacter != null)
        {
            PlayerStatSheet playerStatSheet = matchingCharacter.GetComponent<PlayerStatSheet>();

            if (playerStatSheet != null)
            {

                int distance = gridManager.GetTileDistance(characterOriginalPos, clickedGridPos);

                // Validate and execute move
                if (distance <= playerStatSheet.movementPoints)
                {
                    playerStatSheet.movementPoints -= distance;
                    characterPhase = ECharacterPhase.MOVE;
                }
                else
                {
                    return;
                }
                 
            }
        }
    }

    private void HandleBasicAttack()
    {
        if (characterPhase != ECharacterPhase.IDLE)
            return;

        List<GameObject> characterList = movementPointsManager.characters;
        GameObject matchingCharacter = characterList.Find(obj => obj == activeCharacter);

        if (matchingCharacter != null)
        {
            PlayerStatSheet playerStatSheet = matchingCharacter.GetComponent<PlayerStatSheet>();

            if (playerStatSheet != null && playerStatSheet.attackPoints > 0)
            {
                playerStatSheet.attackPoints -= 1;
                characterPhase = ECharacterPhase.ATTACK;
            }
        }
    }

    private void HandleReactionChance()
    {
        float chance = 1f;

        if (chance >= 0.5f)
        {
            EventManager.OnReactionEvent();
        }
    }

    private void HandleReactionEvent()
    {
        inReactionState = true;
        userControlOrchestrator.PushState(userControlOrchestrator.battle_Enemy_Reaction_State);
    }



    private void HandleAttackDamageGiven()
    {
        //pause game
        

        //Debug.Log"HandleFinishBasicAttack RUNNING!! **************");
        if (characterPhase == ECharacterPhase.ATTACK)
        {
            List<GameObject> characterList = movementPointsManager.characters;
            GameObject matchingCharacter = characterList.Find(obj => obj == activeCharacter);

            if (matchingCharacter != null)
            {
                // temp calculation and handling
                PlayerStatSheet playerStatSheet = matchingCharacter.GetComponent<PlayerStatSheet>();
                GameObject enemy = userControlOrchestrator.target;
                PlayerStatSheet enemyStatSheet = enemy.GetComponent<PlayerStatSheet>();
                HealthBar enemyHealthBar = enemy.GetComponent<HealthBar>();


                int maxEnemyHealth = enemyStatSheet.maxHealth;
                enemyStatSheet.health -= playerStatSheet.strength;
                int currentEnemyHealth = enemyStatSheet.health;

                enemyHealthBar.SetHealth(currentEnemyHealth, maxEnemyHealth);
            }
        }
    }

    private void HandleFinishBasicAttack()
    {
        //Debug.Log"HandleFinishBasicAttack RUNNING!! **************");
        if (characterPhase == ECharacterPhase.ATTACK)
        {
            List<GameObject> characterList = movementPointsManager.characters;
            GameObject matchingCharacter = characterList.Find(obj => obj == activeCharacter);
            PlayerStatSheet playerStatSheet = matchingCharacter.GetComponent<PlayerStatSheet>();

            if (matchingCharacter != null)
            {

                characterPhase = ECharacterPhase.IDLE;
                userControlOrchestrator.selectedCharacter.GetComponent<PlayerAnim>().ChangeAnimation("Idle");

                // *** State May Switch
                turnManager.CheckIfTurnComplete(playerStatSheet, userControlOrchestrator);
            }
        }
    }

    private void HandleMovingComplete()
    {
        //Debug.Log"HandleMovingcomplete RUNNING!! **************");

        List<GameObject> characterList = movementPointsManager.characters;
        GameObject matchingCharacter = characterList.Find(obj => obj == activeCharacter);

        if (matchingCharacter != null)
        {
            Logger.LogCategory("Grid", "HandleMovingComplete - Character Match");
            PlayerStatSheet playerStatSheet = matchingCharacter.GetComponent<PlayerStatSheet>();
            //characterPhase = ECharacterPhase.IDLE;

            playerStateHelper.CheckAvailableTilesHelper(userControlOrchestrator.selectedCharacter, gridManager);
            // *** State May Switch
            turnManager.CheckIfTurnComplete(playerStatSheet, userControlOrchestrator);
            
        }
        else
        {
            Logger.LogCategory("Grid", "HandleMovingComplete - No Match");
        }    
    }

    //private void CheckAvailableTilesHelper()
    //{
    //    // highlight available moves
    //    GameObject character = userControlOrchestrator.selectedCharacter;
    //    int characterMovePoints = character.GetComponent<PlayerStatSheet>().movementPoints;
    //    Vector2Int characterGridPos = gridManager.WorldToGridPosition(character.GetComponent<EntityGridLocation>().pos);

    //    gridManager.CheckAvailableMoveTilesAndHighlight(characterMovePoints, characterGridPos);
    //}
}

// Helpers



//
public class InfoObject
{
    public ECharacterPhase characterPhase;
}
