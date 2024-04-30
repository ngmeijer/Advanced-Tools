using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AvoidanceAgent : MLAgent
{
    [SerializeField] private RayPerceptionSensor _sensor;
    [SerializeField] private ObstacleManager _obstacleManager;

    private int _currentHealth;
    [SerializeField] [Range(1, 100)] private int _maxHealth;

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
        transform.localRotation = _startRot;
    }

    protected override void Update()
    {
        base.Update();

        if (!_collidedWithObstacle)
        {
            AddReward(0.05f);
        }

        if(StepCount >= MaxStep - 1)
        {
            AddReward(0.7f);
            Debug.Log("Agent successfull");
            OnSucceededEpisode?.Invoke(_currentEpisodeDuration);
            _currentEpisodeDuration = 0;
            EndEpisode();
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
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];

        float moveSpeed = 13f;
        transform.localPosition += new Vector3(moveX, 0, 0) * Time.deltaTime * moveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
        {
            AddReward(-0.2f); 
            _collidedWithObstacle = true;
            OnFailedEpisode?.Invoke(_currentEpisodeDuration);
            _currentEpisodeDuration = 0;
            EndEpisode();
        }

        if (collision.collider.CompareTag("Wall"))
        {
            AddReward(-0.2f);
            OnFailedEpisode?.Invoke(_currentEpisodeDuration);
            _currentEpisodeDuration = 0;
            EndEpisode();
        }
    }
}