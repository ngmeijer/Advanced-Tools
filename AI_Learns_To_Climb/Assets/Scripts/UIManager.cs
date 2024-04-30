using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _episodeCountText;
    [SerializeField] private TextMeshProUGUI _succesfullEpisodeCountText;
    [SerializeField] private TextMeshProUGUI _successRatioText;
    [SerializeField] private TextMeshProUGUI _averageEpisodeDurationText;
    private TrainingManager _trainer;
    private float _totalEpisodeCount;
    private float _succesfullEpisodeCount;
    private float _totalEpisodeDuration;
    private float _averageEpisodeDuration;
    
    private void Awake()
    {
        _trainer = FindFirstObjectByType<TrainingManager>();
    }

    private void Start()
    {
        MLAgent[] agents = _trainer.Agents.ToArray();
        foreach (var agent in agents)
        {
            agent.OnFinishedEpisode.AddListener(updateEpisodeCount);
            Debug.Log("Added listener.");
            agent.OnSucceededEpisode.AddListener(updateSuccesfullEpisodeCount);
        }
    }

    private void updateEpisodeCount(float pEpisodeDuration)
    {
        _totalEpisodeDuration += pEpisodeDuration;
        _totalEpisodeCount += 1;
        _averageEpisodeDuration = _totalEpisodeDuration / _totalEpisodeCount;
        _averageEpisodeDurationText.SetText($"{_averageEpisodeDuration.ToString("F3")}s");
        _episodeCountText.SetText(_totalEpisodeCount.ToString());
        calculateSuccessRatio();
    }

    private void updateSuccesfullEpisodeCount()
    {
        _succesfullEpisodeCount += 1;
        _succesfullEpisodeCountText.SetText(_succesfullEpisodeCount.ToString());
        calculateSuccessRatio();
    }

    private void calculateSuccessRatio()
    {
        float decimalSuccessRatio = _succesfullEpisodeCount / _totalEpisodeCount;
        _successRatioText.SetText($"{(decimalSuccessRatio * 100):F2}%");
    }
}
