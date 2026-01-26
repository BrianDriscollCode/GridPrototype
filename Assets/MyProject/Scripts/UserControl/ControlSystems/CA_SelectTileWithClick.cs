using UnityEngine;
using UnityEngine.InputSystem;

public class CA_SelectTileWithClick : MonoBehaviour
{
    public UserControlManager userControlManager;

    public void HandleClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = userControlManager.camera.ScreenPointToRay(mousePos);

        userControlManager._lastRay = ray;
        userControlManager._hasRay = true;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, userControlManager.tileLayer))
        {
            userControlManager._lastRayHit = true;
            userControlManager._lastHitPoint = hit.point;
            userControlManager._lastRayLength = hit.distance;

            // get game object hit
            userControlManager.selectedTile = hit.collider.gameObject;

            Vector2Int gridPos = userControlManager.gridManager.WorldToGridPosition(hit.point);
            //Debug.Log($"UserControlInterface::Clicked grid position: ({gridPos.x}, {gridPos.y})");
            //Debug.Log("UserControlInterface::HasTileAt: " + userControlManager.gridManager.HasTileAt(gridPos.x, gridPos.y));

            GridManager gridManager = userControlManager.gridManager;

            if (gridManager.IsTileAccessible(gridPos.x, gridPos.y))
            {
                EventManager.OnClickedTile(gridPos);
            }
            else
            {
                //Debug.Log("Tile In accessible");
            }


            
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
