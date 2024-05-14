using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.MLAgents;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _episodeCountText;
    [SerializeField] private TextMeshProUGUI _succesfullEpisodeCountText;
    [SerializeField] private TextMeshProUGUI _successRatioText;
    [SerializeField] private TextMeshProUGUI _averageDurationText;
    [SerializeField] private TextMeshProUGUI _maxDurationText;
    [SerializeField] private TextMeshProUGUI _averageCollectiblesText;
    [SerializeField] private TextMeshProUGUI _averageCumulativeRewardText;
    private TrainingManager _trainer;
    private float _totalEpisodeCount;
    private float _succesfullEpisodeCount;
    private float _totalCollectiblesFound;
    private float _totalCumulativeReward;
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
            agent.OnFoundCollectible.AddListener(updateCollectibleCount);
        }

        _maxDurationText.SetText($"{agents[0].MaxDuration}s");
    }

    private void updateTotalEpisodeCount()
    {
        _totalEpisodeCount += 1;
        _episodeCountText.SetText(_totalEpisodeCount.ToString());
    }

    private void updateDataOnFail(float pEpisodeDuration, float pCumulativeReward)
    {
        _totalEpisodeDuration += pEpisodeDuration;
        _totalCumulativeReward += pCumulativeReward;
        updateTotalEpisodeCount();
        calculateSuccessRatio();
        calculateAverageDuration();
        calculateAverageCollectibleCount();
        calculateAverageCumulativeReward();
    }

    private void updateCollectibleCount()
    {
        _totalCollectiblesFound += 1;
    }

    private void calculateAverageCollectibleCount()
    {
        float averageCollectibleCount = _totalCollectiblesFound / _totalEpisodeCount;
        _averageCollectiblesText.SetText($"{averageCollectibleCount.ToString("F1")} coins");
    }

    private void updateDataOnSuccess(float pEpisodeDuration, float pCumulativeReward)
    {
        _succesfullEpisodeCount += 1;
        _totalEpisodeDuration += pEpisodeDuration;
        _totalCumulativeReward += pCumulativeReward;
        _succesfullEpisodeCountText.SetText(_succesfullEpisodeCount.ToString());
        updateTotalEpisodeCount();
        calculateSuccessRatio();
        calculateAverageDuration();
        calculateAverageCollectibleCount();
        calculateAverageCumulativeReward();
    }

    private void calculateAverageCumulativeReward()
    {
        float averageReward = _totalCumulativeReward / _totalEpisodeCount;
        _averageCumulativeRewardText.SetText($"{averageReward.ToString("F2")}");
    }

    private void calculateSuccessRatio()
    {
        float decimalSuccessRatio = _succesfullEpisodeCount / _totalEpisodeCount;
        _successRatioText.SetText($"{(decimalSuccessRatio * 100):F2}%");
    }

    private void calculateAverageDuration()
    {
        float averageDuration = _totalEpisodeDuration / _totalEpisodeCount;
        _averageDurationText.SetText($"{averageDuration.ToString("F2")}s");
    }
}
