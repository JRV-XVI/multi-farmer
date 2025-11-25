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
        else if(owner.HasTomatoes())
        {
            // Verificar si llegó a zona de descarga (nuevo sistema o legacy)
            bool isDropZone = (owner.assignedDropZone != null && other.gameObject == owner.assignedDropZone.gameObject);
            bool isLegacyZone = (owner.safeZone != null && other.gameObject == owner.safeZone);
            
            if (isDropZone || isLegacyZone)
            {
                owner.ChangeState(new BotDownloadState(owner));
            }
        }
    }

    public override void OnTriggerStay(Collider other)
    {
        if (other.gameObject == owner.targetPlant)
        {
            owner.ChangeState(new BotCollectingState(owner));
        }
        else if (owner.HasTomatoes())
        {
            // Verificar si llegó a zona de descarga (nuevo sistema o legacy)
            bool isDropZone = (owner.assignedDropZone != null && other.gameObject == owner.assignedDropZone.gameObject);
            bool isLegacyZone = (owner.safeZone != null && other.gameObject == owner.safeZone);
            
            if (isDropZone || isLegacyZone)
            {
                owner.ChangeState(new BotDownloadState(owner));
            }
        }
        //Buscara una planta
        else
        {
            // Si el bot tiene tomates pero no está lleno, decidir si buscar más plantas o ir a descargar
            if (owner.HasTomatoes() && !owner.IsTomatoCapacityFull())
            {
                // Buscar otra planta para llenar capacidad
                if (owner.targetPlant == null)
                {
                    owner.FindPlant();
                }
                
                if(owner.targetPlant != null)
                {
                    owner.destiny = new Vector2(owner.targetPlant.transform.position.x, owner.targetPlant.transform.position.z);
                    owner.ChangeState(new BotMovingState(owner));
                }
                else
                {
                    // No hay más plantas, ir a descargar lo que tiene
                    Debug.Log($"Bot {owner.id}: No hay más plantas disponibles, yendo a descargar");
                    
                    if (owner.RequestDropZone())
                    {
                        owner.destiny = owner.GetDropZonePosition();
                    }
                    else
                    {
                        // Fallback al sistema legacy
                        owner.destiny = new Vector2(owner.safeZone.transform.position.x, owner.safeZone.transform.position.z);
                    }
                    owner.ChangeState(new BotMovingState(owner));
                }
            }
            // Si está lleno, ir a descargar
            else if (owner.IsTomatoCapacityFull())
            {
                Debug.Log($"Bot {owner.id}: Capacidad llena, yendo a zona de recolección");
                
                if (owner.RequestDropZone())
                {
                    owner.destiny = owner.GetDropZonePosition();
                }
                else
                {
                    // Fallback al sistema legacy
                    owner.destiny = new Vector2(owner.safeZone.transform.position.x, owner.safeZone.transform.position.z);
                }
                owner.ChangeState(new BotMovingState(owner));
            }
            // Si no tiene tomates, buscar una planta
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









}
