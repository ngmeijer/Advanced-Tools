using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBlock : Spawnable
{
    private bool _agentOnBlock;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private float _maxCooldown = 15f;
    [SerializeField] private float _cooldownInterval = 1f;
    private float _cooldownLeft;

    public Event_OnHitCollider OnAgentStay = new Event_OnHitCollider();

    private Color _startColor;
    [SerializeField] private Color _saturatedColor;

    private float _agentTimeSpentOnBlock;
    [SerializeField] private float _maxTimeSpentOnBlock = 1f;

    private void Start()
    {
        _startColor = _renderer.material.color;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Agent"))
        {
            OnHitCollider?.Invoke(this);
            _agentOnBlock = true;
            _renderer.material.color = _saturatedColor;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(_agentOnBlock)
        {
            _agentTimeSpentOnBlock += Time.deltaTime;

            if (_agentTimeSpentOnBlock > _maxTimeSpentOnBlock)
            {
                OnAgentStay?.Invoke(this);
                _agentTimeSpentOnBlock = 0;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Agent"))
        {
            OnHitCollider?.Invoke(this);
            _agentOnBlock = false;
            _cooldownLeft = _maxCooldown;
            StartCoroutine(startCooldown());
        }
    }
    
    private IEnumerator startCooldown()
    {
        _cooldownLeft -= _cooldownInterval;
        if (_cooldownLeft > 0)
        {
            Color lerpedColor = Color.Lerp(_startColor, _saturatedColor, _cooldownLeft / _maxCooldown);
            _renderer.material.color = lerpedColor;
            yield return new WaitForSeconds(_cooldownInterval);
            StartCoroutine(startCooldown());
        }
        else
        {
            _renderer.material.color = _startColor;
            yield break;
        }
    }
}
