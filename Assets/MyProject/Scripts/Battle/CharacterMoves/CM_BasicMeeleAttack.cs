using UnityEngine;


public class CM_BasicMeeleAttack
{
    public UserControlOrchestrator userControlOrchestrator;
    public PlayerClickControls playerControls;
    public PlayerAnim playerAnim;

    public void Attack()
    {
        playerAnim.RunBasicMeeleAnimation();
        playerAnim.ChangeAnimation("BasicMeeleAttack");
        Debug.Log("Running attack");
    }

    public void FinishBasicMeeleAttack()
    {
        userControlOrchestrator.selectedCharacter.GetComponent<PlayerAnim>().ChangeAnimation("Idle");
        EventManager.OnFinishBasicMeeleAttack();    
    }
}

