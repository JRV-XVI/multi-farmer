using UnityEngine;

public class CameraFollowingPlayerState : State<Camera>
{
    public CameraFollowingPlayerState(Camera owner) : base(owner) { }

    public override void OnEnterState()
    {
        
    }

    public override void OnStayState()
    {
        if(owner.changeCamaraPressed)
        {
            owner.ChangeState(new CameraFreeState(owner));
        }

        owner.FollowPlayer(new Vector3(0,0,0) );

    }

    public override void OnExitState()
    {

    }

}
