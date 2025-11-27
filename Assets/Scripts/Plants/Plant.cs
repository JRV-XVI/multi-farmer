using UnityEngine;

public class Plant : MonoBehaviour
{
    public int id;

    //Datos de peso para recolector y purgador
    public float plantWeight;
    public float tomatosWeight;
    public int tomatosNumber;

    //Datos de enfermedad para explorer
    public bool plantIsSick;
    public bool tomatosAreSick;
    public bool leavesAreSick;

    public Sprite plantImage;

    // Indica si esta planta ya fue explorada
    [HideInInspector]
    public bool hasBeenExplored = false;

    void Start()
    {
        // Generar ID único si no está asignado
        if (id == 0)
        {
            id = gameObject.GetInstanceID();
        }
    }

    // Determina si la planta está completamente sana
    public bool IsHealthy()
    {
        return !plantIsSick && !tomatosAreSick && !leavesAreSick;
    }

    // Determina si la planta tiene alguna enfermedad
    public bool IsSick()
    {
        return plantIsSick || tomatosAreSick || leavesAreSick;
    }
}