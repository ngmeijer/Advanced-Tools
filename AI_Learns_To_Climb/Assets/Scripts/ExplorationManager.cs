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

    private List<GameObject> _groundBlocks = new List<GameObject>();
    private List<GameObject> _visitedBlocks = new List<GameObject>();

    private void Start()
    {
        for(int x = 0; x < _spawnAreaRadius.x; x++)
        {
            for (int z = 0; z < _spawnAreaRadius.z; z++)
            {
                spawnGroundBlock(x, z);
            }
        }
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
        Spawnable groundComponent = groundBlockInstance.GetComponent<Spawnable>();
        groundBlockInstance.transform.localPosition = _spawnAreaStart + new Vector3(pX * groundComponent._spawnableDimensions.x, 0, pZ * groundComponent._spawnableDimensions.z);
        groundComponent.OnHitCollider.AddListener(checkVisitedBlocks);
        _groundBlocks.Add(groundBlockInstance);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawWireCube(transform.position, _spawnAreaRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + _spawnAreaStart, 1f);
    }
}
