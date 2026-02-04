using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    public static event UnityAction<Vector2Int> ClickedTile;

    public static void OnClickedTile(Vector2Int gridPos)
    {
        ClickedTile?.Invoke(gridPos);
    }

    //public static event UnityAction<IGameState> ChangeGameState;

    //public static void OnGameStateChange(IGameState state)
    //{
    //    ChangeGameState?.Invoke(state);
    //}

    public static event UnityAction RightClickAttack;
    
    public static void OnRightClickAttack()
    {
        RightClickAttack?.Invoke();
    }
}
