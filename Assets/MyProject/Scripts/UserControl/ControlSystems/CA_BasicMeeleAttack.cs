using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CA_BasicMeeleAttack : MonoBehaviour
{
    public UserControlOrchestrator userControlOrchestrator;
    public PlayerClickControls playerControls;
    public PlayerAnim playerAnim;
    public InputSystem_Actions input;
    public CM_BasicMeeleAttack CM_BasicMeeleAttack;

    public float rotationSpeed = 10f;


    public void Start()
    {
        CM_BasicMeeleAttack = new CM_BasicMeeleAttack();
        CM_BasicMeeleAttack.playerAnim = playerAnim;
        CM_BasicMeeleAttack.playerControls = playerControls;
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
            EventManager.OnRightClickAttack();
        }
    }
}