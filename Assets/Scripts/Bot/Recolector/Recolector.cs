using UnityEngine;

public class Recolector : Bot
{

    private Plant[] _listPlant;
    public GameObject safeZone;

    protected override void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();

        manager = GameObject.FindGameObjectWithTag("Manager").gameObject.GetComponent<Manager>();
        _movement = GetComponent<Movement>();
    
        _stateMachine = new StateMachine<Recolector>();
        _stateMachine.ChangeState(new RecolectorIdleState(this));

        _map = new Map(manager.mapSize.x, manager.mapSize.y, new Vector2(0, 0));
        _map.MarkAll();

        isCarring = false;
        numObjectsCarring = 0;
        numObjectsCapacity = 5;
    }
    
    protected override void Update()
    {
        _stateMachine.Update();
    }

    public void FindPlant()
    {
        targetPlant = manager.GetFreePlant();

    }

    //Funcion para buscar una planta libre
    public void SetListPlant(Plant[] newListPlant)
    {
        _listPlant = newListPlant;

    }

    public void DownloadCarringInZone()
    {
        if(safeZone == null)
        {
            Debug.LogError("Safe zone no asignada en Recolector " + id);
            return;
        }

        foreach(Transform child in transform)
        {
            safeZone.GetComponent<SafeZone>().TakeObject(child.gameObject);
        }
    }


}
