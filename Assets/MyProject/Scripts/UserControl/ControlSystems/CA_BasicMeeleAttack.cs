using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CA_BasicMeeleAttack: MonoBehaviour
{
    public UserControlOrchestrator userControlOrchestrator;
    public InputSystem_Actions input;

    public void Action()
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