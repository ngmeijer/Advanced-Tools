﻿using System;
using System.Collections.Generic;
using TMPro;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Events;

public abstract class MLAgent : Agent
{
    public UnityEvent<float, float> OnFailedEpisode = new UnityEvent<float, float>();
    public UnityEvent<float, float> OnSucceededEpisode = new UnityEvent<float, float>();
    public UnityEvent<MLAgent> OnEndEpisode = new UnityEvent<MLAgent>();
    public UnityEvent<WeaponState, GameObject, MLAgent> OnPickedUpWeapon = new UnityEvent<WeaponState, GameObject, MLAgent>();
    public UnityEvent OnFoundCollectible = new UnityEvent();
    public UnityEvent<MLAgent, MLAgent> OnAgentKill = new UnityEvent<MLAgent, MLAgent>();

    public Dictionary<MLAgent, bool> EnemiesWeaponData = new Dictionary<MLAgent, bool>();

    public bool WeaponAvailable = true;
    protected WeaponState _carryingWeapon = WeaponState.DROPPED_WEAPON;
    public WeaponState AgentWeaponState => _carryingWeapon;

    protected GameObject _weaponCollidedWith;

    [SerializeField] protected GameObject _weaponVisualizer;
    [SerializeField] protected AgentReinforcementLearningData _data;
    public AgentReinforcementLearningData TrainingSettings => _data;
    [SerializeField] private TextMeshProUGUI _agentIDOnBodyText;

    protected int _episodeID = -1;
    public int EpisodeID => _episodeID;

    protected float _cumulativeReward;
    public float CumulativeReward => _cumulativeReward;

    protected float _currentEpisodeDuration;
    public float CurrentEpisodeDuration => _currentEpisodeDuration;

    protected int _agentID;
    public int ID => _agentID;

    protected int _amountOfCollectiblesFound;
    public int CollectiblesFound => _amountOfCollectiblesFound;

    [SerializeField] private int _maxDuration = 10;
    public float MaxDuration => _maxDuration;

    protected int _currentHealth;
    public int CurrentHealth => _currentHealth;

    protected int _rockHitCount;
    public int RockHitCount => _rockHitCount;

    protected int _healthPotionsCount;
    public int HealthPotionCount => _healthPotionsCount;

    protected int _killCount;
    public int KillCount => _killCount;

    protected int _deathCount;
    public int DeathCount => _deathCount;

    protected bool _collidedWithDamageDealer;
    public bool CollidedWithDamageDealer
    {
        get => _collidedWithDamageDealer;
        set => _collidedWithDamageDealer = value;
    }

    protected Vector3 _newRandomPosition;

    protected virtual void Start()
    {
        WeaponAvailable = true;
    }

    protected virtual void FixedUpdate()
    {
        _currentEpisodeDuration += Time.fixedDeltaTime;
    }

    public void ModifyHealth(int pDelta)
    {
        _currentHealth += pDelta;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _data.MaxHealth);
    }

    public void ModifyCollectibleCount(int pCount)
    {
        _amountOfCollectiblesFound += pCount;
    }

    public void ModifyObstacleHitCount(int pCount)
    {
        _rockHitCount += pCount;
    }

    public void ModifyHealthPotionCount(int pCount)
    {
        _healthPotionsCount += pCount;
    }

    public void ReceiveNewRandomPosition(Vector3 pPosition)
    {
        _newRandomPosition = pPosition;
    }

    public void SetID(int pID)
    {
        _agentID = pID;
        _agentIDOnBodyText.SetText(pID.ToString());
    }
}