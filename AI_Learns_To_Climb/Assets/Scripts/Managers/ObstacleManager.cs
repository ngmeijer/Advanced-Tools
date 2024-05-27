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

        if (!_enableSpawning)
            return;

        StartCoroutine(spawnObject());
    }

    protected override void disableAllObjects()
    {
        throw new System.NotImplementedException();
    }

    protected override IEnumerator spawnObject()
    {
        for (int i = 0; i < _spawnMultiplier; i++)
        {
            if (_pool.CurrentActiveItems >= _maxSpawnableCountInArea)
                break;
            _pool.ActivateItem();
        }

        yield return new WaitForSeconds(_spawnrate);

        StartCoroutine(spawnObject());
    }
}