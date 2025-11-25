using Unity.VisualScripting;
using UnityEngine;

public class BotMovingState : State<Bot>
{
    private CellState _cellState;
    public BotMovingState(Bot owner) : base(owner) {    }


    public override void OnEnterState()
    {
        Debug.Log("Bot entro a Moving State");
        owner.CalculateRoute(owner.destiny);

    }

    public override void OnStayState()
    {
        if (owner.isMovingPath)
        {
            owner.FollowPath();
        }
        else
        {
            owner.ChangeState(new BotIdleState(owner));
        }
    }

    public override void OnExitState()
    {
        Debug.Log("Bot Salio de Moving State");

    }

    public override void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == owner.targetPlant && !owner.IsTomatoCapacityFull())
        {
            owner.ChangeState(new BotIdleState(owner));
        }
        else if (owner.HasTomatoes())
        {
            // Verificar si llegó a zona de descarga (nuevo sistema o legacy)
            bool isDropZone = (owner.assignedDropZone != null && other.gameObject == owner.assignedDropZone.gameObject);
            bool isLegacyZone = (owner.safeZone != null && other.gameObject == owner.safeZone);
            
            if (isDropZone || isLegacyZone)
            {
                owner.ChangeState(new BotIdleState(owner));
            }
        }

        else if(other.tag == "Bot")
        {
            if(other.GetComponent<Bot>().id > owner.id)
            {
                owner.EvadeOtherBot(other.gameObject);
                owner.ChangeState(new BotMovingState(owner));
                
            }
        }

    }











}
