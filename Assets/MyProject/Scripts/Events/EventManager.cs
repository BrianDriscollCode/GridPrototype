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

    public static event UnityAction FinishBasicMeeleAttack;

    public static void OnFinishBasicMeeleAttack()
    {
        FinishBasicMeeleAttack?.Invoke();
    }

    public static event UnityAction MovingComplete;
    public static void OnMovingComplete()
    {
        MovingComplete?.Invoke();
    }

    public static event UnityAction AttackDamageGiven;

    public static void OnAttackDamageGiven()
    {
        AttackDamageGiven?.Invoke();
    }

    public static event UnityAction MoveEnemy;

    public static void OnMoveEnemy()
    {
        MoveEnemy?.Invoke();
    }

    public static event UnityAction ReactionChance;

    public static void OnReactionChance()
    {
        ReactionChance?.Invoke();
    }

    public static event UnityAction ReactionEvent;

    public static void OnReactionEvent()
    {
        ReactionEvent?.Invoke();
    }

    
}
