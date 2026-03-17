using UnityEngine;
using static PlayerClickControls;

public class CA_MoveCharacter : MonoBehaviour
{
    public CM_Move cm_move;
    public UserControlOrchestrator userControlOrchestrator;
    public PlayerClickControls playerControls;
    public PlayerAnim playerAnim;
    public TurnManager turnManager;

    [Header("Rotation Settings")]
    public float rotationSpeed = 10f;

    public void Start()
    {
        InitializeCMMove();
    }

    private void InitializeCMMove()
    {
        // Find TurnManager
        ManagerRegistry managerRegistry = GameObject.FindAnyObjectByType<ManagerRegistry>();
        if (managerRegistry != null)
        {
            GameObject managerObj = managerRegistry.managerList.Find(obj => obj.GetComponent<TurnManager>() != null);
            if (managerObj != null)
            {
                turnManager = managerObj.GetComponent<TurnManager>();
            }
        }

        // Create CM_Move with only movement dependencies
        if (cm_move == null)
        {
            cm_move = new CM_Move(playerControls, playerAnim);
            cm_move.rotationSpeed = rotationSpeed;
        }
    }

    public void Action()
    {
        // Lazy initialization
        if (cm_move == null)
        {
            InitializeCMMove();
        }

        // Execute movement
        cm_move.Move();
        ////Debug.Log"cm_move.IsComplete = " + cm_move.IsComplete);

        // Handle completion (orchestration logic stays here)
        if (cm_move.IsComplete)
        {
            //Debug.Log"Movement complete - calling handling");
            OnMoveComplete();
        }
    }

    private void OnMoveComplete()
    {
        //Debug.Log"Movement complete - handling orchestration");

        // Reset movement state
        cm_move.Reset();

        // I need to update this to use the event manager
        // Orchestration: Update phase

        // *** This should be handled in player battle state with OnMovingComplete event
        if (userControlOrchestrator.CurrentState == userControlOrchestrator.battle_PlayerTurn_State)
        {
            userControlOrchestrator.CurrentState.SetCharacterPhase(ECharacterPhase.IDLE);
            userControlOrchestrator.selectedCharacter.GetComponent<PlayerAnim>().ChangeAnimation("Idle");
            EventManager.OnMovingComplete();
        }
        else if (userControlOrchestrator.CurrentState == userControlOrchestrator.battle_Player_Reaction_State)
        {
            // Cast to the concrete type to access enemyAI
            IUSO_Battle_Player_Reaction_State reactionState = userControlOrchestrator.CurrentState as IUSO_Battle_Player_Reaction_State;

            if (reactionState != null && reactionState.enemyAI != null)
            {
                userControlOrchestrator.CurrentState.SetCharacterPhase(ECharacterPhase.IDLE);

                // Get the reacting character (player being attacked)
                GameObject reactingCharacter = reactionState.enemyAI.currentTarget;

                if (reactingCharacter != null)
                {
                    reactingCharacter.GetComponent<PlayerAnim>().ChangeAnimation("Idle");
                }

                EventManager.OnMovingComplete();
            }
        }
        else if (userControlOrchestrator.CurrentState == userControlOrchestrator.battle_EnemyTurn_State)
        {
            EventManager.OnMovingComplete();
        }

        //Check for if enemy or player

        IUSO_State orchestratorState = userControlOrchestrator.CurrentState;

        // Orchestration: Check turn completion
        //if (turnManager != null)
        //{
        //    if (orchestratorState is IUSO_Battle_PlayerTurn_State)
        //    {
        //        turnManager.CheckIfTurnComplete(playerControls.GetComponent<PlayerStatSheet>(), userControlOrchestrator);
        //        Logger.LogCategory("Turn", "Player Battle state :: turnManager.CheckPlayerActionComplete");
        //    }
        //    else
        //    {
        //        Logger.LogCategory("Turn", "Not player, no turn check");
        //    }
           
        //}
    }
}