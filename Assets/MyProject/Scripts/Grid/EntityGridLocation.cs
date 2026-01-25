using UnityEngine;

public class EntityGridLocation : MonoBehaviour
{
    public GameObject[,] gridTiles;
    public Vector3 pos;
    GridManager gridManager;
    public Vector2 gridPos;

    private void Start()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();

        if (gridManager != null)
        {
            gridTiles = gridManager.gridTiles;
        }
    }

    private void FixedUpdate()
    {
        pos = gameObject.transform.position;

        float x = pos.x;
        float z = pos.z;

        gridPos = gridManager.WorldToGridPosition(new Vector3(x, 0, z));
        
        //Debug.Log(gameObject + " is in cell: (" + gridPos.x + "," + gridPos.y + ")");
    }
}
