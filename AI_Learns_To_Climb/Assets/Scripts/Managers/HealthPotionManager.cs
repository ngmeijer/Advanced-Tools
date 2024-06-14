using System.Collections;
using UnityEngine;

public class HealthPotionManager : SpawnableManager
{
    protected override void Awake()
    {
        base.Awake();

        if (_enableSpawning)
            StartCoroutine(spawnObject());
    }

    public void SetListeners(MLAgent pAgent)
    {
        Agent = pAgent;
        Agent.OnFailedEpisode.AddListener(resetCollectibles);
        Agent.OnSucceededEpisode.AddListener(resetCollectibles);
    }

    private void resetCollectibles(float arg0, float arg1)
    {
        _pool.DeactivateAlltems();
        StartCoroutine(spawnObject());
    }

    protected override void disableAllObjects()
    {
        if (_pool.CurrentActiveItems == 0)
            return;

        _pool.DeactivateAlltems();
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

    public void RandomizePotionPositions()
    {
        _pool.RandomizeItemPositions();
    }
}