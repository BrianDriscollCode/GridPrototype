using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

public class IUSO_Battle_EnemyTurn_State : IUSO_State
{
    private UserControlOrchestrator orchestrator;
    private EnemyAI enemyAI;

    public ECharacterPhase characterPhase;
    private GridManager gridManager;

    private CA_IdleCharacter CA_IdleCharacter;
    private CA_MoveCharacter CA_MoveCharacter;
    private CA_BasicMeeleAttack CA_BasicMeeleAttack;
    private List<MonoBehaviour> allControlActions;

    private InputSystem_Actions input;
    private TurnManager turnManager;
    private PartyTracker partyTracker;

    private bool inReactionState;

    // ADD: Cache the original target before reaction
    private GameObject cachedOriginalTarget;

    public void EnterState(UserControlOrchestrator UCO)
    {
        inReactionState = false;
        cachedOriginalTarget = null; // Reset cache
        allControlActions = new List<MonoBehaviour>();
        orchestrator = UCO;
        enemyAI = UCO.enemyAI;  // Get AI from orchestrator
        gridManager = UCO.gridManager;
        turnManager = GameObject.FindFirstObjectByType<TurnManager>();
        partyTracker = turnManager.GetPartyTracker();
        partyTracker.SetCurrentParty(PartyTracker.EWhosParty.ENEMY);

        characterPhase = ECharacterPhase.IDLE;

        EventManager.MoveEnemy += HandleMoveEnemy;
        EventManager.MovingComplete += HandleFinishMoving;
        EventManager.FinishBasicMeeleAttack += HandleFinishAttack;
        EventManager.AttackDamageGiven += HandleAttackDamageGiven;
        EventManager.ReactionChance += HandleReactionChance;
        EventManager.ReactionEvent += HandleReactionEvent;

        input = orchestrator.input;

        // hydrate selected enemy
        enemyAI.SelectActiveEnemy();

        // initialize CA 
        InitializeControlActions();

        // Execute AI turn
        enemyAI.ExecuteTurn();
    }

    public void SuspendState()
    {
        if (enemyAI.currentEnemy)
        {
            PlayerAnim enemyAnim = enemyAI.currentEnemy.GetComponent<PlayerAnim>();

            if (enemyAnim != null && enemyAnim.playerAnimator != null)
            {
                enemyAnim.playerAnimator.speed = 0f;
            }
        }
        else
        {
            Debug.Log("enemyAI.currentEnemy = " + enemyAI.currentEnemy);
        }
    }
    public void ResumeState()
    {

        if (enemyAI.currentEnemy)
        {
            PlayerAnim enemyAnim = enemyAI.currentEnemy.GetComponent<PlayerAnim>();
            if (enemyAnim != null && enemyAnim.playerAnimator != null)
            {
                enemyAnim.playerAnimator.speed = 1f;
            }
        }
    }

    public void Update() { }
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

        // CACHE the current target BEFORE pushing reaction state
        cachedOriginalTarget = enemyAI.currentTarget;

