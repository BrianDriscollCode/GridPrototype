using UnityEngine;

public class PlayerClickControls : MonoBehaviour
{

    public Vector3 fromPos;
    public Vector3 toPos;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float arriveThreshold = 0.01f;

    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        EventManager.ClickedTile += HandleTileClicked;
        //Debug.Log("Listener: subscribed to ClickedTile");
    }

    private void OnDisable()
    {
        EventManager.ClickedTile -= HandleTileClicked;
        //Debug.Log("Listener: unsubscribed from ClickedTile");
    }

    private void HandleTileClicked(Vector2Int gridPos)
    {
        //Debug.Log("Listener: I heard the tile click event!");
        //Debug.Log($"Listener: Tile clicked at: {gridPos}");
        // do something useful here
        toPos = new Vector3((gridPos.x * 4) + 2f, transform.position.y, (gridPos.y * 4) + 2f);
    }

    private void FixedUpdate()
    {

    }
}
