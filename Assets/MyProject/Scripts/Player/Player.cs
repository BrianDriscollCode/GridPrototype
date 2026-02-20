using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxSpeed = 5f;
    public float forceMultiplier = 10f;

    // Rotation settings
    public float rotationSpeed = 720f; // degrees per second
    public float minMoveThreshold = 0.05f; // minimum velocity to consider "moving"

    // If your model's forward axis doesn't match Unity's +Z, set this (degrees).
    // Example: model's forward is +X -> set modelYawOffset = -90 (or +90 depending on model orientation).
    public float modelYawOffset = -90f;

    // Animator — Player sets "isRunning" and "isIdle" on this Animator
    public Animator playerAnimator;
    private int isRunningHash;
    private int isIdleHash;

    public Vector3 currentVelocity;

    private Rigidbody rb;
    private Vector3 moveDirection;

    private InputSystem_Actions input;
    private Transform cam;

    void Awake()
    {
    }

    void OnEnable()
    {
    }

    void OnDisable()
    {
    }

    void Start()
    {
        input = GameObject.Find("InputSystem").GetComponent<InputSystem>().input;
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 2f;
        cam = Camera.main != null ? Camera.main.transform : null;

        if (playerAnimator == null)
            playerAnimator = GetComponentInChildren<Animator>();

        // Animator hashes
        isRunningHash = Animator.StringToHash("IsRunning");
        isIdleHash = Animator.StringToHash("IsIdle");
    }

    void Update()
    {
        // --- Read button actions ---
        float x = 0f;
        float z = 0f;

        if (input.Player.Left.IsPressed())
        {
            x -= 1f;
        }

        if (input.Player.Right.IsPressed())
        {
            x += 1f;
        }

        if (input.Player.TrueDown.IsPressed())
        {
            z -= 1f;
        }

        if (input.Player.TrueUp.IsPressed())
        {
            z += 1f;
        }

        Vector3 inputDir = new Vector3(x, 0f, z);

        if (inputDir.sqrMagnitude > 1f)
            inputDir.Normalize();

        // --- Convert input to world (isometric) space using camera yaw ---
        if (inputDir != Vector3.zero)
        {
            float yaw = cam != null ? cam.eulerAngles.y : 45f;
            moveDirection = Quaternion.Euler(0f, yaw, 0f) * inputDir;
            moveDirection.y = 0f;
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        //All Animation stuff

        if (rb != null && playerAnimator != null)
        {
            //float sqrSpeed = rb.linearVelocity.sqrMagnitude;
            //bool moving = sqrSpeed > (minMoveThreshold * minMoveThreshold);

            bool isMoving = false;

            if (input.Player.Left.IsPressed())
            {
                isMoving = true;
            }

            if (input.Player.Right.IsPressed())
            {
                isMoving = true;
            }

            if (input.Player.TrueDown.IsPressed())
            {
                isMoving = true;
            }

            if (input.Player.TrueUp.IsPressed())
            {
                isMoving = true;
            }


            playerAnimator.SetBool(isRunningHash, isMoving);
            playerAnimator.SetBool(isIdleHash, !isMoving);
        }
    }

    void FixedUpdate()
    {
        // --- Apply movement force ---
        if (moveDirection != Vector3.zero)
        {
            rb.AddForce(moveDirection * moveSpeed * forceMultiplier);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        // Track velocity
        currentVelocity = rb.linearVelocity;

        //// Determine if moving (running)
        //bool moving = currentVelocity.sqrMagnitude > (minMoveThreshold * minMoveThreshold);

        //// Set BOTH animator bools correctly
        //if (playerAnimator != null)
        //{
        //    playerAnimator.SetBool(isRunningHash, moving);
        //    //Debug.Log"isRunningHash :" + moving);
        //    playerAnimator.SetBool(isIdleHash, !moving);
        //    //Debug.Log"isIdleHash :" + moving);
        //    //Debug.Log"Player Animator has set states.");
        //}

        // --- Face running direction (isometric-aware) ---
        Vector3 faceDir = Vector3.zero;
        if (rb.linearVelocity.sqrMagnitude > minMoveThreshold * minMoveThreshold)
        {
            faceDir = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        }
        else
        {
            faceDir = moveDirection;
        }

        if (faceDir.sqrMagnitude > 0.0001f)
        {
            float targetYaw = Mathf.Atan2(faceDir.x, faceDir.z) * Mathf.Rad2Deg;
            targetYaw += modelYawOffset;

            float currentYaw = rb.rotation.eulerAngles.y;
            float newYaw = Mathf.MoveTowardsAngle(currentYaw, targetYaw, rotationSpeed * Time.fixedDeltaTime);

            rb.MoveRotation(Quaternion.Euler(0f, newYaw, 0f));
        }
    }
}
