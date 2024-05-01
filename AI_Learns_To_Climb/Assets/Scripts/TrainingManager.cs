using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    [SerializeField] private bool _massTesting;
    [SerializeField] private int _maxTrainingEnvironments = 50;
    [SerializeField] private GameObject _environmentPrefab;
    [SerializeField] private List<MLAgent> _agents;
    [SerializeField] private GameObject _canvas;

    [SerializeField] private Vector3 _areaSize;
    [SerializeField] private Vector3 _areaSpacing;

    public List<MLAgent> Agents => _agents;
    
    private void Awake()
    {
        if (!_massTesting)
        {
            GameObject environment = Instantiate(_environmentPrefab, Vector3.zero, Quaternion.identity, this.transform);
            MLAgent agent = environment.GetComponentInChildren<MLAgent>();
            _agents.Add(agent);

            setCanvasToEnvironmentPosition(environment);
            return;
        }
        
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

                EnvironmentManager envManager = environment.GetComponent<EnvironmentManager>();
                MLAgent agent = environment.GetComponentInChildren<MLAgent>();
                _agents.Add(agent);
                envManager.SetListeners(agent);
                envManager.SetID(_agents.Count - 1);
                environments.Add(environment);
            }
        }

        GameObject lastEnv = environments[environments.Count - 1];
        setCanvasToEnvironmentPosition(lastEnv);
    }

    private void setCanvasToEnvironmentPosition(GameObject pEnvironment)
    {
        Vector3 canvasPos = new Vector3(_canvas.transform.position.x, _canvas.transform.position.y, pEnvironment.transform.position.z + _areaSize.z);
        _canvas.transform.position = canvasPos;
    }
}
