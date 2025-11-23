using UnityEngine;

public class SafeZone : MonoBehaviour
{

    public void TakeObject(GameObject other)
    {
        // Hacer que el objeto sea hijo del player
        other.transform.SetParent(transform);

        //Ajusta la posicion y rotación del objeto a la del player
        other.transform.localPosition = new Vector3(0, 0, 0.9f);
        other.transform.localRotation = Quaternion.identity;

        other.SetActive(false);
    }

    

}

