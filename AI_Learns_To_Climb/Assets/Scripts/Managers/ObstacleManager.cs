using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ObstacleManager : MonoBehaviour
{
    private ObjectPool _obstaclePool;

    [SerializeField] private int _maxObstacleCountInTrainingArea = 10;

    [Tooltip("How often should a new obstacle spawn in seconds?")][SerializeField] private float _obstacleSpawnrate;
    private float _durationSinceObstacleSpawned;

    private void Awake()
    {
        _obstaclePool = GetComponent<ObjectPool>();
        if(_obstaclePool == null)
        {
            transform.AddComponent<ObjectPool>();
        }
    }

    private void Update()
    {
        handleObstacleSpawning();
    }

    private void handleObstacleSpawning()
    {
        if (_obstaclePool.CurrentActiveItems >= _maxObstacleCountInTrainingArea)
            return;

        _durationSinceObstacleSpawned += Time.deltaTime;
        if (_durationSinceObstacleSpawned >= _obstacleSpawnrate)
        {
            _durationSinceObstacleSpawned = 0;

            _obstaclePool.ActivateItem();
        }
    }
}
