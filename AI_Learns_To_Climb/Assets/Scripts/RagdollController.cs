using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using ProjectTools;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;

public class RagdollController : MLAgent
{
    [SerializeField] private Transform _targetTransform;
    private Vector3 _startPos;

    [SerializeField] private SerializableDictionary<BodyPart, BodyPartController> _bodyParts = new SerializableDictionary<BodyPart, BodyPartController>();
    public float GetBodyPartCount()
    {
        return _bodyParts.Count;
    }

    [SerializeField] private float _episodeMaxTime = 5;
    [SerializeField] private float _previousDistance = 100;

    private void Start()
    {
        _startPos = transform.localPosition;
        foreach(KeyValuePair<BodyPart, BodyPartController> part in _bodyParts)
        {
            part.Value.onGiveRewardOrPunishment += receiveRewardForLimb;
            part.Value.onEndEpisode += triggerEndEpisode;
        }
    }

    private void triggerEndEpisode()
    {
        OnFinishedEpisode?.Invoke(_episodeDuration);
        _episodeDuration = 0;
        EndEpisode();
    }

    private void receiveRewardForLimb(float pReward)
    {
        AddReward(pReward);
    }

    private void checkDistanceToTarget()
    {
        float currentDistanceToTarget = Vector3.Distance(transform.localPosition, _targetTransform.localPosition);
        if (currentDistanceToTarget < _previousDistance)
        {
            AddReward(0.1f);
            Debug.Log("Receiving reward. Got closer to target");
            _previousDistance = currentDistanceToTarget;
        }
        else
        {
            Debug.Log("Receiving punishment. Same distance or further from target");
            AddReward(-0.05f);
        }
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("Beginning episode");
        transform.localPosition = _startPos;
        List<BodyPartController> bodyParts = _bodyParts.Values.ToList();
        foreach (BodyPartController part in bodyParts)
        {
            part.ResetLimb();
        }
    }

    /// <summary>
    /// Limbs:
    ///     - position
    ///     - velocity
    ///     - angular velocity
    ///     - applied strength
    ///     - rotation
    /// Overall:
    ///     -
    ///     
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(_targetTransform.position);

        List<BodyPartController> bodyParts = _bodyParts.Values.ToList();
        foreach (BodyPartController part in bodyParts)
        {
            if(part == null)
            {
                Debug.Log($"Part is null.");
                return;
            }

            sensor.AddObservation((int)part.BodyPart);
            sensor.AddObservation(part.transform.localPosition);
            sensor.AddObservation(part.currentEulerJointRotation);
            sensor.AddObservation(part.TouchingGround);
            sensor.AddObservation(part.PunishAgentOnGroundTouch());
            sensor.AddObservation(part.rb.velocity);
            sensor.AddObservation(part.rb.angularVelocity);
            sensor.AddObservation(part.currentStrength);
        }   
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        //ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        //continuousActions[0] = Input.GetAxis("Horizontal");
        //continuousActions[1] = Input.GetAxis("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int outputIndex = -1;
        var continuousActions = actions.ContinuousActions;

        _bodyParts[BodyPart.TORSO].SetJointTargetRotation(continuousActions[++outputIndex], continuousActions[++outputIndex], continuousActions[++outputIndex]);
        _bodyParts[BodyPart.HIPS].SetJointTargetRotation(continuousActions[++outputIndex], continuousActions[++outputIndex], continuousActions[++outputIndex]);

        _bodyParts[BodyPart.L_UPPER_LEG].SetJointTargetRotation(continuousActions[++outputIndex], continuousActions[++outputIndex], 0);
        _bodyParts[BodyPart.R_UPPER_LEG].SetJointTargetRotation(continuousActions[++outputIndex], continuousActions[++outputIndex], 0);

        _bodyParts[BodyPart.L_LOWER_LEG].SetJointTargetRotation(continuousActions[++outputIndex], 0, 0);
        _bodyParts[BodyPart.R_LOWER_LEG].SetJointTargetRotation(continuousActions[++outputIndex], 0, 0);

        _bodyParts[BodyPart.L_FOOT].SetJointTargetRotation(continuousActions[++outputIndex], continuousActions[++outputIndex], continuousActions[++outputIndex]);
        _bodyParts[BodyPart.R_FOOT].SetJointTargetRotation(continuousActions[++outputIndex], continuousActions[++outputIndex], continuousActions[++outputIndex]);

        _bodyParts[BodyPart.L_UPPER_ARM].SetJointTargetRotation(continuousActions[++outputIndex], continuousActions[++outputIndex], 0);
        _bodyParts[BodyPart.R_UPPER_ARM].SetJointTargetRotation(continuousActions[++outputIndex], continuousActions[++outputIndex], 0);

        _bodyParts[BodyPart.L_LOWER_ARM].SetJointTargetRotation(continuousActions[++outputIndex], 0, 0);
        _bodyParts[BodyPart.R_LOWER_ARM].SetJointTargetRotation(continuousActions[++outputIndex], 0, 0);

        _bodyParts[BodyPart.HEAD].SetJointTargetRotation(continuousActions[++outputIndex], continuousActions[++outputIndex], 0);

        List<BodyPartController> parts = _bodyParts.Values.ToList();
        for (int i = 0; i < parts.Count; i++)
        {
            parts[i].SetJointStrength(continuousActions[++outputIndex]);
        }
    }
}