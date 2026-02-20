using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelScriptableObject", menuName = "Scriptable Objects/LevelScriptableObject")]
public class LevelScriptableObject : ScriptableObject
{
    public string levelName;
    public int rows;
    public int columns;

    public Vector2[] gridPos;
    public List<RowCols> rowToCols = new();

    // Managed "2D" grid stored flat (Unity-serializable, O(1) access)
    public List<TileData> tiles = new();

    // ---- Index helpers ----
    public int Index(int col, int row) => row * columns + col;

    public bool IsValid(int col, int row)
    {
        // Check column bounds
        bool columnIsNotNegative = col >= 0;
        bool columnIsWithinGrid = col < columns;

        // Check row bounds
        bool rowIsNotNegative = row >= 0;
        bool rowIsWithinGrid = row < rows;

        // Combine column checks
        bool columnIsValid = columnIsNotNegative && columnIsWithinGrid;

        // Combine row checks
        bool rowIsValid = rowIsNotNegative && rowIsWithinGrid;

        // Grid position is valid only if both column and row are valid
        bool gridPositionIsValid = columnIsValid && rowIsValid;

        return gridPositionIsValid;
    }


    // Ensure tiles list matches rows*columns
    // *** Think of this like a repair function enforcing the grid size contract
    public void EnsureSize()
    {
        int count = Mathf.Max(0, rows * columns);

        if (tiles == null) tiles = new List<TileData>(count);

        // Grow
        while (tiles.Count < count)
            tiles.Add(new TileData());

        // Shrink
        if (tiles.Count > count)
            tiles.RemoveRange(count, tiles.Count - count);

        // Keep positions consistent
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                int i = Index(c, r);
                if (i >= 0 && i < tiles.Count)
                    tiles[i].position = new Vector2Int(c, r);
            }
        }
    }

    // Build the flat grid from your existing rowToCols authoring
    public void BuildFromRowToCols()
    {
        EnsureSize();

        // Clear all tiles first (default to inaccessible)
        foreach (var tile in tiles)
        {
            tile.height = 0;
            tile.isAccessible = false;
        }

        // Populate from rowToCols
        foreach (var rc in rowToCols)
        {
            if (rc == null) continue;

            foreach (int col in rc.cols)
            {
                var tile = GetTile(col, rc.row);
                if (tile != null)
                {
                    tile.isAccessible = true; // Mark as part of grid
                    // Height stays 0 by default, can be edited per-tile later
                }
            }
        }

        //Debug.Log$"Built grid from rowToCols: {tiles.Count} tiles populated");
    }

    public TileData GetTile(int col, int row)
    {
        if (!IsValid(col, row)) return null;
        int i = Index(col, row);
        if (i < 0 || i >= tiles.Count) return null;
        return tiles[i];
    }

    public int GetHeight(int col, int row)
        => GetTile(col, row)?.height ?? 0;

    public bool IsAccessible(int col, int row)
        => GetTile(col, row)?.isAccessible ?? false;

    public void SetHeight(int col, int row, int height)
    {
        var tile = GetTile(col, row);
        if (tile != null)
            tile.height = height;
    }

    public void SetAccessible(int col, int row, bool accessible)
    {
        var tile = GetTile(col, row);
        if (tile != null)
            tile.isAccessible = accessible;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        EnsureSize();
    }
#endif
}

[Serializable]
public class RowCols
{
    public int row;
    public List<int> cols = new();
}

[Serializable]
public class TileData
{
    public Vector2Int position; // x = col, y = row
    public int height = 0;
    public bool isAccessible = true;
}

[Serializable]
public class GridRow<T>
{
    public List<T> cells = new();
}

[Serializable]
public class Grid2D<T>
{
    public List<GridRow<T>> rows = new();
}

//using System;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(fileName = "LevelScriptableObject", menuName = "Scriptable Objects/LevelScriptableObject")]
//public class LevelScriptableObject : ScriptableObject
//{
//    public string levelName;
//    public int rows;
//    public int columns;
//    public Vector2[] gridPos;
//    public List<RowCols> rowToCols;

//    // New: Height data for tiles
//    public List<TileData> tileHeights = new();

//    // Helper method to get height for a specific position
//    public int GetHeight(int col, int row)
//    {
//        var tile = tileHeights.Find(t => t.position.x == col && t.position.y == row);
//        return tile != null ? tile.height : 0;
//    }
//}

//[Serializable]
//public class RowCols
//{
//    public int row;
//    public List<int> cols = new();
//}

//[Serializable]
//public class TileData
//{
//    public Vector2Int position; // x = col, y = row
//    public int height = 0;
//    public bool isAccessible = true;
//}

//[Serializable]
//public class GridRow<T>
//{
//    public List<T> cells = new();
//}

//[Serializable]
//public class Grid2D<T>
//{
//    public List<GridRow<T>> rows = new();
//}


//using System;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(fileName = "LevelScriptableObject", menuName = "Scriptable Objects/LevelScriptableObject")]
//public class LevelScriptableObject : ScriptableObject
//{
//    public string levelName;
//    public int rows;
//    public int columns;

//    public Vector2[] gridPos;
//    public List<RowCols> rowToCols;
//}

//[Serializable]
//public class RowCols
//{
//    public int row;
//    public List<int> cols = new();
//}

//  -------------------
// |---|---|---|---|---|
// |---|---|---|---|---|
// |---|---|---|---|---|
// |---|---|---|---|---|
// |---|---|---|---|---|
// |___|___|___|___|___|

//  .  .  .  .  .  .  .  
//  .  .  .  .  .  .  .
//  .  .  .  .  .  .  .
//  .  .  .  .  .  .  .
//  .  .  .  .  .  .  .


// 0: [0,1], [0, 2], [0, 3]
// 1: [1,1], [1, 2], [1, 3]


