using UnityEngine;


public abstract class UserControlState
{
    public virtual void Enter(UserControlManager manager) { }
    public virtual void Update(UserControlManager manager) { }
    public virtual void FixedUpdate(UserControlManager manager) { }
    public virtual void Exit(UserControlManager manager) { }
    
}

