using UnityEngine;

public class Camera : MonoBehaviour
{
    private StateMachine<Camera> _stateMachine;


    private GameObject _mainCamera;
    private GameObject _player;

    public bool changeCamaraPressed = false;

    private void Awake()
    {
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        _player = GameObject.FindGameObjectWithTag("Player");
    }


    void Start()
    {
        changeCamaraPressed = _mainCamera.GetComponent<CameraInput>().changeCameraPressed;

        _stateMachine = new StateMachine<Camera>();
        _stateMachine.ChangeState(new CameraFreeState(this));
    }

    private void Update()
    {
        _stateMachine.Update();

        changeCamaraPressed = _mainCamera.GetComponent<CameraInput>().changeCameraPressed;
    }

    //Funciones para la maquina de estados
    public void ChangeState(State<Camera> newState)
    {
        _stateMachine.ChangeState(newState);
    }



    //Funciones de la camara
    public void FollowPlayer(Vector3 plus)
    {
        _mainCamera.transform.position = _player.transform.position + plus;
        _mainCamera.transform.rotation = _player.transform.rotation;
    }

    public void RotateCamera()
    {
        //Funcion para rotar la camara
    }
}
