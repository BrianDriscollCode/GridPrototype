using UnityEngine;
using UnityEngine.InputSystem;

public class CA_SelectCharacterWithClick : MonoBehaviour
{
    // new User Control Manager
    public UserControlOrchestrator userControlOrchestrator;
    public InterfaceRaycastSelection interfaceRaycastSelection;
    public InputSystem_Actions input;

    // will be deprecated
    //public UserControlManager userControlManager;

    public void Action()
    {
        if (input.Player.LeftClick.IsPressed())
        {
            HandleClick();
        }  
    }

    public void HandleClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = userControlOrchestrator.camera.ScreenPointToRay(mousePos);

        interfaceRaycastSelection._lastRay = ray;
        interfaceRaycastSelection._hasRay = true;


        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, userControlOrchestrator.characterHoverLayer))
        {
            interfaceRaycastSelection._lastRayHit = true;
            interfaceRaycastSelection._lastHitPoint = hit.point;
            interfaceRaycastSelection._lastRayLength = hit.distance;

            // get game object hit
            userControlOrchestrator.target = hit.collider.gameObject;
            
        }
        else
        {
            //Debug.Log("No hit");
            interfaceRaycastSelection._lastRayHit = false;
            interfaceRaycastSelection._lastRayLength = 100f;
        }

        Debug.DrawRay(ray.origin, ray.direction * interfaceRaycastSelection._lastRayLength,
            interfaceRaycastSelection._lastRayHit ? Color.green : Color.red, 0.2f);
    }
}