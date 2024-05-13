using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectibleManager : SpawnableManager
{
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(spawnObject());
    }

    protected override IEnumerator spawnObject()
    {
        if (_pool.CurrentActiveItems < _maxSpawnableCountInArea)
        {
            _pool.ActivateItem();
        }

        yield return new WaitForSeconds(_spawnrate);

        StartCoroutine(spawnObject());
    }
}
