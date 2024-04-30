using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Events;

public class MLAgent : Agent
{
    public UnityEvent<float> OnFailedEpisode = new UnityEvent<float>();
    public UnityEvent<float> OnSucceededEpisode = new UnityEvent<float>();

    protected float _currentEpisodeDuration;

    protected virtual void Update()
    {
        _currentEpisodeDuration += Time.deltaTime;
    }
}