using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Gestor centralizado para coordinar la asignación de zonas de descarga.
/// Implementa estrategias de balanceo de carga y optimización de rutas.
/// </summary>
public class DropZoneManager : MonoBehaviour
{
    [Header("Configuración")]
    public DropZone[] dropZones;
    
    [Header("Pesos de Criterios de Asignación")]
    [Range(0f, 1f)] public float distanceWeight = 0.4f; // Peso de la distancia
    [Range(0f, 1f)] public float occupancyWeight = 0.4f; // Peso de la ocupación
    [Range(0f, 1f)] public float capacityWeight = 0.2f; // Peso de la capacidad
    
    [Header("Estrategia de Asignación")]
    public AssignmentStrategy strategy = AssignmentStrategy.Balanced;
    
    // Registro de asignaciones activas
    private Dictionary<int, DropZone> _activeAssignments = new Dictionary<int, DropZone>();
    
    public enum AssignmentStrategy
    {
        Nearest,        // Solo distancia
        LeastOccupied,  // Solo ocupación
        LeastLoaded,    // Solo capacidad
        Balanced        // Combinación ponderada
    }
    
    void Start()
    {
        // Encontrar todas las zonas de descarga si no están asignadas
        if (dropZones == null || dropZones.Length == 0)
        {
            dropZones = FindObjectsByType<DropZone>(FindObjectsSortMode.None);
            
            if (dropZones.Length > 0)
            {
                Debug.Log($"DropZoneManager encontró {dropZones.Length} zonas de descarga automáticamente");
            }
            else
            {
                Debug.LogWarning("DropZoneManager: No se encontraron DropZones en la escena. " +
                    "El sistema usará el modo legacy con safeZone única.");
            }
        }
        else
        {
            Debug.Log($"DropZoneManager inicializado con {dropZones.Length} zonas asignadas manualmente");
        }
        
        // Log de configuración
        Debug.Log($"DropZoneManager - Estrategia: {strategy}, " +
            $"Pesos: Distancia={distanceWeight:F2}, Ocupación={occupancyWeight:F2}, Capacidad={capacityWeight:F2}");
    }
    
    /// <summary>
    /// Asigna la mejor zona de descarga disponible para un robot.
    /// </summary>
    /// <param name="bot">El robot que solicita una zona</param>
    /// <param name="tomatoCount">Cantidad de tomates a descargar</param>
    /// <returns>La zona asignada, o null si no hay disponible</returns>
    public DropZone AssignDropZone(Bot bot, int tomatoCount)
    {
        // Verificar si el bot ya tiene una zona asignada
        if (_activeAssignments.ContainsKey(bot.id))
        {
            DropZone currentZone = _activeAssignments[bot.id];
            if (currentZone.IsAvailable() && currentZone.CanAcceptTomatoes(tomatoCount))
            {
                Debug.Log($"Bot {bot.id} mantiene su zona asignada: {currentZone.gameObject.name}");
                return currentZone;
            }
            else
            {
                // Liberar la zona actual si ya no es válida
                ReleaseAssignment(bot);
            }
        }
        
        // Filtrar zonas disponibles
        List<DropZone> availableZones = dropZones
            .Where(z => z != null && z.IsAvailable() && z.CanAcceptTomatoes(tomatoCount))
            .ToList();
        
        if (availableZones.Count == 0)
        {
            Debug.LogWarning($"No hay zonas disponibles para Bot {bot.id}");
            return null;
        }
        
        // Seleccionar zona según estrategia
        DropZone selectedZone = SelectBestZone(bot, availableZones, tomatoCount);
        
        if (selectedZone != null && selectedZone.RequestAccess(bot))
        {
            _activeAssignments[bot.id] = selectedZone;
            Debug.Log($"Bot {bot.id} asignado a {selectedZone.gameObject.name}");
            return selectedZone;
        }
        
        return null;
    }
    
