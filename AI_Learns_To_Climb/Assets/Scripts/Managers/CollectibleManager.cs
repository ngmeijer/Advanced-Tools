using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    private ObjectPool _collectiblePool;
    [SerializeField] private int _maxCollectibleCountInTrainingArea = 3;

    private void Awake()
    {
        _collectiblePool = GetComponent<ObjectPool>();
        if (_collectiblePool == null)
        {
            transform.AddComponent<ObjectPool>();
        }

        _maxCollectibleCountInTrainingArea = Math.Clamp(_maxCollectibleCountInTrainingArea, 0, _collectiblePool.ItemInStorageCount);
        StartCoroutine(handleColllectibleSpawning());
    }

    private IEnumerator handleColllectibleSpawning()
    {
        if (_collectiblePool.CurrentActiveItems < _maxCollectibleCountInTrainingArea)
        {
            _collectiblePool.ActivateItem();
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(handleColllectibleSpawning());
    }
}
