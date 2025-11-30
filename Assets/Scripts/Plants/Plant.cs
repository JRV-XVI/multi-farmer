using UnityEngine;

public class Plant : MonoBehaviour
{
    public int id;

    [Header("Arrastra aquí el hijo 'PuntoInteraccion'")]
    public Transform puntoDeAcceso; 

    //Datos de peso para recolector y purgador
    public float plantWeight;
    public float tomatosWeight;

    public bool isCollected;

    //Datos de enfermedad para explorer
    public float stemSickPercentage;     //0% (0.00f) a 100% (1.00f)
    public float tomatoesSickPercentage; //0% (0.00f) a 100% (1.00f)
    public float leavesSickPercentage;   //0% (0.00f) a 100% (1.00f)

    public string plantImage;




    public void Start()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().StartPlantValuesRandomly(this);
        isCollected = false;
        
    }

    public float ColectPlant(){
        float tw = tomatosWeight;

        isCollected = true;
        tomatosWeight = 0;

        return tw;
    }

    public float PurgePlant(){
        float pw = plantWeight;

        gameObject.SetActive(false);

        return pw;
    }
}