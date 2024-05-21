using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Awaitable;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private GameObject _canvasPrefab;
    [SerializeField] private Transform _canvasParent;
    private MLAgent[] _agents;
    private Dictionary<MLAgent, CanvasManager> _canvasesForAgents = new Dictionary<MLAgent, CanvasManager>();
    [SerializeField] private SpawnArea _agentSpawnArea;

    [Header("Weapons")]
    [SerializeField] private bool _enableWeapons;
    [SerializeField] private Transform _weaponParent;
    [SerializeField] private GameObject _weaponPrefab;
    [SerializeField] private int _maxWeaponCount = 5;
    [SerializeField] private SpawnArea _weaponSpawnArea;
    [SerializeField] private List<GameObject> _spawnedWeapons = new List<GameObject>();
    private List<GameObject> _takenWeapons = new List<GameObject>();
    private Dictionary<MLAgent, bool> AgentsWeaponStates = new Dictionary<MLAgent, bool>();

    private CSVWriter _writer;

    private void Start()
    {
        _writer = GetComponent<CSVWriter>();

        if (!_enableWeapons)
            return;

        for(int i = 0; i < _maxWeaponCount; i++)
        {
            GameObject weaponInstance = Instantiate(_weaponPrefab, _weaponSpawnArea.GetRandomPosition(), Quaternion.identity, _weaponParent);
            _spawnedWeapons.Add(weaponInstance);
        }
    }

    public void SetAgents(MLAgent[] pAgents)
    {
        _agents = pAgents;
        for(int i = 0; i < pAgents.Length; i++)
        {
            pAgents[i].OnPickedUpWeapon.AddListener(handleWeaponPickupNotify);
            pAgents[i].OnAgentKill.AddListener(handleAgentKill);
            pAgents[i].OnEndEpisode?.AddListener(generatePlayerPosition);
            pAgents[i].OnEndEpisode?.AddListener(updateCSVData);
            GameObject canvas = Instantiate(_canvasPrefab, _canvasParent.position, _canvasParent.rotation, _canvasParent);
            CanvasManager canvasManager = canvas.GetComponent<CanvasManager>();
            canvasManager.SetListeners(pAgents[i]);
            _canvasesForAgents.Add(pAgents[i], canvasManager);
            AgentsWeaponStates.Add(pAgents[i], false);
            canvasManager.SetID(i);
            pAgents[i].SetID(i);
        }
    }

    private void updateCSVData(MLAgent pAgent)
    {
        _writer.AddData(pAgent.EpisodeID, pAgent.CumulativeReward, pAgent.CurrentEpisodeDuration, pAgent.CollectiblesFound);
    }

    private void generatePlayerPosition(MLAgent pAgent)
    {
        if (_agentSpawnArea == null)
            return;

        Vector3 position = _agentSpawnArea.GetRandomPosition();
        pAgent.ReceiveNewRandomPosition(position);
    }

    private void handleAgentKill(MLAgent pHasBeenKilled, MLAgent pHasKilled)
    {
        _canvasesForAgents[pHasKilled].UpdateKD();
        _canvasesForAgents[pHasBeenKilled].UpdateKD();

        _weaponPrefab.transform.localPosition = _weaponSpawnArea.GetRandomPosition();
    }

    private void handleWeaponPickupNotify(WeaponState pState, GameObject pWeapon, MLAgent pAgent)
    {
        if (pWeapon == null)
            return;

        int weaponIndex = _spawnedWeapons.IndexOf(pWeapon);
        if (weaponIndex == -1)
        {
            Debug.Log($"Weapon could not be found. Weapon count: {_spawnedWeapons.Count}. Weapon: {pWeapon}");
            return;
        }

        if (pState == WeaponState.DROPPED_WEAPON) {
            _spawnedWeapons[weaponIndex].transform.localPosition = _weaponSpawnArea.GetRandomPosition();
            _spawnedWeapons[weaponIndex].SetActive(true);
            _takenWeapons.Remove(pWeapon);
            AgentsWeaponStates[pAgent] = false;
        }

        if (pState == WeaponState.CARRY_WEAPON)
        {
            _spawnedWeapons[weaponIndex].SetActive(false);
            _takenWeapons.Add(pWeapon);
            AgentsWeaponStates[pAgent] = true;
        }

        for(int i = 0; i <_agents.Length; i++)
        {
            _agents[i].EnemiesWeaponData = AgentsWeaponStates;
            if (_takenWeapons.Count == _spawnedWeapons.Count)
                _agents[i].WeaponAvailable = false;
            else _agents[i].WeaponAvailable = true;
        }
    }
}
