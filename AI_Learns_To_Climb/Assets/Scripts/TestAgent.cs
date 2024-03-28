using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class TestAgent : Agent
{
     [SerializeField] private Transform _targetTransform;
     [SerializeField] private Material _winMat;
     [SerializeField] private Material _loseMat;
     [SerializeField] private Material _tookTooLongMat;
     [SerializeField] private MeshRenderer _floorRenderer = null;
     private Vector3 _startPos;

     public UnityEvent OnFinishedEpisode = new UnityEvent();
     public UnityEvent OnSucceededEpisode = new UnityEvent();

     private void Update()
     {
          if (StepCount >= MaxStep - 1)
          {
               _floorRenderer.material = _tookTooLongMat;
               OnFinishedEpisode.Invoke();
               EndEpisode();
          }
     }

     public override void OnEpisodeBegin()
     {
          transform.localPosition = new Vector3(Random.Range(-3f, 3f), 4.5f, Random.Range(-3.5f, 3.5f));
          _targetTransform.localPosition = new Vector3(Random.Range(-3.5f, 3.5f), 1f, Random.Range(-3.75f, 3.75f));
     }
     
     public override void CollectObservations(VectorSensor sensor)
     {
          sensor.AddObservation(transform.localPosition);
          sensor.AddObservation(_targetTransform.localPosition);
     }
     
     public override void OnActionReceived(ActionBuffers actions)
     {
          float moveX = actions.ContinuousActions[0];
          float moveZ = actions.ContinuousActions[1];

          float moveSpeed = 1.2f;
          transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
     }

     public override void Heuristic(in ActionBuffers actionsOut)
     {
          ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
          continuousActions[0] = Input.GetAxis("Horizontal");
          continuousActions[1] = Input.GetAxis("Vertical");
     }

     private void OnTriggerEnter(Collider other)
     {
          if (other.CompareTag("Wall"))
          {
               SetReward(-1f);
               _floorRenderer.material = _loseMat;
               OnFinishedEpisode.Invoke();
               EndEpisode();
          }

          if (other.CompareTag("Target"))
          {
               SetReward(+1f);
               _floorRenderer.material = _winMat;
               OnSucceededEpisode.Invoke();
               OnFinishedEpisode.Invoke();
               EndEpisode();
          }
     }
     
}
