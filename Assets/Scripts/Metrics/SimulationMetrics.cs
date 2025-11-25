using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;

/// <summary>
/// Sistema de métricas para recopilar y analizar datos de la simulación.
/// Registra tiempo, movimientos, eficiencia y eventos del sistema multi-agente.
/// </summary>
public class SimulationMetrics : MonoBehaviour
{
    [Header("Simulation Control")]
    public bool isSimulationActive = false;
    public float maxSimulationTime = 600f; // 10 minutos por defecto
    
    [Header("Time Metrics")]
    public float simulationStartTime;
    public float currentSimulationTime;
    public float simulationEndTime;
    
    [Header("Movement Metrics")]
    public Dictionary<int, int> movementsPerBot = new Dictionary<int, int>();
    public int totalMovements = 0;
    
    [Header("Collection Metrics")]
    public int totalTomatoesCollected = 0;
    public int totalTomatoesDelivered = 0;
    public int tomatoesInTransit = 0;
    
    [Header("Stack Metrics")]
    public int totalStacks = 0;
    public int completeStacks = 0; // Pilas con 5 tomates
    public float completionPercentage = 0f;
    
    [Header("Efficiency Metrics")]
    public float tomatoesPerMinute = 0f;
    public float averageMovementsPerTomato = 0f;
    public float averageTimePerStack = 0f;
    
    [Header("Collision Metrics")]
    public int totalCollisions = 0;
    public float totalWaitTime = 0f;
    public float averageWaitTime = 0f;
    
    [Header("Data Recording")]
    public bool recordDataPoints = true;
    public float dataPointInterval = 5f; // Registrar cada 5 segundos
    private float lastDataPointTime = 0f;
    private List<DataPoint> dataPoints = new List<DataPoint>();
    
    [Header("References")]
    public Manager manager;
    public DropZoneManager dropZoneManager;
    
    private Bot[] bots;
    private Plant[] plants;
    private bool simulationCompleted = false;
    
    [System.Serializable]
    public class DataPoint
    {
        public float time;
        public int completeStacks;
        public int tomatoesDelivered;
        public int[] botMovements;
        public int collisions;
        public int botsActive;
        public float efficiency;
    }
    
    void Start()
    {
        InitializeMetrics();
    }
    
    void Update()
    {
        if (!isSimulationActive || simulationCompleted)
            return;
        
        currentSimulationTime = Time.time - simulationStartTime;
        
        // Verificar tiempo máximo
        if (currentSimulationTime >= maxSimulationTime)
        {
            EndSimulation("Tiempo máximo alcanzado");
            return;
        }
        
        // Actualizar métricas en tiempo real
        UpdateRealTimeMetrics();
        
        // Registrar puntos de datos
        if (recordDataPoints && Time.time - lastDataPointTime >= dataPointInterval)
        {
            RecordDataPoint();
            lastDataPointTime = Time.time;
        }
        
        // Verificar condición de finalización
        CheckCompletionCondition();
        
        // Mostrar estadísticas en consola (cada 10 segundos)
        if (Mathf.FloorToInt(currentSimulationTime) % 10 == 0 && Time.frameCount % 60 == 0)
        {
            PrintCurrentStats();
        }
    }
    
    public void InitializeMetrics()
    {
        // Encontrar referencias si no están asignadas
        if (manager == null)
            manager = FindFirstObjectByType<Manager>();
        
        if (dropZoneManager == null)
            dropZoneManager = FindFirstObjectByType<DropZoneManager>();
        
        bots = FindObjectsByType<Bot>(FindObjectsSortMode.None);
        plants = FindObjectsByType<Plant>(FindObjectsSortMode.None);
        
        // Inicializar diccionario de movimientos
        movementsPerBot.Clear();
        foreach (Bot bot in bots)
        {
            movementsPerBot[bot.id] = 0;
        }
        
        totalStacks = plants.Length;
        
        Debug.Log($"=== MÉTRICAS INICIALIZADAS ===");
        Debug.Log($"Bots: {bots.Length}");
        Debug.Log($"Pilas (Plantas): {plants.Length}");
        Debug.Log($"Tiempo máximo: {maxSimulationTime}s");
    }
    
    public void StartSimulation()
    {
        isSimulationActive = true;
        simulationStartTime = Time.time;
        currentSimulationTime = 0f;
        simulationCompleted = false;
        
        ResetMetrics();
        
        Debug.Log($"🚀 SIMULACIÓN INICIADA - {System.DateTime.Now:HH:mm:ss}");
    }
    
    public void EndSimulation(string reason)
    {
        if (simulationCompleted)
            return;
        
        simulationCompleted = true;
        isSimulationActive = false;
        simulationEndTime = Time.time;
        
        float totalTime = simulationEndTime - simulationStartTime;
        
        Debug.Log($"🏁 SIMULACIÓN FINALIZADA - {reason}");
        Debug.Log($"⏱️ Tiempo total: {totalTime:F2} segundos ({totalTime / 60f:F2} minutos)");
        
        GenerateFinalReport();
        ExportToCSV("simulation_results");
    }
    
