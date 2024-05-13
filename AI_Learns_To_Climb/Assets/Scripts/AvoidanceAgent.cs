using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AvoidanceAgent : MLAgent
{
    [SerializeField] private RayPerceptionSensor _sensor;
    public AgentReinforcementLearningData ReinforcementData => _data;
    private Dictionary<GroundBlock, float> _groundBlocks;
    private GroundBlock _currentGroundblock;

    private bool _collidedWithDamageDealer;
    private Vector3 _startPos;
    private Quaternion _startRot;


    protected override void OnEnable()
    {
        base.OnEnable();
        _startPos = transform.localPosition;
        _startRot = transform.localRotation;
    }

    public override void OnEpisodeBegin()
    {
        _currentHealth = _data.MaxHealth;
        //transform.localPosition = _startPos;

        _amountOfCollectiblesFound = 0;
        OnFoundCollectible?.Invoke();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!_collidedWithDamageDealer)
        {
            AddReward(_data.ResultOnSurviveFrame);
        }

        if (CurrentEpisodeDuration >= MaxDuration)
        {
            OnSucceededEpisode?.Invoke(_currentEpisodeDuration, GetCumulativeReward());
            _currentEpisodeDuration = 0;
            EndEpisode();
        }

        //if(_amountOfCollectiblesFound == 0)
        //{
        //    AddReward(_data.ResultOnNotFindingCollectibles);
        //}
    }

    private void LateUpdate()
    {
        _collidedWithDamageDealer = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(this.transform.localRotation.eulerAngles);
        sensor.AddObservation(_collidedWithDamageDealer);
        sensor.AddObservation(_amountOfCollectiblesFound);
        sensor.AddObservation(_currentHealth);

        if (_groundBlocks == null)
            return;

        Debug.Assert(sensor.ObservationSize() != _groundBlocks.Count + 6, $"Observation Size ({sensor.ObservationSize()}) of AvoidanceAgent is a different size than the size of " +
            $"GroundBlocks dictionary * 2 (Size {_groundBlocks.Count} = {_groundBlocks.Count * 2}) + x other observations (check the script for x).");

        foreach (KeyValuePair<GroundBlock, float> groundBlock in _groundBlocks)
        {
            sensor.AddObservation(groundBlock.Key.transform.localPosition);
            sensor.AddObservation(groundBlock.Value);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float rotY = actions.ContinuousActions[2];

        Vector3 movementVec = new Vector3(moveX, 0, moveZ);

        transform.localPosition += movementVec * Time.deltaTime * _data.MoveSpeed;
        transform.Rotate(0, rotY, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        checkColliderTagOnEnter(collision.collider.tag);

        if (_currentHealth <= 0)
        {
            OnFailedEpisode?.Invoke(_currentEpisodeDuration, GetCumulativeReward());
            _currentEpisodeDuration = 0;
            EndEpisode();
        }
    }

    private void checkColliderTagOnEnter(string pTag)
    {
        switch (pTag)
        {
            case "Obstacle":
                _currentHealth -= _data.ObstacleDamage;
                AddReward(_data.ResultOnObstacleHit);
                _collidedWithDamageDealer = true;
                break;
            case "Wall":
                _currentHealth -= _data.WallDamage;
                AddReward(_data.ResultOnWallHit);
                _collidedWithDamageDealer = true;
                break;
            case "Collectible":
                _amountOfCollectiblesFound++;
                AddReward(_data.ResultOnCollectibleHit);
                OnFoundCollectible?.Invoke();
                break;
            case "HealthPotion":
                _currentHealth += _data.HealthPotionValue;
                _currentHealth = Mathf.Clamp(_currentHealth, 0, _data.MaxHealth);
                break;
            case "Weapon":
                _currentHealth -= _data.WeaponDamage;
                AddReward(_data.ResultOnWeaponHit);
                break;
            case "InvisibleBarrier":
                EndEpisode();
                break;
        }
    }

    public void SetGroundBlockData(Dictionary<GroundBlock, float> pGroundBlocks)
    {
        _groundBlocks = pGroundBlocks;
    }
}