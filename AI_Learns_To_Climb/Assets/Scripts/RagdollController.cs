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

    [SerializeField] private float _forceMultiplier;
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

    protected override void Update()
    {
        base.Update();

        foreach(KeyValuePair<BodyPart, BodyPartController> pair in _bodyParts)
        {
            pair.Value.ForceMultiplier = _forceMultiplier;
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
        if (pReward < 0)
        {
            SetReward(-1);
        }
        else AddReward(pReward);
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
            if (part == null)
            {
                Debug.Log($"Part is null.");
                return;
            }

            sensor.AddObservation((int)part.BodyPart);
            
            sensor.AddObservation(part.joint.lowAngularXLimit.limit);
            sensor.AddObservation(part.joint.highAngularXLimit.limit);
            
            sensor.AddObservation(-part.joint.angularYLimit.limit);
            sensor.AddObservation(part.joint.angularYLimit.limit);

            sensor.AddObservation(-part.joint.angularZLimit.limit);
            sensor.AddObservation(part.joint.angularZLimit.limit);

            sensor.AddObservation(part.transform.position);
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

        _bodyParts[BodyPart.L_UPPER_ARM].SetJointTargetRotation(continuousActions[++outputIndex], 0, continuousActions[++outputIndex]);
        _bodyParts[BodyPart.R_UPPER_ARM].SetJointTargetRotation(continuousActions[++outputIndex], 0, continuousActions[++outputIndex]);

        _bodyParts[BodyPart.L_LOWER_ARM].SetJointTargetRotation(0, 0, continuousActions[++outputIndex]);
        _bodyParts[BodyPart.R_LOWER_ARM].SetJointTargetRotation(0, 0, continuousActions[++outputIndex]);

        _bodyParts[BodyPart.HEAD].SetJointTargetRotation(continuousActions[++outputIndex], continuousActions[++outputIndex], continuousActions[++outputIndex]);

        //26x output index

        List<BodyPartController> parts = _bodyParts.Values.ToList();
        for (int i = 0; i < parts.Count; i++)
        {
            parts[i].SetJointStrength(continuousActions[++outputIndex]);
        }

        //39x output index
    }
}

public static class ConfigurableJointExtensions
{
    /// <summary>
    /// Sets a joint's targetRotation to match a given local rotation.
    /// The joint transform's local rotation must be cached on Start and passed into this method.
    /// </summary>
    public static void SetTargetRotationLocal(this ConfigurableJoint joint, Quaternion targetLocalRotation, Quaternion startLocalRotation)
    {
        if (joint.configuredInWorldSpace)
        {
            Debug.LogError("SetTargetRotationLocal should not be used with joints that are configured in world space. For world space joints, use SetTargetRotation.", joint);
        }
        SetTargetRotationInternal(joint, targetLocalRotation, startLocalRotation, Space.Self);
    }

    /// <summary>
    /// Sets a joint's targetRotation to match a given world rotation.
    /// The joint transform's world rotation must be cached on Start and passed into this method.
    /// </summary>
    public static void SetTargetRotation(this ConfigurableJoint joint, Quaternion targetWorldRotation, Quaternion startWorldRotation)
    {
        if (!joint.configuredInWorldSpace)
        {
            Debug.LogError("SetTargetRotation must be used with joints that are configured in world space. For local space joints, use SetTargetRotationLocal.", joint);
        }
        SetTargetRotationInternal(joint, targetWorldRotation, startWorldRotation, Space.World);
    }

    static void SetTargetRotationInternal(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
    {
        // Calculate the rotation expressed by the joint's axis and secondary axis
        var right = joint.axis;
        var forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
        var up = Vector3.Cross(forward, right).normalized;
        Quaternion worldToJointSpace = Quaternion.LookRotation(forward, up);

        // Transform into world space
        Quaternion resultRotation = Quaternion.Inverse(worldToJointSpace);

        // Counter-rotate and apply the new local rotation.
        // Joint space is the inverse of world space, so we need to invert our value
        if (space == Space.World)
        {
            resultRotation *= startRotation * Quaternion.Inverse(targetRotation);
        }
        else
        {
            resultRotation *= Quaternion.Inverse(targetRotation) * startRotation;
        }

        // Transform back into joint space
        resultRotation *= worldToJointSpace;

        // Set target rotation to our newly calculated rotation
        joint.targetRotation = resultRotation;
    }
}