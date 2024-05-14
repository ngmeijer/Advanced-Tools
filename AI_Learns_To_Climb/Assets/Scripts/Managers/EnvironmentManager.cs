using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentManager : MonoBehaviour
{
    [SerializeField] private GameObject _canvasPrefab;
    [SerializeField] private Transform _canvasParent;
    [SerializeField] private GameObject _weaponPrefab;
    [SerializeField] private int _maxWeaponCount = 5;
    [SerializeField] private SpawnArea _weaponSpawnArea;

    private MLAgent[] _agents;
    [SerializeField] private List<GameObject> _spawnedWeapons = new List<GameObject>();
    private List<GameObject> _takenWeapons = new List<GameObject>();

    private Dictionary<MLAgent, CanvasManager> _canvasesForAgents = new Dictionary<MLAgent, CanvasManager>();

    private void Start()
    {
        for(int i = 0; i < _maxWeaponCount; i++)
        {
            GameObject weaponInstance = Instantiate(_weaponPrefab, _weaponSpawnArea.GetRandomPosition(), Quaternion.identity, this.transform);
            _spawnedWeapons.Add(weaponInstance);
        }
    }

    public void SetAgents(MLAgent[] pAgents)
    {
        _agents = pAgents;
        for(int i = 0; i < pAgents.Length; i++)
        {
            pAgents[i].OnPickedUpWeapon.AddListener(handleWeaponPickupNotify);
            pAgents[i].OnHasBeenKilledByAgent.AddListener(handleAgentKill);
            Vector3 canvasPos = _canvasParent.position + new Vector3(0, i * 11.5f, 0);
            GameObject canvas = Instantiate(_canvasPrefab, canvasPos, _canvasParent.rotation, _canvasParent);
            CanvasManager canvasManager = canvas.GetComponent<CanvasManager>();
            canvasManager.SetListeners(pAgents[i]);
            _canvasesForAgents.Add(pAgents[i], canvasManager);
            canvasManager.SetID(i);
            pAgents[i].SetID(i);
        }
    }

    private void handleAgentKill(MLAgent pHasBeenKilled, MLAgent pHasKilled)
    {
        _canvasesForAgents[pHasKilled].UpdateKD();
        _canvasesForAgents[pHasBeenKilled].UpdateKD();

        _weaponPrefab.transform.localPosition = _weaponSpawnArea.GetRandomPosition();
    }

    private void handleWeaponPickupNotify(WeaponState pState, GameObject pWeapon)
    {
        int weaponIndex = _spawnedWeapons.IndexOf(pWeapon);
        if (weaponIndex == -1)
        {
            Debug.Log($"Weapon could not be found. Weapon count: {_spawnedWeapons.Count}. Weapon: {pWeapon}");
            return;
        }

        if (pState == WeaponState.DROPPED) {
            _spawnedWeapons[weaponIndex].transform.localPosition = _weaponSpawnArea.GetRandomPosition();
            _spawnedWeapons[weaponIndex].SetActive(true);
            _takenWeapons.Remove(pWeapon);
        }

        if (pState == WeaponState.PICKED_UP)
        {
            _spawnedWeapons[weaponIndex].SetActive(false);
            _takenWeapons.Add(pWeapon);
        }

        for(int i = 0; i <_agents.Length; i++)
        {
            if (_takenWeapons.Count == _spawnedWeapons.Count)
                _agents[i].WeaponAvailable = false;
            else _agents[i].WeaponAvailable = true;
        }
    }
}
