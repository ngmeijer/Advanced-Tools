﻿using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ObjectPool))]
public abstract class SpawnableManager : MonoBehaviour
{
    protected ObjectPool _pool;
    [SerializeField] protected int _maxSpawnableCountInArea = 10;
    [Tooltip("How often should a new obstacle spawn in seconds?")][SerializeField] protected float _spawnrate;
    [SerializeField] protected int _spawnMultiplier = 3;

    protected virtual void Awake()
    {
        _pool = GetComponent<ObjectPool>();

        _maxSpawnableCountInArea = Math.Clamp(_maxSpawnableCountInArea, 0, _pool.ItemInStorageCount);
    }

    protected abstract IEnumerator spawnObject();
}