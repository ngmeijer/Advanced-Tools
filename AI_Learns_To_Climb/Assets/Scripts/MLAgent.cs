using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Events;

public class MLAgent : Agent
{
    public UnityEvent<float> OnFinishedEpisode = new UnityEvent<float>();
    public UnityEvent OnSucceededEpisode = new UnityEvent();

    protected float _episodeDuration;


    protected virtual void Update()
    {
        _episodeDuration += Time.deltaTime;
    }
}
