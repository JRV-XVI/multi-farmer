using UnityEngine;

public class Explorer : MonoBehaviour
{
    private GameManager _gameManager;
    public float scanRange = 2f;
    public LayerMask plantLayer; // Opcional: usar layer para filtrar solo plantas

    void Start()
    {
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
        else
        {
            scanRange = _gameManager.explorerScanRange;
        }
    }

    void Update()
    {
        ScanForPlants();
    }

    // Escanea plantas cercanas usando OverlapSphere
    private void ScanForPlants()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, scanRange);

        foreach (Collider col in nearbyObjects)
        {
            Plant plant = col.GetComponent<Plant>();
            
            if (plant != null && !plant.hasBeenExplored)
            {
                InspectPlant(col.gameObject, plant);
            }
        }
    }

    // Inspecciona y reporta la planta al GameManager
    private void InspectPlant(GameObject plantObject, Plant plantData)
    {
        Debug.Log($"🔍 Explorer inspeccionando planta ID: {plantData.id}");
        
        // Reportar al GameManager
        _gameManager.ReportPlant(plantObject, plantData);
    }

    // Visualización del rango de escaneo en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, scanRange);
    }

    // Alternativa: Usar triggers en lugar de OverlapSphere
    private void OnTriggerEnter(Collider other)
    {
        Plant plant = other.GetComponent<Plant>();
        
        if (plant != null && !plant.hasBeenExplored)
        {
            InspectPlant(other.gameObject, plant);
        }
    }
}
