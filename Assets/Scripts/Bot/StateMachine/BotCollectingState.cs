using Unity.VisualScripting;
using UnityEngine;

public class BotCollectingState : State<Bot>
{
    private bool _hasCollected = false; // Flag para evitar recolección múltiple
    
    public BotCollectingState(Bot owner) : base(owner) { }



    public override void OnEnterState()
    {
        Debug.Log("Bot entro a CollectingState");
        _hasCollected = false;
    }

    public override void OnStayState()
    {

    }

    public override void OnExitState()
    {
        _hasCollected = false;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner.targetPlant && !_hasCollected)
        {
            TryCollectTomatoes();
        }
    }

    public override void OnTriggerStay(Collider other)
    {
        if (other.gameObject == owner.targetPlant && !_hasCollected)
        {
            TryCollectTomatoes();
        }
    }

    private void TryCollectTomatoes()
    {
        if (_hasCollected) return; // Evitar recolecciones múltiples
        
        Debug.Log("Intentando recoger tomates de la planta");

        Plant plant = owner.targetPlant.GetComponent<Plant>();
        if (plant == null)
        {
            Debug.LogWarning("El target plant no tiene componente Plant!");
            owner.targetPlant = null;
            owner.ChangeState(new BotIdleState(owner));
            return;
        }

        int tomatosRecolectadosAhora = 0;
        
        // Recoger tomates mientras haya disponibles y el bot tenga espacio
        while (!owner.IsTomatoCapacityFull() && plant.GetAvailableTomatoCount() > 0)
        {
            GameObject tomato = plant.CollectTomato();
            if (tomato != null)
            {
                owner.CollectTomatoFromPlant(tomato);
                tomatosRecolectadosAhora++;
                
                // Registrar en métricas
                if (owner.metrics != null)
                {
                    owner.metrics.RecordTomatoCollected(owner.id);
                }
            }
            else
            {
                break; // Si no pudo recoger, salir del bucle
            }
        }
        
        _hasCollected = true; // Marcar que ya recolectó
        
        Debug.Log($"Bot {owner.id} recolectó {tomatosRecolectadosAhora} tomates. Total: {owner.GetCollectedTomatoCount()}/{owner.maxTomatoCapacity}");

        // Siempre liberar la planta después de recolectar
        plant.isTaken = false;
        owner.targetPlant = null;

        // Decidir qué hacer después de recoger
        if (owner.IsTomatoCapacityFull())
        {
            Debug.Log($"Bot {owner.id} llenó su capacidad, solicitando zona de descarga");
            
            // Solicitar zona de descarga óptima
            if (owner.RequestDropZone())
            {
                owner.destiny = owner.GetDropZonePosition();
                owner.ChangeState(new BotMovingState(owner));
            }
            else
            {
                Debug.LogWarning($"Bot {owner.id}: No hay zonas de descarga disponibles. Bot esperará.");
                owner.ChangeState(new BotIdleState(owner));
            }
        }
        else if (tomatosRecolectadosAhora == 0)
        {
            Debug.Log($"Bot {owner.id}: Planta sin tomates disponibles, buscando otra planta");
            
            // Buscar otra planta
            owner.ChangeState(new BotIdleState(owner));
        }
        else
        {
            Debug.Log($"Bot {owner.id}: Recolectó algunos tomates pero no llenó capacidad. Buscando más plantas.");
            
            // Tiene tomates pero no está lleno, buscar otra planta
            owner.ChangeState(new BotIdleState(owner));
        }
    }
}
