using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    public Animator playerAnimator;

    private int isAttackingHash;
    private int isRunningHash;
    private int isIdleHash;

    void Awake()
    {
        if (playerAnimator == null)
            playerAnimator = GetComponentInChildren<Animator>();

        isAttackingHash = Animator.StringToHash("IsAttacking");
        isRunningHash = Animator.StringToHash("IsRunning");
        isIdleHash = Animator.StringToHash("IsIdle");
    }

    public void RunBasicMeeleAnimation()
    {
        Debug.Log("Basic Meele Attack Animation");
        playerAnimator.SetBool(isAttackingHash, true);
        playerAnimator.SetBool(isRunningHash, false);
        playerAnimator.SetBool(isIdleHash, false);
    }
    
    public void RunAnimation()
    {
        Debug.Log("Run Animation");
        playerAnimator.SetBool(isAttackingHash, false);
        playerAnimator.SetBool(isRunningHash, true);
        playerAnimator.SetBool(isIdleHash, false);
    }

    public void IdleAnimation()
    {
        Debug.Log("Idle Animation");
        playerAnimator.SetBool(isAttackingHash, false);
        playerAnimator.SetBool(isRunningHash, false);
        playerAnimator.SetBool(isIdleHash, true);
    }

}