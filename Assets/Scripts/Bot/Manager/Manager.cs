using Unity.VisualScripting;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject[] _plantList;
    public int numFreePlants;

    public Vector2Int mapSize;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _plantList = GameObject.FindGameObjectsWithTag("Plant");

        numFreePlants = _plantList.Length;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetFreePlant()
    {
        foreach (GameObject plant in _plantList)
        {
            Plant plantComp = plant.GetComponent<Plant>();
            if (plantComp == null)
            {
                Debug.LogError("El objeto " + plant.name + " no tiene componente Plant!");
                continue; // saltar este objeto
            }

            if (!plantComp.isTaken)
            {
                plantComp.isTaken = true;
                numFreePlants--;
                return plant;
                
            }
        }

        Debug.LogWarning("No hubo plantas libres. Cantidad: " + numFreePlants);
        return null;
    }

}
