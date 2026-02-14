using System;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class GridManager : MonoBehaviour
{
    public GameObject characterPositionTrackerGO;
    public CharacterPositionTracker characterPositionTracker;
    public Dictionary<GameObject, Vector2Int> characterPositionList;

    [Header("Level Data")]
    public LevelScriptableObject levelData; // Reference to your scriptable object
    public bool useLevelData = false; // Toggle to use SO data vs manual settings



    [Header("Grid Settings")]
    public bool generate = true;
    public int gridWidth = 10;      // How many cells wide
    public int gridHeight = 10;     // How many cells deep
    public float cellSize = 2f;     // Each cell is 2x2 units

    [Header("Tile Settings")]
    public GameObject tilePrefab;   // Your tile prefab (4x1x4)

    // Offset applied to each instantiated prefab (keeps grid point the same)
    public Vector3 prefabOffset = new Vector3(1f, 0f, 1f);

    // 2D array to store what's in each grid cell
    public GameObject[,] gridTiles;


    void Start()
    {
        //// Only run in Play mode to avoid issues when stopping the scene
        //if (!Application.isPlaying) return;

        if (characterPositionTrackerGO == null)
        {
            Debug.LogError("CharacterPositionTrackerGO is not assigned in GridManager");
            return;
        }

        characterPositionTracker = characterPositionTrackerGO.GetComponent<CharacterPositionTracker>();
        if (characterPositionTracker == null)
        {
            Debug.LogError("CharacterPositionTracker component not found on assigned GameObject");
            return;
        }

        characterPositionList = characterPositionTracker.GetCharactersList();

        // Use level data dimensions if available
        if (useLevelData && levelData != null)
        {
            gridWidth = levelData.columns;
            gridHeight = levelData.rows;
        }

        // Initialize the grid array
        gridTiles = new GameObject[gridWidth, gridHeight];

        if (generate)
        {
            // Clear any existing tiles first
            ClearGrid();
            GenerateGrid();
        }
    }

    public bool IsTileAccessible(int col, int row)
    {
        if (levelData == null) return false;
        return levelData.IsAccessible(col, row);
    }

    // Clear all existing tiles
    public void ClearGrid()
    {
        if (gridTiles != null)
        {
            for (int x = 0; x < gridTiles.GetLength(0); x++)
            {
                for (int z = 0; z < gridTiles.GetLength(1); z++)
                {
                    if (gridTiles[x, z] != null)
                    {
                        if (Application.isPlaying)
                            Destroy(gridTiles[x, z]);
                        else
                            DestroyImmediate(gridTiles[x, z]);
                    }
                }
            }
        }
    }

    public void GenerateGrid()
    {
        if (useLevelData && levelData != null)
        {
            // Generate based on scriptable object data
            GenerateFromLevelData();
        }
        else
        {
            // Generate all tiles (original behavior)
            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    PlaceTile(x, z);
                }
            }
        }
    }

    // Generate grid based on LevelScriptableObject data
    void GenerateFromLevelData()
    {
        Debug.Log($"Generating grid from level data: {levelData.columns}x{levelData.rows}, cellSize: {cellSize}");
        
        // Option 1: Use rowToCols list (row-column pairs)
        if (levelData.rowToCols != null && levelData.rowToCols.Count > 0)
        {
            foreach (var rowCol in levelData.rowToCols)
            {
                foreach (int col in rowCol.cols)
                {
                    PlaceTile(col, rowCol.row);
                }
            }
        }
        // Option 2: Use gridPos array if rowToCols is empty
        else if (levelData.gridPos != null && levelData.gridPos.Length > 0)
        {
            foreach (Vector2 pos in levelData.gridPos)
            {
                PlaceTile((int)pos.x, (int)pos.y);
            }
        }
        // Fallback: Generate all tiles
        else
        {
            Debug.LogWarning("No tile positions defined in LevelScriptableObject. Generating full grid.");
            for (int x = 0; x < gridWidth; x++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    PlaceTile(x, z);
                }
            }
        }
    }

    public void PlaceTile(int gridX, int gridZ)
    {
        // Check if coordinates are valid
        if (!IsValidGridPosition(gridX, gridZ))
        {
            Debug.LogWarning($"Invalid grid position: ({gridX}, {gridZ}) - gridWidth: {gridWidth}, gridHeight: {gridHeight}");
            return;
        }

        // If there's already a tile here, don't place another
        if (gridTiles[gridX, gridZ] != null)
        {
            Debug.Log($"Tile already exists at ({gridX}, {gridZ})");
            return;
        }

        // Get height from level data if available
        int height = 0;
        if (useLevelData && levelData != null)
        {
            height = levelData.GetHeight(gridX, gridZ);
        }

        // Convert grid position to world position
        Vector3 worldPos = GridToWorldPosition(gridX, gridZ);
        worldPos.y = height; // Apply height

        Debug.Log($"Placing tile at grid ({gridX}, {gridZ}) -> world pos {worldPos + prefabOffset}");

        // Create the tile at the world position plus the prefab offset
        GameObject tile = Instantiate(tilePrefab, worldPos + prefabOffset, Quaternion.identity, transform);
        tile.name = $"Tile_{gridX}_{gridZ}";

        // Store it in the grid
        gridTiles[gridX, gridZ] = tile;
    }

    // Remove a tile from the grid
    public void RemoveTile(int gridX, int gridZ)
    {
        if (IsValidGridPosition(gridX, gridZ) && gridTiles[gridX, gridZ] != null)
        {
            Destroy(gridTiles[gridX, gridZ]);
            gridTiles[gridX, gridZ] = null;
        }
    }

    // Convert grid coordinates to world position
    public Vector3 GridToWorldPosition(int gridX, int gridZ)
    {
        float worldX = gridX * cellSize;
        float worldZ = gridZ * cellSize;
        return new Vector3(worldX, 0, worldZ);
    }

    // Convert world position to grid coordinates
    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        int gridX = Mathf.FloorToInt(worldPos.x / cellSize);
        int gridZ = Mathf.FloorToInt(worldPos.z / cellSize);
        return new Vector2Int(gridX, gridZ);
    }

    // Check if a grid position is valid
    public bool IsValidGridPosition(int gridX, int gridZ)
    {
        return gridX >= 0 && gridX < gridWidth && gridZ >= 0 && gridZ < gridHeight;
    }

    // Check if a tile exists at this grid position
    public bool HasTileAt(int gridX, int gridZ)
    {
        if (!IsValidGridPosition(gridX, gridZ)) return false;
        return gridTiles[gridX, gridZ] != null;
    }

    public GameObject GetTile(int gridX, int gridZ)
    {
        if (!IsValidGridPosition(gridX, gridZ)) return null;
        return gridTiles[gridX, gridZ];

    }

    public int GetTileDistance(Vector2Int pos1, Vector2Int pos2)
    {
        // Dat ChebyShev distance
        int dx = Math.Abs(pos2.x - pos1.x);
        int dy = Math.Abs(pos2.y - pos1.y);

        return Math.Max(dx, dy);
    }

    public bool IsGridPosOccupied(Vector2Int gridPos)
    {
        if (characterPositionTracker == null) return false;
        
        foreach (Vector2Int pos in characterPositionTracker.GetCharactersList().Values)
        {
            bool xMatch = gridPos.x == pos.x;
            bool yMatch = gridPos.y == pos.y;

            if (xMatch && yMatch) return true;
        }

        return false;
    }

    // Visualize the grid in the editor
    void OnDrawGizmos()
    {
        int width = useLevelData && levelData != null ? levelData.columns : gridWidth;
        int height = useLevelData && levelData != null ? levelData.rows : gridHeight;

        Gizmos.color = Color.green;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = GridToWorldPosition(x, z);
                // Draw a small cube at each grid position
                Gizmos.DrawWireCube(pos + new Vector3(cellSize / 2, 0, cellSize / 2),
                                    new Vector3(cellSize, 0.1f, cellSize));
            }
        }
    }
}
