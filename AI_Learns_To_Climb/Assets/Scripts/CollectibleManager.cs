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
    }

    private void Update()
    {
        handleColllectibleSpawning();
    }

    private void handleColllectibleSpawning()
    {
        if (_collectiblePool.CurrentActiveItems == 0)
        {
            _collectiblePool.ActivateItem();
        }
    }
}
