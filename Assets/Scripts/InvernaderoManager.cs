using UnityEngine;
using System.Linq; // Necesario para ordenar listas
using System.Collections.Generic;

public class InvernaderoManager : MonoBehaviour
{
    public RobotExplorador explorador;
    public RobotRecolector[] recolectores; // Arrastra los 2 aquí

    void Start()
    {
        // 1. Encontrar todas las plantas
var todasLasPlantas = FindObjectsByType<PlantaDeTomate>(FindObjectsSortMode.None).ToList();
        // 2. Ordenarlas "en serpiente" (Por X y luego por Z)
        // Esto hace que el explorador no salte a lo loco
        var rutaOrdenada = todasLasPlantas
            .OrderBy(p => p.transform.position.x)
            .ThenBy(p => p.transform.position.z)
            .ToList();

        // 3. Mandar al explorador
        explorador.Configurar(rutaOrdenada, this);
    }

    // Esta función la llama el Explorador cuando ve algo maduro
    public void ReportarHallazgo(PlantaDeTomate planta)
{
    RobotRecolector mejorCandidato = null;
    int menosTareas = 9999; // Un número muy alto para empezar
    float mejorDistancia = Mathf.Infinity;

    // Recorremos todos los recolectores para ver quién está más libre
    foreach (RobotRecolector robot in recolectores)
    {
        int tareasDelRobot = robot.ObtenerTareasPendientes();

        // LOGICA 1: BALANCEO DE CARGA
        // Si este robot tiene menos tareas que el récord actual, es el nuevo candidato
        if (tareasDelRobot < menosTareas)
        {
            menosTareas = tareasDelRobot;
            mejorCandidato = robot;
            // Guardamos la distancia de este robot a la planta para desempatar luego
            mejorDistancia = Vector3.Distance(robot.transform.position, planta.transform.position);
        }
        // LOGICA 2: DESEMPATE POR CERCANÍA (Optimización de vecinos)
        // Si tienen la MISMA cantidad de tareas, elegimos al que esté más cerca
        // Esto evita que el Robot 1 cruce todo el mapa por una planta si el Robot 2 ya está ahí.
        else if (tareasDelRobot == menosTareas)
        {
            float dist = Vector3.Distance(robot.ObtenerUltimoDestino(), planta.transform.position);
            if (dist < mejorDistancia)
            {
                mejorDistancia = dist;
                mejorCandidato = robot;
            }
        }
    }

    if (mejorCandidato != null)
    {
        mejorCandidato.AgregarTarea(planta);
        Debug.Log($"🚨 Hallazgo asignado al {mejorCandidato.name} (Tenía {menosTareas} pendientes)");
    }
}
}