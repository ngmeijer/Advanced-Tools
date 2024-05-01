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
    [SerializeField] private TextMeshProUGUI _averageDurationText;
    [SerializeField] private TextMeshProUGUI _maxDurationText;
    private TrainingManager _trainer;
    private float _totalEpisodeCount;
    private float _succesfullEpisodeCount;

    private float _totalEpisodeDuration;
    private float _successRatio;
    
    private void Awake()
    {
        _trainer = FindObjectOfType<TrainingManager>();
    }

    private void Start()
    {
        MLAgent[] agents = _trainer.Agents.ToArray();
        foreach (var agent in agents)
        {
            agent.OnFailedEpisode.AddListener(updateDataOnFail);
            agent.OnSucceededEpisode.AddListener(updateDataOnSuccess);
        }

        _maxDurationText.SetText($"{agents[0].MaxDuration.ToString()}s");
    }

    private void updateTotalEpisodeCount()
    {
        _totalEpisodeCount += 1;
        _episodeCountText.SetText(_totalEpisodeCount.ToString());
    }

    private void updateDataOnFail(float pEpisodeDuration)
    {
        _totalEpisodeDuration += pEpisodeDuration;
        updateTotalEpisodeCount();
        calculateSuccessRatio();
        calculateAverageDuration();
    }

    private void updateDataOnSuccess(float pEpisodeDuration)
    {
        _succesfullEpisodeCount += 1;
        _totalEpisodeDuration += pEpisodeDuration;
        _succesfullEpisodeCountText.SetText(_succesfullEpisodeCount.ToString());
        updateTotalEpisodeCount();
        calculateSuccessRatio();
        calculateAverageDuration();
    }

    private void calculateSuccessRatio()
    {
        float decimalSuccessRatio = _succesfullEpisodeCount / _totalEpisodeCount;
        _successRatioText.SetText($"{(decimalSuccessRatio * 100):F2}%");
    }

    private void calculateAverageDuration()
    {
        float averageDuration = _totalEpisodeDuration / _totalEpisodeCount;
        _averageDurationText.SetText($"{averageDuration.ToString("F3")}s");
    }
}
