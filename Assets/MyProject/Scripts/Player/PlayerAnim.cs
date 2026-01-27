using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    public Animator playerAnimator;

    private int isRunningHash;
    private int isIdleHash;

    void Awake()
    {
        if (playerAnimator == null)
            playerAnimator = GetComponentInChildren<Animator>();

        isRunningHash = Animator.StringToHash("IsRunning");
        isIdleHash = Animator.StringToHash("IsIdle");
    }
    
    public void RunAnimation()
    {
        Debug.Log("Run Animation");
        playerAnimator.SetBool(isRunningHash, true);
        playerAnimator.SetBool(isIdleHash, false);
    }

    public void IdleAnimation()
    {
        Debug.Log("Idle Animation");
        playerAnimator.SetBool(isRunningHash, false);
        playerAnimator.SetBool(isIdleHash, true);
    }

}