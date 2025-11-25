using UnityEngine;

/// <summary>
/// Controlador automático de simulación.
/// Adjuntar a un GameObject para iniciar la simulación automáticamente después de un retraso.
/// </summary>
public class AutoSimulationStarter : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tiempo de espera antes de iniciar la simulación (segundos)")]
    public float startDelay = 3f;
    
    [Tooltip("Iniciar automáticamente al cargar la escena")]
    public bool autoStart = true;
    
    [Header("Referencias")]
    public SimulationMetrics metrics;
    
    private bool hasStarted = false;
    
    void Start()
    {
        // Buscar sistema de métricas si no está asignado
        if (metrics == null)
        {
            metrics = FindFirstObjectByType<SimulationMetrics>();
            
            if (metrics == null)
            {
                Debug.LogError("❌ No se encontró SimulationMetrics en la escena. " +
                    "Asegúrate de tener el componente agregado.");
                enabled = false;
                return;
            }
        }
        
        if (autoStart)
        {
            Debug.Log($"⏳ Simulación iniciará en {startDelay} segundos...");
            Invoke(nameof(StartSimulation), startDelay);
        }
    }
    
    public void StartSimulation()
    {
        if (hasStarted)
        {
            Debug.LogWarning("⚠️ La simulación ya ha sido iniciada.");
            return;
        }
        
        hasStarted = true;
        metrics.StartSimulation();
        
        Debug.Log("🎬 Simulación iniciada por AutoSimulationStarter");
    }
    
    void Update()
    {
        // Permitir inicio manual con tecla Space
        if (!hasStarted && Input.GetKeyDown(KeyCode.Space))
        {
            CancelInvoke(nameof(StartSimulation));
            StartSimulation();
        }
        
        // Permitir detener con tecla Escape
        if (hasStarted && Input.GetKeyDown(KeyCode.Escape))
        {
            metrics.EndSimulation("Detenido con tecla ESC");
        }
        
        // Exportar con tecla E
        if (hasStarted && Input.GetKeyDown(KeyCode.E))
        {
            metrics.ExportToCSV($"manual_export_{System.DateTime.Now:HHmmss}");
            Debug.Log("💾 Datos exportados manualmente");
        }
    }
    
    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperLeft;
        
        if (!hasStarted)
        {
            string message = autoStart 
                ? $"Presiona ESPACIO para iniciar ahora (auto en {Mathf.CeilToInt(startDelay - Time.timeSinceLevelLoad)}s)"
                : "Presiona ESPACIO para iniciar la simulación";
                
            GUI.Label(new Rect(10, Screen.height - 30, 600, 25), message, style);
        }
        else
        {
            GUI.Label(new Rect(10, Screen.height - 60, 400, 25), "ESC: Detener simulación", style);
            GUI.Label(new Rect(10, Screen.height - 35, 400, 25), "E: Exportar datos", style);
        }
    }
}
