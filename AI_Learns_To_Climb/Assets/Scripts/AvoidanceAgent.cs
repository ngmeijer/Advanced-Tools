using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public enum WeaponState
{
    PICKED_UP,
    DROPPED
}

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
        transform.localPosition = _startPos;

        _amountOfCollectiblesFound = 0;
        //OnFoundCollectible?.Invoke();

        handleWeapon(WeaponState.DROPPED);
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

    /// <summary>
    /// If <paramref name="pActive"/> is TRUE, then the weapon has been picked up. The visualizer on the agent should be enabled and the instance in the world disabled.
    /// If <paramref name="pActive"/> is FALSE, then the agent has dropped the weapon. The visualizer on the agent should be disabled and the instance in the world enabled again.
    /// </summary>
    /// <param name="pActive"></param>
    private void handleWeapon(WeaponState pState)
    {
        _carryingWeapon = pState;
        if (pState == WeaponState.PICKED_UP)
        {
            _weaponVisualizer.SetActive(true);
            OnPickedUpWeapon?.Invoke(pState, _weaponCollidedWith);
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

        //if (_groundBlocks == null)
        //    return;

        //Debug.Assert(sensor.ObservationSize() != _groundBlocks.Count + 6, $"Observation Size ({sensor.ObservationSize()}) of AvoidanceAgent is a different size than the size of " +
        //    $"GroundBlocks dictionary * 2 (Size {_groundBlocks.Count} = {_groundBlocks.Count * 2}) + x other observations (check the script for x).");

        //foreach (KeyValuePair<GroundBlock, float> groundBlock in _groundBlocks)
        //{
        //    sensor.AddObservation(groundBlock.Key.transform.localPosition);
        //    sensor.AddObservation(groundBlock.Value);
        //}
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
        checkColliderTagOnEnter(collision.collider.gameObject);

        if (_currentHealth <= 0)
        {
            OnFailedEpisode?.Invoke(_currentEpisodeDuration, GetCumulativeReward());
            _currentEpisodeDuration = 0;
            EndEpisode();
        }
    }

    private void checkColliderTagOnEnter(GameObject pCollisionObject)
    {
        switch (pCollisionObject.tag)
        {
            case "Obstacle":
                handleObstacleCollision();
                break;
            case "Wall":
                handleWallCollision();
                break;
            case "Collectible":
                handleCollectibleCollision();
                break;
            case "HealthPotion":
                handleHealthPotionCollision();
                break;
            case "Weapon":
                handleWeaponCollision(pCollisionObject);
                break;
            case "Agent":
                handleDamageDealing(pCollisionObject);
                break;
            case "InvisibleBarrier":
                EndEpisode();
                break;
        }
    }

    private void handleHealthPotionCollision()
    {
        _currentHealth += _data.HealthPotionValue;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _data.MaxHealth);
    }

    private void handleWeaponCollision(GameObject pCollisionObject)
    {
        if (_carryingWeapon == WeaponState.PICKED_UP)
            return;

        _weaponCollidedWith = pCollisionObject;
        handleWeapon(WeaponState.PICKED_UP);
        AddReward(_data.ResultOnWeaponPickup);
    }

    private void handleDamageDealing(GameObject pCollisionObject)
    {
        if (_carryingWeapon != WeaponState.PICKED_UP)
            return;

        MLAgent enemy = pCollisionObject.GetComponent<MLAgent>();
        enemy.ReceiveDamage(_data.WeaponDamage);
        if (enemy.CurrentHealth > 0)
        {
            AddReward(_data.ResultOnDamagedEnemy);
            Debug.Log("Damaged enemy.");
            return;
        }

        _killCount += 1;
        Debug.Log("Killed enemy");
        AddReward(_data.ResultOnKilledEnemy);
        OnHasBeenKilledByAgent.Invoke(enemy, this);
    }

    private void handleDamageReceiving(MLAgent pCollisionAgent)
    {
        _currentHealth -= _data.WeaponDamage;

        if (CurrentHealth <= 0)
        {
            _deathCount += 1;
            Debug.Log("Died in combat");
            OnHasBeenKilledByAgent.Invoke(this, pCollisionAgent);
        }
        return;

    }

    private void handleObstacleCollision()
    {
        _currentHealth -= _data.ObstacleDamage;
        AddReward(_data.ResultOnObstacleHit);
        _collidedWithDamageDealer = true;
    }

    private void handleWallCollision()
    {
        _currentHealth -= _data.WallDamage;
        AddReward(_data.ResultOnWallHit);
        _collidedWithDamageDealer = true;
    }

    private void handleCollectibleCollision()
    {
        _amountOfCollectiblesFound++;
        AddReward(_data.ResultOnCollectibleHit);
        OnFoundCollectible?.Invoke();
    }

    public void SetGroundBlockData(Dictionary<GroundBlock, float> pGroundBlocks)
    {
        _groundBlocks = pGroundBlocks;
    }
}