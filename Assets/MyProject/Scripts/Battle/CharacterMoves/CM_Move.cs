using UnityEngine;
using static PlayerClickControls;

public class CM_Move
{
    public PlayerClickControls playerControls;
    public PlayerAnim playerAnim;

    [Header("Rotation Settings")]
    public float rotationSpeed = 10f;

    // Properties to check completion status
    public bool IsComplete { get; private set; }
    public bool IsMoving { get; private set; }

    // Simple constructor - only movement dependencies
    public CM_Move(PlayerClickControls controls, PlayerAnim anim)
    {
        playerControls = controls;
        playerAnim = anim;
        rotationSpeed = 10f;
        IsComplete = false;
        IsMoving = false;
    }

    public void Move()
    {
        Vector3 target = playerControls.toPos;
        float step = playerControls.moveSpeed * Time.fixedDeltaTime;

        if (playerControls.rb != null)
        {
            IsMoving = true;

            // Calculate direction to target (ignore Y axis for rotation)
            Vector3 directionToTarget = target - playerControls.rb.position;
            directionToTarget.y = 0;

            // Only rotate if there's a significant direction to move
            if (directionToTarget.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                playerControls.rb.MoveRotation(targetRotation);
            }

            Vector3 next = Vector3.MoveTowards(playerControls.rb.position, target, step);
            playerControls.rb.MovePosition(next);
            playerAnim.ChangeAnimation("Run");

            // Check if arrived
            if ((playerControls.rb.position - target).sqrMagnitude <= playerControls.arriveThreshold * playerControls.arriveThreshold)
            {
                playerControls.rb.position = target;
                IsComplete = true;
                IsMoving = false;
            }
        }
    }

    public void Reset()
    {
        IsComplete = false;
        IsMoving = false;
    }
}
