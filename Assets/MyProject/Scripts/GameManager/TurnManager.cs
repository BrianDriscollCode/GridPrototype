using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private AudioClip turnCompleteClip;
    [SerializeField] private AudioSource audioSource;

    private void Start()
    {
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
            Debug.Log("CM_Move: Turn complete - switching to enemy turn");
            RunTurnCompleteSound();
            userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
        }
        else
        {
            Debug.Log($"CM_Move: Actions remaining - MP: {stats.movementPoints}, AP: {stats.attackPoints}");
        }
    }

    private void RunTurnCompleteSound()
    {
        if (audioSource != null && turnCompleteClip != null)
        {
            audioSource.PlayOneShot(turnCompleteClip);
        }
    }
}
