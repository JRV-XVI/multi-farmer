using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class Player : MonoBehaviour
{
    private StateMachine<Player> _stateMachine;

    private Rigidbody _rb;
    private BoxCollider _boxCollider;

    public float movementSpeed = 4;
    public float rotationSpeed = 2;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();

        _stateMachine = new StateMachine<Player>();
        _stateMachine.ChangeState(new PlayerIdleState(this));
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    //Funciones para la maquina de estados
    public void ChangeState(State<Player> newState)
    {
        _stateMachine.ChangeState(newState);
    }


    //Detectar Colliders y Triggers
    private void OnTriggerEnter(Collider other)
    {
        _stateMachine.CurrentState.OnTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        _stateMachine.CurrentState.OnTriggerStay(other);
    }





    //////Funciones del Player

    //Funciones de movimiento
    public void Move(Vector3 moving)
    {
        Vector3 direction = this.transform.TransformDirection(moving);
        this.transform.position += direction * movementSpeed * Time.deltaTime;
    }


    private float currentX =0, currentY=0;
    public void Rotate(float visionX, float visionY)
    {
        currentX += visionX * rotationSpeed;
        currentY += visionY * rotationSpeed;

        // Limitar el ángulo vertical para evitar que el personaje se voltee
        currentY = Mathf.Clamp(currentY, -60f, 70f);

        this.transform.rotation = Quaternion.Euler(-currentY, currentX, 0);
    }



    //Funciones de acciones con otros objetos

    public void TakeObject(GameObject other)
    {
        // Hacer que el objeto sea hijo del player
        other.transform.SetParent(transform);

        //Ajusta la posicion y rotación del objeto a la del player
        other.transform.localPosition = new Vector3(0, 0, 0.9f);
        other.transform.localRotation = Quaternion.identity;
    }


    public void DropObject(GameObject other)
    {
        //Quitar la relación de hijo con el player
        other.transform.SetParent(null);

        //Dar una posición justo enfrente del player al soltarlo
        other.transform.position = transform.position + transform.forward * 0.9f;
    }





    public void ChangeColor(Color color)
    {
        
    }




}