    void ResetMetrics()
    {
        totalMovements = 0;
        totalTomatoesCollected = 0;
        totalTomatoesDelivered = 0;
        totalCollisions = 0;
        totalWaitTime = 0f;
        dataPoints.Clear();
        
        foreach (var key in movementsPerBot.Keys.ToList())
        {
            movementsPerBot[key] = 0;
        }
    }
    
    void UpdateRealTimeMetrics()
    {
        // Contar tomates entregados y pilas completas
        int delivered = 0;
        int complete = 0;
        
        if (dropZoneManager != null && dropZoneManager.dropZones != null)
        {
            foreach (var zone in dropZoneManager.dropZones)
            {
                if (zone != null)
                {
                    delivered += zone.currentLoad;
                    complete += zone.currentLoad / 5; // Cada 5 tomates es una pila completa
                }
            }
        }
        
        totalTomatoesDelivered = delivered;
        completeStacks = complete;
        
        // Tomates en tránsito (cargados por bots)
        tomatoesInTransit = 0;
        foreach (Bot bot in bots)
        {
            if (bot != null)
            {
                tomatoesInTransit += bot.GetCollectedTomatoCount();
            }
        }
        
        // Calcular porcentaje de completitud
        int totalPossibleStacks = totalStacks; // Asumiendo que cada planta puede generar múltiples pilas
        if (totalPossibleStacks > 0)
        {
            completionPercentage = (completeStacks / (float)totalPossibleStacks) * 100f;
        }
        
        // Eficiencia
        if (currentSimulationTime > 0)
        {
            tomatoesPerMinute = (totalTomatoesDelivered / currentSimulationTime) * 60f;
        }
        
        if (totalTomatoesDelivered > 0)
        {
            averageMovementsPerTomato = totalMovements / (float)totalTomatoesDelivered;
        }
        
        if (completeStacks > 0)
        {
            averageTimePerStack = currentSimulationTime / completeStacks;
        }
        
        if (totalCollisions > 0)
        {
            averageWaitTime = totalWaitTime / totalCollisions;
        }
    }
    
    void RecordDataPoint()
    {
        DataPoint point = new DataPoint
        {
            time = currentSimulationTime,
            completeStacks = completeStacks,
            tomatoesDelivered = totalTomatoesDelivered,
            botMovements = movementsPerBot.Values.ToArray(),
            collisions = totalCollisions,
            botsActive = bots.Count(b => b != null && b.gameObject.activeInHierarchy),
            efficiency = tomatoesPerMinute
        };
        
        dataPoints.Add(point);
    }
    
    void CheckCompletionCondition()
    {
        // Verificar si todas las pilas están completas
        // O si todos los tomates disponibles han sido entregados
        
        int totalAvailableTomatoes = 0;
        foreach (Plant plant in plants)
        {
            if (plant != null)
            {
                totalAvailableTomatoes += plant.GetAvailableTomatoCount();
            }
        }
        
        // Si no hay tomates disponibles y los bots no llevan nada
        if (totalAvailableTomatoes == 0 && tomatoesInTransit == 0)
        {
            EndSimulation("Todos los tomates han sido recolectados y entregados");
        }
    }
    
    // Métodos públicos para registrar eventos
    
    public void RecordMovement(int botId)
    {
        if (movementsPerBot.ContainsKey(botId))
        {
            movementsPerBot[botId]++;
            totalMovements++;
        }
    }
    
    public void RecordTomatoCollected(int botId)
    {
        totalTomatoesCollected++;
    }
    
    public void RecordTomatoDelivered(int botId, int count)
    {
        // La entrega ya se cuenta en UpdateRealTimeMetrics
    }
    
    public void RecordCollision(int bot1Id, int bot2Id, float waitTime = 0f)
    {
        totalCollisions++;
        totalWaitTime += waitTime;
        
        Debug.Log($"⚠️ Colisión #{totalCollisions}: Bot {bot1Id} vs Bot {bot2Id}");
    }
    
    void PrintCurrentStats()
    {
        Debug.Log($"📊 [T={currentSimulationTime:F0}s] " +
            $"Pilas: {completeStacks}/{totalStacks} | " +
            $"Tomates: {totalTomatoesDelivered} entregados, {tomatoesInTransit} en tránsito | " +
            $"Movimientos: {totalMovements} | " +
            $"Eficiencia: {tomatoesPerMinute:F1} tomates/min");
    }
    
