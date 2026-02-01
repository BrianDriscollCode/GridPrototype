using UnityEngine;

public class GameStateManager : MonoBehaviour
{

    public IGameState currentState;

    public IGameState Battle;
    public IGameState Buffer;
    public IGameState Neutral;


    private void Start()
    {
        currentState = Battle;
        currentState.EnterState();
    }

}