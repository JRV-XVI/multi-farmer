using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Explorer : MonoBehaviour
{
    // Objetos externos
    private GameManager _gameManager;
    
    // Componentes internos
    [SerializeField] private NavMeshAgent _navMeshAgent;
    
    // Atributos de exploración
    public float scanRange = 2f;
    public LayerMask plantLayer; // Opcional: usar layer para filtrar solo plantas
    
    // Control de navegación
    [SerializeField] private GameObject _currentTarget;
    private List<GameObject> _unexploredPlants;
    private bool _hasArrived = false;
    private bool _isMoving = false;
    private bool _explorationComplete = false;
    
    // Punto de inicio/base para volver después de explorar
    public GameObject homePosition;

    void Awake()
    {
        // Inicializar componentes críticos antes que cualquier otro script
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if(_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent not found in the Explorer!");
        }
        
        // Inicializar lista de plantas no exploradas
        _unexploredPlants = new List<GameObject>();
    }

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

        // Verificar que el NavMeshAgent se inicializó correctamente
        if(_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent sigue siendo null después de Awake()!");
            return;
        }

        // Comenzar explorando todas las plantas
        FindAllUnexploredPlants();
        if (_unexploredPlants.Count > 0)
        {
            MoveToNextPlant();
        }
    }

    void Update()
    {
        // Escanear plantas mientras se mueve
        ScanForPlants();
        
        // Verificar estado de navegación
        CheckNavigationStatus();
        
        // Debug temporal para monitorear estado
        /*if (_currentTarget != null && _navMeshAgent != null && Time.frameCount % 60 == 0)
        {
            Debug.Log($"📊 Explorer Estado: Moving={_isMoving}, HasPath={_navMeshAgent.hasPath}, " +
                     $"RemainingDistance={_navMeshAgent.remainingDistance:F2}, " +
                     $"Target={_currentTarget.name}");
        }*/
    }

    // Encuentra todas las plantas en la escena que aún no han sido exploradas
    private void FindAllUnexploredPlants()
    {
        Plant[] allPlants = FindObjectsByType<Plant>(FindObjectsSortMode.None);
        _unexploredPlants.Clear();
        
        foreach (Plant plant in allPlants)
        {
            if (!plant.hasBeenExplored)
            {
                _unexploredPlants.Add(plant.gameObject);
            }
        }
        
        Debug.Log($"🔍 Explorer encontró {_unexploredPlants.Count} plantas sin explorar");
    }

    // Método mejorado basado en Recolector para detectar llegada
    private void CheckNavigationStatus()
    {
        if (_currentTarget != null && _navMeshAgent != null && _isMoving)
        {
            // Detectar si ya llegó al destino
            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    if (!_hasArrived)
                    {
                        OnArriveAtDestination();
                    }
                }
            }
        }
    }

    private void OnArriveAtDestination()
    {
        _hasArrived = true;
        _isMoving = false;

        Debug.Log($"🎯 Explorer llegó a: {_currentTarget.name}");

        // Escanear en detalle en la ubicación actual
        ScanForPlants();

        // Moverse a la siguiente planta
        MoveToNextPlant();
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
        
        // Remover de la lista de no exploradas
        _unexploredPlants.Remove(plantObject);
    }

    // Navega a la siguiente planta no explorada
    private void MoveToNextPlant()
    {
        // Limpiar plantas ya exploradas de la lista
        _unexploredPlants.RemoveAll(plant => 
        {
            if (plant == null) return true;
            Plant plantComponent = plant.GetComponent<Plant>();
            return plantComponent == null || plantComponent.hasBeenExplored;
        });

        if (_unexploredPlants.Count > 0)
        {
            // Obtener la planta más cercana
            _currentTarget = GetClosestPlant();
            
            if (_currentTarget != null)
            {
                Debug.Log($"🎯 Explorer objetivo seleccionado: {_currentTarget.name}");
                NavigateToTarget(_currentTarget);
            }
        }
        else
        {
            // Exploración completada
            if (!_explorationComplete)
            {
                _explorationComplete = true;
                Debug.Log("✅ Explorer ha completado la exploración de todas las plantas!");
                
                // Volver a la posición inicial si está definida
                if (homePosition != null)
                {
                    _currentTarget = homePosition;
                    NavigateToTarget(homePosition);
                }
            }
        }
    }

    // Obtiene la planta no explorada más cercana
    private GameObject GetClosestPlant()
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        
        foreach (GameObject plant in _unexploredPlants)
        {
            if (plant == null) continue;
            
            float distance = Vector3.Distance(transform.position, plant.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = plant;
            }
        }
        
        return closest;
    }

    // Método de navegación inspirado en Recolector
    private void NavigateToTarget(GameObject target)
    {
        if (_navMeshAgent == null)
        {
            Debug.LogError("❌ NavMeshAgent es null!");
            return;
        }
        
        if (target == null)
        {
            Debug.LogError("❌ Target es null!");
            return;
        }
        
        if (!_navMeshAgent.isOnNavMesh)
        {
            Debug.LogWarning("⚠️ El Explorer no está en el NavMesh!");
            return;
        }

        Vector3 destination;
        
        // Buscar si tiene un componente Plant
        Plant plantComponent = target.GetComponent<Plant>();
        if (plantComponent != null)
        {
            // Buscar el punto de acceso
            Transform accessPoint = plantComponent.puntoDeAcceso;
            destination = accessPoint != null ? accessPoint.position : target.transform.position;
            //Debug.Log($"🌱 Navegando hacia planta con punto de acceso: {accessPoint != null}");
        }
        else
        {
            destination = target.transform.position;
            //Debug.Log($"🏠 Navegando hacia objetivo sin componente Plant");
        }

        //Debug.Log($"🗺️ Destino calculado: {destination}");
        
        bool pathSet = _navMeshAgent.SetDestination(destination);
        if (pathSet)
        {
            //Debug.Log($"🤖 Explorer navegando hacia: {target.name} - Path establecido correctamente");
            _hasArrived = false;
            _isMoving = true;
        }
        else
        {
            //Debug.LogWarning($"⚠️ No se pudo establecer el path hacia: {target.name}");
            _isMoving = false;
        }
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

    // Métodos públicos para control externo
    public bool IsExplorationComplete()
    {
        return _explorationComplete;
    }

    public int GetRemainingPlantsCount()
    {
        return _unexploredPlants.Count;
    }

    public bool IsMoving()
    {
        return _isMoving;
    }

    // Método para forzar reexploración si es necesario
    public void RestartExploration()
    {
        _explorationComplete = false;
        FindAllUnexploredPlants();
        if (_unexploredPlants.Count > 0)
        {
            MoveToNextPlant();
        }
    }
}
