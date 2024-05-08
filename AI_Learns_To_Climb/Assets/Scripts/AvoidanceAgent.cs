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
    [SerializeField] private AgentReinforcementLearningData _data;
    public AgentReinforcementLearningData ReinforcementData => _data;
    private Dictionary<GroundBlock, float> _groundBlocks;
    private GroundBlock _currentGroundblock;

    private int _currentHealth;

    private bool _collidedWithObstacle;
    private Vector3 _startPos;
    private Quaternion _startRot;


    private void Start()
    {
        _startPos = transform.localPosition;
        _startRot = transform.localRotation;
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = _startPos;

        _amountOfCollectiblesFound = 0;
        OnFoundCollectible?.Invoke();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        transform.rotation = Quaternion.identity;

        if (!_collidedWithObstacle)
        {
            AddReward(_data.ResultOnSurviveFrame);
        }

        if (CurrentEpisodeDuration > MaxDuration)
        {
            OnSucceededEpisode?.Invoke(_currentEpisodeDuration);
            _currentEpisodeDuration = 0;
            EndEpisode();
        }

        if(_amountOfCollectiblesFound == 0)
        {
            AddReward(_data.ResultOnNotFindingCollectibles);
        }
    }

    private void LateUpdate()
    {
        _collidedWithObstacle = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(_collidedWithObstacle);
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

        float moveSpeed = 13f;
        transform.localPosition += new Vector3(moveX, 0, 0) * Time.fixedDeltaTime * moveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        checkColliderTagOnEnter(collision.collider.tag);

        if (_currentHealth <= 0)
        {
            OnFailedEpisode?.Invoke(_currentEpisodeDuration);
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
                _collidedWithObstacle = true;
                break;
            case "Wall":
                _currentHealth -= _data.WallDamage;
                AddReward(_data.ResultOnWallHit);
                break;
            case "Collectible":
                AddReward(_data.ResultOnCollectibleHit);
                _amountOfCollectiblesFound++;
                OnFoundCollectible?.Invoke();
                break;
            case "HealthPotion":
                _currentHealth += _data.HealthPotionValue;
                _currentHealth = Mathf.Clamp(_currentHealth, 0, _data.MaxHealth);
                break;
        }
    }

    public void SetGroundBlockData(Dictionary<GroundBlock, float> pGroundBlocks)
    {
        _groundBlocks = pGroundBlocks;
    }
}