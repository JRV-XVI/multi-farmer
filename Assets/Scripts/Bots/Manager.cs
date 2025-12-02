using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private GameManager _gameManager;

    private List<GameObject> _plantsFoundList;
    private List<GameObject> _healtyPlantFoundList;
    private List<GameObject> _sickPlantFoundList;

    private int _plantsFoundCount;
    private int _sickPlantFoundCount;
    private int _healtyPlantFoundCount;

    [SerializeField]
    public GameObject[][] _recolectorsInZones; // [Zona][Recolector]
    [SerializeField]
    public GameObject[][] _purgatorsInZones; // [Zona][Purgator]

    void Awake()
    {
        _plantsFoundList = new List<GameObject>();
        _healtyPlantFoundList = new List<GameObject>();
        _sickPlantFoundList = new List<GameObject>();

        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        if(_gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!!");
        }
    }

    void Start()
    {

        

    }

    void Update()
    {
        
    }

    public void AnalizePlants(List<GameObject> plantsList)
    {
        Debug.Log($"🌱 Analizando lista de plantas recibida con {plantsList.Count} plantas.");
        AddPlantList(plantsList);

        FindRecolectorsInZones();
        FindPurgatorsInZone();

        AsignPlantsToZone();
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
            Debug.LogWarning($"⚠️ El objeto {plant.name} no tiene componente Plant. No se puede agregar.");
            return;
        }

        if (plant.GetComponent<Plant>().isCollected)
        {
            Debug.LogWarning($"⚠️ La planta {plant.name} ya ha sido recolectada. No se puede agregar.");
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
        if (plant.stemSickPercentage > _gameManager.plantMaxStemSickPercentage)
        {
            return true;
        }
        if(plant.tomatoesSickPercentage > _gameManager.plantMaxTomatoesSickPercentage)
        {
            return true;
        }
        if(plant.leavesSickPercentage > _gameManager.plantMaxLeavesSickPercentage)
        {
            return true;
        }
        return false;
    }

    private void FindRecolectorsInZones()
    {
        List<GameObject> zones = new List<GameObject>(GameObject.FindGameObjectsWithTag("SafeZone"));
        List<GameObject> recolectors = new List<GameObject>(GameObject.FindGameObjectsWithTag("BotRecolector"));

        Debug.Log($"🔍 Buscando recolectores en {zones.Count} zonas.");

        // Inicializar la matriz con el tamaño del número de zonas
        _recolectorsInZones = new GameObject[zones.Count][];

        for (int i = 0; i < zones.Count; i++)
        {
            GameObject zone = zones[i];

            // Lista temporal para recolectores que pertenecen a esta zona
            List<GameObject> recolectorsInZone = new List<GameObject>();

            foreach (GameObject recolectorObj in recolectors)
            {
                if (recolectorObj == null) continue;

                Recolector recolectorComp = recolectorObj.GetComponent<Recolector>();
                if (recolectorComp == null)
                {
                    Debug.LogWarning($"⚠️ Objeto {recolectorObj.name} taggeado como Recolector pero sin componente Recolector.");
                    continue;
                }

                GameObject recolectorSafeZone = recolectorComp.safeZone;
                if (recolectorSafeZone == null)
                {
                    // Recolector no tiene asignada zona segura
                    continue;
                }

                // Si la zona segura del recolector es la zona actual, lo añadimos
                if (recolectorSafeZone == zone)
                {
                    recolectorsInZone.Add(recolectorObj);
                }
            }

            // Guardamos el arreglo de recolectores en la posición correspondiente
            _recolectorsInZones[i] = recolectorsInZone.ToArray();

            // Log de los recolectores asignados a la zona
            foreach (GameObject recolector in _recolectorsInZones[i])
            {
                Debug.Log($"✅ Recolector {recolector.name} asignado a zona {zone.name}.");
            }
        }
    }




    private void FindPurgatorsInZone()
    {
        List<GameObject> zones = new List<GameObject>(GameObject.FindGameObjectsWithTag("TrashZone"));
        List<GameObject> purgators = new List<GameObject>(GameObject.FindGameObjectsWithTag("BotPurgator"));

        // Inicializar la matriz con el tamaño del número de zonas
        _purgatorsInZones = new GameObject[zones.Count][];

        for (int i = 0; i < zones.Count; i++)
        {
            GameObject zone = zones[i];

            // Lista temporal para purgadores que pertenecen a esta zona
            List<GameObject> purgatorInZones = new List<GameObject>();

            foreach (GameObject purgatorObj in purgators)
            {
                if (purgatorObj == null) continue;

                Purgator purgatorComp = purgatorObj.GetComponent<Purgator>();
                if (purgatorComp == null)
                {
                    Debug.LogWarning($"⚠️ Objeto {purgatorObj.name} taggeado como Purgator pero sin componente Purgatos<>.");
                    continue;
                }

                GameObject purgatorTrashZone = purgatorComp.TrashZone;
                if (purgatorTrashZone == null)
                {
                    // Purgador no tiene asignada zona segura
                    continue;
                }

                // Si la zona segura del purgador es la zona actual, lo añadimos
                if (purgatorTrashZone == zone)
                {
                    purgatorInZones.Add(purgatorObj);
                }
            }

            // Guardamos el arreglo de purgadores en la posición correspondiente
            _purgatorsInZones[i] = purgatorInZones.ToArray();

            // Log de los pugadores asignados a la zona
            foreach (GameObject purgator in _purgatorsInZones[i])
            {
                Debug.Log($"✅ Purgator {purgator.name} asignado a zona {zone.name}.");
            }
        }
    }


    public void AsignPlantsToZone()
    {
        AsignHealtyPlantToZone();
    }


    private void AsignHealtyPlantToZone()
    {
        // Crear listas vacias por zona
        List<GameObject>[] plantsForZone = new List<GameObject>[_recolectorsInZones.Length];

        for (int i = 0; i < plantsForZone.Length; i++)
        {
            plantsForZone[i] = new List<GameObject>();
        }

        Debug.Log($"🌱 Asignando {_healtyPlantFoundList.Count} plantas saludables a zonas de recolectores.");

        // Recorrer todas las plantas
        foreach (GameObject plant in _healtyPlantFoundList)
        {
            float minDist = Mathf.Infinity;
            int closestZone = -1;

            // Buscar zona mas cercana
            for (int i = 0; i < _recolectorsInZones.Length; i++)
            {
                Debug.Log($"🔍 Evaluando zona {i} para planta {plant.name}. Zona tiene {_purgatorsInZones[i].Length}");
                if (_purgatorsInZones[i].Length == 0)
                    continue;

                float dist = (plant.transform.position - _recolectorsInZones[i][0].transform.position).sqrMagnitude;

                if (dist < minDist)
                {
                    minDist = dist;
                    closestZone = i;
                }
            }

            // Agregar planta a la zona encontrada
            if (closestZone != -1)
            {
                plantsForZone[closestZone].Add(plant);
            }
        }

        // Aqui ya tienes todas las plantas ordenadas por zona

        int index = 0;
        foreach (var zonePlants in plantsForZone)
        {
            //Debug.Log($"Zona {_purgatorsInZones[zonePlants.IndexOf][0].GetComponent<Recolector>().safeZone.name} tiene {zonePlants.Count} plantas asignadas.");
            Debug.Log($"Zona {index} tiene {zonePlants.Count} plantas asignadas.");
            index++;
        }
    }


}
