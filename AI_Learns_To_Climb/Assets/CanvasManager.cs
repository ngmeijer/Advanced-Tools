using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private Image _successIndicator;

    [SerializeField] private TextMeshProUGUI _agentIDText;
    [SerializeField] private TextMeshProUGUI _currentDurationText;
    [SerializeField] private TextMeshProUGUI _collectiblesFoundText;
    [SerializeField] private TextMeshProUGUI _cumulativeRewardText;
    [SerializeField] private TextMeshProUGUI _currentHealthText;
    [SerializeField] private TextMeshProUGUI _kdText;
    [SerializeField] private Image _currentHealthIndicator;

    [HideInInspector] public MLAgent Agent;

    private void LateUpdate()
    {
        setCurrentDurationText();
        setRewardText();
        setCurrentHealthUI();
        setFoundCollectiblesText();
    }

    public void SetListeners(MLAgent pAgent)
    {
        Agent = pAgent;
        Agent.OnFailedEpisode.AddListener(setFailedGroundMat);
        Agent.OnSucceededEpisode.AddListener(setSucceededGroundMat);
    }

    public void UpdateKD()
    {
        _kdText.SetText($"{Agent.KillCount}/{Agent.DeathCount}");
    }

    private void setCurrentHealthUI()
    {
        float healthPercentage = Agent.CurrentHealth / (float)Agent.TrainingSettings.MaxHealth;
        _currentHealthText.SetText($"{healthPercentage * 100:F0}%");
        _currentHealthIndicator.transform.localScale = new Vector3(healthPercentage, 1, 1);
    }

    private void setRewardText()
    {
        _cumulativeRewardText.SetText(Agent.GetCumulativeReward().ToString("F2"));
    }

    private void setSucceededGroundMat(float arg0, float arg1)
    {
        _successIndicator.color = Color.green;
    }

    private void setFailedGroundMat(float arg0, float arg1)
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

    public void SetID(int pAgentID)
    {
        _agentIDText.SetText($"Agent {pAgentID}");
    }
}
