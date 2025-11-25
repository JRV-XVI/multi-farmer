using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject[] _plantList;
    public DropZoneManager dropZoneManager; // Referencia al gestor de zonas

    public Vector2Int mapSize;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _plantList = GameObject.FindGameObjectsWithTag("Plant");
        
        // Buscar el DropZoneManager si no está asignado
        if (dropZoneManager == null)
        {
            dropZoneManager = FindFirstObjectByType<DropZoneManager>();
            if (dropZoneManager == null)
            {
                Debug.LogWarning("Manager: No se encontró DropZoneManager en la escena");
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetFreePlant()
    {
        // Limpiar referencias null primero
        _plantList = _plantList.Where(p => p != null).ToArray();
        
        foreach (GameObject plant in _plantList)
        {
            Plant plantComp = plant.GetComponent<Plant>();
            if (plantComp == null)
            {
                Debug.LogError("El objeto " + plant.name + " no tiene componente Plant!");
                continue; // saltar este objeto
            }

            // Verificar que la planta tenga tomates disponibles y no esté ocupada
            if (!plantComp.isTaken && plantComp.GetAvailableTomatoCount() > 0)
            {
                plantComp.isTaken = true;
                Debug.Log($"Planta {plant.name} asignada. Tomates disponibles: {plantComp.GetAvailableTomatoCount()}");
                return plant;
            }
        }

        Debug.Log("No hay plantas libres con tomates disponibles");
        return null;
    }

}
