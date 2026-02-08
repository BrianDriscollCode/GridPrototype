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
                    Debug.Log($"TurnManager: CharacterRegisterManager found on attempt {attempts + 1}");
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
        if (stats == null)
        {
            Debug.LogWarning("CM_Move: No PlayerStatSheet found");
            return;
        }

        // Check if character has any actions left
        if (stats.movementPoints <= 0 && stats.attackPoints <= 0)
        {
            RunTurnCompleteSound();

            // Switch character here or switch to enemy party

            GameObject nextPartyMember = CheckIfPartyMemberHasPoints();

            if (nextPartyMember != null)
            {
                userControlOrchestrator.selectedCharacter = nextPartyMember;

                IUSO_Battle_PlayerTurn_State battleState = userControlOrchestrator.userControlState as IUSO_Battle_PlayerTurn_State;

                if (battleState != null)
                {
                    battleState.ResetState();
                }

                Debug.Log("CM_Move: Turn complete - switching to next party member");
            }
            else
            {
                userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
                Debug.Log("CM_Move: Turn complete - switching to next enemy turn");
            }
                
        }
        else
        {
            Debug.Log($"CM_Move: Actions remaining - MP: {stats.movementPoints}, AP: {stats.attackPoints}");
        }
    }

    public GameObject CheckIfPartyMemberHasPoints()
    {
        List<GameObject> partyMembers = characterRegisterManager.playerParty;

        foreach (GameObject member in partyMembers)
        {
            PlayerStatSheet stats = member.GetComponent<PlayerStatSheet>();

            if (stats.movementPoints > 0 || stats.attackPoints > 0)
            {
                return member;
            }
        }

        return null;
    }

    private void RunTurnCompleteSound()
    {
        if (audioSource != null && turnCompleteClip != null)
        {
            audioSource.PlayOneShot(turnCompleteClip);
        }
    }
}
