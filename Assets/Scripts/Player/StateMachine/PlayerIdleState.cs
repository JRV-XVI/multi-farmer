using Unity.VisualScripting;
using UnityEngine;

public class PlayerIdleState : State<Player>
{
    public PlayerIdleState(Player owner) : base(owner) { }

    private float _movingX, _movingY, _movingZ;
    private float _rotateX, _rotateY;
    private float _fire1, _fire2;




    public override void OnEnterState()
    {
        owner.ChangeColor(Color.blue);
    }

    public override void OnStayState()
    {
        //Mover jugador
        _movingX = owner.GetComponent<PlayerInput>().movingX;
        _movingY = owner.GetComponent<PlayerInput>().movingY;
        _movingZ = owner.GetComponent<PlayerInput>().movingZ;


        if (_movingX != 0  || _movingY != 0 || _movingZ != 0)
        {
            owner.Move(new Vector3(_movingX, _movingY, _movingZ));
            owner.ChangeState(new PlayerWalkingState(owner));
        }


        //Rotar jugador
        _rotateX = owner.GetComponent<PlayerInput>().rotateX;
        _rotateY = owner.GetComponent<PlayerInput>().rotateY;

        _fire1 = owner.GetComponent<PlayerInput>().fire1;

        if (_fire1 != 0)
        {
            owner.Rotate(_rotateX, _rotateY);
        }

        //Tomar objeto
        _fire2 = owner.GetComponent<PlayerInput>().fire2;
        Debug.Log("Fire2: " +  _fire2);
        
    }

    public override void OnExitState()
    {
        owner.ChangeColor(Color.white);
    }




    //Detectar colisiones y triggers


    public override void OnTriggerStay(Collider other)
    {
        Debug.Log("Se checa el Trigger");

        if (_fire2 != 0 && other.CompareTag("Tomato"))
        {
            Debug.Log("Objeto tomado");

            Tomato tomato = other.gameObject.GetComponent<Tomato>();
            if (tomato != null)
            {
                tomato.isTaken = true;
                owner.TakeObject(other.gameObject);
            }
            else
            {
                Debug.LogWarning("El objeto no tiene componente Tomato");
            }
        }
    }





}
