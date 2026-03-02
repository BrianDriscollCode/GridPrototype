using UnityEngine;

public class BasicMeeleAttackEvent : MonoBehaviour
{
    public void EmitFinishEvent()
    {
        EventManager.OnFinishBasicMeeleAttack();
    }

    public void EmitDamageGivenEvent()
    {
        EventManager.OnAttackDamageGiven();
    }

    public void EmitReactionChance()
    {
        EventManager.OnReactionChance();
    }
}
