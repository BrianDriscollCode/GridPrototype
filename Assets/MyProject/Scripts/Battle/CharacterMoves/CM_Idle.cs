using UnityEngine;

public class CM_Idle
{

    public PlayerClickControls playerControls;

    public PlayerAnim playerAnim;

    public CM_Idle(PlayerClickControls playerControls, PlayerAnim playerAnim)
    {
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