        orchestrator.PushState(orchestrator.battle_Player_Reaction_State);
    }

    private void HandleAttackDamageGiven()
    {
        GameObject enemy = enemyAI.currentEnemy;
        PlayerStatSheet enemyStatSheet = enemy.GetComponent<PlayerStatSheet>();

        // USE cached target if in reaction state, otherwise use current
        GameObject player = cachedOriginalTarget != null ? cachedOriginalTarget : enemyAI.currentTarget;

        // Guard against null (in case cached target was destroyed)
        if (player == null)
        {
            Debug.LogWarning("Target is null in HandleAttackDamageGiven - using currentTarget fallback");
            player = enemyAI.currentTarget;
        }

        if (player == null)
        {
            Debug.LogError("No valid target found for attack damage");
            return;
        }

        PlayerStatSheet playerStatSheet = player.GetComponent<PlayerStatSheet>();
        HealthBar playerHealthBar = player.GetComponent<HealthBar>();

        playerStatSheet.health -= enemyStatSheet.strength;
        playerHealthBar.SetHealth(playerStatSheet.health, playerStatSheet.maxHealth);
    }

    private void HandleFinishAttack()
    {
        Logger.LogCategory("Turn", "HandleFinishAttack");
        GameObject enemy = enemyAI.currentEnemy;
        PlayerStatSheet enemyStatSheet = enemy.GetComponent<PlayerStatSheet>();

        characterPhase = ECharacterPhase.IDLE;
        CA_IdleCharacter.Action();

        enemyStatSheet.attackPoints = 0;

        // CLEAR the cached target after attack completes
        cachedOriginalTarget = null;

        turnManager.CheckEnemyActionComplete(enemyStatSheet, orchestrator);
    }

    // Begins the move process by activating control actions
    // and setting the move characterPhase
    private void HandleMoveEnemy()
    {
        GameObject enemy = enemyAI.currentEnemy;
        PlayerStatSheet enemyStatSheet = enemy.GetComponent<PlayerStatSheet>();

        characterPhase = ECharacterPhase.MOVE;

    }

    // When enemy reaches destination
    private void HandleFinishMoving()
    {
        GameObject enemy = enemyAI.currentEnemy;
        PlayerStatSheet enemyStatSheet = enemy.GetComponent<PlayerStatSheet>();

        enemyStatSheet.movementPoints = 0;

        if (enemyStatSheet.attackPoints > 0)
        {
            // If in reaction state, DON'T recalculate target
            if (cachedOriginalTarget == null)
            {
                // Normal flow: re-evaluate target after movement
                enemyAI.ExecuteTurn();
            }
            else
            {
                // Reaction flow: keep original target, just execute attack
                Logger.LogCategory("Turn", $"Using cached target: {cachedOriginalTarget.name}");
                enemyAI.ExecuteTurn(); // This will still work, but target won't change
            }

            Logger.LogCategory("Turn", "Executing turn");
        }
        else
        {
            characterPhase = ECharacterPhase.IDLE;
            Logger.LogCategory("Turn", "Flipping to Idle");
        }
    }

    public void ExitState()
    {
        EventManager.MoveEnemy -= HandleMoveEnemy;
        EventManager.MovingComplete -= HandleFinishMoving;
        EventManager.FinishBasicMeeleAttack -= HandleFinishAttack;
        EventManager.AttackDamageGiven -= HandleAttackDamageGiven;
        EventManager.ReactionChance -= HandleReactionChance;
        EventManager.ReactionEvent -= HandleReactionEvent;
        //Debug.Log"=== ENEMY TURN END ===");
    }

    public void InitializeControlActions()
    {
        CreateCA(E_CA_Type.IDLE_CHARACTER);
        CreateCA(E_CA_Type.MOVE_CHARACTER);
        CreateCA(E_CA_Type.BASIC_MEELE_ATTACK);
    }

    private void CreateCA(E_CA_Type type)
    {
        // Problem with initialization order. Selecte character is not the correct character to
        // get playerClickControls from. I need the enemy in enemyAI.
        GameObject GO = orchestrator.gameObject;
        //GameObject selectedCharacter = orchestrator.selectedCharacter;
        GameObject selectedCharacter = enemyAI.currentEnemy;

        if (type == E_CA_Type.MOVE_CHARACTER)
        {
            CA_MoveCharacter = GO.AddComponent<CA_MoveCharacter>();
            CA_MoveCharacter.userControlOrchestrator = orchestrator;
            CA_MoveCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
            CA_MoveCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
            allControlActions.Add(CA_MoveCharacter);
        }
        else if (type == E_CA_Type.IDLE_CHARACTER)
        {
            CA_IdleCharacter = GO.AddComponent<CA_IdleCharacter>();
            CA_IdleCharacter.userControlOrchestrator = orchestrator;
            CA_IdleCharacter.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
            CA_IdleCharacter.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
            allControlActions.Add(CA_IdleCharacter);
        }
        else if (type == E_CA_Type.BASIC_MEELE_ATTACK)
        {
            CA_BasicMeeleAttack = GO.AddComponent<CA_BasicMeeleAttack>();
            CA_BasicMeeleAttack.userControlOrchestrator = orchestrator;
            CA_BasicMeeleAttack.enemyAI = enemyAI;
            CA_BasicMeeleAttack.playerControls = selectedCharacter.GetComponent<PlayerClickControls>();
            CA_BasicMeeleAttack.playerAnim = selectedCharacter.GetComponent<PlayerAnim>();
            CA_BasicMeeleAttack.input = input;
            allControlActions.Add(CA_BasicMeeleAttack);
        }
    }

    public void DeleteCA(E_CA_Type type) { }

    public InfoObject GetStateInfo()
    {
        return new InfoObject { characterPhase = characterPhase };
    }

    public ECharacterPhase GetCharacterPhase() => characterPhase;
    public void SetCharacterPhase(ECharacterPhase phase) => characterPhase = phase;


    public void SetPlayerControlsAnim(ECharacterPhase phase)
    {
        if (phase == ECharacterPhase.IDLE)
        {
            CA_IdleCharacter.playerControls = enemyAI.currentEnemy.GetComponent<PlayerClickControls>();
            CA_IdleCharacter.playerAnim = enemyAI.currentEnemy.GetComponent<PlayerAnim>();
        }
    }
}