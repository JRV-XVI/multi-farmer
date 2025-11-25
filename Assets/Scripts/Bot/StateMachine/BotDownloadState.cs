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
        // Verificar si llegó a su zona asignada o a la safeZone legacy
        bool isAssignedZone = (owner.assignedDropZone != null && other.gameObject == owner.assignedDropZone.gameObject);
        bool isLegacyZone = (owner.safeZone != null && other.gameObject == owner.safeZone);
        
        if ((isAssignedZone || isLegacyZone) && owner.HasTomatoes())
        {
            DownloadTomatoes(other);
        }
    }

    public override void OnTriggerStay(Collider other)
    {
        // Verificar si está en su zona asignada o en la safeZone legacy
        bool isAssignedZone = (owner.assignedDropZone != null && other.gameObject == owner.assignedDropZone.gameObject);
        bool isLegacyZone = (owner.safeZone != null && other.gameObject == owner.safeZone);
        
        if ((isAssignedZone || isLegacyZone) && owner.HasTomatoes())
        {
            DownloadTomatoes(other);
        }
    }

    private void DownloadTomatoes(Collider other)
    {
        Debug.Log($"Bot {owner.id} descargando tomates en la zona de recolección");

        // Contar tomates antes de descargar para métricas
        int tomatoCount = owner.GetCollectedTomatoCount();
        
        // Descargar todos los tomates (esto notifica automáticamente a la zona)
        owner.DropAllTomatoes();
        
        // Registrar en métricas
        if (owner.metrics != null)
        {
            owner.metrics.RecordTomatoDelivered(owner.id, tomatoCount);
        }
        
        // Liberar la zona asignada
        owner.ReleaseDropZone();

        // Volver al estado idle para buscar más plantas
        owner.ChangeState(new BotIdleState(owner));
    }
}
