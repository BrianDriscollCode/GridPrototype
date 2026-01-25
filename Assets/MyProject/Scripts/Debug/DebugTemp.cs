using UnityEngine;
using UnityEngine.InputSystem;

public class DebugTemp : MonoBehaviour
{
    public GridManager gridSystem;
    public LayerMask tileLayer;
    private InputSystem_Actions input;
    public Camera camera;

    // --- Gizmo Debug Data ---
    private Ray _lastRay;
    private bool _hasRay;
    private bool _lastRayHit;
    private Vector3 _lastHitPoint;
    private float _lastRayLength = 100f;

    private void Start()
    {
        input = GameObject.Find("InputSystem").GetComponent<InputSystem>().input;

        if (camera == null)
            camera = Camera.main;
    }

    void Update()
    {
        if (input.Player.LeftClick.IsPressed())
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        //Debug.Log("mousePos: " + mousePos);
        Ray ray = camera.ScreenPointToRay(mousePos);
        //Debug.Log(ray);

        // store ray so Gizmos can draw it
        _lastRay = ray;
        _hasRay = true;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileLayer))
        {
            //Debug.Log("hit");
            _lastRayHit = true;
            _lastHitPoint = hit.point;


            // length to the hit point so we draw exactly to impact
            _lastRayLength = hit.distance;

            // ****Refactor work - .y and .z being the same can get confusing because of the
            //                     Vector2Int data type
            //Debug.Log($"Hit Point: " + hit.point);
            Vector2Int gridPos = gridSystem.WorldToGridPosition(hit.point);
            Debug.Log($"Clicked grid position: ({gridPos.x}, {gridPos.y})");

            Debug.Log("HasTileAt: " + gridSystem.HasTileAt(gridPos.x, gridPos.y));

        }
        else
        {
            Debug.Log("No hit");
            _lastRayHit = false;

            // draw a long ray if no hit
            _lastRayLength = 100f;
        }

        // Optional: also draw in Scene view immediately for 0.2 sec
        Debug.DrawRay(ray.origin, ray.direction * _lastRayLength,
            _lastRayHit ? Color.green : Color.red, 0.2f);
    }

    private void OnDrawGizmos()
    {
        if (!_hasRay) return;

        // draw ray
        Gizmos.color = _lastRayHit ? Color.green : Color.red;
        Gizmos.DrawLine(_lastRay.origin, _lastRay.origin + _lastRay.direction * _lastRayLength);

        // draw hit point sphere if hit
        if (_lastRayHit)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_lastHitPoint, 0.15f);
        }
    }
}
