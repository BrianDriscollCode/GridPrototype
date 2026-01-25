using UnityEngine;

public class PlayerClickControls : MonoBehaviour
{
    public enum PlayerState
    {
        Nuetral,
        Moving,
        Attacking
    }

    public PlayerState currentState;

    // NOTE: per your request this will move toward fromPos (i.e. move "from toPos to fromPos")
    public Vector3 fromPos;
    public Vector3 toPos;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float arriveThreshold = 0.01f;

    private Rigidbody rb;

    private void Start()
    {
        currentState = PlayerState.Nuetral;
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        EventManager.ClickedTile += HandleTileClicked;
        Debug.Log("Listener: subscribed to ClickedTile");
    }

    private void OnDisable()
    {
        EventManager.ClickedTile -= HandleTileClicked;
        Debug.Log("Listener: unsubscribed from ClickedTile");
    }

    private void HandleTileClicked(Vector2Int gridPos)
    {
        Debug.Log("Listener: I heard the tile click event!");
        Debug.Log($"Listener: Tile clicked at: {gridPos}");
        // do something useful here
        toPos = new Vector3((gridPos.x * 4) + 2f, 0, (gridPos.y * 4) + 2f);
        currentState = PlayerState.Moving;
    }

    private void FixedUpdate()
    {
        if (currentState == PlayerState.Moving)
        {
            // target = fromPos (moving from toPos -> fromPos as requested)
            Vector3 target = toPos;
            float step = moveSpeed * Time.fixedDeltaTime;

            if (rb != null)
            {
                Vector3 next = Vector3.MoveTowards(rb.position, target, step);
                rb.MovePosition(next);

                if ((rb.position - target).sqrMagnitude <= arriveThreshold * arriveThreshold)
                {
                    rb.position = target;
                    currentState = PlayerState.Nuetral;
                }
            }
            else
            {
                Vector3 next = Vector3.MoveTowards(transform.position, target, step);
                transform.position = next;

                if ((transform.position - target).sqrMagnitude <= arriveThreshold * arriveThreshold)
                {
                    transform.position = target;
                    currentState = PlayerState.Nuetral;
                }
            }
        }
    }
}
