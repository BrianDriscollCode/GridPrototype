using UnityEngine;

public class UserControlState_Select : UserControlState
{
    public override void Enter(UserControlManager manager)
    {
        manager.currentControlMode = manager.SelectionCA;
    }

    public override void Update(UserControlManager manager)
    {
        
    }

    public override void FixedUpdate(UserControlManager manager)
    {
       
    }

    public override void Exit(UserControlManager manager)
    {
    
    }
}
