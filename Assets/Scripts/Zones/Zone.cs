using UnityEngine;

public class Zone : MonoBehaviour
{
    private GameManager _gameManager;
    [SerializeField] private float _maxCarryWeight;
    [SerializeField] private float _currentCarryWeight;
    [SerializeField] private int _currentThingsNumberDeposited;

    public bool isSafeZoneForRecolector;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        if(_gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!!");
        }


        if( isSafeZoneForRecolector)
        {
            _maxCarryWeight = _gameManager.safeZoneMaxCarryWeight;
        } else
        {
            _maxCarryWeight = _gameManager.trashoneMaxCarryWeight;
        }
        _currentCarryWeight = 0f;

    }

    
    public float DepositeThings(float weight, int number)
    {
        _currentCarryWeight += weight;
        _currentThingsNumberDeposited += number;

        if(_currentCarryWeight > _maxCarryWeight)
        {
            float exceededWeight = _currentCarryWeight - _maxCarryWeight;
            _currentCarryWeight = _maxCarryWeight;
            return exceededWeight;
        }
        return 0f;
        
    }
}