    /// <summary>
    /// Selecciona la mejor zona según la estrategia configurada.
    /// </summary>
    private DropZone SelectBestZone(Bot bot, List<DropZone> zones, int tomatoCount)
    {
        Vector3 botPosition = bot.transform.position;
        
        switch (strategy)
        {
            case AssignmentStrategy.Nearest:
                return zones.OrderBy(z => Vector3.Distance(botPosition, z.transform.position)).First();
            
            case AssignmentStrategy.LeastOccupied:
                return zones.OrderBy(z => z.GetCurrentBotCount()).First();
            
            case AssignmentStrategy.LeastLoaded:
                return zones.OrderBy(z => z.GetCapacityPercentage()).First();
            
            case AssignmentStrategy.Balanced:
            default:
                return SelectBalancedZone(bot, zones);
        }
    }
    
    /// <summary>
    /// Selecciona zona usando algoritmo balanceado con múltiples criterios.
    /// </summary>
    private DropZone SelectBalancedZone(Bot bot, List<DropZone> zones)
    {
        Vector3 botPosition = bot.transform.position;
        
        // Calcular valores máximos para normalización
        float maxDistance = zones.Max(z => Vector3.Distance(botPosition, z.transform.position));
        int maxOccupancy = zones.Max(z => z.GetCurrentBotCount());
        
        // Calcular puntuación para cada zona
        var scoredZones = zones.Select(zone =>
        {
            float distance = Vector3.Distance(botPosition, zone.transform.position);
            int occupancy = zone.GetCurrentBotCount();
            float capacity = zone.GetCapacityPercentage();
            
            // Normalizar valores (invertir para que menor sea mejor)
            float distanceFactor = maxDistance > 0 ? 1f - (distance / maxDistance) : 1f;
            float occupancyFactor = maxOccupancy > 0 ? 1f - ((float)occupancy / maxOccupancy) : 1f;
            float capacityFactor = 1f - capacity;
            
            // Calcular puntuación ponderada (mayor es mejor)
            float score = (distanceWeight * distanceFactor) +
                         (occupancyWeight * occupancyFactor) +
                         (capacityWeight * capacityFactor);
            
            Debug.Log($"Zona {zone.gameObject.name}: " +
                     $"Dist={distance:F1}m ({distanceFactor:F2}), " +
                     $"Ocup={occupancy} ({occupancyFactor:F2}), " +
                     $"Cap={capacity:P0} ({capacityFactor:F2}), " +
                     $"Score={score:F3}");
            
            return new { Zone = zone, Score = score };
        })
        .OrderByDescending(x => x.Score)
        .ToList();
        
        return scoredZones.First().Zone;
    }
    
    /// <summary>
    /// Libera la asignación de un bot, permitiendo que otros usen la zona.
    /// </summary>
    public void ReleaseAssignment(Bot bot)
    {
        if (_activeAssignments.TryGetValue(bot.id, out DropZone zone))
        {
            zone.ReleaseAccess(bot);
            _activeAssignments.Remove(bot.id);
            Debug.Log($"Bot {bot.id} liberó zona {zone.gameObject.name}");
        }
    }
    
    /// <summary>
    /// Obtiene la zona actualmente asignada a un bot.
    /// </summary>
    public DropZone GetAssignedZone(int botId)
    {
        _activeAssignments.TryGetValue(botId, out DropZone zone);
        return zone;
    }
    
    /// <summary>
    /// Obtiene estadísticas globales del sistema.
    /// </summary>
    public void PrintStatistics()
    {
        Debug.Log("=== ESTADÍSTICAS DE ZONAS DE DESCARGA ===");
        foreach (var zone in dropZones)
        {
            if (zone != null)
            {
                Debug.Log($"{zone.gameObject.name}: " +
                         $"Carga: {zone.currentLoad}/{zone.maxCapacity} ({zone.GetCapacityPercentage():P0}), " +
                         $"Bots: {zone.GetCurrentBotCount()}/{zone.maxSimultaneousBots}, " +
                         $"Total recibido: {zone.totalTomatoesReceived}, " +
                         $"Bots atendidos: {zone.totalBotsServed}");
            }
        }
    }
}
