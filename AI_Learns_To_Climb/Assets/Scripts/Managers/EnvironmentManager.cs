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
    [SerializeField] private GameObject _weapon;

    private MLAgent[] _agents;

    public void SetAgents(MLAgent[] pAgents)
    {
        _agents = pAgents;
        for(int i = 0; i < pAgents.Length; i++)
        {
            pAgents[i].OnPickedUpWeapon.AddListener(handleWeaponPickupNotify);
            Vector3 canvasPos = _canvasParent.position + new Vector3(0, i * 11.5f, 0);
            GameObject canvas = Instantiate(_canvasPrefab, canvasPos, Quaternion.identity, _canvasParent);
            CanvasManager canvasManager = canvas.GetComponent<CanvasManager>();
            canvasManager.SetListeners(pAgents[i]);
        }
    }

    private void handleWeaponPickupNotify(bool pState)
    {
        _weapon.SetActive(pState);
        for(int i = 0; i <_agents.Length; i++)
        {
            _agents[i].WeaponAvailable = pState;
            if(pState == true)
            {
                _agents[i].DisableWeapon();
            }
        }
    }
}
