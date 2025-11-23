using Unity.VisualScripting;
using UnityEngine;

public class BotIdleState : State<Bot>
{
    public BotIdleState(Bot owner) : base(owner) { }





    public override void OnEnterState()
    {
        Debug.Log("Bot entro a IdleState");
        owner.WaitSeconds(2f);
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
            owner.ChangeState(new BotCollectingState(owner));
        }

        else if(other.gameObject == owner.safeZone && owner.targetPlant != null)
        { 
            owner.ChangeState(new BotDownloadState(owner));
        }

    }

    public override void OnTriggerStay(Collider other)
    {
        if (other.gameObject == owner.targetPlant)
        {
            owner.ChangeState(new BotCollectingState(owner));
        }

        else if (other.gameObject == owner.safeZone && owner.targetPlant != null)
        {
            owner.ChangeState(new BotDownloadState(owner));
        }

        //Buscara una planta
        else
        {
            if (owner.targetPlant == null)
            {
                owner.FindPlant();
            }
            if(owner.targetPlant != null)
            {
                owner.destiny = new Vector2(owner.targetPlant.transform.position.x, owner.targetPlant.transform.position.z);
                owner.ChangeState(new BotMovingState(owner));
            }

            
        }
    }









}
