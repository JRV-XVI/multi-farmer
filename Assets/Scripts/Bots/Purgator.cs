using System.Collections.Generic;
using UnityEngine;

public class Purgator : MonoBehaviour
{
    private GameManager _gameManager;
    

    private GameObject _currentTrack;
    private List<GameObject> _trackList;

    private float _maxCarryWeight;
    private float _currentCarryWeight;
    private int _currentPlantsCollected;
    private int _currentTomatosCollected;

    public GameObject zone;

    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        if(_gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!!");
        }

        _maxCarryWeight = _gameManager.purgatorMaxCarryWeight;
        _currentCarryWeight = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        CheckDistanceToTrack();
    }

    private void CheckDistanceToTrack()
    {
        float distance= 10f;
        
        if (distance <= _gameManager.purgatorDistanceToDeposit)
        {
            if(_currentTrack == zone)
            {
                DownloadWeight();
            } else
            {
                ColectPlant(_currentTrack);
            }
        }
    }

    private void TrackNextObject()
    {
        if(_trackList.Count > 0 && _currentCarryWeight < _maxCarryWeight)
        {
            _currentTrack = _trackList[0];
        } else
        {
            _currentTrack = zone;
        }
        
    }


    private void ColectPlant(GameObject plant)
    {
        _trackList.Remove(plant);

        plant.SetActive(false);

        _currentCarryWeight += plant.GetComponent<Plant>().plantWeight;
        _currentPlantsCollected += 1;

        TrackNextObject();

    }

    private void DownloadWeight()
    {
        Zone safeZone = zone.GetComponent<Zone>();
        float exceededWeight = safeZone.DepositeThings(_currentCarryWeight, _currentPlantsCollected);
        _currentCarryWeight = exceededWeight;
        _currentPlantsCollected = 0;
    }
}
