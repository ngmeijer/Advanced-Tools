using System;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Events;

public abstract class MLAgent : Agent
{
    public UnityEvent<float, float> OnFailedEpisode = new UnityEvent<float, float>();
    public UnityEvent<float, float> OnSucceededEpisode = new UnityEvent<float, float>();
    public UnityEvent<bool> OnPickedUpWeapon = new UnityEvent<bool>();
    public UnityEvent OnFoundCollectible = new UnityEvent();

    public bool WeaponAvailable = true;
    protected bool _carryingWeapon;

    [SerializeField] protected GameObject _weapon;
    [SerializeField] protected AgentReinforcementLearningData _data;
    public AgentReinforcementLearningData TrainingSettings => _data;

    protected float _currentEpisodeDuration;
    public float CurrentEpisodeDuration => _currentEpisodeDuration;

    protected int _amountOfCollectiblesFound;
    public int CollectiblesFound => _amountOfCollectiblesFound;

    [SerializeField] private int _maxDuration = 30;
    public float MaxDuration => _maxDuration;

    protected int _currentHealth;
    public int CurrentHealth => _currentHealth;

    protected virtual void Start()
    {
        WeaponAvailable = true;
    }
    

    protected virtual void FixedUpdate()
    {
        _currentEpisodeDuration += Time.fixedDeltaTime;
    }

    public void DisableWeapon()
    {
        _weapon.SetActive(false);
        _carryingWeapon = false;
    }
}