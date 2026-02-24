using System;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class GridManager : MonoBehaviour
{
    public GameObject characterPositionTrackerGO;
    public CharacterPositionTracker characterPositionTracker;
    public Dictionary<GameObject, Vector2Int> characterPositionList;

    [Header("Tracking Available Tiles Per Character")]
    public List<GameObject> availableTiles;

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

    [Header("Debug - Grid Visualization")]
    [SerializeField] private List<TileDebugInfo> tilesDebugList = new List<TileDebugInfo>();

    [Header("Tile Highlighting")]
    public Material highlightedMoveMaterial;
    private Dictionary<GameObject, Material> _highlightedTileOriginalMaterials = new Dictionary<GameObject, Material>();
    private List<GameObject> _currentlyHighlightedTiles = new List<GameObject>();

    [System.Serializable]
    public class TileDebugInfo
    {
        public Vector2Int gridPosition;
        public GameObject tile;
    }

    void Start()
    {
        availableTiles = new List<GameObject>();
        //// Only run in Play mode to avoid issues when stopping the scene
        //if (!Application.isPlaying) return;

        if (characterPositionTrackerGO == null)
        {
            //Debug.LogError("CharacterPositionTrackerGO is not assigned in GridManager");
            return;
        }

        characterPositionTracker = characterPositionTrackerGO.GetComponent<CharacterPositionTracker>();
        if (characterPositionTracker == null)
        {
            //Debug.LogError("CharacterPositionTracker component not found on assigned GameObject");
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
        else
        {
            // If not generating, populate the array with existing tiles
            PopulateGridFromExistingTiles();
        }

        // Update debug visualization
        UpdateDebugList();
    }

    // Update the inspector-visible debug list
    private void UpdateDebugList()
    {
        tilesDebugList.Clear();

        if (gridTiles == null) return;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                if (gridTiles[x, z] != null)
                {
                    tilesDebugList.Add(new TileDebugInfo
                    {
                        gridPosition = new Vector2Int(x, z),
                        tile = gridTiles[x, z]
                    });
                }
            }
        }
    }

    // Populate the gridTiles array from existing tile GameObjects in the scene
    // ******** This was purely AI generated with little review besides menu debug
    // verification of output, needs review. Since prototype, only address
    // if causes issues.
    public void PopulateGridFromExistingTiles()
    {
        //Debug.Log"Populating gridTiles array from existing tiles in scene...");

        // Find all children of this GameObject (assuming tiles are children)
        Transform[] children = GetComponentsInChildren<Transform>();
        int foundTiles = 0;

        foreach (Transform child in children)
        {
            // Skip self
            if (child == transform) continue;

            // Try to parse tile name (e.g., "Tile_3_5")
            if (child.name.StartsWith("Tile_"))
            {
                string[] parts = child.name.Split('_');
                if (parts.Length >= 3)
                {
                    if (int.TryParse(parts[1], out int gridX) && int.TryParse(parts[2], out int gridZ))
                    {
                        if (IsValidGridPosition(gridX, gridZ))
                        {
                            gridTiles[gridX, gridZ] = child.gameObject;
                            foundTiles++;
                        }
                        else
                        {
                            Debug.LogWarning($"Found tile {child.name} with out-of-bounds position ({gridX}, {gridZ})");
                        }
                    }
                }
            }
        }

        //Debug.Log$"Populated gridTiles array with {foundTiles} existing tiles");
        UpdateDebugList();
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
        UpdateDebugList();
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
        UpdateDebugList();
    }

    // Generate grid based on LevelScriptableObject data
    void GenerateFromLevelData()
    {
        //Debug.Log$"Generating grid from level data: {levelData.columns}x{levelData.rows}, cellSize: {cellSize}");
        
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
            //Debug.Log$"Tile already exists at ({gridX}, {gridZ})");
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

        //Debug.Log$"Placing tile at grid ({gridX}, {gridZ}) -> world pos {worldPos + prefabOffset}");

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
            UpdateDebugList();
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
        if (!IsValidGridPosition(gridX, gridZ))
        {
            //Debug.LogWarning($"HasTileAt: Invalid position ({gridX}, {gridZ})");
            return false;
        }

        GameObject tile = gridTiles[gridX, gridZ];
        bool result = tile != null;

        ////Debug.Log$"HasTileAt({gridX}, {gridZ}): tile={tile?.name ?? "null"}, result={result}");
        return result;
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

    // ****** Path and Availability Management for Characters

    public void CheckAvailableMoveTilesAndHighlight(int movePoints, Vector2Int characterPos)
    {
        Logger.LogCategory("Grid", "CheckAvailableMoveTilesAndHighlight");

        // Clear highlighted tiles FIRST (restores materials while we still have references)
        ClearHighlightedTiles();
        
        // Then clear the available tiles list
        ClearAvailableTiles();

        // Iterate through grid using nested loops (can't foreach a 2D array)
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // Skip if no tile exists
                if (gridTiles[x, z] == null) continue;

                Vector2Int tilePos = new Vector2Int(x, z);
                
                // Validation pipeline (same as EnemyAI)
                if (!IsValidGridPosition(x, z)) continue;
                if (!HasTileAt(x, z)) continue;
                if (!IsTileAccessible(x, z)) continue;
                if (IsGridPosOccupied(tilePos)) continue;
                
                // Check if tile is within movement range
                if (GetTileDistance(characterPos, tilePos) <= movePoints)
                {
                    availableTiles.Add(gridTiles[x, z]);
                }
            }
        }

        // Highlight all available tiles
        foreach (GameObject tile in availableTiles)
        {
            HighlightTile(tile);
        }
    }

    private void HighlightTile(GameObject tile)
    {
        var renderer = tile.GetComponent<Renderer>();
        if (renderer == null) return;

        // Store original material if not already stored
        if (!_highlightedTileOriginalMaterials.ContainsKey(tile))
        {
            _highlightedTileOriginalMaterials[tile] = renderer.material;
        }

        // Apply highlight material
        if (highlightedMoveMaterial != null)
        {
            renderer.material = highlightedMoveMaterial;
            _currentlyHighlightedTiles.Add(tile);
        }
        else
        {
            Debug.LogWarning("highlightedMoveMaterial is not assigned in GridManager!");
        }
    }

    public void ClearAvailableTiles()
    {
        availableTiles.Clear();
    }

    public void ClearHighlightedTiles()
    {
        foreach (GameObject tile in _currentlyHighlightedTiles)
        {
            if (tile == null) continue;
            
            var renderer = tile.GetComponent<Renderer>();
            if (renderer != null && _highlightedTileOriginalMaterials.ContainsKey(tile))
            {
                renderer.material = _highlightedTileOriginalMaterials[tile];
            }
        }

        _currentlyHighlightedTiles.Clear();
        _highlightedTileOriginalMaterials.Clear();
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
