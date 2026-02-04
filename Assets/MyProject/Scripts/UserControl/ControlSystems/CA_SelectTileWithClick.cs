using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class CA_SelectTileWithClick : MonoBehaviour
{
    // new User Control Manager
    public UserControlOrchestrator userControlOrchestrator;
    public InputSystem_Actions input;

    // will be deprecated
    //public UserControlManager userControlManager;

    public void Action()
    {
        Debug.Log("Select Tile CA Run");
        HandleLeftClick();
    }

    private void HandleLeftClick()
    {

        if (input.Player.LeftClick.WasPressedThisFrame())
        {
            Debug.Log("Handle Click is pressed");

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = userControlOrchestrator.camera.ScreenPointToRay(mousePos);

            userControlOrchestrator.interfaceRaycastSelection._lastRay = ray;
            userControlOrchestrator.interfaceRaycastSelection._hasRay = true;

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, userControlOrchestrator.interfaceRaycastSelection.tileLayer))
            {
                userControlOrchestrator.interfaceRaycastSelection._lastRayHit = true;
                userControlOrchestrator.interfaceRaycastSelection._lastHitPoint = hit.point;
                userControlOrchestrator.interfaceRaycastSelection._lastRayLength = hit.distance;

                // get game object hit
                userControlOrchestrator.selectedTile = hit.collider.gameObject;

                Vector2Int gridPos = userControlOrchestrator.interfaceRaycastSelection.gridManager.WorldToGridPosition(hit.point);
                //Debug.Log($"UserControlInterface::Clicked grid position: ({gridPos.x}, {gridPos.y})");
                //Debug.Log("UserControlInterface::HasTileAt: " + userControlOrchestrator.interfaceRaycastSelection.gridManager.HasTileAt(gridPos.x, gridPos.y));

                GridManager gridManager = userControlOrchestrator.interfaceRaycastSelection.gridManager;

                if (gridManager.IsTileAccessible(gridPos.x, gridPos.y))
                {
                    EventManager.OnClickedTile(gridPos);
                    Debug.Log("CA_SelectTileWithClick::EventManager.OnClickedTile ran");
                }
                else
                {
                    Debug.Log("Tile In accessible");
                }
            }
            else
            {
                Debug.Log("No hit");
                userControlOrchestrator.interfaceRaycastSelection._lastRayHit = false;
                userControlOrchestrator.interfaceRaycastSelection._lastRayLength = 100f;
            }
            Debug.DrawRay(ray.origin, ray.direction * userControlOrchestrator.interfaceRaycastSelection._lastRayLength,
            userControlOrchestrator.interfaceRaycastSelection._lastRayHit ? Color.green : Color.red, 0.2f);
        }
       
    }
}
