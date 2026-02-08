using UnityEngine;
using static PlayerClickControls;

public class CM_Move
{
    // New Control Manager
    public UserControlOrchestrator userControlOrchestrator;

    // Will be deprecated
    public UserControlManager userControlManager;
    
    public PlayerClickControls playerControls;
    public PlayerAnim playerAnim;

    [Header("Rotation Settings")]
    public float rotationSpeed = 10f; // How fast the character rotates toward target

    // Constructor to initialize dependencies
    public CM_Move(UserControlOrchestrator orchestrator, UserControlManager manager, PlayerClickControls controls, PlayerAnim anim)
    {
        userControlOrchestrator = orchestrator;
        userControlManager = manager;
        playerControls = controls;
        playerAnim = anim;
        rotationSpeed = 10f;
    }

    public void Move()
    {
        Vector3 target = playerControls.toPos;
        float step = playerControls.moveSpeed * Time.fixedDeltaTime;

        if (playerControls.rb != null)
        {
            // Calculate direction to target (ignore Y axis for rotation)
            Vector3 directionToTarget = target - playerControls.rb.position;
            directionToTarget.y = 0; // Keep rotation only on horizontal plane

            // Only rotate if there's a significant direction to move
            if (directionToTarget.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                playerControls.rb.MoveRotation(targetRotation);
                Debug.Log("Running targetRotation: " + targetRotation);
            }

            Vector3 next = Vector3.MoveTowards(playerControls.rb.position, target, step);
            playerControls.rb.MovePosition(next);
            playerAnim.RunAnimation();
            playerAnim.ChangeAnimation("Run");

            if ((playerControls.rb.position - target).sqrMagnitude <= playerControls.arriveThreshold * playerControls.arriveThreshold)
            {
                Debug.Log("TEST");
                playerControls.rb.position = target;
                //playerControls.currentState = PlayerState.Neutral;
                //userControlOrchestrator.ExitState(userControlManager.currentState);
                //userControlOrchestrator.EnterState(userControlManager.SELECT);
                userControlOrchestrator.userControlState.SetCharacterPhase(ECharacterPhase.IDLE);
                //userControlOrchestrator.userControlState.DeleteCA(E_CA_Type.MOVE_CHARACTER);

                CheckIfTurnComplete();
            }
        }
    }

    private void CheckIfTurnComplete()
    {
        PlayerStatSheet stats = playerControls.GetComponent<PlayerStatSheet>();

        if (stats == null)
        {
            Debug.LogWarning("CM_Move: No PlayerStatSheet found");
            return;
        }

        // Check if character has any actions left
        if (stats.movementPoints <= 0 && stats.attackPoints <= 0)
        {
            Debug.Log("CM_Move: Turn complete - switching to enemy turn");
            userControlOrchestrator.SwitchState(userControlOrchestrator.battle_EnemyTurn_State);
        }
        else
        {
            Debug.Log($"CM_Move: Actions remaining - MP: {stats.movementPoints}, AP: {stats.attackPoints}");
        }
    }
}
