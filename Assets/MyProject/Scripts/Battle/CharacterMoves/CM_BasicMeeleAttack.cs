using UnityEngine;


public class CM_BasicMeeleAttack
{
    public UserControlOrchestrator userControlOrchestrator;
    public PlayerClickControls playerControls;
    public PlayerAnim playerAnim;

    public void Attack()
    {
        playerAnim.RunBasicMeeleAnimation();
        Debug.Log("Running attack");
    }

    public void FinishBasicMeeleAttack()
    {
        EventManager.OnFinishBasicMeeleAttack();
    }
}

