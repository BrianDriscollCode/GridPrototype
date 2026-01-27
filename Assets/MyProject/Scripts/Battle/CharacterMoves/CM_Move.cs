using UnityEngine;
using static PlayerClickControls;

public class CM_Move
{
    public UserControlManager userControlManager;
    public PlayerClickControls playerControls;
    public PlayerAnim playerAnim;

    [Header("Rotation Settings")]
    public float rotationSpeed = 10f; // How fast the character rotates toward target

    // Constructor to initialize dependencies
    public CM_Move(UserControlManager manager, PlayerClickControls controls, PlayerAnim anim)
    {
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

            if ((playerControls.rb.position - target).sqrMagnitude <= playerControls.arriveThreshold * playerControls.arriveThreshold)
            {
                playerControls.rb.position = target;
                //playerControls.currentState = PlayerState.Neutral;
                userControlManager.ExitState(userControlManager.currentState);
                userControlManager.EnterState(userControlManager.SELECT);
            }
        }
        //if (playerControls.currentState == PlayerClickControls.PlayerState.Moving)
        //{
        //Vector3 target = playerControls.toPos;
        //float step = playerControls.moveSpeed * Time.fixedDeltaTime;

        //if (playerControls.rb != null)
        //{
        //    // Calculate direction to target (ignore Y axis for rotation)
        //    Vector3 directionToTarget = target - playerControls.rb.position;
        //    directionToTarget.y = 0; // Keep rotation only on horizontal plane

        //    // Only rotate if there's a significant direction to move
        //    if (directionToTarget.sqrMagnitude > 0.01f)
        //    {
        //        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        //        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        //        playerControls.rb.MoveRotation(targetRotation);
        //        Debug.Log("Running targetRotation: " + targetRotation);
        //    }

        //    Vector3 next = Vector3.MoveTowards(playerControls.rb.position, target, step);
        //    playerControls.rb.MovePosition(next);
        //    playerAnim.RunAnimation();

        //    if ((playerControls.rb.position - target).sqrMagnitude <= playerControls.arriveThreshold * playerControls.arriveThreshold)
        //    {
        //        playerControls.rb.position = target;
        //        //playerControls.currentState = PlayerState.Neutral;
        //        userControlManager.ExitState(userControlManager.currentState);
        //        userControlManager.EnterState(userControlManager.IDLE);
        //    }
        //}
        //else
        //{
        //    // Calculate direction to target (ignore Y axis for rotation)
        //    Vector3 directionToTarget = target - playerControls.rb.position;
        //    directionToTarget.y = 0; // Keep rotation only on horizontal plane

        //    // Only rotate if there's a significant direction to move
        //    if (directionToTarget.sqrMagnitude > 0.01f)
        //    {
        //        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        //        playerControls.rb.MoveRotation(targetRotation);
        //        Debug.Log("Running targetRotation 2: " + targetRotation);
        //    }

        //    Vector3 next = Vector3.MoveTowards(playerControls.rb.position, target, step);
        //    playerControls.rb.position = next;
        //    playerAnim.RunAnimation();

        //    if ((playerControls.rb.position - target).sqrMagnitude <= playerControls.arriveThreshold * playerControls.arriveThreshold)
        //    {
        //        playerControls.rb.position = target;
        //        playerControls.currentState = PlayerState.Neutral;
        //        userControlManager.ExitState(userControlManager.currentState);
        //        userControlManager.EnterState(userControlManager.IDLE);
        //    }
        //}
        // }
    }
}
