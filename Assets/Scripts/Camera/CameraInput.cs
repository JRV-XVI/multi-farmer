using UnityEngine;
using static UnityEditor.PlayerSettings;

public class CameraInput : MonoBehaviour
{
    public float changeCameraCurrent, changeCameraLast;
    public bool changeCameraPressed = false;

    public float visionX, visionY;

    void Start()
    {
        
    }


    void Update()
    {
        //Detectar que se presiono el cambio de camara
        changeCameraCurrent = Input.GetAxis("Camera");

        if (changeCameraCurrent != 0 && changeCameraLast == 0)
        {
            changeCameraPressed = true;
        }
        else
        {
            changeCameraPressed = false;
        }

        changeCameraLast = changeCameraCurrent;

        visionX = Input.GetAxis("Mouse X");
        visionY = Input.GetAxis("Mouse Y");

        //Debug.Log("visX: " +  visionX + " visY: " + visionY + "/n");

    }
}
