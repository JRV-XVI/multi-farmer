using UnityEngine;

/// <summary>
/// Script de ayuda para configurar rápidamente el sistema de múltiples zonas de descarga.
/// Adjuntar a cada zona de descarga en la escena.
/// </summary>
public class DropZoneSetup : MonoBehaviour
{
    [Header("Configuración Rápida")]
    [Tooltip("Si está marcado, configurará automáticamente este objeto como DropZone")]
    public bool autoSetup = true;
    
    [Header("Configuración Inicial")]
    public int initialCapacity = 100;
    public int maxBots = 2;
    
    void Awake()
    {
        if (!autoSetup) return;
        
        // Agregar componente DropZone si no existe
        DropZone dropZone = GetComponent<DropZone>();
        if (dropZone == null)
        {
            dropZone = gameObject.AddComponent<DropZone>();
            dropZone.maxCapacity = initialCapacity;
            dropZone.maxSimultaneousBots = maxBots;
            Debug.Log($"DropZone configurada automáticamente en {gameObject.name}");
        }
        
        // Asegurar que tiene tag correcto
        if (!gameObject.CompareTag("Zone"))
        {
            Debug.LogWarning($"{gameObject.name} debería tener el tag 'Zone' para compatibilidad");
        }
        
        // Asegurar que tiene collider trigger
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} no tiene Collider. Agregue uno y márquelo como Trigger.");
        }
    }
}
