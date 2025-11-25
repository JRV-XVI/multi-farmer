using Unity.VisualScripting;
using UnityEngine;

public class BotDownloadState : State<Bot>
{
    public BotDownloadState(Bot owner) : base(owner) { }



    public override void OnEnterState()
    {
        Debug.Log("Bot entro a DownloadState");

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

            owner.ChangeState(new BotIdleState(owner));
        }
    }

    public override void OnTriggerStay(Collider other)
    {
        if (other.gameObject == owner.safeZone && owner.targetPlant != null)
        {
            Debug.Log("Triger activado por safe zone");
            owner.DropObjects();

            owner.targetPlant = null;

            owner.ChangeState(new BotIdleState(owner));
        }
    }





}
