using UnityEngine;
using static PlayerClickControls;

public class CA_MoveCharacter : MonoBehaviour
{
    public UserControlManager userControlManager;

    public PlayerClickControls playerControls;
    public void Move()

    {
        if (playerControls.currentState == PlayerClickControls.PlayerState.Moving)
        {
            // target = fromPos (moving from toPos -> fromPos as requested)
            Vector3 target = playerControls.toPos;
            float step = playerControls.moveSpeed * Time.fixedDeltaTime;

            if (playerControls.rb != null)
            {
                Vector3 next = Vector3.MoveTowards(playerControls.rb.position, target, step);
                playerControls.rb.MovePosition(next);

                if ((playerControls.rb.position - target).sqrMagnitude <= playerControls.arriveThreshold * playerControls.arriveThreshold)
                {
                    playerControls.rb.position = target;
                    playerControls.currentState = PlayerState.Nuetral;
                }
            }
            else
            {
                Vector3 next = Vector3.MoveTowards(transform.position, target, step);
                transform.position = next;

                if ((transform.position - target).sqrMagnitude <= playerControls.arriveThreshold * playerControls.arriveThreshold)
                {
                    transform.position = target;
                    playerControls.currentState = PlayerState.Nuetral;
                }
            }
        }
    }

}
