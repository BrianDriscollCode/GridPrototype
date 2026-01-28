using UnityEngine;

public class CA_IdleCharacter : MonoBehaviour
{
	public UserControlManager userControlManager;

	public PlayerClickControls playerControls;

	public PlayerAnim playerAnim;

	public CM_Idle CM_Idle;

    public void Start()
    {
		CM_Idle = new CM_Idle(userControlManager, playerControls, playerAnim); 
    }

    public void Action()
	{

        // Add null check to prevent NullReferenceException
        if (CM_Idle == null)
        {
            // Initialize immediately if not ready
            CM_Idle = new CM_Idle(userControlManager, playerControls, playerAnim);
        }

        CM_Idle.Idle();
	}
}