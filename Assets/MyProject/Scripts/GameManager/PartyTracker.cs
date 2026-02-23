using UnityEngine;

public class PartyTracker : MonoBehaviour
{
    public enum EWhosParty
    {
        PLAYER,
        ENEMY
    }

    private EWhosParty currentParty;

    private void Awake()
    {
        currentParty = EWhosParty.PLAYER;
    }

    public EWhosParty GetCurrentParty()
    {
        return currentParty;
    }

    public void SetCurrentParty(EWhosParty party)
    {
        currentParty = party;
        //Debug.Log$"PartyTracker: Current party set to {party}");
    }
}
