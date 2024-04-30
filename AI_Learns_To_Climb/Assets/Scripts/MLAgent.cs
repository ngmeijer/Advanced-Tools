using Unity.MLAgents;
using UnityEngine;
using UnityEngine.Events;

public class MLAgent : Agent
{
    [HideInInspector] public UnityEvent<float> OnFinishedEpisode = new UnityEvent<float>();
    [HideInInspector] public UnityEvent OnSucceededEpisode = new UnityEvent();

    protected float _episodeDuration;

    protected virtual void Update()
    {
        _episodeDuration += Time.deltaTime;

        if(StepCount >= MaxStep - 1)
        {
            Debug.Log("Invoked event");
            OnFinishedEpisode?.Invoke(_episodeDuration);
            OnSucceededEpisode?.Invoke();
            _episodeDuration = 0;
        }    
    }
}
