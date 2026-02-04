using UnityEngine;
using static PlayerClickControls;

public class CA_MoveCharacter : MonoBehaviour
{
    public CM_Move cm_move;

    // new User Control Manager
    public UserControlOrchestrator userControlOrchestrator;

    // will be deprecated
    public UserControlManager userControlManager;

    public PlayerClickControls playerControls;
    public PlayerAnim playerAnim;

    [Header("Rotation Settings")]
    public float rotationSpeed = 10f; // How fast the character rotates toward target

    public void Start()
    {
        InitializeCMMove();
    }
    private void InitializeCMMove()
    {
        if (cm_move == null)
        {
            cm_move = new CM_Move(userControlOrchestrator, userControlManager, playerControls, playerAnim);
            cm_move.rotationSpeed = rotationSpeed;
        }
    }

    public void Action()
    {
        // Lazy initialization - create if not ready
        if (cm_move == null)
        {
            InitializeCMMove();
        }

        cm_move.Move();
    }
    
}