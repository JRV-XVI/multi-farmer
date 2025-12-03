using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [Header("Recolectores")]
    public float recolectorDistanceToCollect = 0.5f;
    public float recolectorMaxCarryWeight = 10f;

    [Header("Purgadores")]
    public float purgatorDistanceToDeposit = 0.5f;
    public float purgatorMaxCarryWeight = 10f;

    [Header("Zones")]
    public float safeZoneMaxCarryWeight = 50f;
    public float trashoneMaxCarryWeight = 50f;

    [Header("Plantas en escena")]
    public float plantMinWeight = 5f;
    public float plantMaxWeight = 20f;
    public float tomatoesMinWeight = 2f;
    public float tomatoesMaxWeight = 5f;

    [Header("Margen de enfermedad de planta")]
    [Tooltip("Máximo porcentaje de enfermedad para considerar saludable (0.0 - 1.0)")]
    public float plantMaxStemSickPercentage = 0.3f;
    public float plantMaxTomatoesSickPercentage = 0.3f;
    public float plantMaxLeavesSickPercentage = 0.3f;

    [Tooltip("Probabilidad de enfermarse por cosecha (0.0 - 1.0)")]
    public float plantStemSickChancePerHarvest = 0.1f;
    public float plantTomatoesSickChancePerHarvest = 0.1f;
    public float plantLeavesSickChancePerHarvest = 0.1f;

    void Start()
    {
        GameObject recolector = GameObject.FindWithTag("BotRecolector");
        if(recolector == null)
        {
            Debug.LogError("Recolector not found in the scene!!");
            return;
        }

        GameObject purgator = GameObject.FindWithTag("BotPurgator");
        if(purgator == null)
        {
            Debug.LogError("Purgator not found in the scene!!");
            return;
        }

        List<GameObject> plantsFound = FindPlantsInScene();
        
        if(plantsFound == null)
        {
            Debug.LogError("❌ FindPlantsInScene retornó null!");
            plantsFound = new List<GameObject>();
        }
        
        if(plantsFound.Count > 0)
        {
            Debug.Log($"🌱 Se encontraron {plantsFound.Count} plantas en la escena");
            
            // Validar el componente Recolector antes de llamar el método
            Recolector recolectorComponent = recolector.GetComponent<Recolector>();
            if(recolectorComponent == null)
            {
                Debug.LogError("❌ El GameObject BotRecolector no tiene el componente Recolector!");
                return;
            }
            //recolectorComponent.InitializePlantList(plantsFound);
            

            Purgator purgatorComponent = purgator.GetComponent<Purgator>();
            if(purgatorComponent == null)
            {
                Debug.LogError("❌ El GameObject BotPurgator no tiene el componente Purgator!");
                return;
            }
            //purgatorComponent.InitializePlantList(plantsFound);

        }
        else
        {
            Debug.LogWarning("⚠️ No se encontraron plantas en la escena!");
            // Inicializar con lista vacía para evitar null reference
            Recolector recolectorComponent = recolector.GetComponent<Recolector>();
            if(recolectorComponent != null)
            {
                recolectorComponent.InitializePlantList(new List<GameObject>());
            }
        }


        /// Comenta esto cuando tengas el bot Explorador.
        GameObject.FindGameObjectWithTag("BotManager").GetComponent<Manager>().AnalizePlants(plantsFound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> FindPlantsInScene()
    {
        GameObject[] plantsArray = GameObject.FindGameObjectsWithTag("Plant");
        
        if(plantsArray == null || plantsArray.Length == 0)
        {
            Debug.LogWarning("⚠️ No se encontraron GameObjects con tag 'Plant'");
            
            // Buscar alternativamente por componente Plant
            Plant[] plantComponents = FindObjectsByType<Plant>(FindObjectsSortMode.None);
            
            if (plantComponents != null && plantComponents.Length > 0)
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





    //Esto es lo unico que se mantendra del GameManager original
    public void StartPlantValuesRandomly(Plant plant)
    {
        //Asigancion de valroes de forma random
        plant.plantWeight = Random.Range(plantMinWeight, plantMaxWeight);
        plant.tomatosWeight = Random.Range(tomatoesMinWeight, tomatoesMaxWeight);

        //Valoeres de enfermedad
        if (Random.Range(0f, 1f) < plantStemSickChancePerHarvest)
            plant.stemSickPercentage = Random.Range(plantMaxStemSickPercentage, 1f);
        else
            plant.stemSickPercentage = Random.Range(0f, plantMaxStemSickPercentage);

        if (Random.Range(0f, 1f) < plantTomatoesSickChancePerHarvest)
            plant.tomatoesSickPercentage = Random.Range(plantMaxTomatoesSickPercentage, 1f);
        else
            plant.tomatoesSickPercentage = Random.Range(0f, plantMaxTomatoesSickPercentage);

        if (Random.Range(0f, 1f) < plantLeavesSickChancePerHarvest)
            plant.leavesSickPercentage = Random.Range(plantMaxLeavesSickPercentage, 1f);
        else
            plant.leavesSickPercentage = Random.Range(0f, plantMaxLeavesSickPercentage);



        //Y tambien falta asignar la imagen
    }

}
