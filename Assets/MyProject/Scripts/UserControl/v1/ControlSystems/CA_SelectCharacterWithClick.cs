using UnityEngine;
using UnityEngine.InputSystem;

public class CA_SelectCharacterWithClick : MonoBehaviour
{
    // new User Control Manager
    public UserControlOrchestrator userControlOrchestrator;

    // will be deprecated
    public UserControlManager userControlManager;

    public void Action()
    {
        HandleClick();
    }

    public void HandleClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = userControlManager.camera.ScreenPointToRay(mousePos);

        userControlManager._lastRay = ray;
        userControlManager._hasRay = true;


        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, userControlManager.characterHoverLayer))
        {
            userControlManager._lastRayHit = true;
            userControlManager._lastHitPoint = hit.point;
            userControlManager._lastRayLength = hit.distance;

            // get game object hit
            userControlManager.selectedCharacter = hit.collider.gameObject;
            

        }
        else
        {
            Debug.Log("No hit");
            userControlManager._lastRayHit = false;
            userControlManager._lastRayLength = 100f;
        }

        Debug.DrawRay(ray.origin, ray.direction * userControlManager._lastRayLength,
            userControlManager._lastRayHit ? Color.green : Color.red, 0.2f);
    }
}