using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    public Animator playerAnimator;
    public string currentAnimation = "";


    void Awake()
    {
        if (playerAnimator == null)
            playerAnimator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        ChangeAnimation("Idle");
    }

    public void ChangeAnimation(string animation, float crossfade = 0.1f)
    {
        if (currentAnimation != animation)
        {
            currentAnimation = animation;

            if (currentAnimation == "Idle")
            {
                crossfade = 0.2f;
            }

            playerAnimator.CrossFade(animation, crossfade);
        }
    }

    public void RunBasicMeeleAnimation()
    {
        ////Debug.Log"Basic Meele Attack Animation");
        //playerAnimator.SetBool(isAttackingHash, true);
        //playerAnimator.SetBool(isRunningHash, false);
        //playerAnimator.SetBool(isIdleHash, false);
    }
    
    public void RunAnimation()
    {
        ////Debug.Log"Run Animation");
        //playerAnimator.SetBool(isAttackingHash, false);
        //playerAnimator.SetBool(isRunningHash, true);
        //playerAnimator.SetBool(isIdleHash, false);
    }

    public void IdleAnimation()
    {
        ////Debug.Log"Idle Animation");
        //playerAnimator.SetBool(isAttackingHash, false);
        //playerAnimator.SetBool(isRunningHash, false);
        //playerAnimator.SetBool(isIdleHash, true);
    }


}