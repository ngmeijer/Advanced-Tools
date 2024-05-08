using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundBlock : Spawnable
{
    private bool _agentOnBlock;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private float _maxCooldown = 15f;
    [SerializeField] private float _cooldownInterval = 1f;
    [HideInInspector] public bool EnableReinforcement;
    private float _cooldownLeft;

    public Event_OnHitCollider OnAgentStay = new Event_OnHitCollider();
    public UnityEvent<float, GroundBlock> OnCooldownNotify = new UnityEvent<float, GroundBlock>();

    private Color _startColor;
    [SerializeField] private Color _saturatedColor;

    private float _agentTimeSpentOnBlock;
    [SerializeField] private float _maxTimeSpentOnBlock = 1f;

    private void Start()
    {
        _startColor = _renderer.material.color;
    }

    private void FixedUpdate()
    {
        if (!EnableReinforcement)
            return;

        if(_cooldownLeft >= 0)
            OnCooldownNotify?.Invoke(_cooldownLeft, this);

        if (_agentOnBlock)
        {
            _agentTimeSpentOnBlock += Time.fixedDeltaTime;
            if (_agentTimeSpentOnBlock > _maxTimeSpentOnBlock)
            {
                OnAgentStay?.Invoke(this);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!EnableReinforcement)
            return;

        if (collision.collider.CompareTag("Agent"))
        {
            OnHitCollider?.Invoke(this);
            _agentOnBlock = true;
            _renderer.material.color = _saturatedColor;
            StopCoroutine(startCooldown());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!EnableReinforcement)
            return;

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
        if (_agentOnBlock)
            yield break;

        _cooldownLeft -= _cooldownInterval;
        if (_cooldownLeft <= 0)
        {
            _renderer.material.color = _startColor;
            _agentTimeSpentOnBlock = 0;
            yield break;
        }

        Color lerpedColor = Color.Lerp(_startColor, _saturatedColor, _cooldownLeft / _maxCooldown);
        _renderer.material.color = lerpedColor;
        yield return new WaitForSeconds(_cooldownInterval);
        StartCoroutine(startCooldown());
    }
}
