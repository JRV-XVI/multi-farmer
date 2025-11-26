using UnityEngine;

public class Plant : MonoBehaviour
{
    private bool _isSick;
    public bool isTaken;
    public bool enableAutoGeneration = true;
    private int _availableTomatoCount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
        return _availableTomatoCount;
    }
}
