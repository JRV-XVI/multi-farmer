using UnityEngine;

public class ZonaDeDescarga : MonoBehaviour
{
    [Header("Contador")]
    public int totalRecolectado = 0;

    // Opcional: Si tienes un TextMesh (texto 3D) arrástralo aquí
    public TextMesh textoContador; 

    public void AgregarTomates(int cantidad)
    {
        totalRecolectado += cantidad;
        Debug.Log($"💰 CAJA: Recibí {cantidad} tomates. TOTAL ACUMULADO: {totalRecolectado}");

        if (textoContador != null)
        {
            textoContador.text = $"{totalRecolectado}";
        }
    }
}