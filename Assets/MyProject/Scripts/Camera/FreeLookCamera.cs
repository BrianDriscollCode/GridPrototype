using UnityEngine;
using UnityEngine.InputSystem;

public class FreeLookCamera : MonoBehaviour
{
    [SerializeField] bool Active;

    [Header("Movement")]
    public float moveSpeed = 8f;
    public float fastMultiplier = 2.5f;
    public float slowMultiplier = 0.35f;

    [Header("Look")]
    public float mouseSensitivity = 0.15f;
    public float pitchMin = -89f;
    public float pitchMax = 89f;

    float yaw;
    float pitch;

    InputSystem_Actions input;

    Vector2 moveInput;
    Vector2 lookInput;

    bool rmb;
    bool fast;
    bool slow;

    bool moveUp;
    bool moveDown;

    void Awake()
    {
        if (!Active)
        {
            return;
        }
        input = new InputSystem_Actions();
    }

    void OnEnable()
    {
        if (!Active)
        {
            return;
        }

        input.Player.Enable();

        input.Player.Move.performed += OnMovePerformed;
        input.Player.Move.canceled += OnMoveCanceled;

        input.Player.Look.performed += OnLookPerformed;
        input.Player.Look.canceled += OnLookCanceled;

        input.Player.RightClick.started += OnRightClickStarted;
        input.Player.RightClick.canceled += OnRightClickCanceled;

        input.Player.Fast.performed += OnFastPerformed;
        input.Player.Fast.canceled += OnFastCanceled;

        input.Player.Slow.performed += OnSlowPerformed;
        input.Player.Slow.canceled += OnSlowCanceled;

        // Q / E
        input.Player.Up.started += OnUpStarted;
        input.Player.Up.canceled += OnUpCanceled;

        input.Player.Down.started += OnDownStarted;
        input.Player.Down.canceled += OnDownCanceled;
    }

    void OnDisable()
    {
        if (!Active)
        {
            return;
        }
        input.Player.Move.performed -= OnMovePerformed;
        input.Player.Move.canceled -= OnMoveCanceled;

        input.Player.Look.performed -= OnLookPerformed;
        input.Player.Look.canceled -= OnLookCanceled;

        input.Player.RightClick.started -= OnRightClickStarted;
        input.Player.RightClick.canceled -= OnRightClickCanceled;

        input.Player.Fast.performed -= OnFastPerformed;
        input.Player.Fast.canceled -= OnFastCanceled;

        input.Player.Slow.performed -= OnSlowPerformed;
        input.Player.Slow.canceled -= OnSlowCanceled;

        input.Player.Up.started -= OnUpStarted;
        input.Player.Up.canceled -= OnUpCanceled;

        input.Player.Down.started -= OnDownStarted;
        input.Player.Down.canceled -= OnDownCanceled;

        input.Player.Disable();
    }

    void OnDestroy()
    {
        input?.Disable();
        input?.Dispose();
        input = null;
    }

    void Start()
    {
        Vector3 e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x;
    }

    void Update()
    {
        if (!Active)
        {
            return;
        }
        HandleLook();
        HandleMove();
    }

    void HandleLook()
    {
        if (!rmb)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yaw += lookInput.x * mouseSensitivity;
        pitch -= lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void HandleMove()
    {
        float speed = moveSpeed;
        if (fast) speed *= fastMultiplier;
        if (slow) speed *= slowMultiplier;

        float vertical = 0f;
        if (moveUp) vertical += 1f;
        if (moveDown) vertical -= 1f;

        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y +
            Vector3.up * vertical;

        transform.position += move * speed * Time.deltaTime;
    }

    // ---- Input callbacks ----

    void OnMovePerformed(InputAction.CallbackContext ctx) => moveInput = ctx.ReadValue<Vector2>();
    void OnMoveCanceled(InputAction.CallbackContext _) => moveInput = Vector2.zero;

    void OnLookPerformed(InputAction.CallbackContext ctx) => lookInput = ctx.ReadValue<Vector2>();
    void OnLookCanceled(InputAction.CallbackContext _) => lookInput = Vector2.zero;

    void OnRightClickStarted(InputAction.CallbackContext _) => rmb = true;
    void OnRightClickCanceled(InputAction.CallbackContext _) => rmb = false;

    void OnFastPerformed(InputAction.CallbackContext _) => fast = true;
    void OnFastCanceled(InputAction.CallbackContext _) => fast = false;

    void OnSlowPerformed(InputAction.CallbackContext _) => slow = true;
    void OnSlowCanceled(InputAction.CallbackContext _) => slow = false;

    void OnUpStarted(InputAction.CallbackContext _) => moveUp = true;
    void OnUpCanceled(InputAction.CallbackContext _) => moveUp = false;

    void OnDownStarted(InputAction.CallbackContext _) => moveDown = true;
    void OnDownCanceled(InputAction.CallbackContext _) => moveDown = false;
}