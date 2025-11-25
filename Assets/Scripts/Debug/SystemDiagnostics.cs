using UnityEngine;
using System.Linq;

/// <summary>
/// Script de diagnóstico para depurar problemas del sistema multi-agente.
/// Adjuntar a cualquier GameObject en la escena para ver información en tiempo real.
/// </summary>
public class SystemDiagnostics : MonoBehaviour
{
    [Header("Configuración")]
    public bool showDebugGUI = true;
    public bool printToConsole = false;
    public float updateInterval = 2f;
    
    private float _lastUpdate;
    private string _diagnosticInfo = "";
    
    void Update()
    {
        if (Time.time - _lastUpdate >= updateInterval)
        {
            _lastUpdate = Time.time;
            RunDiagnostics();
        }
    }
    
    void RunDiagnostics()
    {
        _diagnosticInfo = "=== DIAGNÓSTICO DEL SISTEMA ===\n\n";
        
        // Verificar Bots
        Bot[] bots = FindObjectsByType<Bot>(FindObjectsSortMode.None);
        _diagnosticInfo += $"🤖 BOTS: {bots.Length}\n";
        foreach (var bot in bots)
        {
            string state = bot.GetType().GetField("_stateMachine", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(bot)?.ToString() ?? "Unknown";
            
            _diagnosticInfo += $"  Bot {bot.id}: {bot.GetCollectedTomatoCount()}/{bot.maxTomatoCapacity} tomates\n";
            _diagnosticInfo += $"    Planta objetivo: {(bot.targetPlant != null ? bot.targetPlant.name : "ninguna")}\n";
            _diagnosticInfo += $"    Zona asignada: {(bot.assignedDropZone != null ? bot.assignedDropZone.gameObject.name : "ninguna")}\n";
            _diagnosticInfo += $"    SafeZone (legacy): {(bot.safeZone != null ? "✓" : "✗")}\n";
        }
        
        // Verificar Plantas
        _diagnosticInfo += "\n🌱 PLANTAS:\n";
        Plant[] plants = FindObjectsByType<Plant>(FindObjectsSortMode.None);
        int plantsWithTomatoes = 0;
        int totalTomatoes = 0;
        
        foreach (var plant in plants)
        {
            int tomatoes = plant.GetAvailableTomatoCount();
            totalTomatoes += tomatoes;
            if (tomatoes > 0) plantsWithTomatoes++;
            
            _diagnosticInfo += $"  {plant.gameObject.name}: {tomatoes} tomates, ";
            _diagnosticInfo += $"Ocupada: {(plant.isTaken ? "SÍ" : "NO")}, ";
            _diagnosticInfo += $"AutoGen: {plant.enableAutoGeneration}\n";
        }
        
        _diagnosticInfo += $"\n  Total plantas: {plants.Length}\n";
        _diagnosticInfo += $"  Plantas con tomates: {plantsWithTomatoes}\n";
        _diagnosticInfo += $"  Total tomates disponibles: {totalTomatoes}\n";
        
        // Verificar Zonas de Descarga
        _diagnosticInfo += "\n📦 ZONAS DE DESCARGA:\n";
        DropZone[] dropZones = FindObjectsByType<DropZone>(FindObjectsSortMode.None);
        
        if (dropZones.Length > 0)
        {
            _diagnosticInfo += $"  Sistema NUEVO activo: {dropZones.Length} zonas\n";
            foreach (var zone in dropZones)
            {
                _diagnosticInfo += $"  {zone.gameObject.name}: ";
                _diagnosticInfo += $"Carga {zone.currentLoad}/{zone.maxCapacity}, ";
                _diagnosticInfo += $"Bots {zone.GetCurrentBotCount()}/{zone.maxSimultaneousBots}\n";
            }
        }
        else
        {
            GameObject legacyZone = GameObject.FindGameObjectWithTag("Zone");
            if (legacyZone != null)
            {
                _diagnosticInfo += $"  Sistema LEGACY: 1 zona única ({legacyZone.name})\n";
            }
            else
            {
                _diagnosticInfo += "  ⚠️ NO HAY ZONAS DE DESCARGA\n";
            }
        }
        
        // Verificar Manager
        _diagnosticInfo += "\n⚙️ MANAGERS:\n";
        Manager manager = FindFirstObjectByType<Manager>();
        if (manager != null)
        {
            _diagnosticInfo += $"  Manager: ✓\n";
            _diagnosticInfo += $"  Plantas registradas: {manager._plantList?.Length ?? 0}\n";
        }
        else
        {
            _diagnosticInfo += "  ⚠️ NO HAY MANAGER\n";
        }
        
        DropZoneManager dzManager = FindFirstObjectByType<DropZoneManager>();
        if (dzManager != null)
        {
            _diagnosticInfo += $"  DropZoneManager: ✓\n";
            _diagnosticInfo += $"  Estrategia: {dzManager.strategy}\n";
        }
        else
        {
            _diagnosticInfo += "  ⚠️ NO HAY DropZoneManager (sistema legacy)\n";
        }
        
        // Problemas detectados
        _diagnosticInfo += "\n🔍 PROBLEMAS DETECTADOS:\n";
        bool hasProblems = false;
        
        if (bots.Length == 0)
        {
            _diagnosticInfo += "  ❌ No hay bots en la escena\n";
            hasProblems = true;
        }
        
        if (plants.Length == 0)
        {
            _diagnosticInfo += "  ❌ No hay plantas en la escena\n";
            hasProblems = true;
        }
        
        if (totalTomatoes == 0)
        {
            _diagnosticInfo += "  ⚠️ No hay tomates disponibles (generación puede estar desactivada)\n";
            hasProblems = true;
        }
        
        if (dropZones.Length == 0 && GameObject.FindGameObjectWithTag("Zone") == null)
        {
            _diagnosticInfo += "  ❌ No hay zonas de descarga (ni nuevas ni legacy)\n";
            hasProblems = true;
        }
        
        if (manager == null)
        {
            _diagnosticInfo += "  ❌ Falta el Manager\n";
            hasProblems = true;
        }
        
        int botsWithoutTarget = bots.Count(b => b.targetPlant == null && b.GetCollectedTomatoCount() == 0);
        if (botsWithoutTarget > 0 && plantsWithTomatoes > 0)
        {
            _diagnosticInfo += $"  ⚠️ {botsWithoutTarget} bots sin planta objetivo pero hay plantas con tomates\n";
            hasProblems = true;
        }
        
        if (!hasProblems)
        {
            _diagnosticInfo += "  ✅ No se detectaron problemas obvios\n";
        }
        
        if (printToConsole)
        {
            Debug.Log(_diagnosticInfo);
        }
    }
    
    void OnGUI()
    {
        if (!showDebugGUI) return;
        
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = 12;
        style.normal.textColor = Color.white;
        style.padding = new RectOffset(10, 10, 10, 10);
        
        GUI.Box(new Rect(10, 10, 500, Screen.height - 20), _diagnosticInfo, style);
    }
}
