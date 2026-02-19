using UnityEngine;

public class EntityGridLocation : MonoBehaviour
{
    public GameObject[,] gridTiles;
    public Vector3 pos;
    GridManager gridManager;
    public Vector2Int gridPos;

    private Vector2 pastGridPos;

    private void Start()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();

        if (gridManager != null)
        {
            gridTiles = gridManager.gridTiles;
        }

        pos = gameObject.transform.position;

        float x = pos.x;
        float z = pos.z;

        gridPos = gridManager.WorldToGridPosition(new Vector3(x, 0, z));
    }

    private void FixedUpdate()
    {
        pos = gameObject.transform.position;

        float x = pos.x;
        float z = pos.z;

        gridPos = gridManager.WorldToGridPosition(new Vector3(x, 0, z));

        bool gridPosXEqual = gridPos.x != pastGridPos.x ? false : true;
        bool gridPosYEqual = gridPos.y != pastGridPos.y ? false : true;
        //Debug.Log(gameObject + " is in cell: (" + gridPos.x + "," + gridPos.y + ")");
        if (!gridPosXEqual || !gridPosYEqual)
        {
            pastGridPos.x = gridPos.x;
            pastGridPos.y = gridPos.y;
            gridManager.characterPositionTracker.UpdateCharacterLocations();
        }
    }
}
