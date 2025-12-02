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

    private int _plantsFoundCount;
    private int _sickPlantFoundCount;
    private int _healtyPlantFoundCount;

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

    public void AnalizePlants(List<GameObject> plantsList)
    {
        Debug.Log($"Analizando lista de plantas recibida con {plantsList.Count} plantas.");
        AddPlantList(plantsList);

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
        foreach (GameObject bot in recolectors)
        {
            if (bot == null) continue;

            Recolector comp = bot.GetComponent<Recolector>();
            if (comp == null) continue;
            if (comp.safeZone == null) continue;

            foreach (var zone in _zonesInfo)
            {
                if (zone.zone == comp.safeZone)
                {
                    zone.botsInZone.Add(bot);
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

            foreach (var zone in _zonesInfo)
            {
                if (zone.zone == comp.TrashZone)
                {
                    zone.botsInZone.Add(bot);
                    break;
                }
            }
        }

        // 5. Log de resultado final
        for (int i = 0; i < _zonesInfo.Count; i++)
        {
            Debug.Log($"Zona {i} ({_zonesInfo[i].zone.tag}) tiene {_zonesInfo[i].botsInZone.Count} bots.");
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
        _plantsFoundCount = _plantsFoundList.Count;
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
    }

    private void AsignHealtyPlants()
    {
        Debug.Log($"Asignando {_healtyPlantFoundList.Count} plantas saludables a zonas.");

        List<ZoneInfo> zones = new List<ZoneInfo>();

        foreach (ZoneInfo zz in _zonesInfo)
        {
            if(zz.zone.GetComponent<Zone>().zoneType == ZoneType.SafeZone)
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

        AsignPlantToZone2(_healtyPlantFoundList, zones);
    }

    private void AsignPlantToZone2(List<GameObject> plants, List<ZoneInfo> zonesInfo)
    {
        foreach (GameObject plant in plants)
        {
            float minDist = Mathf.Infinity;
            int bestZone = -1;

            for (int i = 0; i < zonesInfo.Count; i++)
            {
                if (zonesInfo[i].botsInZone.Count == 0) continue;

                float dist = (plant.transform.position - _zonesInfo[i].zone.transform.position).sqrMagnitude;

                if (dist < minDist)
                {
                    minDist = dist;
                    bestZone = i;
                }
            }

            if (bestZone != -1)
            {
                _zonesInfo[bestZone].plantsInZone.Add(plant);
            }
        }

        // Log final
        for (int i = 0; i < _zonesInfo.Count; i++)
        {
            Debug.Log($"Zona {i} tiene {_zonesInfo[i].plantsInZone.Count} plantas asignadas.");
        }
    }





}
