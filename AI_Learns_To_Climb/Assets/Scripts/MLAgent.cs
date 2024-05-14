using System;
using TMPro;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Events;

public abstract class MLAgent : Agent
{
    public UnityEvent<float, float> OnFailedEpisode = new UnityEvent<float, float>();
    public UnityEvent<float, float> OnSucceededEpisode = new UnityEvent<float, float>();
    public UnityEvent<WeaponState, GameObject> OnPickedUpWeapon = new UnityEvent<WeaponState, GameObject>();
    public UnityEvent OnFoundCollectible = new UnityEvent();
    public UnityEvent<MLAgent, MLAgent> OnHasBeenKilledByAgent = new UnityEvent<MLAgent, MLAgent>();

    public bool WeaponAvailable = true;
    protected WeaponState _carryingWeapon;

    protected GameObject _weaponCollidedWith;

    [SerializeField] protected GameObject _weaponVisualizer;
    [SerializeField] protected AgentReinforcementLearningData _data;
    public AgentReinforcementLearningData TrainingSettings => _data;
    [SerializeField] private TextMeshProUGUI _agentIDOnBodyText;

    protected float _currentEpisodeDuration;
    public float CurrentEpisodeDuration => _currentEpisodeDuration;

    protected int _amountOfCollectiblesFound;
    public int CollectiblesFound => _amountOfCollectiblesFound;

    [SerializeField] private int _maxDuration = 30;
    public float MaxDuration => _maxDuration;

    protected int _currentHealth;
    public int CurrentHealth => _currentHealth;

    protected int _killCount;
    public int KillCount => _killCount;

    protected int _deathCount;
    public int DeathCount => _deathCount;

    protected virtual void Start()
    {
        WeaponAvailable = true;
    }
    

    protected virtual void FixedUpdate()
    {
        _currentEpisodeDuration += Time.fixedDeltaTime;
    }

    public void ReceiveDamage(int pDamage)
    {
        _currentHealth -= pDamage;
        AddReward(_data.ResultOnDamageReceive);
        if (CurrentHealth <= 0)
        {
            _deathCount += 1;
            Debug.Log("Died in combat");
        }
    }

    public void SetID(int pID)
    {
        _agentIDOnBodyText.SetText(pID.ToString());
    }

    public void DisableWeapon()
    {
        _weaponVisualizer.SetActive(false);
        _carryingWeapon = WeaponState.DROPPED;
    }
}