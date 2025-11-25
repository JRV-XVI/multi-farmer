using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    private bool _isSick;
    public bool isTaken;

    [Header("Tomato Generation")]
    public GameObject tomatoPrefab; // Asignar en el Inspector
    public Transform tomatoSpawnPoint; // Punto donde aparecen los tomates
    public int maxTomatoes = 5;
    public int initialTomatoes = 5; // Siempre generar exactamente 5 tomates
    public float generationTime = 5f; // Tiempo fijo de generaci�n: 5 segundos
    public bool enableAutoGeneration = true; // Activar generaci�n autom�tica

    private List<GameObject> _availableTomatoes = new List<GameObject>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Asegurar que la planta no se mueva
        Rigidbody plantRb = GetComponent<Rigidbody>();
        if (plantRb != null)
        {
            plantRb.isKinematic = true; // La planta no se mueve por física
            plantRb.useGravity = false;
        }

        // Hacer el collider de la planta un trigger para evitar bloqueos del bot
        Collider plantCollider = GetComponent<Collider>();
        if (plantCollider != null)
        {
            plantCollider.isTrigger = true;
        }

        // Generar exactamente 5 tomates iniciales
        for (int i = 0; i < initialTomatoes; i++)
        {
            GenerateTomato();
        }

        // Iniciar generación automática solo si está habilitada
        if (enableAutoGeneration)
        {
            StartCoroutine(AutoGenerateTomatoes());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsSick()
    {
        return _isSick;
    }

    public int GetAvailableTomatoCount()
    {
        // Limpiar referencias null
        _availableTomatoes.RemoveAll(t => t == null);
        return _availableTomatoes.Count;
    }

    public GameObject CollectTomato()
    {
        // Limpiar referencias null
        _availableTomatoes.RemoveAll(t => t == null);

        if (_availableTomatoes.Count > 0)
        {
            GameObject tomato = _availableTomatoes[0];
            _availableTomatoes.RemoveAt(0);
            return tomato;
        }
        return null;
    }

    private void GenerateTomato()
    {
        if (_availableTomatoes.Count >= maxTomatoes)
            return;

        // Generar tomates arriba de la caja
        Vector3 spawnPosition = tomatoSpawnPoint != null 
            ? tomatoSpawnPoint.position 
            : transform.position + Vector3.up * (transform.localScale.y / 2 + 0.3f) + 
              new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));

        GameObject newTomato;
        
        // Si hay prefab asignado, usarlo
        if (tomatoPrefab != null)
        {
            newTomato = Instantiate(tomatoPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            // Crear tomate simple si no hay prefab
            newTomato = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newTomato.name = "Tomato";
            newTomato.tag = "Tomato";
            newTomato.transform.position = spawnPosition;
            newTomato.transform.localScale = Vector3.one * 0.2f;
            
            // Agregar componente Tomato si no lo tiene
            if (newTomato.GetComponent<Tomato>() == null)
            {
                newTomato.AddComponent<Tomato>();
            }
            
            // Hacer el collider trigger para que no empuje la planta
            Collider col = newTomato.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
            
            // Color rojo para el tomate
            Renderer renderer = newTomato.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.red;
            }
            
            // Agregar Rigidbody pero desactivar gravedad (el tomate está en la planta)
            Rigidbody rb = newTomato.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = newTomato.AddComponent<Rigidbody>();
            }
            rb.useGravity = false;
            rb.isKinematic = true; // No se mueve por física
            rb.mass = 0.1f;
        }
        
        newTomato.transform.SetParent(transform); // Hijo de la planta
        _availableTomatoes.Add(newTomato);
    }

    private IEnumerator AutoGenerateTomatoes()
    {
        while (true)
        {
            // Esperar tiempo fijo de 5 segundos
            yield return new WaitForSeconds(generationTime);

            // Generar un tomate si no se ha alcanzado el m�ximo
            if (_availableTomatoes.Count < maxTomatoes)
            {
                GenerateTomato();
                Debug.Log($"Planta {gameObject.name} gener� nuevo tomate. Total: {_availableTomatoes.Count}/{maxTomatoes}");
            }
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
