using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : SpawnableManager
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
