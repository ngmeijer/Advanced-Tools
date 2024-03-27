using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class TestAgent : Agent
{
     [SerializeField] private Transform _targetTransform;

     public override void OnEpisodeBegin()
     {
          transform.position = Vector3.zero;
     }
     
     public override void CollectObservations(VectorSensor sensor)
     {
          sensor.AddObservation(transform.position);
          sensor.AddObservation(_targetTransform.position);
     }
     
     public override void OnActionReceived(ActionBuffers actions)
     {
          float moveX = actions.ContinuousActions[0];
          float moveZ = actions.ContinuousActions[1];

          float moveSpeed = 1f;
          transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
     }

     public override void Heuristic(in ActionBuffers actionsOut)
     {
          
     }

     private void OnTriggerEnter(Collider other)
     {
          float reward = 0f;
          if (other.CompareTag("Wall"))
               reward = -1f;

          if (other.CompareTag("Target"))
               reward = 1f;
          
          SetReward(reward);
          EndEpisode();
     }
}
