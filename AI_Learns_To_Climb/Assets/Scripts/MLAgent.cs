using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Events;

public class MLAgent : Agent
{
    public UnityEvent<float> OnFailedEpisode = new UnityEvent<float>();
    public UnityEvent<float> OnSucceededEpisode = new UnityEvent<float>();

    protected float _currentEpisodeDuration;
    public float CurrentEpisodeDuration => _currentEpisodeDuration;

    [SerializeField] private int _maxDuration = 90;
    public float MaxDuration => _maxDuration;

    protected virtual void Update()
    {
        _currentEpisodeDuration += Time.deltaTime;
    }
}