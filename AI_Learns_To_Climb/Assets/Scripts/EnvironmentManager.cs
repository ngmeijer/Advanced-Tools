using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private Image _successIndicator;
    [HideInInspector] public MLAgent Agent;

    [SerializeField] private TextMeshProUGUI _agentIDText;
    [SerializeField] private TextMeshProUGUI _currentDurationText;
    [SerializeField] private TextMeshProUGUI _collectiblesFoundText;
    [SerializeField] private TextMeshProUGUI _cumulativeRewardText;

    private void Update()
    {
        setCurrentDurationText();
        setRewardText();
    }

    private void setRewardText()
    {
        _cumulativeRewardText.SetText(Agent.GetCumulativeReward().ToString("F2"));
    }

    private void setSucceededGroundMat(float arg0)
    {
        _successIndicator.color = Color.green;
    }

    private void setFailedGroundMat(float arg0)
    {
        _successIndicator.color = Color.red;
    }

    private void setCurrentDurationText()
    {
        _currentDurationText.SetText($"{Agent.CurrentEpisodeDuration.ToString("F2")}s");
    }

    private void setFoundCollectiblesText()
    {
        _collectiblesFoundText.SetText($"{Agent.CollectiblesFound}");
    }

    public void SetListeners(MLAgent pAgent)
    {
        Agent = pAgent;
        Agent.OnFailedEpisode.AddListener(setFailedGroundMat);
        Agent.OnSucceededEpisode.AddListener(setSucceededGroundMat);
        Agent.OnFoundCollectible.AddListener(setFoundCollectiblesText);
    }

    public void SetID(int pAgentID)
    {
        _agentIDText.SetText($"Agent {pAgentID}");
    }
}
