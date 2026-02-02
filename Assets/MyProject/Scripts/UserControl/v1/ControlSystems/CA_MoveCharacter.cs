using UnityEngine;
using static PlayerClickControls;

public class CA_MoveCharacter : MonoBehaviour
{
    public CM_Move cm_move;

    // new User Control Manager
    public UserControlOrchestrator userControlOrchestrator;

    // will be deprecated
    public UserControlManager userControlManager;

    public PlayerClickControls playerControls;
    public PlayerAnim playerAnim;

    [Header("Rotation Settings")]
    public float rotationSpeed = 10f; // How fast the character rotates toward target

    public void Start()
    {
        InitializeCMMove();
    }
    private void InitializeCMMove()
    {
        if (cm_move == null)
        {
            cm_move = new CM_Move(userControlOrchestrator, userControlManager, playerControls, playerAnim);
            cm_move.rotationSpeed = rotationSpeed;
        }
    }

    public void Action()
    {
        // Lazy initialization - create if not ready
        if (cm_move == null)
        {
            InitializeCMMove();
        }

        cm_move.Move();
    }
    //public void Move()
    //{
    //    if (playerControls.currentState == PlayerClickControls.PlayerState.Moving)
    //    {
    //        Vector3 target = playerControls.toPos;
    //        float step = playerControls.moveSpeed * Time.fixedDeltaTime;

    //        if (playerControls.rb != null)
    //        {
    //            // Calculate direction to target (ignore Y axis for rotation)
    //            Vector3 directionToTarget = target - playerControls.rb.position;
    //            directionToTarget.y = 0; // Keep rotation only on horizontal plane

    //            // Only rotate if there's a significant direction to move
    //            if (directionToTarget.sqrMagnitude > 0.01f)
    //            {
    //                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
    //                //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    //                playerControls.rb.MoveRotation(targetRotation);
    //                Debug.Log("Running targetRotation: " + targetRotation);
    //                Debug.Log("Running transformRotation: " + transform.rotation);
    //            }

    //            Vector3 next = Vector3.MoveTowards(playerControls.rb.position, target, step);
    //            playerControls.rb.MovePosition(next);
    //            playerAnim.RunAnimation();

    //            if ((playerControls.rb.position - target).sqrMagnitude <= playerControls.arriveThreshold * playerControls.arriveThreshold)
    //            {
    //                playerControls.rb.position = target;
    //                playerControls.currentState = PlayerState.Neutral;
    //                userControlManager.ExitState(userControlManager.currentState);
    //                userControlManager.EnterState(userControlManager.IDLE);
    //            }
    //        }
    //        else
    //        {
    //            // Calculate direction to target (ignore Y axis for rotation)
    //            Vector3 directionToTarget = target - transform.position;
    //            directionToTarget.y = 0; // Keep rotation only on horizontal plane

    //            // Only rotate if there's a significant direction to move
    //            if (directionToTarget.sqrMagnitude > 0.01f)
    //            {
    //                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
    //                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    //                Debug.Log("Running targetRotation 2: " + targetRotation);
    //                Debug.Log("Running transformRotation 2: " + transform.rotation);
    //            }

    //            Vector3 next = Vector3.MoveTowards(transform.position, target, step);
    //            transform.position = next;
    //            playerAnim.RunAnimation();

    //            if ((transform.position - target).sqrMagnitude <= playerControls.arriveThreshold * playerControls.arriveThreshold)
    //            {
    //                transform.position = target;
    //                playerControls.currentState = PlayerState.Neutral;
    //                userControlManager.ExitState(userControlManager.currentState);
    //                userControlManager.EnterState(userControlManager.IDLE);
    //            }
    //        }
    //    }
    //}
}

//using UnityEngine;
//using static PlayerClickControls;

//public class CA_MoveCharacter : MonoBehaviour
//{
//    public UserControlManager userControlManager;

//    public PlayerClickControls playerControls;

//    public PlayerAnim playerAnim;
//    public void Move()
//    {
//        //Debug.Log($"Move() called. State: {playerControls.currentState}, ToPos: {playerControls.toPos}");

//        if (playerControls.currentState == PlayerClickControls.PlayerState.Moving)
//        {
//            // target = fromPos (moving from toPos -> fromPos as requested)
//            Vector3 target = playerControls.toPos;
//            float step = playerControls.moveSpeed * Time.fixedDeltaTime;

//            if (playerControls.rb != null)
//            {
//                //Debug.Log($"Current pos: {playerControls.rb.position}, Target: {target}, Distance: {Vector3.Distance(playerControls.rb.position, target)}");
//                //Debug.Log("Moving To Target Target");
//                Vector3 next = Vector3.MoveTowards(playerControls.rb.position, target, step);
//                playerControls.rb.MovePosition(next);
//                playerAnim.RunAnimation();

//                if ((playerControls.rb.position - target).sqrMagnitude <= playerControls.arriveThreshold * playerControls.arriveThreshold)
//                {
//                    //Debug.Log("Reached Target");
//                    playerControls.rb.position = target;
//                    playerControls.currentState = PlayerState.Neutral;
//                    userControlManager.ExitState(userControlManager.currentState);
//                    userControlManager.EnterState(userControlManager.IDLE);
//                    //userControlManager.currentState = userControlManager.IDLE;
//                }
//            }
//            else
//            {
//                //Debug.Log("Moving To Target Target 2");
//                Vector3 next = Vector3.MoveTowards(transform.position, target, step);
//                playerAnim.RunAnimation();
//                transform.position = next;

//                if ((transform.position - target).sqrMagnitude <= playerControls.arriveThreshold * playerControls.arriveThreshold)
//                {
//                    //Debug.Log("Reached Target 2");
//                    transform.position = target;
//                    playerControls.currentState = PlayerState.Neutral;
//                    userControlManager.ExitState(userControlManager.currentState);
//                    userControlManager.EnterState(userControlManager.IDLE);
//                    //userControlManager.currentState = userControlManager.IDLE;
//                }
//            }
//        }
//        else
//        {
//            //Debug.Log($"Not in Moving state! Current state: {playerControls.currentState}");
//        }
//    }

//}
