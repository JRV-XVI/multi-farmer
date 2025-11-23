using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkingState : State<Player>
{
    public PlayerWalkingState(Player owner) : base(owner) { }

    private float _movingX, _movingY, _movingZ; //Movmiento
    private float _rotateX, _rotateY; //Rotacion de camara
    private float _fire1, _fire2; //Clicks

    public override void OnEnterState()
    {
        owner.ChangeColor(Color.red);
    }

    public override void OnStayState()
    {
        _movingX = owner.GetComponent<PlayerInput>().movingX;
        _movingY = owner.GetComponent<PlayerInput>().movingY;
        _movingZ = owner.GetComponent<PlayerInput>().movingZ;

        _fire1 = owner.GetComponent<PlayerInput>().fire1;
        _fire2 = owner.GetComponent<PlayerInput>().fire2;

        _rotateX = owner.GetComponent<PlayerInput>().rotateX;
        _rotateY = owner.GetComponent<PlayerInput>().rotateY;
            
        //El jugador se mueve continuamente en este estado
        owner.Move(new Vector3(_movingX, _movingY, _movingZ));
            
        //Movimiento de camara
        if(_fire1 != 0)
        {
            owner.Rotate(_rotateX, _rotateY);
        }
        
        //Checa si el jugador se ha detenido
        if (_movingX == 0 && _movingY == 0 && _movingZ == 0)
        {
            owner.ChangeState(new PlayerIdleState(owner));
        }
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
            owner.TakeObject(other.gameObject);
        }
    }
}
