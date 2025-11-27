using UnityEngine;

public class Zone : MonoBehaviour
{
    private GameManager _gameManager;
    private float _maxCarryWeight;
    private float _currentCarryWeight;
    private int _currentTomatosDeposited;

    public bool isSafeZoneForRecolector;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<Manager>();
        if(_gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!!");
        }


        if( isSafeZoneForRecolector)
        {
            _maxCarryWeight = _gameManager.recolectorMaxCarryWeight;
        } else
        {
            _maxCarryWeight = _gameManager.trashoneMaxCarryWeight;
        }
        _currentCarryWeight = 0f;

    }

    
    public float DepositeTomatos(float weight, int number)
    {
        _currentCarryWeight += weight;
        _currentTomatosDeposited += number;

        if(_currentCarryWeight > _maxCarryWeight)
        {
            float exceededWeight = _currentCarryWeight - _maxCarryWeight;
            _currentCarryWeight = _maxCarryWeight;
            return exceededWeight;
        }
        return 0f;
        
    }
}
