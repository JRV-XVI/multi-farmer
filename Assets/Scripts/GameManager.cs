using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Atributos de los recolectores
    public float recolectorDistanceToCollect = 0.5f;
    public float recolectorMaxCarryWeight = 10f;

    //Atributos de los purgadores
    public float purgatorDistanceToDeposit = 0.5f;
    public float purgatorMaxCarryWeight = 10f;

    //Atributos de las Zones
    public float safeZoneMaxCarryWeight = 50f;
    public float trashoneMaxCarryWeight = 50f;

    //Atributos del explorador
    public float explorerScanRange = 2f;

    // Listas de plantas catalogadas
    private List<GameObject> healthyPlants = new List<GameObject>();
    private List<GameObject> sickPlants = new List<GameObject>();
    private int totalPlantsExplored = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Método llamado por el Explorer para reportar una planta
    public void ReportPlant(GameObject plantObject, Plant plantData)
    {
        if (plantData.hasBeenExplored)
        {
            return; // Ya fue reportada
        }

        plantData.hasBeenExplored = true;
        totalPlantsExplored++;

        if (plantData.IsHealthy())
        {
            healthyPlants.Add(plantObject);
            Debug.Log($"✓ Planta sana detectada (ID: {plantData.id}). Total sanas: {healthyPlants.Count}");
        }
        else if (plantData.IsSick())
        {
            sickPlants.Add(plantObject);
            Debug.Log($"✗ Planta enferma detectada (ID: {plantData.id}). Total enfermas: {sickPlants.Count}");
        }
    }

    // Métodos para que Recolector y Purgador obtengan sus objetivos
    public List<GameObject> GetHealthyPlants()
    {
        return new List<GameObject>(healthyPlants);
    }

    public List<GameObject> GetSickPlants()
    {
        return new List<GameObject>(sickPlants);
    }

    public void RemovePlantFromList(GameObject plant)
    {
        healthyPlants.Remove(plant);
        sickPlants.Remove(plant);
    }

    public int GetTotalPlantsExplored()
    {
        return totalPlantsExplored;
    }
}
