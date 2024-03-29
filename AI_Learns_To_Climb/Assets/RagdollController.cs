using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private float _forwardForce = 1500f;

    [SerializeField] private List<Rigidbody> _torsoRigidbodies;

    [SerializeField] private Rigidbody _rightLeg;
    [SerializeField] private Rigidbody _leftLeg;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _leftLeg.AddForce(new Vector3(0,0,_forwardForce));
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            _rightLeg.AddForce(new Vector3(0,0,_forwardForce));
        }
    }
}