    void GenerateFinalReport()
    {
        StringBuilder report = new StringBuilder();
        report.AppendLine("╔════════════════════════════════════════════════════════════╗");
        report.AppendLine("║           REPORTE FINAL DE SIMULACIÓN                      ║");
        report.AppendLine("╚════════════════════════════════════════════════════════════╝");
        report.AppendLine();
        
        float totalTime = simulationEndTime - simulationStartTime;
        
        report.AppendLine("⏱️  TIEMPO:");
        report.AppendLine($"   • Duración total: {totalTime:F2} segundos ({totalTime / 60f:F2} minutos)");
        report.AppendLine($"   • Tiempo por pila completa: {averageTimePerStack:F2} segundos");
        report.AppendLine();
        
        report.AppendLine("📦 RECOLECCIÓN:");
        report.AppendLine($"   • Tomates entregados: {totalTomatoesDelivered}");
        report.AppendLine($"   • Pilas completas (5 tomates): {completeStacks}");
        report.AppendLine($"   • Porcentaje de completitud: {completionPercentage:F1}%");
        report.AppendLine();
        
        report.AppendLine("🚶 MOVIMIENTOS:");
        report.AppendLine($"   • Movimientos totales: {totalMovements}");
        foreach (var kvp in movementsPerBot.OrderBy(x => x.Key))
        {
            float percentage = totalMovements > 0 ? (kvp.Value / (float)totalMovements) * 100f : 0f;
            report.AppendLine($"   • Bot {kvp.Key}: {kvp.Value} movimientos ({percentage:F1}%)");
        }
        report.AppendLine($"   • Promedio por bot: {totalMovements / (float)bots.Length:F1}");
        report.AppendLine($"   • Movimientos por tomate: {averageMovementsPerTomato:F2}");
        report.AppendLine();
        
        report.AppendLine("⚡ EFICIENCIA:");
        report.AppendLine($"   • Tomates por minuto: {tomatoesPerMinute:F2}");
        report.AppendLine($"   • Tomates por bot por minuto: {tomatoesPerMinute / bots.Length:F2}");
        report.AppendLine();
        
        report.AppendLine("⚠️  COLISIONES:");
        report.AppendLine($"   • Colisiones totales: {totalCollisions}");
        report.AppendLine($"   • Tiempo total de espera: {totalWaitTime:F2} segundos");
        report.AppendLine($"   • Tiempo promedio por colisión: {averageWaitTime:F2} segundos");
        report.AppendLine();
        
        report.AppendLine("═══════════════════════════════════════════════════════════");
        
        Debug.Log(report.ToString());
    }
    
    public void ExportToCSV(string filename)
    {
        if (dataPoints.Count == 0)
        {
            Debug.LogWarning("No hay datos para exportar");
            return;
        }
        
        string path = Path.Combine(Application.dataPath, $"{filename}_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv");
        
        StringBuilder csv = new StringBuilder();
        
        // Encabezados
        csv.Append("Time,CompleteStacks,TomatoesDelivered,TotalMovements");
        for (int i = 0; i < bots.Length; i++)
        {
            csv.Append($",Bot{i + 1}Moves");
        }
        csv.AppendLine(",Collisions,ActiveBots,Efficiency");
        
        // Datos
        foreach (DataPoint point in dataPoints)
        {
            csv.Append($"{point.time:F2},{point.completeStacks},{point.tomatoesDelivered}");
            csv.Append($",{point.botMovements.Sum()}");
            
            foreach (int moves in point.botMovements)
            {
                csv.Append($",{moves}");
            }
            
            csv.AppendLine($",{point.collisions},{point.botsActive},{point.efficiency:F2}");
        }
        
        // Resumen final
        csv.AppendLine();
        csv.AppendLine("=== RESUMEN FINAL ===");
        csv.AppendLine($"Tiempo Total (s),{simulationEndTime - simulationStartTime:F2}");
        csv.AppendLine($"Tomates Entregados,{totalTomatoesDelivered}");
        csv.AppendLine($"Pilas Completas,{completeStacks}");
        csv.AppendLine($"Movimientos Totales,{totalMovements}");
        csv.AppendLine($"Colisiones Totales,{totalCollisions}");
        csv.AppendLine($"Eficiencia (tomates/min),{tomatoesPerMinute:F2}");
        
        File.WriteAllText(path, csv.ToString());
        
        Debug.Log($"📄 Datos exportados a: {path}");
    }
    
    // UI Debug (opcional)
    void OnGUI()
    {
        if (!isSimulationActive && !simulationCompleted)
        {
            if (GUI.Button(new Rect(10, 10, 200, 30), "Iniciar Simulación"))
            {
                StartSimulation();
            }
        }
        
        if (isSimulationActive)
        {
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = 14;
            style.normal.textColor = Color.white;
            
            string stats = $"⏱️ Tiempo: {currentSimulationTime:F1}s / {maxSimulationTime:F0}s\n" +
                          $"📦 Pilas: {completeStacks} | Tomates: {totalTomatoesDelivered}\n" +
                          $"🚶 Movimientos: {totalMovements}\n" +
                          $"⚡ Eficiencia: {tomatoesPerMinute:F1} t/min\n" +
                          $"⚠️ Colisiones: {totalCollisions}";
            
            GUI.Box(new Rect(Screen.width - 310, 10, 300, 120), stats, style);
            
            if (GUI.Button(new Rect(Screen.width - 310, 140, 145, 30), "Detener"))
            {
                EndSimulation("Detenido manualmente");
            }
            
            if (GUI.Button(new Rect(Screen.width - 155, 140, 145, 30), "Exportar CSV"))
            {
                ExportToCSV("manual_export");
            }
        }
    }
}
