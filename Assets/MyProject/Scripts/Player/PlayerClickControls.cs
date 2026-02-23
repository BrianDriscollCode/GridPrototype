using UnityEngine;

public class PlayerClickControls : MonoBehaviour
{

    private Vector3 fromPos;
    private Vector3 toPos;

    public GridManager gridManager;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float arriveThreshold = 0.01f;

    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gridManager = GameObject.FindAnyObjectByType<GridManager>();
    }

    private void OnEnable()
    {
        EventManager.ClickedTile += HandleTileClicked;
        ////Debug.Log"Listener: subscribed to ClickedTile");
    }

    private void OnDisable()
    {
        EventManager.ClickedTile -= HandleTileClicked;
        ////Debug.Log"Listener: unsubscribed from ClickedTile");
    }

    private void HandleTileClicked(Vector2Int gridPos)
    {
        ////Debug.Log"Listener: I heard the tile click event!");
        ////Debug.Log$"Listener: Tile clicked at: {gridPos}");
        // do something useful here
        float cellSize = 2f;
        float offset = cellSize / 2;

        if (gridManager)
        {
            cellSize = gridManager.cellSize;
            offset = gridManager.cellSize / 2;
        }
        
        toPos = new Vector3((gridPos.x * cellSize) + offset, transform.position.y, (gridPos.y * cellSize) + offset);
    }

    private void FixedUpdate()
    {

    }

    public Vector3 GetFromPos()
    {
        return fromPos;
    }
    
    public void SetFromPos(Vector3 pos)
    {
        fromPos = pos;
    }

    public Vector3 GetToPos()
    {
        return toPos;
    }

    public void SetToPos(Vector3 pos)
    {
        toPos = pos;
    }
}
