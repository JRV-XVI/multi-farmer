using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Purgator : MonoBehaviour
{
    //Objetos externos
    private GameManager _gameManager;

    //Componentes internos
    [SerializeField] private NavMeshAgent _navMeshAgent;
    
    //Atributos internos
    [SerializeField] private GameObject _currentTrack;
    private List<GameObject> _trackList;

    [SerializeField] private float _maxCarryWeight;
    [SerializeField] private float _currentCarryWeight;

    // Posición de inicio para retornar cuando no hay plantas asignadas
    private Vector3 _homePosition;

        //Control de navegación
    private bool _hasArrived = false;
    private bool _isMoving = false;

    

    
    void Awake()
    {
        // Inicializar componentes críticos antes que cualquier otro script
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if(_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent not found in the Purgator!!");
        }
        else
        {
            // Configurar NavMeshAgent para evitar colisiones entre múltiples bots
            _navMeshAgent.stoppingDistance = 1.5f; // Distancia de parada aumentada para evitar solapamiento
            _navMeshAgent.radius = 0.5f; // Radio del agente para cálculo de colisiones
            _navMeshAgent.avoidancePriority = Random.Range(40, 60); // Prioridad aleatoria para romper empates
        }
        
        // Inicializar lista de seguimiento
        _trackList = new List<GameObject>();
    }


    void Start()
    {
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        if(_gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!!");
        }

        // Guardar posición inicial para retornar cuando esté idle
        _homePosition = transform.position;

        // Verificar que el NavMeshAgent se inicializó correctamente
        if(_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent sigue siendo null después de Awake()!");
            return;
        }

        _maxCarryWeight = _gameManager.purgatorMaxCarryWeight;
        _currentCarryWeight = 0f;

        
    }

    // Update is called once per frame
    void Update()
    {
        CheckNavigationStatus();
        
        /*
        // Debug temporal para monitorear estado
        if (_currentTrack != null && _navMeshAgent != null && Time.frameCount % 60 == 0) // Solo cada segundo aprox
        {
            Debug.Log($"📊 Estado: Moving={_isMoving}, HasPath={_navMeshAgent.hasPath}, " +
                     $"RemainingDistance={_navMeshAgent.remainingDistance:F2}, " +
                     $"Target={_currentTrack.name}");
        }
        */
    }

    // Método mejorado basado en RobotPrueba para detectar llegada
    private void CheckNavigationStatus()
    {
        if (_navMeshAgent != null && _isMoving)
        {
            // Detectar si ya llegó al destino (lógica de RobotPrueba)
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

        // Si _currentTrack es null, significa que estamos retornando a home
        if (_currentTrack == null)
        {
            Debug.Log("🏡 Purgador llegó a su posición inicial (idle)");
            return; // Quedarse idle en home
        }

        if (_currentTrack.CompareTag("TrashZoneInteract"))
        {
            DownloadWeight();
        }
        else
        {
            PurgePlant(_currentTrack);
        }
    }


    private void TrackNextObject()
    {
        if (_trackList.Count > 0 && _currentCarryWeight < _maxCarryWeight)
        {
            _currentTrack = _trackList[0];
            //Debug.Log($"🎯 Objetivo seleccionado: {_currentTrack.name}");
            NavigateToTarget(_currentTrack);
        }
        else if (_currentCarryWeight > 0)
        {
            // Tiene peso que depositar: Buscar la TrashZone más cercana dinámicamente
            _currentTrack = GetNearestTrashZone();
            if (_currentTrack != null)
            {
                Debug.Log($"🗑️ Purgador dirigiéndose a zona de basura más cercana: {_currentTrack.name}");
                NavigateToTarget(_currentTrack);
            }
            else
            {
                Debug.LogError("⚠️ No se encontraron TrashZones con tag 'TrashZoneInteract'. Asegúrate de que existan zonas con este tag.");
                _isMoving = false;
            }
        }
        else
        {
            // Sin plantas asignadas y sin peso: Retornar a posición inicial
            Debug.Log($"🏡 Purgador retornando a posición inicial (sin plantas asignadas)");
            _currentTrack = null;
            NavigateToPosition(_homePosition);
        }
    }

    // NUEVO: Encuentra la TrashZone más cercana al Purgador
    private GameObject GetNearestTrashZone()
    {
        GameObject[] trashZones = GameObject.FindGameObjectsWithTag("TrashZoneInteract");
        
        if (trashZones.Length == 0)
        {
            Debug.LogWarning("⚠️ No se encontraron TrashZones con tag 'TrashZoneInteract'");
            return null;
        }

        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject zone in trashZones)
        {
            if (zone == null) continue;

            float distance = Vector3.Distance(transform.position, zone.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = zone;
            }
        }

        return nearest;
    }

    // Nuevo método inspirado en RobotPrueba
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
            Debug.LogWarning("⚠️ El purgador no está en el NavMesh!");
            return;
        }

        Vector3 destination;
        
        //checar que tipo de objeto es el target
        if (target.tag == "Plant")
        {
            // Buscar el hijo "PuntoInteraccion" directamente
            Transform puntoInteraccion = target.transform.Find("PuntoInteraccion");
            if (puntoInteraccion != null)
            {
                destination = puntoInteraccion.position;
                
                // NUEVO: Añadir pequeño offset aleatorio para evitar colisiones entre múltiples bots
                Vector3 randomOffset = new Vector3(
                    Random.Range(-0.3f, 0.3f),
                    0f,
                    Random.Range(-0.3f, 0.3f)
                );
                destination += randomOffset;
                
                //Debug.Log($"🌱 Navegando hacia PuntoInteraccion de {target.name}");
            }
            else
            {
                // Fallback: usar puntoDeAcceso del componente Plant
                Plant plantComponent = target.GetComponent<Plant>();
                Transform accessPoint = plantComponent != null ? plantComponent.puntoDeAcceso : null;
                destination = accessPoint != null ? accessPoint.position : target.transform.position;
                //Debug.LogWarning($"⚠️ No se encontró hijo 'PuntoInteraccion' en {target.name}, usando fallback");
            }
        }
        else if (target.tag == "Zone" && target.GetComponent<Zone>().zoneType == ZoneType.TrashZone)
        {
            // Buscar el punto de acceso
            Transform accessPoint = target.GetComponent<Zone>().puntoDeAcceso;
            destination = accessPoint != null ? accessPoint.position : target.transform.position;
            
            // NUEVO: Offset para zonas también
            Vector3 randomOffset = new Vector3(
                Random.Range(-0.5f, 0.5f),
                0f,
                Random.Range(-0.5f, 0.5f)
            );
            destination += randomOffset;
            
            //Debug.Log($"🗑️ Navegando hacia TrashZone");
        }
        else
        {
            Debug.LogWarning($"⚠️ Target con tag inesperado: {target.tag}. Usando posición directa.");
            destination = target.transform.position;
        }

        //Debug.Log($"🗺️ Destino calculado: {destination}");
        
        bool pathSet = _navMeshAgent.SetDestination(destination);
        if (pathSet)
        {
            //Debug.Log($"🤖 Navegando hacia: {target.name} - Path establecido correctamente");
            _hasArrived = false;
            _isMoving = true;
        }
        else
        {
            Debug.LogWarning($"⚠️ No se pudo establecer el path hacia: {target.name}");
            _isMoving = false;
        }
    }

    // Navegar directamente a una posición (para retornar a home)
    private void NavigateToPosition(Vector3 position)
    {
        if (_navMeshAgent == null)
        {
            Debug.LogError("❌ NavMeshAgent es null!");
            return;
        }
        
        if (!_navMeshAgent.isOnNavMesh)
        {
            Debug.LogWarning("⚠️ El purgador no está en el NavMesh!");
            return;
        }

        bool pathSet = _navMeshAgent.SetDestination(position);
        if (pathSet)
        {
            _hasArrived = false;
            _isMoving = true;
        }
        else
        {
            Debug.LogWarning($"⚠️ No se pudo establecer el path hacia la posición: {position}");
            _isMoving = false;
        }
    }


    private void PurgePlant(GameObject targetObject)
    {
        // Buscar el hijo "PuntoInteraccion" para orientación
        Transform puntoInteraccion = plant.transform.Find("PuntoInteraccion");
        
        if (puntoInteraccion != null)
        {
            // Orientarse hacia el PuntoInteraccion
            transform.rotation = puntoInteraccion.rotation;
        }
        else
        {
            // Fallback: usar puntoDeAcceso del componente o LookAt
            Plant plantComponent = plant.GetComponent<Plant>();
            if (plantComponent != null && plantComponent.puntoDeAcceso != null)
            {
                transform.rotation = plantComponent.puntoDeAcceso.rotation;
            }
            else
            {
                transform.LookAt(plant.transform);
            }
        }

        // Purgar la planta
        float plantWeight = plant.GetComponent<Plant>().PurgePlant();
        _currentCarryWeight += plantWeight;
        
        _trackList.Remove(targetObject);

        //Debug.Log($"🦠 Purgado: {plant.name}. Peso actual: {_currentCarryWeight}");

        TrackNextObject();
    }

    private void DownloadWeight()
    {
        if (_currentTrack == null)
        {
            Debug.LogError("⚠️ No se puede descargar peso: _currentTrack es null");
            return;
        }

        Zone trashZone = _currentTrack.GetComponent<Zone>();
        if (trashZone == null)
        {
            Debug.LogError("⚠️ El objeto actual no tiene componente Zone");
            return;
        }

        float exceededWeight = trashZone.DepositeThings(_currentCarryWeight);
        _currentCarryWeight = exceededWeight;

        Debug.Log($"📦 Descargado en zona segura. Peso restante: {_currentCarryWeight}");

        TrackNextObject();
    }

    // Método público para agregar plantas a la lista de seguimiento
    public void AddPlantToTrack(GameObject plant)
    {
        if (plant != null && !_trackList.Contains(plant))
        {
            // Buscar el PuntoInteraccion en el plant
            Transform puntoInteraccion = plant.transform.Find("PuntoInteraccion");
            GameObject targetObject = (puntoInteraccion != null) ? puntoInteraccion.gameObject : plant;
            
            _trackList.Add(targetObject);
            Debug.Log($"📋 Purgador {gameObject.name} recibió planta: {plant.name}, usando destino: {targetObject.name}. Total en lista: {_trackList.Count}");
            
            // NUEVO: Si el bot está idle (no moviéndose), iniciar movimiento inmediatamente
            if (!_isMoving && _currentTrack == null)
            {
                Debug.Log($"🚀 Purgador {gameObject.name} disponible, iniciando movimiento...");
                TrackNextObject();
            }
        }
    }

    // Método público para obtener el estado del recolector
    public bool IsMoving()
    {
        return _isMoving;
    }

    public float GetCurrentWeight()
    {
        return _currentCarryWeight;
    }

    // Método para verificar si puede recolectar más
    public bool CanCarryMore()
    {
        return _currentCarryWeight < _maxCarryWeight;
    }

    // Método para inicializar plantas desde el GameManager
    // Recibe una lista ya filtrada y optimizada por el GameManager
    public void InitializePlantList(List<GameObject> validPlants)
    {
        if(validPlants == null)
        {
            Debug.LogError("❌ La lista de plantas es null!");
            return;
        }
        
        // Verificar que el NavMeshAgent esté listo
        if(_navMeshAgent == null)
        {
            Debug.LogError("❌ NavMeshAgent no está inicializado al llamar InitializePlantList!");
            return;
        }
        
        _trackList.Clear();
        _trackList.AddRange(validPlants);
        
        Debug.Log($"🌱 Purgador {this.name} inicializado con {_trackList.Count} plantas válidas");
        
        // Comenzar con el primer objetivo si hay plantas
        if (_trackList.Count > 0)
        {
            TrackNextObject();
        }
        else
        {
            Debug.LogWarning("⚠️ No hay plantas disponibles para recolectar");
        }
    }

    // Método para limpiar plantas ya recolectadas de la lista
    public void RefreshAvailablePlants()
    {
        int originalCount = _trackList.Count;
        _trackList.RemoveAll(plant => 
        {
            if (plant == null) return true;
            Plant plantComponent = plant.GetComponent<Plant>();
            return plantComponent == null || plantComponent.isCollected;
        });
        
        int removedCount = originalCount - _trackList.Count;
        if(removedCount > 0)
        {
            Debug.Log($"🔄 Limpieza completada: {removedCount} plantas removidas, {_trackList.Count} restantes");
        }
    }
}
