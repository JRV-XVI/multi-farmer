using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Recolector : MonoBehaviour
{
    //Objetos externos
    private GameManager _gameManager;

    public GameObject safeZone;

    //Componentes internos
    [SerializeField] private NavMeshAgent _navMeshAgent;
    
    //Atributos internos
    [SerializeField] private GameObject _currentTrack;
    private List<GameObject> _trackList;

    [SerializeField] private float _maxCarryWeight;
    [SerializeField] private float _currentCarryWeight;
    [SerializeField] private int _currentTomatosCollected;


        //Control de navegación
    private bool _hasArrived = false;
    private bool _isMoving = false;

    

    
    void Awake()
    {
        // Inicializar componentes críticos antes que cualquier otro script
        _navMeshAgent = GetComponent<NavMeshAgent>();
        if(_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent not found in the Recolector!!");
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

        // Verificar que el NavMeshAgent se inicializó correctamente
        if(_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent sigue siendo null después de Awake()!");
            return;
        }

        _maxCarryWeight = _gameManager.recolectorMaxCarryWeight;
        _currentCarryWeight = 0f;

        
    }

    // Update is called once per frame
    void Update()
    {
        CheckNavigationStatus();
        
        // Debug temporal para monitorear estado
        if (_currentTrack != null && _navMeshAgent != null && Time.frameCount % 60 == 0) // Solo cada segundo aprox
        {
            Debug.Log($"📊 Estado: Moving={_isMoving}, HasPath={_navMeshAgent.hasPath}, " +
                     $"RemainingDistance={_navMeshAgent.remainingDistance:F2}, " +
                     $"Target={_currentTrack.name}");
        }
    }

    // Método mejorado basado en RobotPrueba para detectar llegada
    private void CheckNavigationStatus()
    {
        if (_currentTrack != null && _navMeshAgent != null && _isMoving)
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

        if (_currentTrack == safeZone)
        {
            DownloadWeight();
        }
        else
        {
            ColectPlant(_currentTrack);
        }
    }


    private void TrackNextObject()
    {
        if (_trackList.Count > 0 && _currentCarryWeight < _maxCarryWeight)
        {
            _currentTrack = _trackList[0];
            Debug.Log($"🎯 Objetivo seleccionado: {_currentTrack.name}");
        }
        else
        {
            _currentTrack = safeZone;
            Debug.Log($"🏠 Dirigiéndose a zona segura: {(_currentTrack != null ? _currentTrack.name : "NULL")}");
        }

        // Navegar al nuevo objetivo si existe
        if (_currentTrack != null)
        {
            Debug.Log($"🚀 Iniciando navegación hacia: {_currentTrack.name}");
            NavigateToTarget(_currentTrack);
        }
        else
        {
            Debug.LogWarning("⚠️ No hay objetivo disponible para navegar");
            _isMoving = false;
        }
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
            Debug.LogWarning("⚠️ El recolector no está en el NavMesh!");
            return;
        }

        Vector3 destination;
        
        // Buscar si tiene un componente Plant (que debería ser MonoBehaviour)
        Plant plantComponent = target.GetComponent<Plant>();
        if (plantComponent != null)
        {
            // Buscar el punto de acceso
            Transform accessPoint = plantComponent.puntoDeAcceso;
            destination = accessPoint != null ? accessPoint.position : target.transform.position;
            Debug.Log($"🌱 Navegando hacia planta con punto de acceso: {accessPoint != null}");
        }
        else
        {
            destination = target.transform.position;
            Debug.Log($"🏠 Navegando hacia objetivo sin componente Plant");
        }

        Debug.Log($"🗺️ Destino calculado: {destination}");
        
        bool pathSet = _navMeshAgent.SetDestination(destination);
        if (pathSet)
        {
            Debug.Log($"🤖 Navegando hacia: {target.name} - Path establecido correctamente");
            _hasArrived = false;
            _isMoving = true;
        }
        else
        {
            Debug.LogWarning($"⚠️ No se pudo establecer el path hacia: {target.name}");
            _isMoving = false;
        }
    }


    private void ColectPlant(GameObject plant)
    {
        Plant plantComponent = plant.GetComponent<Plant>();
        
        // Orientarse hacia la planta (como en RobotPrueba)
        if (plantComponent != null && plantComponent.puntoDeAcceso != null)
        {
            transform.rotation = plantComponent.puntoDeAcceso.rotation;
        }
        else
        {
            transform.LookAt(plant.transform);
        }

        _trackList.Remove(plant);
        
        // Marcar como recolectada antes de desactivar
        if (plantComponent != null)
        {
            plantComponent.isCollected = true;
            _currentCarryWeight += plantComponent.tomatosWeight;
            _currentTomatosCollected += plantComponent.tomatosNumber;
        }
        
        plant.SetActive(false);

        Debug.Log($"🍅 Recolectado: {plant.name}. Peso actual: {_currentCarryWeight}");

        TrackNextObject();
    }

    private void DownloadWeight()
    {
        Zone safeZone = this.safeZone.GetComponent<Zone>();
        float exceededWeight = safeZone.DepositeThings(_currentCarryWeight, _currentTomatosCollected);
        _currentCarryWeight = exceededWeight;
        _currentTomatosCollected = 0;

        Debug.Log($"📦 Descargado en zona segura. Peso restante: {_currentCarryWeight}");

        TrackNextObject();
    }

    // Método público para agregar plantas a la lista de seguimiento
    public void AddPlantToTrack(GameObject plant)
    {
        if (plant != null && !_trackList.Contains(plant))
        {
            _trackList.Add(plant);
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
        
        Debug.Log($"🌱 Recolector inicializado con {_trackList.Count} plantas válidas");
        
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

    // Método para verificar si puede recolectar más
    public bool CanCarryMore()
    {
        return _currentCarryWeight < _maxCarryWeight;
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
