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
    private TrainingManager _trainer;
    private float _totalEpisodeCount;
    private float _succesfullEpisodeCount;
    
    private void Awake()
    {
        _trainer = FindObjectOfType<TrainingManager>();
    }

    private void Start()
    {
        TestAgent[] agents = _trainer.Agents.ToArray();
        foreach (var agent in agents)
        {
            agent.OnFinishedEpisode.AddListener(updateEpisodeCount);
            agent.OnSucceededEpisode.AddListener(updateSuccesfullEpisodeCount);
        }
    }

    private void updateEpisodeCount()
    {
        _totalEpisodeCount += 1;
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
