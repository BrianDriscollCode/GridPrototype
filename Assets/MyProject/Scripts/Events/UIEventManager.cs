using UnityEngine;
using UnityEngine.Events;

public static class UIEventManager
{

    public static event UnityAction EndTurnButtonClicked;

    public static void OnEndTurnButtonClicked()
    {
        EndTurnButtonClicked?.Invoke();
    }

    public static event UnityAction MoveButtonClicked;

    public static void OnMoveButtonClicked()
    {
        MoveButtonClicked?.Invoke();
    }

}