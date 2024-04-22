using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;

public class RagdollController : Agent
{
    [SerializeField] private Transform _targetTransform;
    private Vector3 _startPos;

    [SerializeField] private List<BodyPartController> _bodyParts = new List<BodyPartController>();

    private void Start()
    {
        _startPos = transform.localPosition;
        foreach(BodyPartController part in _bodyParts)
        {
            part.OnGiveReward = receiveRewardForLimb;
        }
    }

    private void receiveRewardForLimb(BodyPart pPart, float pReward)
    {
        AddReward(pReward);
        EndEpisode();
    }

    public override void OnEpisodeBegin()
    {
        //transform.localPosition = new Vector3(Random.Range(0.75f, 8.75f), 2f, Random.Range(1.5f, 8.5f));
        //_targetTransform.localPosition = new Vector3(Random.Range(0.75f, 8.75f), 1f, Random.Range(1.5f, 8.5f));

        Debug.Log("Beginning episode");
        transform.localPosition = _startPos;
        foreach (BodyPartController part in _bodyParts)
        {
            //part.ResetLimb();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        foreach(BodyPartController part in _bodyParts)
        {
            sensor.AddObservation(part.transform.position);
            sensor.AddObservation(part.TouchingGround);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        for(int i = 0; i < _bodyParts.Count; i++) 
        {
            _bodyParts[i].AddForceToLimb(new Vector3(0, actions.ContinuousActions[0], 0));
        }
    }
}
