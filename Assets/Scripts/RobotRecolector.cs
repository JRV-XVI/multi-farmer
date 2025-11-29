using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class RobotRecolector : MonoBehaviour
{
    [Header("Lugares")]
    public Transform zonaDescarga; // Arrastra tu objeto vacío "PuntoDescarga"
    public Transform zonaEspera;   // Arrastra tu objeto vacío "ZonaEspera"

    [Header("Configuración")]
    public float capacidadMax = 3f;
    
    [Header("Debug")]
    public float cargaActual = 0f;
    public string estadoActual = "IDLE"; 

    private List<PlantaDeTomate> misTareas = new List<PlantaDeTomate>();
    private NavMeshAgent agente;
    private PlantaDeTomate objetivoActual;
    
    // Banderas de estado
    private bool yendoABase = false;
    private bool yendoAEspera = false;
    private bool estoyDescargando = false;

    void Awake() 
    { 
        agente = GetComponent<NavMeshAgent>(); 
        agente.stoppingDistance = 0.5f; 
    }

    // --- FUNCIONES PÚBLICAS PARA EL MANAGER ---
    public void AgregarTarea(PlantaDeTomate planta)
    {
        misTareas.Add(planta);
        
        // Si estaba dormido o sin hacer nada, reactivarlo
        if (!yendoABase && !estoyDescargando) 
        {
            if (agente.isStopped) agente.isStopped = false;
            DecidirProximoPaso();
        }
    }

    public int ObtenerTareasPendientes() 
    { 
        return misTareas.Count; 
    }

    public Vector3 ObtenerUltimoDestino()
    {
        if (misTareas.Count > 0) return misTareas[misTareas.Count - 1].puntoDeAcceso.position;
        return transform.position;
    }
    // ------------------------------------------

    void DecidirProximoPaso()
    {
        // Si estamos descargando, no interrumpir
        if (estoyDescargando) return;

        // CONDICIÓN 1: IR A DESCARGAR
        // Vamos si estamos llenos... O SI terminamos la lista y traemos carga (limpieza final)
        bool deboDescargar = cargaActual >= capacidadMax || (misTareas.Count == 0 && cargaActual > 0);

        if (deboDescargar)
        {
            Debug.Log($"⚠️ {name}: Yendo a descargar (Carga: {cargaActual})");
            objetivoActual = null;
            yendoABase = true;
            yendoAEspera = false;
            estadoActual = "YENDO A BASE";
            
            agente.isStopped = false;
            agente.SetDestination(zonaDescarga.position);
            return;
        }

        // CONDICIÓN 2: IR A DORMIR (Solo si lista vacía y carga 0)
        if (misTareas.Count == 0) 
        {
            estadoActual = "YENDO A ESPERA";
            objetivoActual = null;
            yendoABase = false;
            
            if (zonaEspera != null)
            {
                yendoAEspera = true;
                agente.isStopped = false;

                // TRUCO ANTI-CHOQUE:
                // Elegimos un punto aleatorio en un radio de 3 metros alrededor de la zona
                // Así cada robot se estaciona en un lugar un poquito diferente
                Vector3 destinoRandom = zonaEspera.position + Random.insideUnitSphere * 3f;
                destinoRandom.y = zonaEspera.position.y; // Mantenerlo en el suelo
                
                agente.SetDestination(destinoRandom);
            }
            return;
        }

        // CONDICIÓN 3: TRABAJAR (Vecino más cercano)
        PlantaDeTomate mejor = null;
        float minDist = Mathf.Infinity;
        foreach (var p in misTareas)
        {
            float d = Vector3.Distance(transform.position, p.puntoDeAcceso.position);
            if (d < minDist) { minDist = d; mejor = p; }
        }

        objetivoActual = mejor;
        yendoABase = false;
        yendoAEspera = false;
        estadoActual = "YENDO A PLANTA";
        
        agente.isStopped = false; 
        agente.SetDestination(objetivoActual.puntoDeAcceso.position);
    }

    void Update()
    {
        if (estoyDescargando) return;
        if (agente.pathPending) return;

        float dist = agente.remainingDistance;

        // Usamos 2.0f como tolerancia general para zonas grandes
        if (dist <= 2.0f) 
        {
            if (yendoABase)
            {
                // Llegó a la caja -> Iniciar descarga
                StartCoroutine(RutinaDescarga());
            }
            else if (yendoAEspera)
            {
                // CORRECCIÓN ANTI-SPAM: Solo entra si NO está dormido todavía
                if (estadoActual != "DORMIDO")
                {
                    Debug.Log($"🅿️ {name}: Estacionado.");
                    agente.isStopped = true; // Apagar motor
                    agente.ResetPath();      // Borrar ruta
                    estadoActual = "DORMIDO";
                }
            }
            // Para las plantas usamos la precisión exacta
            else if (objetivoActual != null && dist <= agente.stoppingDistance)
            {
                Cosechar();
            }
        }
    }

    IEnumerator RutinaDescarga()
    {
        estoyDescargando = true;
        estadoActual = "DESCARGANDO...";
        
        // Frenar visualmente
        agente.isStopped = true;
        agente.ResetPath();

        yield return new WaitForSeconds(1.5f); // Tiempo de simulación

        cargaActual = 0;
        estoyDescargando = false;
        yendoABase = false;
        
        // Volver a decidir (Irá a Espera o a Planta)
        DecidirProximoPaso(); 
    }

    void Cosechar()
    {
        if(objetivoActual != null) 
        {
            objetivoActual.Recolectar();
            cargaActual++;
            misTareas.Remove(objetivoActual);
            DecidirProximoPaso();
        }
    }

    // Ayuda visual para ver a dónde va
    void OnDrawGizmos()
    {
        if (agente != null && agente.hasPath)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, agente.destination);
            Gizmos.DrawSphere(agente.destination, 0.3f);
        }
    }
}