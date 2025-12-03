using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Explorer : MonoBehaviour
{
    // Objetos externos
    private Manager _manager;
    
    // Componentes internos
    [SerializeField] private NavMeshAgent _navMeshAgent;
    
    // Atributos de exploración
    public float scanRange = 2f;
    public LayerMask plantLayer; // Opcional: usar layer para filtrar solo plantas
    
    // Control de navegación
    [SerializeField] private GameObject _currentTarget;
    private List<GameObject> _unexploredPlants;
    private List<GameObject> _exploredPlants; // Lista de plantas exploradas para enviar al Manager
    private bool _hasArrived = false;
    private bool _isMoving = false;
    private bool _explorationComplete = false;
    private bool _reportSent = false;
    
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
        
        // Inicializar listas
        _unexploredPlants = new List<GameObject>();
        _exploredPlants = new List<GameObject>();
    }

    void Start()
    {
        _manager = GameObject.FindWithTag("BotManager").GetComponent<Manager>();
        if (_manager == null)
        {
            Debug.LogError("Manager not found in the scene! Make sure there's a GameObject tagged 'BotManager'");
        }

        // Verificar que el NavMeshAgent se inicializó correctamente
        if(_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent sigue siendo null después de Awake()!");
            return;
        }

        // NUEVO: No empezar automáticamente, esperar a que Manager llame StartExploration()
        Debug.Log("🤖 Explorer listo y esperando orden del Manager para comenzar exploración...");
    }

    void Update()
    {
        // Escanear plantas mientras se mueve
        ScanForPlants();
        
        // Verificar estado de navegación
        CheckNavigationStatus();
    }

    // NUEVO MÉTODO PÚBLICO: El Manager llama este método para iniciar la exploración
    public void StartExploration(List<GameObject> plantsToExplore)
    {
        if (plantsToExplore == null || plantsToExplore.Count == 0)
        {
            Debug.LogWarning("⚠️ Explorer recibió una lista vacía o null del Manager!");
            return;
        }

        _unexploredPlants.Clear();
        _unexploredPlants.AddRange(plantsToExplore);

        // OPTIMIZACIÓN: Ordenar plantas por posición para crear un recorrido más eficiente
        // Ordena primero por X (columnas), luego por Z (filas)
        _unexploredPlants.Sort((a, b) =>
        {
            if (a == null || b == null) return 0;
            
            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;
            
            // Comparar primero por coordenada X (izquierda a derecha)
            int compareX = posA.x.CompareTo(posB.x);
            if (compareX != 0) return compareX;
            
            // Si están en la misma X, comparar por Z (adelante a atrás)
            return posA.z.CompareTo(posB.z);
        });
        
        Debug.Log($"🔍 Explorer recibió {_unexploredPlants.Count} plantas del Manager (ordenadas por posición)");
        
        // Comenzar la exploración
        if (_unexploredPlants.Count > 0)
        {
            MoveToNextPlant();
        }
    }

    // [MÉTODO LEGACY - Mantener como backup]
    // Encuentra todas las plantas en la escena de forma independiente
    private void FindAllUnexploredPlants()
    {
        Plant[] allPlants = FindObjectsByType<Plant>(FindObjectsSortMode.None);
        _unexploredPlants.Clear();
        
        foreach (Plant plant in allPlants)
        {
            _unexploredPlants.Add(plant.gameObject);
        }
        
        // OPTIMIZACIÓN: Ordenar plantas por posición para crear un recorrido más eficiente
        // Ordena primero por X (columnas), luego por Z (filas)
        _unexploredPlants.Sort((a, b) =>
        {
            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;
            
            // Comparar primero por coordenada X (izquierda a derecha)
            int compareX = posA.x.CompareTo(posB.x);
            if (compareX != 0) return compareX;
            
            // Si están en la misma X, comparar por Z (adelante a atrás)
            return posA.z.CompareTo(posB.z);
        });
        
        Debug.Log($"🔍 Explorer encontró {_unexploredPlants.Count} plantas para explorar (ordenadas por posición)");
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
            
            if (plant != null)
            {
                // Añadir a la lista de exploradas si no está ya
                if (!_exploredPlants.Contains(col.gameObject) && !_unexploredPlants.Contains(col.gameObject))
                {
                    InspectPlant(col.gameObject);
                }
                else if (_unexploredPlants.Contains(col.gameObject))
                {
                    InspectPlant(col.gameObject);
                }
            }
        }
    }

    // Inspecciona la planta y la añade a la lista de exploradas
    private void InspectPlant(GameObject plantObject)
    {
        if (!_exploredPlants.Contains(plantObject))
        {
            _exploredPlants.Add(plantObject);
            Debug.Log($"🔍 Explorer inspeccionó planta: {plantObject.name} (Total: {_exploredPlants.Count})");
        }
        
        // Remover de la lista de no exploradas
        _unexploredPlants.Remove(plantObject);
    }

    // Navega a la siguiente planta no explorada
    private void MoveToNextPlant()
    {
        // Limpiar plantas null de la lista
        _unexploredPlants.RemoveAll(plant => plant == null);

        if (_unexploredPlants.Count > 0)
        {
            // OPTIMIZACIÓN: Tomar la primera planta de la lista en lugar de la más cercana
            // Esto evita el zigzag constante y crea un patrón más eficiente
            _currentTarget = _unexploredPlants[0];
            
            if (_currentTarget != null)
            {
                Debug.Log($"🎯 Explorer objetivo seleccionado: {_currentTarget.name} (Quedan {_unexploredPlants.Count})");
                NavigateToTarget(_currentTarget);
            }
        }
        else
        {
            // Exploración completada
            if (!_explorationComplete)
            {
                _explorationComplete = true;
                Debug.Log($"✅ Explorer ha completado la exploración! Total plantas encontradas: {_exploredPlants.Count}");
                
                // Enviar reporte al Manager
                SendReportToManager();
                
                // Volver a la posición inicial si está definida
                if (homePosition != null)
                {
                    _currentTarget = homePosition;
                    NavigateToTarget(homePosition);
                }
            }
        }
    }

    // Envía la lista completa de plantas exploradas al Manager
    private void SendReportToManager()
    {
        if (_reportSent)
        {
            Debug.LogWarning("⚠️ El reporte ya fue enviado al Manager");
            return;
        }

        if (_manager == null)
        {
            Debug.LogError("❌ No se puede enviar reporte: Manager no encontrado!");
            return;
        }

        if (_exploredPlants.Count == 0)
        {
            Debug.LogWarning("⚠️ No hay plantas exploradas para reportar");
            return;
        }

        Debug.Log($"📤 Explorer enviando reporte de {_exploredPlants.Count} plantas al Manager...");
        
        // Enviar la lista completa de plantas al Manager
        _manager.AnalizePlants(_exploredPlants);
        
        _reportSent = true;
        Debug.Log("✅ Reporte enviado exitosamente al Manager!");
    }

    // [MÉTODO DE UTILIDAD - NO USADO ACTUALMENTE]
    // Obtiene la planta no explorada más cercana
    // Nota: Este método causaba zigzag. Ahora se usa orden secuencial en MoveToNextPlant()
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
        }
        else
        {
            destination = target.transform.position;
        }
        
        bool pathSet = _navMeshAgent.SetDestination(destination);
        if (pathSet)
        {
            _hasArrived = false;
            _isMoving = true;
        }
        else
        {
            Debug.LogWarning($"⚠️ No se pudo establecer el path hacia: {target.name}");
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
        
        if (plant != null && !_exploredPlants.Contains(other.gameObject))
        {
            InspectPlant(other.gameObject);
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

    public int GetExploredPlantsCount()
    {
        return _exploredPlants.Count;
    }

    public bool IsMoving()
    {
        return _isMoving;
    }

    public bool HasSentReport()
    {
        return _reportSent;
    }

    // Método para forzar reexploración si es necesario
    public void RestartExploration()
    {
        _explorationComplete = false;
        _reportSent = false;
        _exploredPlants.Clear();
        FindAllUnexploredPlants();
        if (_unexploredPlants.Count > 0)
        {
            MoveToNextPlant();
        }
    }

    // Método para forzar envío de reporte manual (útil para testing)
    public void ForceReportToManager()
    {
        SendReportToManager();
    }
}
