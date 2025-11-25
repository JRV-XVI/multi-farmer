using UnityEngine;

public class SafeZone : MonoBehaviour
{
    [Header("Legacy Zone")]
    public DropZone dropZoneComponent; // Referencia al DropZone si existe
    
    void Start()
    {
        // Intentar obtener el componente DropZone del mismo GameObject
        dropZoneComponent = GetComponent<DropZone>();
        
        if (dropZoneComponent == null)
        {
            Debug.LogWarning($"SafeZone '{gameObject.name}' no tiene componente DropZone. " +
                "Considera agregar DropZone para mejor funcionalidad.");
        }
    }

    public void TakeObject(GameObject other)
    {
        // Hacer que el objeto sea hijo del player
        other.transform.SetParent(transform);

        //Ajusta la posicion y rotaci�n del objeto a la del player
        other.transform.localPosition = new Vector3(0, 0, 0.9f);
        other.transform.localRotation = Quaternion.identity;

        other.SetActive(false);
    }

    /// <summary>
    /// Verifica si esta zona est� disponible para recibir tomates
    /// </summary>
    public bool IsAvailable()
    {
        if (dropZoneComponent != null)
        {
            return dropZoneComponent.IsAvailable();
        }
        return true; // Legacy: siempre disponible
    }
    
    /// <summary>
    /// Registra que un bot deposit� tomates aqu�
    /// </summary>
    public void DepositTomatoes(int count)
    {
        if (dropZoneComponent != null)
        {
            dropZoneComponent.DepositTomatoes(count);
        }
    }
}

