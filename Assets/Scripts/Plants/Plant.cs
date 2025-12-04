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
    public bool plantIsSick;
    public bool tomatosAreSick;
    public bool leavesAreSick;

    public string plantImage;

    // Control para asegurar que solo se inicialice cuando el Explorer la visite
    private bool _hasBeenScanned = false;

    public void Start()
    {
        // No inicializar valores aquí - Los valores se generarán cuando el Explorer escanee la planta
        isCollected = false;
        _hasBeenScanned = false;
    }

    // Método público que el Explorer llamará para inicializar la planta
    public void ScanPlant()
    {
        if (_hasBeenScanned)
        {
            return;
        }

        // Generar valores aleatorios cuando el Explorer la escanea
        GameObject gameManagerObj = GameObject.FindGameObjectWithTag("GameManager");
        if (gameManagerObj != null)
        {
            GameManager gm = gameManagerObj.GetComponent<GameManager>();
            if (gm != null)
            {
                gm.StartPlantValuesRandomly(this);
                _hasBeenScanned = true;
            }
        }
    }

    // Verifica si la planta ha sido escaneada
    public bool HasBeenScanned()
    {
        return _hasBeenScanned;
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