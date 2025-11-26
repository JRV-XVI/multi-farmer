using UnityEngine;
using UnityEngine.AI;

public class RobotPrueba : MonoBehaviour
{
    [Header("¿A cuál planta quieres que vaya?")]
    public PlantaDeTomate plantaObjetivo; // Aquí arrastras la planta específica de la escena

    private NavMeshAgent agente;
    private bool yaLlegue = false;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();

        if (plantaObjetivo != null)
        {
            // MAGIA: Obtenemos la posición del punto de interacción de ESA planta
            Vector3 destino = plantaObjetivo.puntoDeAcceso.position;
            
            Debug.Log("🤖 Yendo al punto de acceso de: " + plantaObjetivo.name);
            agente.SetDestination(destino);
        }
    }

    void Update()
    {
        // Detectar si ya llegó para ejecutar la acción
        if (!agente.pathPending && agente.remainingDistance <= agente.stoppingDistance)
        {
            if (!agente.hasPath || agente.velocity.sqrMagnitude == 0f)
            {
                // Si no habíamos llegado antes, interactuamos ahora
                if (!yaLlegue && plantaObjetivo != null)
                {
                    LlegarYRecolectar();
                }
            }
        }
    }

    void LlegarYRecolectar()
    {
        yaLlegue = true;
        
        // 1. Aseguramos que mire a la planta (alinearse con la flecha azul del punto)
        transform.rotation = plantaObjetivo.puntoDeAcceso.rotation;

        // 2. Llamamos a la función de la planta
        plantaObjetivo.Interactuar();
    }
}