using UnityEngine;

public class TomatoFreeState : State<Tomato>
{
    public TomatoFreeState(Tomato owner) : base(owner) { }

    public override void OnEnterState()
    {
        owner.isTaken= false;
        owner.ChangeColor(Color.white);
    }

    public override void OnStayState()
    {

    }

    public override void OnExitState()
    {
    }

}
