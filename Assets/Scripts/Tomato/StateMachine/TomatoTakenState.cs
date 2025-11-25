using UnityEngine;

public class TomatoTakenState : State<Tomato>
{
    public TomatoTakenState(Tomato owner) : base(owner) { }



    public override void OnEnterState()
    {
        owner.isTaken = false;
        owner.ChangeColor(Color.red);
    }

    public override void OnStayState()
    {
        

    }

    public override void OnExitState()
    {
    }

}
