using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.GridLayoutGroup;

public class Bot : MonoBehaviour
{
    private StateMachine<Bot> _stateMachine;
    private Rigidbody _rb;
    private CapsuleCollider _capsuleCollider;

    public Manager manager;
    
    public GameObject targetPlant;
    public GameObject safeZone;
    public Vector2 destiny;
    private Map _map;

    [SerializeField] private List<Cell> _currentPath;

    private Movement _movement;
    public bool isCarring;

    public int maxTomatoCapacity = 10;
    public Transform assignedDropZone;
    private int _collectedTomatoCount = 0;

    //Para movimiento
    [SerializeField] private int _pathIndex = 0;
    public bool isMovingPath = false;

    public int id;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        manager = GameObject.FindGameObjectWithTag("Manager").gameObject.GetComponent<Manager>();
        _movement = GetComponent<Movement>();
    
        _stateMachine = new StateMachine<Bot>();
        _stateMachine.ChangeState(new BotIdleState(this));

        _map = new Map(manager.mapSize.x, manager.mapSize.y, new Vector2(0, 0));
        _map.MarkAll();

        isCarring = false;

    }



    private void Update()
    {
        _stateMachine.Update();


    }

    //Funcion para maquina de estados
    public void ChangeState(State<Bot> newState)
    {
        _stateMachine.ChangeState(newState);
    }

    private void OnTriggerEnter(Collider other)
    {
        _stateMachine.OnTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        _stateMachine.OnTriggerStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _stateMachine.OnTriggerExit(other);
    }




    //Funciones propias
    public void CalculateRoute(Vector2 destiny)
    {
        
        if(destiny != null) {
            

            Vector2 botPos = new Vector2(this.transform.position.x, this.transform.position.z);

            _currentPath.Clear();
            _currentPath = _map.FindPath(botPos, destiny);

            string camino = "";
            foreach (Cell cell in _currentPath)
            {
                camino += "(" + cell.worldX + ", " + cell.worldY + "), ";
            }

            Debug.Log("bot: " + botPos + " des: " + destiny);
            Debug.Log("Path: " + camino);

            
            isMovingPath = true;


        }
        else
        {
            Debug.LogWarning("No hay posicion destino. Es null");
        }

        _pathIndex = 0;
    }



    public void FindPlant()
    {
        targetPlant = manager.GetFreePlant();

    }


    public void FollowPath()
    {
        if (_currentPath == null || _currentPath.Count == 0)
        {
            isMovingPath = false;
            return;
        }

        // Si ya llego al punto anterior, pasar al siguiente
        if (_movement.HasArrived())
        {
            if (_pathIndex >= _currentPath.Count)
            {
                isMovingPath = false;
                return;
            }

            Cell c = _currentPath[_pathIndex];
            Vector3 target = new Vector3(c.worldX, transform.position.y, c.worldY);


            _movement.MoveTo(target);
            _pathIndex++;
        }
    }

    public void EvadeOtherBot(GameObject otherBot)
    {
        Debug.LogWarning("Se intenta evadir otro bot");

        _map.MarkTemporalObstacle(_map, otherBot);
        //CalculateRoute(destiny);
    }


    //Funcion para esperar tiempo
    public IEnumerator WaitSeconds(float sec)
    {
        Debug.Log("Bot empieza a esperar...");

        // Espera 5 segundos
        yield return new WaitForSeconds(sec);

        Debug.Log("Bot termin� de esperar, ahora act�a");
        // Aqu� pones la acci�n que quieres que haga despu�s de esperar
    }




    //Funciones con otros objetos
    public void TakeObject(GameObject other)
    {
        //Hacer que el objeto sea hijo del player
        other.transform.SetParent(transform);

        //Ajusta la posicion y rotaci�n del objeto a la del player
        other.transform.localPosition = new Vector3(0, 2, 0);
        other.transform.localRotation = Quaternion.identity;

        isCarring = true;
    }

    public void DropObject(GameObject other)
    {
        //Quitar la relación de hijo con el player
        other.transform.SetParent(null);

        //Dar una posición justo enfrente del player al soltarlo
        other.transform.position = transform.position + transform.forward * 0.9f;

        isCarring = false;
    }

    public int GetCollectedTomatoCount()
    {
        return _collectedTomatoCount;
    }

}
