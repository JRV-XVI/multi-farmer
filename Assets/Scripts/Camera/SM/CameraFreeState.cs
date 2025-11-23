using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFreeState : State<Camera>
{
    public CameraFreeState(Camera owner) : base(owner) { }

    public override void OnEnterState()
    {
        
    }

    public override void OnStayState()
    {
        if (owner.changeCamaraPressed)
        {
            owner.ChangeState(new CameraFollowingPlayerState(owner));
        }

    }

    public override void OnExitState()
    {
        
    }

}
