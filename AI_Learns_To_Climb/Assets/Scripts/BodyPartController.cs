using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyPart
{
    HEAD,
    TORSO,
    HIPS,

    L_UPPER_LEG,
    L_LOWER_LEG,
    L_FOOT,

    L_UPPER_ARM,
    L_LOWER_ARM,
    L_HAND,

    R_UPPER_LEG,
    R_LOWER_LEG,
    R_FOOT,

    R_UPPER_ARM,
    R_LOWER_ARM,
    R_HAND,
}

public enum PUNISH_OR_REWARD
{
    PUNISH,
    REWARD
}


public class BodyPartController : MonoBehaviour
{
    public Rigidbody rb;
    public ConfigurableJoint joint;
    [SerializeField] private RagdollSettings _ragdollSettings;
    [SerializeField] private BodyPart _bodyPart;
    public BodyPart BodyPart { get { return _bodyPart; } }

    public float ForceMultiplier()
    {
        float forceMultiplier = 0f;
        if (_ragdollSettings.forceMultiplierOnLimbs.TryGetValue(_bodyPart, out float _forceMultiplier))
        {
            forceMultiplier = _forceMultiplier;
        }
        return forceMultiplier;
    }

    public delegate void OnGiveReward(float pReward);
    public OnGiveReward onGiveRewardOrPunishment;

    public delegate void OnEndEpisode();
    public OnEndEpisode onEndEpisode;

    protected Vector3 _startPos;
    protected Quaternion _startRot;

    public float currentXNormalizedRot;
    public float currentYNormalizedRot;
    public float currentZNormalizedRot;
    public Vector3 currentEulerJointRotation;

    public bool PunishAgentOnGroundTouch()
    {
        _ragdollSettings.rewardsOnGroundHit.TryGetValue(_bodyPart, out PunishOrRewardContainer data);
        if (data.Type == PUNISH_OR_REWARD.PUNISH)
            return true;

        return false;
    }
    [SerializeField] private bool _endEpisodeOnGroundTouch;

    [SerializeField] private bool _rewardAgentOnHeightCheck = true;
    [SerializeField][Range(0f, 1f)] private float _rewardHeightCheck;

    [SerializeField] private bool _touchingGround;
    public bool TouchingGround => _touchingGround;

    public float currentStrength;

    private void Awake()
    {
        if (rb == null)
        {
            Debug.Log($"A Rigidbody has not been referenced for {gameObject.name}.");
            rb = GetComponent<Rigidbody>();
        }

        _startPos = transform.localPosition;
        _startRot = transform.localRotation;
    }

    public void SetLimbRotation(Vector3 pEulerRotation)
    {
        Quaternion converted = Quaternion.Euler(pEulerRotation.x, pEulerRotation.y, pEulerRotation.z);
        joint.targetRotation = converted;
    }

    public void ResetLimb()
    {
        transform.localPosition = _startPos; 
        transform.localRotation = _startRot;

        _touchingGround = false;
    }

    public Vector3 GetVelocity() => rb.velocity;
    public Vector3 GetAngularVelocity() => rb.angularVelocity;

    private void Update()
    {
        if (!TouchingGround && _rewardAgentOnHeightCheck)
        {
            onGiveRewardOrPunishment?.Invoke(_rewardHeightCheck);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            _touchingGround = true;

            _ragdollSettings.rewardsOnGroundHit.TryGetValue(_bodyPart, out PunishOrRewardContainer groundHitReaction);
            Debug.Log($"Granting reward: {groundHitReaction.Amount} because {_bodyPart} hit the ground.");
            onGiveRewardOrPunishment?.Invoke(groundHitReaction.Amount);

            if (_endEpisodeOnGroundTouch)
            {
                onEndEpisode?.Invoke();
            }
        }
    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            if (!PunishAgentOnGroundTouch())
            {
                _ragdollSettings.rewardsOnGroundHit.TryGetValue(_bodyPart, out PunishOrRewardContainer groundHitReaction);
                Debug.Log($"Granting reward in OnStay: {groundHitReaction.Amount} because {_bodyPart} hit the ground.");
                onGiveRewardOrPunishment?.Invoke(groundHitReaction.Amount);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            _touchingGround = false;
    }

    public void SetJointTargetRotation(float x, float y, float z)
    {
        x = (x + 1f) * 0.5f;
        y = (y + 1f) * 0.5f;
        z = (z + 1f) * 0.5f;

        var xRot = Mathf.Lerp(joint.lowAngularXLimit.limit, joint.highAngularXLimit.limit, x);
        var yRot = Mathf.Lerp(-joint.angularYLimit.limit, joint.angularYLimit.limit, y);
        var zRot = Mathf.Lerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, z);

        currentXNormalizedRot =
            Mathf.InverseLerp(joint.lowAngularXLimit.limit, joint.highAngularXLimit.limit, xRot);
        currentYNormalizedRot = Mathf.InverseLerp(-joint.angularYLimit.limit, joint.angularYLimit.limit, yRot);
        currentZNormalizedRot = Mathf.InverseLerp(-joint.angularZLimit.limit, joint.angularZLimit.limit, zRot);

        joint.targetRotation = Quaternion.Euler(xRot, yRot, zRot);
        currentEulerJointRotation = new Vector3(xRot, yRot, zRot);
    }

    public void SetJointStrength(float strength)
    {
        var rawVal = (strength + 1f) * 0.5f * 20000;
        var jd = new JointDrive
        {
            positionSpring = 40000,
            positionDamper = 5000,
            maximumForce = 20000
        };
        joint.slerpDrive = jd;
        currentStrength = jd.maximumForce;
    }
}
