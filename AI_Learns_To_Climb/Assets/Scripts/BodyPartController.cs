using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyPart
{
    HEAD,
    TORSO,
    HIPS,
    LEFT_UPPER_LEG,
    LEFT_LOWER_LEG,
    LEFT_FOOT,

    RIGHT_UPPER_LEG,
    RIGHT_LOWER_LEG,
    RIGHT_FOOT,

    LEFT_SHOULDER,
    LEFT_UPPER_ARM,
    LEFT_LOWER_ARM,
    LEFT_HAND,

    RIGHT_SHOULDER,
    RIGHT_UPPER_ARM,
    RIGHT_LOWER_ARM,
    RIGHT_HAND,
}

public class BodyPartController : MonoBehaviour
{
    [SerializeField] protected BodyPart part;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] private float _forceMultiplier = 1500f;

    public Action<BodyPart, float> OnGiveReward;

    protected Vector3 _startPos;
    protected Quaternion _startRot;

    [SerializeField] private float _penalty;
    [SerializeField] private float _reward;
    [SerializeField] private bool _touchingGround;
    public bool TouchingGround => _touchingGround;

    private void Awake()
    {
        if (rb == null)
        {
            Debug.Log($"A Rigidbody has not been referenced for {gameObject.name} of type {part}.");
            rb = GetComponent<Rigidbody>();
        }

        _startPos = transform.localPosition;
        _startRot = transform.localRotation;
    }

    public void AddForceToLimb(Vector3 pForce)
    {
        rb.AddForce(pForce * _forceMultiplier);
    }

    public void ResetLimb()
    {
        transform.localPosition = _startPos;
        transform.localRotation = _startRot;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            _touchingGround = true;
        }
    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        if(TouchingGround)
        {
            OnGiveReward(part, -0.1f);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            _touchingGround = false;
        }
    }
}
