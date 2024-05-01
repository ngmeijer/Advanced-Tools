using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Events;

public class MLAgent : Agent
{
    public UnityEvent<float> OnFailedEpisode = new UnityEvent<float>();
    public UnityEvent<float> OnSucceededEpisode = new UnityEvent<float>();
    public UnityEvent OnFoundCollectible = new UnityEvent();

    protected float _currentEpisodeDuration;
    public float CurrentEpisodeDuration => _currentEpisodeDuration;

    protected int _collectiblesFound;
    public int CollectiblesFound => _collectiblesFound;

    [SerializeField] private int _maxDuration = 90;
    public float MaxDuration => _maxDuration;

    protected virtual void Update()
    {
        _currentEpisodeDuration += Time.deltaTime;
    }
}