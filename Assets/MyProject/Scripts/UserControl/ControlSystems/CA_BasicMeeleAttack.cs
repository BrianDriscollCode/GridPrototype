using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CA_BasicMeeleAttack : MonoBehaviour
{
    public UserControlOrchestrator userControlOrchestrator;
    public EnemyAI enemyAI;
    public PlayerClickControls playerControls;
    public PlayerAnim playerAnim;
    public InputSystem_Actions input;
    public CM_BasicMeeleAttack CM_BasicMeeleAttack;

    public bool IsEnemy = false;

    public float rotationSpeed = 10f;


    public void Start()
    {
        CM_BasicMeeleAttack = new CM_BasicMeeleAttack();
        CM_BasicMeeleAttack.playerAnim = playerAnim;
        CM_BasicMeeleAttack.playerControls = playerControls;
        CM_BasicMeeleAttack.userControlOrchestrator = userControlOrchestrator;
        CM_BasicMeeleAttack.enemyAI = enemyAI;
        CM_BasicMeeleAttack.rotationSpeed = rotationSpeed;
    }

    public void Action()
    {
        if (CM_BasicMeeleAttack != null)
        {
            CM_BasicMeeleAttack.Attack();
        }
    }

    public void ActionHandler()
    {     
        HandleRightClick();
    }

    public void HandleRightClick()
    {
        if (input.Player.RightClick.WasPressedThisFrame())
        {
            int distance = userControlOrchestrator.gridManager.GetTileDistance(userControlOrchestrator.target.GetComponent<EntityGridLocation>().gridPos, userControlOrchestrator.selectedCharacter.GetComponent<EntityGridLocation>().gridPos);

            // TODO: Prompt warn the player "Enemy too far away"
            // Probably should move this to it's own script to handle
            // checks and warnings

            if (userControlOrchestrator.target && distance == 1)
                EventManager.OnRightClickAttack();
        }
    }
}