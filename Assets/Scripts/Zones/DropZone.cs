using UnityEngine;

/// <summary>
/// Representa una zona de descarga individual para tomates.
/// Mantiene información sobre su capacidad, ocupación y estado.
/// </summary>
public class DropZone : MonoBehaviour
{
    [Header("Configuración de Zona")]
    public int maxCapacity = 100; // Capacidad máxima de tomates
    public int currentLoad = 0; // Tomates actualmente almacenados
    public bool isOperational = true; // Si la zona está operativa
    
    [Header("Control de Congestión")]
    public int maxSimultaneousBots = 2; // Máximo de robots simultáneos
    private int currentBotCount = 0; // Robots actualmente descargando
    
    [Header("Estadísticas")]
    public int totalTomatoesReceived = 0; // Total histórico
    public int totalBotsServed = 0; // Total de robots atendidos
    
    private Renderer _renderer;
    
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        UpdateVisualState();
    }
    
    /// <summary>
    /// Un robot solicita reservar espacio en esta zona
    /// </summary>
    public bool RequestAccess(Bot bot)
    {
        if (!isOperational || currentBotCount >= maxSimultaneousBots)
        {
            return false;
        }
        
        currentBotCount++;
        Debug.Log($"Bot {bot.id} reservó acceso a {gameObject.name}. Bots actuales: {currentBotCount}");
        return true;
    }
    
    /// <summary>
    /// Un robot libera su espacio reservado
    /// </summary>
    public void ReleaseAccess(Bot bot)
    {
        currentBotCount = Mathf.Max(0, currentBotCount - 1);
        Debug.Log($"Bot {bot.id} liberó acceso de {gameObject.name}. Bots actuales: {currentBotCount}");
        UpdateVisualState();
    }
    
    /// <summary>
    /// Descarga tomates en esta zona
    /// </summary>
    public void DepositTomatoes(int count)
    {
        currentLoad = Mathf.Min(currentLoad + count, maxCapacity);
        totalTomatoesReceived += count;
        totalBotsServed++;
        
        Debug.Log($"{gameObject.name} recibió {count} tomates. Carga actual: {currentLoad}/{maxCapacity}");
        UpdateVisualState();
    }
    
    /// <summary>
    /// Obtiene el porcentaje de capacidad utilizada
    /// </summary>
    public float GetCapacityPercentage()
    {
        return (float)currentLoad / maxCapacity;
    }
    
    /// <summary>
    /// Verifica si la zona puede aceptar más tomates
    /// </summary>
    public bool CanAcceptTomatoes(int count)
    {
        return isOperational && (currentLoad + count) <= maxCapacity;
    }
    
    /// <summary>
    /// Obtiene el número de robots esperando actualmente
    /// </summary>
    public int GetCurrentBotCount()
    {
        return currentBotCount;
    }
    
    /// <summary>
    /// Verifica si la zona está disponible para nuevos robots
    /// </summary>
    public bool IsAvailable()
    {
        return isOperational && currentBotCount < maxSimultaneousBots && currentLoad < maxCapacity;
    }
    
    /// <summary>
    /// Actualiza el color visual según el estado de la zona
    /// </summary>
    private void UpdateVisualState()
    {
        if (_renderer == null) return;
        
        if (!isOperational)
        {
            _renderer.material.color = Color.gray; // Inoperativa
        }
        else
        {
            float capacityRatio = GetCapacityPercentage();
            
            if (capacityRatio < 0.5f)
            {
                _renderer.material.color = Color.green; // Baja ocupación
            }
            else if (capacityRatio < 0.8f)
            {
                _renderer.material.color = Color.yellow; // Media ocupación
            }
            else
            {
                _renderer.material.color = Color.red; // Alta ocupación
            }
        }
    }
    
    /// <summary>
    /// Reinicia la zona (vaciar tomates)
    /// </summary>
    public void EmptyZone()
    {
        currentLoad = 0;
        UpdateVisualState();
        Debug.Log($"{gameObject.name} ha sido vaciada");
    }
}
