using Unity.VisualScripting;
using UnityEngine;

public class RecolectorDownloadState : State<Recolector>
{
    public RecolectorDownloadState(Recolector owner) : base(owner) { }


    public override void OnEnterState()
    {
        Debug.Log("Recolector entro a DownloadState");

    }

    public override void OnStayState()
    {

    }

    public override void OnExitState()
    {

    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner.safeZone && owner.targetPlant != null)
        {
            Debug.Log("Triger activado por safe zone");
            owner.DropObjects();

            owner.targetPlant = null;

            owner.ChangeState(new RecolectorIdleState(owner));
        }
    }

    public override void OnTriggerStay(Collider other)
    {
        if (other.gameObject == owner.safeZone && owner.targetPlant != null)
        {
            Debug.Log("Triger activado por safe zone");
            
            owner.DownloadCarringInZone();

            owner.ChangeState(new RecolectorIdleState(owner));
        }
    }






}
