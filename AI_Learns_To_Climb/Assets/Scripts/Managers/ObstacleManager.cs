using System;
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

        _maxObstacleCountInTrainingArea = Math.Clamp(_maxObstacleCountInTrainingArea, 0, _obstaclePool.ItemInStorageCount);
    }

    private void FixedUpdate()
    {
        handleObstacleSpawning();
    }

    private void handleObstacleSpawning()
    {
        if (_obstaclePool.CurrentActiveItems >= _maxObstacleCountInTrainingArea)
            return;

        _durationSinceObstacleSpawned += Time.fixedDeltaTime;
        if (_durationSinceObstacleSpawned >= _obstacleSpawnrate)
        {
            _durationSinceObstacleSpawned = 0;

            _obstaclePool.ActivateItem();
        }
    }
}
