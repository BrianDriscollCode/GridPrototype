using UnityEngine;

public class BasicMeeleAttackEvent : MonoBehaviour
{
    public void EmitFinishEvent()
    {
        EventManager.OnFinishBasicMeeleAttack();
    }
}
