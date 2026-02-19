using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRig : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float smoothTime = 0.1f;

    private Vector3 velocity = Vector3.zero;
    private Vector2 moveInput;
    public InputSystem inputSystem;
    private InputSystem_Actions input;

    private void Start()
    {
        if (inputSystem == null)
        {
            inputSystem = GameObject.FindFirstObjectByType<InputSystem>();
        }

        if (inputSystem != null && inputSystem.input != null)
        {
            input = inputSystem.input;
            SubscribeToInputEvents();
        }
        else
        {
            Debug.LogError("CameraRig: InputSystem or InputSystem_Actions is null!");
        }
    }

    private void OnDisable()
    {
        UnsubscribeFromInputEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromInputEvents();
    }

    private void SubscribeToInputEvents()
    {
        if (input == null) return;

        input.Player.Enable();

        input.Player.Left.performed += OnLeftPerformed;
        input.Player.Left.canceled += OnLeftCanceled;

        input.Player.Right.performed += OnRightPerformed;
        input.Player.Right.canceled += OnRightCanceled;

        input.Player.TrueUp.performed += OnTrueUpPerformed;
        input.Player.TrueUp.canceled += OnTrueUpCanceled;

        input.Player.TrueDown.performed += OnTrueDownPerformed;
        input.Player.TrueDown.canceled += OnTrueDownCanceled;
    }

    private void UnsubscribeFromInputEvents()
    {
        if (input == null) return;

        input.Player.Left.performed -= OnLeftPerformed;
        input.Player.Left.canceled -= OnLeftCanceled;

        input.Player.Right.performed -= OnRightPerformed;
        input.Player.Right.canceled -= OnRightCanceled;

        input.Player.TrueUp.performed -= OnTrueUpPerformed;
        input.Player.TrueUp.canceled -= OnTrueUpCanceled;

        input.Player.TrueDown.performed -= OnTrueDownPerformed;
        input.Player.TrueDown.canceled -= OnTrueDownCanceled;
    }

    private void OnLeftPerformed(InputAction.CallbackContext context)
    {
        moveInput.x = -1f;
    }

    private void OnLeftCanceled(InputAction.CallbackContext context)
    {
        moveInput.x = 0f;
    }

    private void OnRightPerformed(InputAction.CallbackContext context)
    {
        moveInput.x = 1f;
    }

    private void OnRightCanceled(InputAction.CallbackContext context)
    {
        moveInput.x = 0f;
    }

    private void OnTrueUpPerformed(InputAction.CallbackContext context)
    {
        moveInput.y = 1f;
    }

    private void OnTrueUpCanceled(InputAction.CallbackContext context)
    {
        moveInput.y = 0f;
    }

    private void OnTrueDownPerformed(InputAction.CallbackContext context)
    {
        moveInput.y = -1f;
    }

    private void OnTrueDownCanceled(InputAction.CallbackContext context)
    {
        moveInput.y = 0f;
    }

    private void Update()
    {
        // Calculate movement direction aligned with isometric view
        Vector3 inputDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        // Rotate input to match isometric angle (45° on Y-axis)
        Vector3 moveDirection = Quaternion.Euler(0, 45, 0) * inputDirection;

        // Apply movement with smoothing
        Vector3 targetPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}