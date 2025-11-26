using UnityEngine;

public class PlantaDeTomate : MonoBehaviour
{
    [Header("Arrastra aquí el hijo 'PuntoInteraccion'")]
    public Transform puntoDeAcceso; 

    // Esta función la llamará el robot cuando llegue
    public void Interactuar()
    {
        Debug.Log("🍅 ¡Robot recolectando tomates de: " + gameObject.name + "!");
    }
}