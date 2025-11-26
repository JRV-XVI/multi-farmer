using Unity.VisualScripting;
using UnityEngine;

public class RecolectorIdleState : State<Recolector>
{
    public RecolectorIdleState(Recolector owner) : base(owner) { }




    public override void OnEnterState()
    {
        Debug.Log("Recolector entro a IdleState");
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
            owner.ChangeState(new RecolectorCollectingState(owner));
        }

        else if(other.gameObject == owner.safeZone && owner.targetPlant != null)
        { 
            owner.ChangeState(new RecolectorDownloadState(owner));
        }

    }

    public override void OnTriggerStay(Collider other)
    {
        if (other.gameObject == owner.targetPlant)
        {
            owner.ChangeState(new RecolectorCollectingState(owner));
        }

        else if (other.gameObject == owner.safeZone && owner.targetPlant != null)
        {
            owner.ChangeState(new RecolectorDownloadState(owner));
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
                owner.ChangeState(new RecolectorMovingState(owner));
            }

            
        }
    }









}

