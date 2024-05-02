using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    private ObjectPool _collectiblePool;

    private void Awake()
    {
        _collectiblePool = GetComponent<ObjectPool>();
        if (_collectiblePool == null)
        {
            transform.AddComponent<ObjectPool>();
        }

        StartCoroutine(handleColllectibleSpawning());
    }

    private IEnumerator handleColllectibleSpawning()
    {
        if (_collectiblePool.CurrentActiveItems == 0)
        {
            _collectiblePool.ActivateItem();
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(handleColllectibleSpawning());
    }
}
