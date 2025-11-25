using UnityEditor;
using UnityEngine;

public class PlayerInput : MonoBehaviour 
{
    Player player;

    public float movingX, movingY, movingZ;

    public float rotateX, rotateY;
    public float fire1, fire2;


    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        //Movimiento
        movingX = Input.GetAxis("Horizontal");
        movingY = Input.GetAxis("Jump");
        movingZ = Input.GetAxis("Vertical");


        //Rotacion
        rotateX = Input.GetAxis("Mouse X");
        rotateY = Input.GetAxis("Mouse Y");
        fire1 = Input.GetAxis("Fire1");

        fire2 = Input.GetAxis("Fire2");
    }
}
