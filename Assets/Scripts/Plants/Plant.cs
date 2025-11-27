using UnityEngine;

public class Plant : MonoBehaviour
{
    public int id;

    [Header("Arrastra aquí el hijo 'PuntoInteraccion'")]
    public Transform puntoDeAcceso; 

    //Datos de peso para recolector y purgador
    public float plantWeight;
    public float tomatosWeight;
    public int tomatosNumber;

    public bool isCollected;

    //Datos de enfermedad para explorer
    public bool plantIsSick;
    public bool tomatosAreSick;
    public bool leavesAreSick;

    public string plantImage;


}