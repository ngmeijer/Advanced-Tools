using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ObstacleManager : SpawnableManager
{
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(spawnObject());
    }

    protected override void disableAllObjects()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator spawnObject()
    {
        if (_pool.CurrentActiveItems < _maxSpawnableCountInArea)
        {
            for (int i = 0; i < _spawnMultiplier; i++)
            {
                _pool.ActivateItem();
            }
        }

        yield return new WaitForSeconds(_spawnrate);

        StartCoroutine(spawnObject());
    }
}