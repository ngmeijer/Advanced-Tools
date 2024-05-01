using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer _groundRenderer;
    [SerializeField] private Material _loseMaterial;
    [SerializeField] private Material _winMaterial;
    [HideInInspector] public MLAgent Agent;

    [SerializeField] private TextMeshProUGUI _currentDurationText;

    private void Update()
    {
        setCurrentDurationText();
    }

    private void setSucceededGroundMat(float arg0)
    {
        _groundRenderer.material = _winMaterial;
    }

    private void setFailedGroundMat(float arg0)
    {
        _groundRenderer.material = _loseMaterial;
    }

    private void setCurrentDurationText()
    {
        _currentDurationText.SetText($"{Agent.CurrentEpisodeDuration.ToString("F2")}s");
    }

    public void SetListeners(MLAgent pAgent)
    {
        Agent = pAgent;
        Agent.OnFailedEpisode.AddListener(setFailedGroundMat);
        Agent.OnSucceededEpisode.AddListener(setSucceededGroundMat);
    }
}
