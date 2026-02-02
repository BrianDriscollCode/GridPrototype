using UnityEngine;

public class InterfaceRaycastSelection : MonoBehaviour
{
    [Header("Systems")]
    public Camera camera;
    public InputSystem_Actions input;
    public GridManager gridManager;

    [Header("Collisions")]
    public LayerMask tileLayer;

    [Header("Hover Detection")]
    public bool enableHover = true;
    public LayerMask hoverLayer; // Set in Inspector (can use same as tileLayer)

    public GameObject _currentHoveredObject;
    public Vector2Int _currentHoveredGridPos = new Vector2Int(-1, -1);

    public Material _originalMaterial;
    public Renderer _hoveredRenderer;

    [Header("Hover Character Detection")]
    public bool enableCharacterHover;
    public LayerMask characterHoverLayer;

    public GameObject _currentCharacterSelected;
    public Vector2Int _currentCharacterGridPos;

    // --- Gizmo Debug Data ---
    public Ray _lastRay;
    public bool _hasRay;
    public bool _lastRayHit;
    public Vector3 _lastHitPoint;
    public float _lastRayLength = 100f;

    // Hover ray debug
    public Ray _hoverRay;
    public bool _hasHoverRay;
    public bool _hoverHit;
    public Vector3 _hoverHitPoint;
    public float _hoverRayLength = 100f;

    public void Start()
    {
        // Set LayerMasks programmatically
        // Layer 6 = Tiles
        tileLayer = 1 << 6;
        hoverLayer = 1 << 6;

        // Layer 7 = Characters
        characterHoverLayer = 1 << 7;
    }

    private void OnDrawGizmos()
    {
        // Click ray
        if (_hasRay)
        {
            Gizmos.color = _lastRayHit ? Color.green : Color.red;
            Gizmos.DrawLine(_lastRay.origin, _lastRay.origin + _lastRay.direction * _lastRayLength);

            if (_lastRayHit)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(_lastHitPoint, 0.15f);
            }
        }

        // Hover ray
        if (_hasHoverRay)
        {
            Gizmos.color = _hoverHit ? Color.cyan : Color.magenta;
            Gizmos.DrawLine(_hoverRay.origin, _hoverRay.origin + _hoverRay.direction * _hoverRayLength);

            if (_hoverHit)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_hoverHitPoint, 0.25f);
            }
        }
    }
}
