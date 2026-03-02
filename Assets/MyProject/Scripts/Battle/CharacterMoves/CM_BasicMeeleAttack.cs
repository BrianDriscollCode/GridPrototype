using UnityEngine;


public class CM_BasicMeeleAttack
{
    public UserControlOrchestrator userControlOrchestrator;
    public EnemyAI enemyAI;
    public PlayerClickControls playerControls;
    public PlayerAnim playerAnim;
    public float rotationSpeed = 10f;

    public void Attack()
    {
        // Rotate to face the target before attacking
        RotateTowardsTarget();
        
        playerAnim.RunBasicMeeleAnimation();
        playerAnim.ChangeAnimation("BasicMeeleAttack");
        ////Debug.Log"Running attack");
    }

    private void RotateTowardsTarget()
    {
        if (userControlOrchestrator == null || userControlOrchestrator.target == null)
            return;

        GameObject attacker;
        GameObject target;

        if (userControlOrchestrator.userControlState is IUSO_Battle_EnemyTurn_State)
        {
            attacker = enemyAI.currentEnemy;
            target = enemyAI.currentTarget;
        }
        else
        {
            attacker = playerAnim.gameObject;
            target = userControlOrchestrator.target;
        }

            // Calculate direction to target (ignore Y axis for rotation)
        Vector3 directionToTarget = target.transform.position - attacker.transform.position;
        directionToTarget.y = 0;

        // Only rotate if there's a significant direction
        if (directionToTarget.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            
            // Instant rotation (since attack is immediate)
            attacker.transform.rotation = targetRotation;
            
            // OR use smooth rotation
            // attacker.transform.rotation = Quaternion.Slerp(
            //     attacker.transform.rotation, 
            //     targetRotation, 
            //     rotationSpeed * Time.deltaTime
            // );
        }
    }

    public void FinishBasicMeeleAttack()
    {
        EventManager.OnFinishBasicMeeleAttack();    
    }
}

