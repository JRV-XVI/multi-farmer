using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

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

    //Atributos de plantas en escena
    public float plantWeightMin = 5f; //No asignada aun
    public float tomatosWeightMin = 2f; //No asignada aun

    void Start()
    {
        // El flujo ahora es: Explorer escanea -> reporta a Manager -> Manager asigna bots
        Debug.Log("GameManager iniciado. Las plantas se inicializarán cuando el Explorer las escanee.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<GameObject> FindPlantsInScene()
    {
        GameObject[] plantsArray = GameObject.FindGameObjectsWithTag("Plant");
        
        if(plantsArray == null || plantsArray.Length == 0)
        {
            Debug.LogWarning("⚠️ No se encontraron GameObjects con tag 'Plant'");
            
            // Buscar alternativamente por componente Plant
            Plant[] plantComponents = FindObjectsOfType<Plant>();
            if(plantComponents != null && plantComponents.Length > 0)
            {
                Debug.Log($"🔍 Se encontraron {plantComponents.Length} objetos con componente Plant");
                return FilterValidPlants(plantComponents);
            }
            
            return new List<GameObject>();
        }
        
        Debug.Log($"✅ Se encontraron {plantsArray.Length} plantas con tag 'Plant'");
        return FilterValidPlants(plantsArray);
    }

    // Método para filtrar y validar plantas, devuelve solo plantas válidas
    List<GameObject> FilterValidPlants(GameObject[] plantsArray)
    {
        List<GameObject> validPlants = new List<GameObject>();
        
        int nullCount = 0;
        int missingComponentCount = 0;
        int alreadyCollectedCount = 0;
        
        foreach(GameObject plant in plantsArray)
        {
            try
            {
                // Verificar que el GameObject no sea null y no esté destruido
                if(plant == null || !plant)
                {
                    nullCount++;
                    continue;
                }
                
                Plant plantComponent = plant.GetComponent<Plant>();
                if (plantComponent == null)
                {
                    missingComponentCount++;
                    continue;
                }
                
                if (plantComponent.isCollected)
                {
                    alreadyCollectedCount++;
                    continue;
                }
                
                // Solo agregar plantas válidas y no recolectadas
                validPlants.Add(plant);
            }
            catch(System.Exception ex)
            {
                Debug.LogWarning($"⚠️ Error validando planta: {ex.Message}");
                nullCount++;
                continue;
            }
        }
        
        // Log consolidado de estadísticas
        Debug.Log($"📊 Plantas procesadas: {plantsArray.Length} total, {validPlants.Count} válidas");
        
        if(nullCount > 0)
            Debug.LogWarning($"⚠️ Plantas null/destruidas: {nullCount}");
        
        if(missingComponentCount > 0)
            Debug.LogWarning($"⚠️ Plantas sin componente Plant: {missingComponentCount}");
            
        if(alreadyCollectedCount > 0)
            Debug.Log($"ℹ️ Plantas ya recolectadas: {alreadyCollectedCount}");
        
        return validPlants;
    }

    // Sobrecarga para cuando se pasa array de componentes Plant
    List<GameObject> FilterValidPlants(Plant[] plantComponents)
    {
        List<GameObject> validPlants = new List<GameObject>();
        
        int nullCount = 0;
        int alreadyCollectedCount = 0;
        
        foreach(Plant plant in plantComponents)
        {
            try
            {
                if(plant == null || plant.gameObject == null || !plant.gameObject)
                {
                    nullCount++;
                    continue;
                }
                
                if (plant.isCollected)
                {
                    alreadyCollectedCount++;
                    continue;
                }
                
                validPlants.Add(plant.gameObject);
            }
            catch(System.Exception ex)
            {
                Debug.LogWarning($"⚠️ Error validando componente Plant: {ex.Message}");
                nullCount++;
                continue;
            }
        }
        
        Debug.Log($"📊 Componentes procesados: {plantComponents.Length} total, {validPlants.Count} válidos");
        
        if(nullCount > 0)
            Debug.LogWarning($"⚠️ Componentes null: {nullCount}");
            
        if(alreadyCollectedCount > 0)
            Debug.Log($"ℹ️ Plantas ya recolectadas: {alreadyCollectedCount}");
        
        return validPlants;
    }

    // Método público para refrescar la lista de plantas disponibles
    public void RefreshPlantsForRecolector()
    {
        GameObject recolector = GameObject.FindWithTag("BotRecolector");
        if(recolector != null)
        {
            Recolector recolectorComponent = recolector.GetComponent<Recolector>();
            if(recolectorComponent != null)
            {
                List<GameObject> freshPlants = FindPlantsInScene();
                recolectorComponent.InitializePlantList(freshPlants);
            }
        }
    }

    public void RefreshPlantsForPurgator()
    {
        GameObject purgator = GameObject.FindWithTag("BotPurgator");
        if(purgator != null)
        {
            Purgator purgatorComponent = purgator.GetComponent<Purgator>();
            if(purgatorComponent != null)
            {
                List<GameObject> freshPlants = FindPlantsInScene();
                purgatorComponent.InitializePlantList(freshPlants);
            }
        }
    }

    // Método para obtener plantas válidas disponibles (sin inicializar recolector)
    public List<GameObject> GetValidPlants()
    {
        return FindPlantsInScene();
    }


    public void StartPlantValuesRandomly(Plant plant)
    {
        // Generar peso aleatorio de la planta y tomates
        plant.plantWeight = Random.Range(plantWeightMin, plantWeightMin + 15f);
        plant.tomatosWeight = Random.Range(tomatosWeightMin, tomatosWeightMin + 3f);

        // 30% de probabilidad de que la planta esté enferma
        // Si random >= 0.7, la planta está enferma (30% probabilidad)
        // Si random < 0.7, la planta está sana (70% probabilidad)
        float randomValue = Random.Range(0f, 1f);
        
        if (randomValue >= 0.7f)
        {
            // Planta enferma: al menos una parte está enferma
            plant.plantIsSick = Random.Range(0f, 1f) > 0.5f;
            plant.tomatosAreSick = Random.Range(0f, 1f) > 0.5f;
            plant.leavesAreSick = Random.Range(0f, 1f) > 0.5f;
            
            // Asegurar que al menos una parte esté enferma
            if (!plant.plantIsSick && !plant.tomatosAreSick && !plant.leavesAreSick)
            {
                int randomPart = Random.Range(0, 3);
                if (randomPart == 0) plant.plantIsSick = true;
                else if (randomPart == 1) plant.tomatosAreSick = true;
                else plant.leavesAreSick = true;
            }
        }
        else
        {
            // Planta sana
            plant.plantIsSick = false;
            plant.tomatosAreSick = false;
            plant.leavesAreSick = false;
        }
    }

}
