using UnityEngine;
using System.Collections;

public class PlantaDeTomate : MonoBehaviour
{
    public Transform puntoDeAcceso;
    
    [Header("Estado Lógico")]
    public bool estaMadura = false; // El explorador revisará esta casilla
    public bool yaFueReportada = false;

    [Header("Tiempos de Maduración")]
    public float tiempoMin = 10f;
    public float tiempoMax = 30f;

    void Start()
    {
        // Arranca el ciclo invisible
        StartCoroutine(CicloDeCrecimiento());
    }

    IEnumerator CicloDeCrecimiento()
    {
        // 1. Fase Crecimiento
        estaMadura = false;
        yaFueReportada = false;

        // Espera silenciosa...
        float tiempo = Random.Range(tiempoMin, tiempoMax);
        yield return new WaitForSeconds(tiempo);

        // 2. Fase Madura
        estaMadura = true; 
        // Aquí no cambiamos nada visual, solo la variable interna cambia a TRUE
    }

    public void Recolectar()
    {
        Debug.Log($"🍅 Lógica: Planta {name} cosechada. Reiniciando ciclo...");
        
        // Reiniciamos el ciclo matemático
        StartCoroutine(CicloDeCrecimiento());
    }
}