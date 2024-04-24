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

    UPPER_LEG,
    LOWER_LEG,
    FOOT,

    UPPER_ARM,
    LOWER_ARM,
    HAND,
}

public enum PUNISH_OR_REWARD 
{ 
    PUNISH, 
    REWARD
}


public class BodyPartController : MonoBehaviour
{
    [SerializeField] protected Rigidbody rb;
    [SerializeField] private RagdollSettings _ragdollSettings;
    [SerializeField] private BodyPart _bodyPart;

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
    public OnGiveReward onGiveReward;

    public delegate void OnEndEpisode();
    public OnEndEpisode onEndEpisode;

    protected Vector3 _startPos;
    protected Quaternion _startRot;

    public bool PunishAgentOnGroundTouch() {
        _ragdollSettings.rewardsOnGroundHit.TryGetValue(_bodyPart, out CustomKeyValuePair data);
        if (data.Type == PUNISH_OR_REWARD.PUNISH)
            return true;

        return false;
    }
    [SerializeField] private bool _endEpisodeOnGroundTouch;

    [SerializeField] private bool _rewardAgentOnHeightCheck = true;
    [SerializeField][Range(0f, 1f)] private float _rewardHeightCheck;

    [SerializeField] private bool _touchingGround;
    public bool TouchingGround => _touchingGround;

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

    public void AddForceToLimb(Vector3 pForce)
    {
        _ragdollSettings.forceMultiplierOnLimbs.TryGetValue(_bodyPart, out float _forceMultiplier);
        rb.AddForce(pForce * 15);
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
            onGiveReward?.Invoke(_rewardHeightCheck);
        }
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            _touchingGround = true;

            _ragdollSettings.rewardsOnGroundHit.TryGetValue(_bodyPart, out CustomKeyValuePair groundHitReaction);
            onGiveReward?.Invoke(groundHitReaction.Amount);

            if (_endEpisodeOnGroundTouch)
            {
                Debug.Log($"Hit the ground with body part {_bodyPart}");
                onEndEpisode?.Invoke();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            _touchingGround = false;
    }
}
