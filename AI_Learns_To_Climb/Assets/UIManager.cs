using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TestAgent _agent;
    [SerializeField] private TextMeshProUGUI _episodeCountText;
    
    
    private void Update()
    {
        _episodeCountText.SetText(_agent.CompletedEpisodes.ToString());
    }
}
