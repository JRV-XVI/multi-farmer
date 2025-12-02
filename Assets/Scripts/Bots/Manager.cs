using NUnit.Framework;
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
    }

    void Start()
    {
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        FindRecolectorsInZones();
        FindPurgatorsInZone();
    }

    void Update()
    {
        
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
        List<GameObject> zones = new List<GameObject>(GameObject.FindGameObjectsWithTag("Zone"));
        List<GameObject> recolectors = new List<GameObject>(GameObject.FindGameObjectsWithTag("Recolector"));

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
        }
    }




    private void FindPurgatorsInZone()
    {
        List<GameObject> zones = new List<GameObject>(GameObject.FindGameObjectsWithTag("Zone"));
        List<GameObject> purgators = new List<GameObject>(GameObject.FindGameObjectsWithTag("Purgator"));

        // Inicializar la matriz con el tamaño del número de zonas
        _purgatorsInZones = new GameObject[zones.Count][];

        for (int i = 0; i < zones.Count; i++)
        {
            GameObject zone = zones[i];

            // Lista temporal para recolectores que pertenecen a esta zona
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
                    // Recolector no tiene asignada zona segura
                    continue;
                }

                // Si la zona segura del recolector es la zona actual, lo añadimos
                if (purgatorTrashZone == zone)
                {
                    purgatorInZones.Add(purgatorObj);
                }
            }

            // Guardamos el arreglo de recolectores en la posición correspondiente
            _purgatorsInZones[i] = purgatorInZones.ToArray();
        }
    }

}
