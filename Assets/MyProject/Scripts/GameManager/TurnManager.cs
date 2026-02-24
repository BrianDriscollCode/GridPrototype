using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private AudioClip turnCompleteClip;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private ManagerRegistry managerRegistry;
    [SerializeField] private CharacterRegisterManager characterRegisterManager;
    [SerializeField] private PartyTracker partyTracker;

    private async void Start()
    {
        await InitializeManagersWithRetryAsync();

        // Get or add AudioSource component
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    private async Task InitializeManagersWithRetryAsync()
    {
        int maxAttempts = 10;
        int attempts = 0;
        int retryDelayMs = 100; // 100ms between attempts

        while (attempts < maxAttempts)
        {
            managerRegistry = GameObject.FindAnyObjectByType<ManagerRegistry>();

            if (managerRegistry != null && managerRegistry.managerList != null && managerRegistry.managerList.Count > 0)
            {
                GameObject managerObj = managerRegistry.managerList.Find(obj => obj.GetComponent<CharacterRegisterManager>() != null);

                if (managerObj != null)
                {
                    characterRegisterManager = managerObj.GetComponent<CharacterRegisterManager>();
                    //Debug.Log$"TurnManager: CharacterRegisterManager found on attempt {attempts + 1}");
                    return; // Success!
                }
            }

            attempts++;
            Debug.LogWarning($"TurnManager: Attempt {attempts}/{maxAttempts} - CharacterRegisterManager not found, retrying...");
            await Task.Delay(retryDelayMs);
        }

        Debug.LogError("TurnManager: Failed to find CharacterRegisterManager after all retry attempts!");
    }

    public void CheckIfTurnComplete(PlayerStatSheet stats, UserControlOrchestrator userControlOrchestrator)
    {
        Logger.LogCategory("Turn", "CheckIfTurnComplete");

        ECharacterType characterType;

        if (partyTracker.GetCurrentParty() == PartyTracker.EWhosParty.ENEMY)
        {
            characterType = ECharacterType.ENEMY;
            Logger.LogCategory("Turn", "IsEnemy - *SHOULD NOT HAPPEN*");
        }
        else if (partyTracker.GetCurrentParty() == PartyTracker.EWhosParty.PLAYER)
        {
            characterType = ECharacterType.PLAYER;
        }
        else
        {
            characterType = ECharacterType.UNKNOWN_CHARACTER_TYPE;
        }

        if (stats == null)
        {
            Logger.LogCategory("Turn", "Unknown character type - *SHOULD NOT HAPPEN*");
            return;
        }

        if (stats.movementPoints <= 0 && stats.attackPoints <= 0 || stats.turnComplete)
        {
            RunTurnCompleteSound();

            // Switch character here or switch to enemy party

            // Fetches from either player or enemy party depending on character type
            GameObject nextPartyMember = CheckIfPartyMemberHasPoints(characterType);


            if (nextPartyMember != null && characterType == ECharacterType.PLAYER)
            {
                userControlOrchestrator.selectedCharacter = nextPartyMember;

                IUSO_Battle_PlayerTurn_State battleState = userControlOrchestrator.userControlState as IUSO_Battle_PlayerTurn_State;

                if (battleState != null)
                {
                    battleState.ResetState();
                }
            }
            else if (nextPartyMember == null && characterType == ECharacterType.PLAYER)
            {
                Logger.LogCategory("Turn", "ECharacterType.PLAYER, nextPartyMember = null");
                RestorePartyAttackAndMovementPoints(characterType);
                userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
            }
            else
            {
                Logger.LogCategory("Turn", "TurnCheckComplete - Mislogic if statement, needs fixing");
            }
            // Viable Next Party Member in Player party
            //if (nextPartyMember != null && characterType == ECharacterType.PLAYER)
            //{
            //    userControlOrchestrator.selectedCharacter = nextPartyMember;

            //    IUSO_Battle_PlayerTurn_State battleState = userControlOrchestrator.userControlState as IUSO_Battle_PlayerTurn_State;

            //    if (battleState != null)
            //    {
            //        battleState.ResetState();
            //    }

            //    //Debug.Log"CM_Move: Turn complete - switching to next party member");
            //}
            //// ?? Seems to be for when player party no longer viable, enemy character returned???
            //else if (nextPartyMember != null && characterType == ECharacterType.ENEMY)
            //{
            //    Logger.LogCategory("Turn", "For enemy turns?");
            //    userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
            //}
            //// still in player party, but no viable characters, switch to enemy state
            //else if (nextPartyMember == null && characterType == ECharacterType.PLAYER)
            //{
            //    Logger.LogCategory("Turn", "ECharacterType.PLAYER, nextPartyMember = null");
            //    RestorePartyAttackAndMovementPoints(characterType);
            //    userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
            //}
            //// 
            //else if (nextPartyMember == null && characterType == ECharacterType.ENEMY)
            //{
            //    Logger.LogCategory("Turn", "ECharacterType.PLAYER, nextPartyMember = null");
            //    RestorePartyAttackAndMovementPoints(characterType);
            //    nextPartyMember = CheckIfPartyMemberHasPoints(ECharacterType.PLAYER);
            //    userControlOrchestrator.selectedCharacter = nextPartyMember;
            //    userControlOrchestrator.SwitchState(userControlOrchestrator.battle_PlayerTurn_State);

            //    IUSO_Battle_PlayerTurn_State battleState = userControlOrchestrator.userControlState as IUSO_Battle_PlayerTurn_State;

            //    if (battleState != null)
            //    {
            //        battleState.ResetState();
            //    }
            //    else
            //    {
            //        userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
            //        //Debug.Log"CM_Move: Turn complete - switching to next enemy turn");
            //    }

            //}
            //else
            //{
            //    //Debug.Log$"CM_Move: Actions remaining - MP: {stats.movementPoints}, AP: {stats.attackPoints}");
            //}
        }
    } 

    //public void CheckIfTurnComplete(PlayerStatSheet stats, UserControlOrchestrator userControlOrchestrator)
    //{
    //    Logger.LogCategory("Turn", "CheckIfTurnComplete");

    //    ECharacterType characterType;

    //    if (partyTracker.GetCurrentParty() == PartyTracker.EWhosParty.ENEMY)
    //    {
    //        characterType = ECharacterType.ENEMY;
    //        //Debug.LogError("CHECKTURNCOMPLETE:: Enemy");
    //    }
    //    else if (partyTracker.GetCurrentParty() == PartyTracker.EWhosParty.PLAYER)
    //    {
    //        characterType = ECharacterType.PLAYER;
    //        //Debug.LogError("CHECKTURNCOMPLETE:: Player");
    //    }
    //    else
    //    {
    //        characterType = ECharacterType.UNKNOWN_CHARACTER_TYPE;
    //        //Debug.Log"CHECKTURNCOMPLETE:: UNKNOWN");
    //    }

    //    if (stats == null)
    //    {
    //        //Debug.LogWarning("CM_Move: No PlayerStatSheet found");
    //        return;
    //    }


    //    //
    //    // Check if character has any actions left
    //    if (stats.movementPoints <= 0 && stats.attackPoints <= 0)
    //    {
    //        RunTurnCompleteSound();

    //        // Switch character here or switch to enemy party

    //        // Fetches from either player or enemy party depending on character type
    //        GameObject nextPartyMember = CheckIfPartyMemberHasPoints(characterType);

    //        if (nextPartyMember != null && characterType == ECharacterType.PLAYER)
    //        {
    //            userControlOrchestrator.selectedCharacter = nextPartyMember;

    //            IUSO_Battle_PlayerTurn_State battleState = userControlOrchestrator.userControlState as IUSO_Battle_PlayerTurn_State;

    //            if (battleState != null)
    //            {
    //                battleState.ResetState();
    //            }

    //            //Debug.Log"CM_Move: Turn complete - switching to next party member");
    //        }
    //        else if (nextPartyMember != null && characterType == ECharacterType.ENEMY)
    //        {
    //            userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
    //        }
    //        else if (nextPartyMember == null && characterType == ECharacterType.PLAYER)
    //        {
    //            RestorePartyAttackAndMovementPoints(characterType);
    //            userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
    //        }
    //        else if (nextPartyMember == null && characterType == ECharacterType.ENEMY)
    //        {
    //            RestorePartyAttackAndMovementPoints(characterType);
    //            nextPartyMember = CheckIfPartyMemberHasPoints(ECharacterType.PLAYER);
    //            userControlOrchestrator.selectedCharacter = nextPartyMember;
    //            userControlOrchestrator.SwitchState(userControlOrchestrator.battle_PlayerTurn_State);

    //            IUSO_Battle_PlayerTurn_State battleState = userControlOrchestrator.userControlState as IUSO_Battle_PlayerTurn_State;

    //            if (battleState != null)
    //            {
    //                battleState.ResetState();
    //            }
    //            else
    //            {
    //                userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
    //                //Debug.Log"CM_Move: Turn complete - switching to next enemy turn");
    //            }

    //        }
    //        else
    //        {
    //            //Debug.Log$"CM_Move: Actions remaining - MP: {stats.movementPoints}, AP: {stats.attackPoints}");
    //        }
    //    }
    //}

    /// <summary>
    /// Enemy-specific turn completion - called after enemy has taken its action (move OR attack)
    /// Enemies only take one action per turn, so we force completion regardless of remaining points
    /// </summary>
    public void CheckEnemyActionComplete(PlayerStatSheet stats, UserControlOrchestrator userControlOrchestrator)
    {
        Logger.LogCategory("Turn", "CheckEnemyActionComplete");

        if (stats == null)
        {
            Debug.LogWarning("CheckEnemyActionComplete: No PlayerStatSheet found");
            return;
        }

        RunTurnCompleteSound();

        // Mark enemy action complete by zeroing points
        stats.movementPoints = 0;
        stats.attackPoints = 0;

        // Check if another enemy can act
        GameObject nextEnemy = CheckIfPartyMemberHasPoints(ECharacterType.ENEMY);

        if (nextEnemy != null)
        {
            // Another enemy has actions - switch to that enemy
            Logger.LogCategory("Turn", "Switching to next enemy");
            userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
        }
        else
        {
            // No more enemies with actions - restore all enemy points and switch to player turn
            Logger.LogCategory("Turn", "All enemies done - switching to player turn");
            RestorePartyAttackAndMovementPoints(ECharacterType.ENEMY);

            GameObject firstPlayer = CheckIfPartyMemberHasPoints(ECharacterType.PLAYER);
            userControlOrchestrator.selectedCharacter = firstPlayer;
            userControlOrchestrator.SwitchState(userControlOrchestrator.battle_PlayerTurn_State);

            IUSO_Battle_PlayerTurn_State battleState = userControlOrchestrator.userControlState as IUSO_Battle_PlayerTurn_State;
            if (battleState != null)
            {
                battleState.ResetState();
            }
        }
    }
    public void RestorePartyAttackAndMovementPoints(ECharacterType characterType)
    {
        List<GameObject> partyMembers;

        if (characterType == ECharacterType.ENEMY)
        {
            partyMembers = characterRegisterManager.enemyParty;
        }
        else
        {
            partyMembers = characterRegisterManager.playerParty;
        }

        foreach (GameObject member in partyMembers)
        {
            PlayerStatSheet stats = member.GetComponent<PlayerStatSheet>();

            stats.attackPoints = stats.maxAttackPoints;
            stats.movementPoints = stats.maxMovementPoints;
            stats.turnComplete = false;
        }
    }

    public GameObject CheckIfPartyMemberHasPoints(ECharacterType characterType)
    {
        List<GameObject> partyMembers;

        if (characterType == ECharacterType.ENEMY)
        {
            partyMembers = characterRegisterManager.enemyParty;
        }
        else
        {
            partyMembers = characterRegisterManager.playerParty;
        }

        foreach (GameObject member in partyMembers)
        {
            PlayerStatSheet stats = member.GetComponent<PlayerStatSheet>();
            Logger.LogCategory("Turn", $"{member.name}: MP={stats.movementPoints}, AP={stats.attackPoints}, TurnComplete={stats.turnComplete}");

            if (!stats.turnComplete && (stats.movementPoints > 0 || stats.attackPoints > 0))
            {
                return member;
            }
        }

        return null;
    }
    //public GameObject CheckIfPartyMemberHasPoints(ECharacterType characterType)
    //{
    //    List<GameObject> partyMembers;


    //    if (characterType == ECharacterType.ENEMY)
    //    {
    //        partyMembers = characterRegisterManager.enemyParty;
    //    }
    //    else
    //    {
    //        partyMembers = characterRegisterManager.playerParty;
    //    }

    //    foreach (GameObject member in partyMembers)
    //    {
    //        PlayerStatSheet stats = member.GetComponent<PlayerStatSheet>();
    //        Logger.LogCategory("Turn", $"{member.name}: MP={stats.movementPoints}, AP={stats.attackPoints}, TurnComplete={stats.turnComplete}");

    //        if (stats.movementPoints > 0 || stats.attackPoints > 0)
    //        {

    //            return member;
    //        }
    //    }

    //    return null;
    //}

    private void RunTurnCompleteSound()
    {
        if (audioSource != null && turnCompleteClip != null)
        {
            audioSource.PlayOneShot(turnCompleteClip);
        }
    }

    public PartyTracker GetPartyTracker()
    {
        return partyTracker;
    }
}
