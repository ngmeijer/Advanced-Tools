using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Awaitable;

/// <summary>
/// Handles the generation of training environments.
/// </summary>
public class TrainingManager : MonoBehaviour
{
    [SerializeField] private bool _massTesting;
    [Range(1, 40)][SerializeField] private int _maxTrainingEnvironments = 30;
    [SerializeField] private GameObject _environmentPrefab;
    [SerializeField] private GameObject _canvas;

    [SerializeField] private Vector3 _areaSize;
    [SerializeField] private Vector3 _areaSpacing;

    [SerializeField] private List<MLAgent> _agents = new List<MLAgent>();
    public List<MLAgent> Agents => _agents;

    private CSVWriter _writer;
    [SerializeField] private int _maxEpisodeCount = 500;

    private ObstacleManager _obstacleManager;
    private CollectibleManager _collectibleManager;

    private void Awake()
    {
        _writer = GetComponent<CSVWriter>();

        //In case I want to focus on a single environment to test out a new feature
        if (!_massTesting)
        {
            GameObject environment = Instantiate(_environmentPrefab, Vector3.zero, Quaternion.identity, this.transform);
            setEnvironmentVariables(environment);
            setCanvasToEnvironmentPosition(environment);
            return;
        }

        generateEnvironments();
    }

    private void Start()
    {
        if (_agents.Count == 1)
        {
            MLAgent agent = _agents[0];
            agent.OnEndEpisode?.AddListener(updateCSVData);
            agent.OnEndEpisode?.AddListener(resetCollectibles);
            _writer.SetTestingProperties(
                _maxEpisodeCount,
                agent.TrainingSettings.MaxHealth, 
                _obstacleManager.MaxItemCount, 
                agent.TrainingSettings.ObstacleDamage, 
                agent.TrainingSettings.ResultOnObstacleHit, 
                _collectibleManager.MaxItemCount, 
                agent.TrainingSettings.ResultOnCollectibleHit);
        }
    }

    private void resetCollectibles(MLAgent arg0)
    {
        _collectibleManager.RandomizeCollectiblePositions();
    }

    private void generateEnvironments()
    {
        int count = (int)Mathf.Sqrt(_maxTrainingEnvironments);

        Vector3 position = Vector3.zero;
        List<GameObject> environments = new List<GameObject>();
        for (int x = 0; x < count; x++)
        {
            position.x = (x * _areaSize.x) + _areaSpacing.x;
            for (int z = 0; z < count; z++)
            {
                position.z = (z * _areaSize.z) + _areaSpacing.z;
                GameObject environment = Instantiate(_environmentPrefab, position, Quaternion.identity, this.transform);

                setEnvironmentVariables(environment);
                environments.Add(environment);
            }
        }

        GameObject lastEnv = environments[environments.Count - 1];
        setCanvasToEnvironmentPosition(lastEnv);
    }

    private void setEnvironmentVariables(GameObject pEnvironment)
    {
        EnvironmentManager envManager = pEnvironment.GetComponentInChildren<EnvironmentManager>();
        Debug.Assert(envManager != null, "Environment Manager is null. Ensure there is a child with the EnvironmentManager component attached.");

        _obstacleManager = pEnvironment.GetComponentInChildren<ObstacleManager>();
        Debug.Assert(envManager != null, "ObstacleManager is null. Ensure there is a child with the ObstacleManager component attached.");

        _collectibleManager = pEnvironment.GetComponentInChildren<CollectibleManager>();
        Debug.Assert(_collectibleManager != null, "Collectible Manager is null. Ensure there is a child with the CollectibleManager component attached.");

        MLAgent[] foundAgents = pEnvironment.GetComponentsInChildren<MLAgent>();
        Debug.Assert(foundAgents != null, "MLAgent is null. Ensure there is a GameObject with a class that derives from MLAgent attached.");

        for (int i = 0; i < foundAgents.Length; i++)
        {
            _agents.Add(foundAgents[i]);
        }
        envManager.SetAgents(foundAgents);

        //collectibleManager.SetListeners(foundAgents);
    }

    private void updateCSVData(MLAgent pAgent)
    {
        _writer.AddData(pAgent.EpisodeID, pAgent.CumulativeReward, pAgent.CurrentEpisodeDuration, pAgent.RockHitCount, pAgent.CollectiblesFound);

        if (pAgent.EpisodeID >= _maxEpisodeCount - 1)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    private void setCanvasToEnvironmentPosition(GameObject pEnvironment)
    {
        Vector3 canvasPos = new Vector3(
            _canvas.transform.position.x,
            _canvas.transform.position.y,
            pEnvironment.transform.position.z + _areaSize.z / 2);
        _canvas.transform.position = canvasPos;
    }
}
