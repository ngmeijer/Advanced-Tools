using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplorationManager : MonoBehaviour
{
    [SerializeField] private GameObject _groundBlockPrefab;
    [SerializeField] private Vector3 _spawnAreaStart;
    [SerializeField] private Vector3Int _spawnAreaRadius;
    [SerializeField] private Color _gizmoColor;
    [SerializeField] private MLAgent _agent;
    [SerializeField] private bool _enableReinforcement;

    private Dictionary<GroundBlock, float> _groundBlocks = new Dictionary<GroundBlock, float>();
    private List<GameObject> _visitedBlocks = new List<GameObject>();

    private void Start()
    {
        for(int x = 0; x <= _spawnAreaRadius.x; x++)
        {
            for (int z = 0; z < _spawnAreaRadius.z; z++)
            {
                spawnGroundBlock(x, z);
            }
        }
    }

    private void Update()
    {
        //(_agent as CombatMLAgent).SetGroundBlockData(_groundBlocks);
    }

    private void checkVisitedBlocks(Spawnable pBlock)
    {
        if(!_visitedBlocks.Contains(pBlock.gameObject))
            _visitedBlocks.Add(pBlock.gameObject);
        else _visitedBlocks.Remove(pBlock.gameObject);
    }

    private void spawnGroundBlock(int pX, int pZ)
    {
        GameObject groundBlockInstance = Instantiate(_groundBlockPrefab.gameObject, Vector3.zero, Quaternion.identity, this.transform);
        GroundBlock groundComponent = groundBlockInstance.GetComponent<GroundBlock>();
        groundBlockInstance.transform.localPosition = _spawnAreaStart + new Vector3(pX * groundComponent._spawnableDimensions.x, 0, pZ * groundComponent._spawnableDimensions.z);

        if (_enableReinforcement)
        {
            groundComponent.OnHitCollider.AddListener(checkVisitedBlocks);
            groundComponent.OnCooldownNotify.AddListener(updateCooldown);
            groundComponent.EnableReinforcement = true;
        }

        _groundBlocks.Add(groundComponent, 0);
    }

    private void updateCooldown(float pCooldownLeft, GroundBlock pGroundBlock)
    {
        _groundBlocks[pGroundBlock] = pCooldownLeft;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawWireCube(transform.position, _spawnAreaRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + _spawnAreaStart, 1f);
    }
}
