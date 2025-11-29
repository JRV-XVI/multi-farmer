using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class RobotExplorador : MonoBehaviour
{
    private List<PlantaDeTomate> rutaDeExploracion = new List<PlantaDeTomate>();
    private NavMeshAgent agente;
    private int indiceActual = 0;
    private InvernaderoManager manager;

    public void Configurar(List<PlantaDeTomate> ruta, InvernaderoManager jefe)
    {
        agente = GetComponent<NavMeshAgent>();
        rutaDeExploracion = ruta;
        manager = jefe;
        IrAlSiguiente(); // Arranca el movimiento
    }

    void Update()
    {
        // 1. SEGURIDAD: Si no hay ruta, no hacemos nada
        if (rutaDeExploracion.Count == 0) return;

        // 2. SEGURIDAD ANTI-CRASH: Si el índice se pasó, reiniciamos al 0 (Bucle)
        if (indiceActual >= rutaDeExploracion.Count)
        {
            Debug.Log("🔄 Exploración completada. Reiniciando patrullaje...");
            indiceActual = 0;
            IrAlSiguiente();
            return; // Salimos de este Update para evitar errores abajo
        }

        // 3. Checar si llegó
        if (!agente.pathPending && agente.remainingDistance <= 0.1f)
        {
            // Analizar planta (Ahora es seguro porque indiceActual está validado)
            PlantaDeTomate plantaActual = rutaDeExploracion[indiceActual];
            
            // Si está madura y nadie la ha visto...
            if (plantaActual.estaMadura && !plantaActual.yaFueReportada)
            {
                plantaActual.yaFueReportada = true;
                manager.ReportarHallazgo(plantaActual);
            }

            // Seguir a la siguiente
            indiceActual++;
            IrAlSiguiente(); // Llamamos a movernos
        }
    }

    void IrAlSiguiente()
    {
        // Doble chequeo por seguridad antes de mover
        if (indiceActual < rutaDeExploracion.Count)
        {
            agente.SetDestination(rutaDeExploracion[indiceActual].puntoDeAcceso.position);
        }
    }
}