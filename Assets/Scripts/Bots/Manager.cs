using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour
{
    private GameManager _gameManager;

    private List<GameObject> _plantsFoundList;
    private List<GameObject> _healtyPlantFoundList;
    private List<GameObject> _sickPlantFoundList;

    private int _plantsFoundCount;
    private int _sickPlantFoundCount;
    private int _healtyPlantFoundCount;


    void Awake()
    {
        _plantsFoundList = new List<GameObject>();
        _healtyPlantFoundList = new List<GameObject>();
        _sickPlantFoundList = new List<GameObject>();
    }


    void Start()
    {
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }


    void Update()
    {
        
    }

    public void AddPlantFound(GameObject plant)
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
}
