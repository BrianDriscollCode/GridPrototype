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

        isRunningHash = Animator.StringToHash("isRunning");
        isIdleHash = Animator.StringToHash("isIdle");
    }

    // This script doesn't need Update.
    // Player.cs will set the animator bools every FixedUpdate.
}