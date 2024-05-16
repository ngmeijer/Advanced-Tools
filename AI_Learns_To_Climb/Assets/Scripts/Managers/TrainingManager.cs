using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Handles the generation of training environments.
/// </summary>
public class TrainingManager : MonoBehaviour
{
    [SerializeField] private bool _massTesting;
    [Range(1,40)][SerializeField] private int _maxTrainingEnvironments = 30;
    [SerializeField] private GameObject _environmentPrefab;
    [SerializeField] private GameObject _canvas;

    [SerializeField] private Vector3 _areaSize;
    [SerializeField] private Vector3 _areaSpacing;

    [SerializeField] private List<MLAgent> _agents = new List<MLAgent>();
    public List<MLAgent> Agents => _agents;
    
    private void Awake()
    {
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

        CollectibleManager collectibleManager = pEnvironment.GetComponentInChildren<CollectibleManager>();
        Debug.Assert(collectibleManager != null, "Collectible Manager is null. Ensure there is a child with the CollectibleManager component attached.");

        MLAgent[] foundAgents = pEnvironment.GetComponentsInChildren<MLAgent>();
        Debug.Assert(foundAgents != null, "MLAgent is null. Ensure there is a GameObject with a class that derives from MLAgent attached.");

        for (int i = 0; i < foundAgents.Length; i++)
        {
            _agents.Add(foundAgents[i]);
        }
        envManager.SetAgents(foundAgents);

        //collectibleManager.SetListeners(foundAgents);
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
