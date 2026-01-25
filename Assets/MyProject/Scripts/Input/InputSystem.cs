using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public InputSystem_Actions input;

    private void Awake()
    {
        input = new InputSystem_Actions();
        input.Player.Enable();
    }
}
