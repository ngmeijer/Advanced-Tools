using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Events;

public abstract class MLAgent : Agent
{
    public UnityEvent<float> OnFailedEpisode = new UnityEvent<float>();
    public UnityEvent<float> OnSucceededEpisode = new UnityEvent<float>();
    public UnityEvent OnFoundCollectible = new UnityEvent();

    protected float _currentEpisodeDuration;
    public float CurrentEpisodeDuration => _currentEpisodeDuration;

    protected int _amountOfCollectiblesFound;
    public int CollectiblesFound => _amountOfCollectiblesFound;

    [SerializeField] private int _maxDuration = 30;
    public float MaxDuration => _maxDuration;

    protected virtual void FixedUpdate()
    {
        _currentEpisodeDuration += Time.fixedDeltaTime;
    }
}