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
    public List<RowCols> rowToCols;

    // New: Height data for tiles
    public List<TileData> tileHeights = new();

    // Helper method to get height for a specific position
    public int GetHeight(int col, int row)
    {
        var tile = tileHeights.Find(t => t.position.x == col && t.position.y == row);
        return tile != null ? tile.height : 0;
    }
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


