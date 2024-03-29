using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    [SerializeField] private bool _runTest;
    [SerializeField] private bool _massTesting;
    [SerializeField] private int _maxTrainingEnvironments = 50;
    [SerializeField] private GameObject _environmentPrefab;
    [SerializeField] private List<TestAgent> _agents;
    [SerializeField] private GameObject _canvas;
    public List<TestAgent> Agents => _agents;
    
    private void Awake()
    {
        if (!_runTest)
            return;
        
        if (!_massTesting)
        {
            GameObject environment = Instantiate(_environmentPrefab, Vector3.zero, Quaternion.identity, this.transform);
            TestAgent agent = environment.GetComponentInChildren<TestAgent>();
            _agents.Add(agent);

            setCanvasToEnvironmentPosition(environment);
            return;
        }
        
        int count = (int)Mathf.Sqrt(_maxTrainingEnvironments);
        
        Vector3 position = Vector3.zero;
        List<GameObject> environments = new List<GameObject>();
        for (int x = 0; x < count; x++)
        {
            position.x = x * 15;
            for (int z = 0; z < count; z++)
            {
                position.z = z * 15;
                GameObject environment = Instantiate(_environmentPrefab, position, Quaternion.identity, this.transform);
                TestAgent agent = environment.GetComponentInChildren<TestAgent>();
                _agents.Add(agent);
                environments.Add(environment);
            }
        }

        GameObject lastEnv = environments[environments.Count - 1];
        setCanvasToEnvironmentPosition(lastEnv);
    }

    private void setCanvasToEnvironmentPosition(GameObject pEnvironment)
    {
        Vector3 canvasPos = new Vector3(-6, pEnvironment.transform.position.y,
            pEnvironment.transform.position.z + 6);
        _canvas.transform.position = canvasPos;
        Debug.Log($"set canvas to position {canvasPos}");
    }
}
