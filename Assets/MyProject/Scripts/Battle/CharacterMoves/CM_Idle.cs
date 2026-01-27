using UnityEngine;

public class CM_Idle
{
    public UserControlManager userControlManager;

    public PlayerClickControls playerControls;

    public PlayerAnim playerAnim;

    public CM_Idle(UserControlManager userControlManager, PlayerClickControls playerControls, PlayerAnim playerAnim)
    {
        this.userControlManager = userControlManager;
        this.playerControls = playerControls;
        this.playerAnim = playerAnim;
    }


    public void Idle()
    {
        playerAnim.IdleAnimation();
        //if (playerControls.currentState == PlayerClickControls.PlayerState.Neutral)
        //{
        //playerAnim.IdleAnimation();
        //}
    }
}
