using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public struct ZoneInfo
{
    public GameObject zone;
    public List<GameObject> botsInZone;
    public List<GameObject> plantsInZone;
}

public class Manager : MonoBehaviour
{
    private GameManager _gameManager;

    [SerializeField]
    private List<GameObject> _plantsFoundList;
    [SerializeField]
    private List<GameObject> _healtyPlantFoundList;
    [SerializeField]
    private List<GameObject> _sickPlantFoundList;

    [SerializeField]
    private List<ZoneInfo> _zonesInfo;   // <--- AHORA SOLO ESTO MANEJA TODO

    void Awake()
    {
        _plantsFoundList = new List<GameObject>();
        _healtyPlantFoundList = new List<GameObject>();
        _sickPlantFoundList = new List<GameObject>();

        _zonesInfo = new List<ZoneInfo>();

        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!!");
        }
    }

    void Start()
    {
        // Encontrar al Explorer y enviarle la lista de plantas para explorar
        GameObject explorerObj = GameObject.FindWithTag("BotExplorer");
        if (explorerObj == null)
        {
            Debug.LogError("❌ Manager no pudo encontrar Explorer! Asegúrate de que tenga tag 'BotExplorer'");
            return;
        }

        Explorer explorer = explorerObj.GetComponent<Explorer>();
        if (explorer == null)
        {
            Debug.LogError("❌ GameObject BotExplorador no tiene componente Explorer!");
            return;
        }

        // Obtener todas las plantas de la escena
        Plant[] allPlants = FindObjectsByType<Plant>(FindObjectsSortMode.None);
        List<GameObject> plantsList = new List<GameObject>();
        
        foreach (Plant plant in allPlants)
        {
            if (plant != null && plant.gameObject != null)
            {
                plantsList.Add(plant.gameObject);
            }
        }

        Debug.Log($"📋 Manager encontró {plantsList.Count} plantas y las enviará al Explorer");
        
        // Enviar lista al Explorer para que comience la exploración
        explorer.StartExploration(plantsList);
    }

    public void AnalizePlants(List<GameObject> plantsList)
    {
        Debug.Log($"Analizando lista de plantas recibida con {plantsList.Count} plantas.");
        AddPlantList(plantsList);

        Debug.Log($"Se encontraron (Plantas) {_healtyPlantFoundList.Count} saludables y {_sickPlantFoundList.Count} enfermas");

        BuildZonesInfo();        // <--- Construye la lista de ZoneInfo
        AsignPlantsToZones();    // <--- Usa ZoneInfo.plantsInZone
    }

    private void BuildZonesInfo()
    {
        _zonesInfo.Clear();

        // 1. Encontrar todas las zonas y bots
        List<GameObject> safeZones = new List<GameObject>(GameObject.FindGameObjectsWithTag("SafeZone"));
        List<GameObject> trashZones = new List<GameObject>(GameObject.FindGameObjectsWithTag("TrashZone"));

        List<GameObject> recolectors = new List<GameObject>(GameObject.FindGameObjectsWithTag("BotRecolector"));
        List<GameObject> purgators = new List<GameObject>(GameObject.FindGameObjectsWithTag("BotPurgator"));

        // 2. Crear ZoneInfo por cada zona (SafeZone y TrashZone)
        foreach (GameObject zone in safeZones)
        {
            ZoneInfo z = new ZoneInfo();
            z.zone = zone;
            z.botsInZone = new List<GameObject>();
            z.plantsInZone = new List<GameObject>();
            _zonesInfo.Add(z);
        }

        foreach (GameObject zone in trashZones)
        {
            ZoneInfo z = new ZoneInfo();
            z.zone = zone;
            z.botsInZone = new List<GameObject>();
            z.plantsInZone = new List<GameObject>();
            _zonesInfo.Add(z);
        }

        // 3. Asignar bots recolectores a SafeZones
        /*
        for (int i = 0; i < _zonesInfo.Count; i++)
        {
            if (_zonesInfo[i].botsInZone == null) _zonesInfo[i].botsInZone = new List<GameObject>();
            if (_zonesInfo[i].plantsInZone == null) _zonesInfo[i].plantsInZone = new List<GameObject>();
        }
        */

        foreach (GameObject bot in recolectors)
        {
            if (bot == null) continue;

            Recolector comp = bot.GetComponent<Recolector>();
            if (comp == null) continue;
            if (comp.safeZone == null) continue;

            for (int i = 0; i < _zonesInfo.Count; i++)
            {
                if (_zonesInfo[i].zone == comp.safeZone)
                {
                    _zonesInfo[i].botsInZone.Add(bot);
                    break;
                }
            }
        }

        // 4. Asignar bots purgadores a TrashZones
        foreach (GameObject bot in purgators)
        {
            if (bot == null) continue;

            Purgator comp = bot.GetComponent<Purgator>();
            if (comp == null) continue;
            if (comp.TrashZone == null) continue;

            for (int i = 0; i < _zonesInfo.Count; i++)
            {
                if (_zonesInfo[i].zone == comp.TrashZone)
                {
                    _zonesInfo[i].botsInZone.Add(bot);
                    break;
                }
            }
        }

        // 5. Log de resultado final
        for (int i = 0; i < _zonesInfo.Count; i++)
        {
            Debug.Log($"Zona {_zonesInfo[i].zone.name} tiene {_zonesInfo[i].botsInZone.Count} bots.");
        }
    }


    public void AddPlantList(List<GameObject> plants)
    {
        foreach (GameObject plant in plants)
        {
            AddPlantFoundToList(plant);
        }
    }

    public void AddPlantFoundToList(GameObject plant)
    {
        if (plant.GetComponent<Plant>() == false)
        {
            Debug.LogWarning($"El objeto {plant.name} no tiene componente Plant. No se puede agregar.");
            return;
        }

        if (plant.GetComponent<Plant>().isCollected)
        {
            Debug.LogWarning($"La planta {plant.name} ya ha sido recolectada. No se puede agregar.");
            return;
        }

        if (PlantIsSick(plant.GetComponent<Plant>()))
        {
            _sickPlantFoundList.Add(plant);
        }
        else
        {
            _healtyPlantFoundList.Add(plant);
        }
    }

    private bool PlantIsSick(Plant plant)
    {
        if (plant.stemSickPercentage > _gameManager.plantMaxStemSickPercentage) return true;
        if (plant.tomatoesSickPercentage > _gameManager.plantMaxTomatoesSickPercentage) return true;
        if (plant.leavesSickPercentage > _gameManager.plantMaxLeavesSickPercentage) return true;
        return false;
    }

    // ============================================================================================
    // ASIGNACION DE PLANTAS A ZONAS
    // ============================================================================================

    private void AsignPlantsToZones()
    {
        // Vaciar plantas en todas las zonas
        for (int i = 0; i < _zonesInfo.Count; i++)
        {
            _zonesInfo[i].plantsInZone.Clear();
        }

        AsignHealtyPlants();
        AsignSickPlants();

        AsignPlantsToBots();
    }

    private void AsignHealtyPlants()
    {
        Debug.Log($"Asignando {_healtyPlantFoundList.Count} plantas saludables a zonas.");

        List<ZoneInfo> zones = new List<ZoneInfo>();

        foreach (ZoneInfo zz in _zonesInfo)
        {
            if (zz.zone.GetComponent<Zone>().zoneType == ZoneType.SafeZone)
                zones.Add(zz);
        }

        AsignPlantToZone2(_healtyPlantFoundList, zones);
    }

    private void AsignSickPlants()
    {
        Debug.Log($"Asignando {_sickPlantFoundList.Count} plantas enfermas a zonas.");

        List<ZoneInfo> zones = new List<ZoneInfo>();

        foreach (ZoneInfo zz in _zonesInfo)
        {
            if (zz.zone.GetComponent<Zone>().zoneType == ZoneType.TrashZone)
                zones.Add(zz);
        }

        // FIXED: pass sick plants list (was mistakenly passing healthy list)
        AsignPlantToZone2(_sickPlantFoundList, zones);
    }

    private void AsignPlantToZone2(List<GameObject> plants, List<ZoneInfo> zonesInfo)
    {
        Debug.Log($"Asignando {plants.Count} plantas a {zonesInfo.Count} zonas.");
        foreach (GameObject plant in plants)
        {
            float minDist = Mathf.Infinity;
            int bestZone = -1;

            for (int i = 0; i < zonesInfo.Count; i++)
            {
                if (zonesInfo[i].botsInZone == null || zonesInfo[i].botsInZone.Count == 0) continue;

                // USE the filtered zonesInfo for distance calculation (avoid using _zonesInfo[i])
                float dist = (plant.transform.position - zonesInfo[i].zone.transform.position).sqrMagnitude;

                if (dist < minDist)
                {
                    minDist = dist;
                    bestZone = i;
                }
            }

            if (bestZone != -1)
            {
                GameObject targetZone = zonesInfo[bestZone].zone;
                for (int j = 0; j < _zonesInfo.Count; j++)
                {
                    if (_zonesInfo[j].zone == targetZone)
                    {
                        _zonesInfo[j].plantsInZone.Add(plant);
                        break;
                    }
                }
            }
        }

        // Log final
        for (int i = 0; i < _zonesInfo.Count; i++)
        {
            Debug.Log($"Zona {_zonesInfo[i].zone.name} tiene {_zonesInfo[i].plantsInZone.Count} plantas asignadas.");
        }
    }

    private void AsignPlantsToBots()
    {
        for (int zi = 0; zi < _zonesInfo.Count; zi++)
        {
            ZoneInfo zone = _zonesInfo[zi];

            int totalPlants = zone.plantsInZone?.Count ?? 0;
            int totalBots = zone.botsInZone?.Count ?? 0;

            if (totalPlants == 0 || totalBots == 0)
            {
                Debug.Log($"zona {zone.zone.name} tiene {totalBots} bots y {totalPlants} plantas -> nada que asignar.");
                // Still clear bots' lists to avoid stale assignments
                foreach (GameObject bot in zone.botsInZone)
                {
                    if (bot == null) continue;
                    if (bot.tag == "BotRecolector")
                    {
                        Recolector rc = bot.GetComponent<Recolector>();
                        if (rc != null) rc.InitializePlantList(new List<GameObject>());
                    }
                    else if (bot.tag == "BotPurgator")
                    {
                        Purgator pg = bot.GetComponent<Purgator>();
                        if (pg != null) pg.InitializePlantList(new List<GameObject>());
                    }
                }
                continue;
            }

            int baseCount = totalPlants / totalBots;
            int remainder = totalPlants % totalBots;

            Debug.Log($"zona {zone.zone.name} tiene {totalBots} bots y {totalPlants} plantas. base={baseCount} resto={remainder}");

            int startIndex = 0;

            for (int b = 0; b < zone.botsInZone.Count; b++)
            {
                GameObject bot = zone.botsInZone[b];
                if (bot == null) continue;

                int countForBot = baseCount + (remainder > 0 ? 1 : 0);
                if (remainder > 0) remainder--;

                if (countForBot <= 0)
                {
                    // assign empty list to ensure bot internal lists are cleared
                    if (bot.tag == "BotRecolector")
                    {
                        Recolector rc = bot.GetComponent<Recolector>();
                        if (rc != null) rc.InitializePlantList(new List<GameObject>());
                    }
                    else if (bot.tag == "BotPurgator")
                    {
                        Purgator pg = bot.GetComponent<Purgator>();
                        if (pg != null) pg.InitializePlantList(new List<GameObject>());
                    }
                    continue;
                }

                // Ensure we don't go out of bounds
                int available = Math.Max(0, totalPlants - startIndex);
                int take = Math.Min(countForBot, available);

                List<GameObject> plantsForBot = new List<GameObject>();
                if (take > 0)
                {
                    plantsForBot = zone.plantsInZone.GetRange(startIndex, take);
                }

                if (bot.tag == "BotRecolector")
                {
                    Recolector rc = bot.GetComponent<Recolector>();
                    if (rc != null) rc.InitializePlantList(plantsForBot);
                }
                else if (bot.tag == "BotPurgator")
                {
                    Purgator pg = bot.GetComponent<Purgator>();
                    if (pg != null) pg.InitializePlantList(plantsForBot);
                }

                startIndex += take;
                if (startIndex >= totalPlants) break;
            }
        }
    }


}