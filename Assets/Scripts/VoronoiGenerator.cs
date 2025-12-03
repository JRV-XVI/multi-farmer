using UnityEngine;
using System.Collections.Generic;

public class VoronoiGenerator : MonoBehaviour
{
    public List<Transform> puntos;   // Tus puntos generadores en la escena
    public int sizeX = 50;           // Tamaño en X del mapa
    public int sizeZ = 50;           // Tamaño en Z del mapa
    public float cellSize = 1f;      // Tamaño de cada celda

    private int[,] regiones;         // Almacena a que punto pertenece cada celda

    void Start()
    {
        regiones = new int[sizeX, sizeZ];
        GenerarVoronoi();
    }

    void GenerarVoronoi()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0f, z * cellSize);

                int indexCercano = -1;
                float distMin = Mathf.Infinity;

                // Buscar generador mas cercano
                for (int i = 0; i < puntos.Count; i++)
                {
                    float dist = (pos - puntos[i].position).sqrMagnitude;
                    if (dist < distMin)
                    {
                        distMin = dist;
                        indexCercano = i;
                    }
                }

                regiones[x, z] = indexCercano;
            }
        }
    }

    // Visualizacion con Gizmos (solo en modo escena)
    void OnDrawGizmos()
    {
        if (regiones == null) return;

        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                Gizmos.color = ColorFromIndex(regiones[x, z]);
                Vector3 pos = new Vector3(x * cellSize, 0f, z * cellSize);
                Gizmos.DrawCube(pos, Vector3.one * (cellSize * 0.95f));
            }
        }
    }

    Color ColorFromIndex(int i)
    {
        Random.InitState(i * 9973);   // Genera colores repetibles
        return new Color(Random.value, Random.value, Random.value);
    }
}
