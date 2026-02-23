using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

//DEPRECATED

//public class UserControlInterface : MonoBehaviour
//{
//    public Camera camera;
//    private InputSystem_Actions input;
//    public GridManager gridSystem;
//    public LayerMask tileLayer;

//    public GameObject currentCharacter;

//    [Header("Hover Detection")]
//    public bool enableHover = true;
//    public LayerMask hoverLayer; // Set in Inspector (can use same as tileLayer)
    
//    private GameObject _currentHoveredObject;
//    private Vector2Int _currentHoveredGridPos = new Vector2Int(-1, -1);

//    // --- Gizmo Debug Data ---
//    private Ray _lastRay;
//    private bool _hasRay;
//    private bool _lastRayHit;
//    private Vector3 _lastHitPoint;
//    private float _lastRayLength = 100f;

//    // Hover ray debug
//    private Ray _hoverRay;
//    private bool _hasHoverRay;
//    private bool _hoverHit;
//    private Vector3 _hoverHitPoint;
//    private float _hoverRayLength = 100f;

//    private void Start()
//    {
//        input = GameObject.Find("InputSystem").GetComponent<InputSystem>().input;

//        if (camera == null)
//            camera = Camera.main;
        
//        if (hoverLayer == 0)
//            hoverLayer = tileLayer;
//    }

//    void Update()
//    {
//        if (input.Player.LeftClick.IsPressed())
//        {
//            HandleClick();
//        }

//        if (enableHover)
//        {
//            HandleHover();
//        }
//    }

//    void HandleHover()
//    {
//        // Raycast from camera center (like a crosshair)
//        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        
//        _hoverRay = ray;
//        _hasHoverRay = true;

//        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, hoverLayer))
//        {
//            _hoverHit = true;
//            _hoverHitPoint = hit.point;
//            _hoverRayLength = hit.distance;

//            GameObject hitObject = hit.collider.gameObject;
//            Vector2Int gridPos = gridSystem.WorldToGridPosition(hit.point);

//            // Check if we started hovering a new object
//            if (_currentHoveredObject != hitObject)
//            {
//                // Exit previous hover
//                if (_currentHoveredObject != null)
//                {
//                    OnHoverExit(_currentHoveredObject, _currentHoveredGridPos);
//                }

//                // Enter new hover
//                _currentHoveredObject = hitObject;
//                _currentHoveredGridPos = gridPos;
//                OnHoverEnter(hitObject, gridPos);
//            }
//        }
//        else
//        {
//            _hoverHit = false;
//            _hoverRayLength = 100f;

//            // Clear hover if we had one
//            if (_currentHoveredObject != null)
//            {
//                OnHoverExit(_currentHoveredObject, _currentHoveredGridPos);
//                _currentHoveredObject = null;
//                _currentHoveredGridPos = new Vector2Int(-1, -1);
//            }
//        }

//    }

//    // Override these methods to implement your hover behavior
//    protected virtual void OnHoverEnter(GameObject obj, Vector2Int gridPos)
//    {
//        //Debug.Log$"Hover Enter: {obj.name} at grid ({gridPos.x}, {gridPos.y})");
//        // Add your hover enter logic here (e.g., highlight tile)
//    }

//    protected virtual void OnHoverExit(GameObject obj, Vector2Int gridPos)
//    {
//        //Debug.Log$"Hover Exit: {obj.name} at grid ({gridPos.x}, {gridPos.y})");
//        // Add your hover exit logic here (e.g., remove highlight)
//    }

//    void HandleClick()
//    {
//        Vector2 mousePos = Mouse.current.position.ReadValue();
//        Ray ray = camera.ScreenPointToRay(mousePos);

//        _lastRay = ray;
//        _hasRay = true;

//        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileLayer))
//        {
//            _lastRayHit = true;
//            _lastHitPoint = hit.point;
//            _lastRayLength = hit.distance;

//            Vector2Int gridPos = gridSystem.WorldToGridPosition(hit.point); 
//            //Debug.Log$"UserControlInterface::Clicked grid position: ({gridPos.x}, {gridPos.y})");
//            //Debug.Log"UserControlInterface::HasTileAt: " + gridSystem.HasTileAt(gridPos.x, gridPos.y));

//            EventManager.OnClickedTile(gridPos);
//        }
//        else
//        {
//            //Debug.Log"No hit");
//            _lastRayHit = false;
//            _lastRayLength = 100f;
//        }

//        Debug.DrawRay(ray.origin, ray.direction * _lastRayLength,
//            _lastRayHit ? Color.green : Color.red, 0.2f);
//    }

//    private void OnDrawGizmos()
//    {
//        // Click ray
//        if (_hasRay)
//        {
//            Gizmos.color = _lastRayHit ? Color.green : Color.red;
//            Gizmos.DrawLine(_lastRay.origin, _lastRay.origin + _lastRay.direction * _lastRayLength);

//            if (_lastRayHit)
//            {
//                Gizmos.color = Color.yellow;
//                Gizmos.DrawSphere(_lastHitPoint, 0.15f);
//            }
//        }

//        // Hover ray
//        if (_hasHoverRay)
//        {
//            Gizmos.color = _hoverHit ? Color.cyan : Color.magenta;
//            Gizmos.DrawLine(_hoverRay.origin, _hoverRay.origin + _hoverRay.direction * _hoverRayLength);

//            if (_hoverHit)
//            {
//                Gizmos.color = Color.blue;
//                Gizmos.DrawWireSphere(_hoverHitPoint, 0.25f);
//            }
//        }
//    }       
//}
