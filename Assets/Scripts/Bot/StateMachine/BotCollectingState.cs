using Unity.VisualScripting;
using UnityEngine;

public class BotCollectingState : State<Bot>
{
    public BotCollectingState(Bot owner) : base(owner) { }



    public override void OnEnterState()
    {
        Debug.Log("Bot entro a CollectingState");

    }

    public override void OnStayState()
    {

    }

    public override void OnExitState()
    {

    }

    public override void OnTriggerEnter(Collider other)
    {

        if (other.gameObject == owner.targetPlant)
        {
            Debug.Log("Triger activado por planta");
            owner.TakeObject(other.gameObject);

            owner.destiny = new Vector2(owner.safeZone.transform.position.x, owner.safeZone.transform.position.z);
            owner.ChangeState(new BotMovingState(owner));
        }
    }

    public override void OnTriggerStay(Collider other)
    {
        if (other.gameObject == owner.targetPlant)
        {
            Debug.Log("Triger activado por planta");
            owner.TakeObject(other.gameObject);

            owner.destiny = new Vector2(owner.safeZone.transform.position.x, owner.safeZone.transform.position.z);
            owner.ChangeState(new BotMovingState(owner));
        }
    }





}
