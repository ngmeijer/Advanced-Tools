using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public enum WeaponState
{
    CARRY_WEAPON,
    DROPPED_WEAPON
}

public class CombatMLAgent : MLAgent
{
    [SerializeField] private RayPerceptionSensor _sensor;
    public AgentReinforcementLearningData ReinforcementData => _data;
    private Dictionary<GroundBlock, float> _groundBlocks;
    private GroundBlock _currentGroundblock;
    [SerializeField] private AgentCollisionManager _collisionManager;

    private bool _collidedWithDamageDealer;
    private Vector3 _startPos;
    private Quaternion _startRot;


    protected override void OnEnable()
    {
        base.OnEnable();
        _collisionManager.CombatAgent = this;
        _startPos = transform.localPosition;
        _startRot = transform.localRotation;
    }

    public override void OnEpisodeBegin()
    {
        _currentHealth = _data.MaxHealth;
        transform.localPosition = _newRandomPosition;
        _amountOfCollectiblesFound = 0;
    }

    public void TriggerEpisodeEnd()
    {
        OnEndEpisode?.Invoke(this);
        _currentEpisodeDuration = 0;
        SetWeaponState(WeaponState.DROPPED_WEAPON);
        EndEpisode();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        transform.rotation = Quaternion.identity;

        if (!_collidedWithDamageDealer)
        {
            AddReward(_data.ResultOnSurviveFrame);
        }

        if (CurrentEpisodeDuration >= MaxDuration)
        {
            OnSucceededEpisode?.Invoke(_currentEpisodeDuration, GetCumulativeReward());
            TriggerEpisodeEnd();
        }
    }

    private void LateUpdate()
    {
        if (CurrentHealth <= 0)
            TriggerEpisodeEnd();
        _collidedWithDamageDealer = false;
    }

    /// <summary>
    /// Switches weapon on/off and notifies manager about it.
    /// </summary>
    /// <param name="pActive"></param>
    public void SetWeaponState(WeaponState pState, GameObject pWorldspaceWeapon = null)
    {
        _carryingWeapon = pState;
        if (pState == WeaponState.CARRY_WEAPON)
        {
            _weaponCollidedWith = pWorldspaceWeapon;
            _weaponVisualizer.SetActive(true);
            OnPickedUpWeapon?.Invoke(pState, _weaponCollidedWith);
        }
        else if (pState == WeaponState.DROPPED_WEAPON)
        {
            _weaponVisualizer.SetActive(false);
            OnPickedUpWeapon?.Invoke(pState, _weaponCollidedWith);
            _weaponCollidedWith = null;
        }
    }

    public void UpdateKD(int pKillCount, int pDeathCount)
    {
        _killCount += pKillCount;
        _deathCount += pDeathCount;
    }

    public void ReceiveWeaponDamage(int pDamage)
    {
        _currentHealth -= pDamage;
        AddReward(_data.ResultOnDamageReceive);
        if (CurrentHealth <= 0)
        {
            UpdateKD(0, 1);
            Debug.Log("Died in combat");
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(_collidedWithDamageDealer);
        sensor.AddObservation(_amountOfCollectiblesFound);
        sensor.AddObservation(_currentHealth);
        sensor.AddObservation((float)_carryingWeapon);
        sensor.AddObservation(WeaponAvailable);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        Vector3 movementVec = new Vector3(moveX, 0, moveZ);

        transform.localPosition += movementVec * Time.deltaTime * _data.MoveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        _collisionManager.CheckColliderTagOnEnter(collision.collider.gameObject);

        if (_currentHealth <= 0)
        {
            OnFailedEpisode?.Invoke(_currentEpisodeDuration, GetCumulativeReward());
            TriggerEpisodeEnd();
        }
    }

    public void SetGroundBlockData(Dictionary<GroundBlock, float> pGroundBlocks)
    {
        _groundBlocks = pGroundBlocks;
    }
}