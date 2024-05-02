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
        (groundComponent as GroundBlock).OnAgentStay.AddListener(punishAgentStay);
        _groundBlocks.Add(groundBlockInstance);
    }

    /// <summary>
    /// Punish an agent for every 
    /// </summary>
    /// <param name="arg0"></param>
    private void punishAgentStay(Spawnable arg0)
    {
        Debug.Log("Punish agent for staying too long on a block");
        _agent.AddReward(-0.01f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawWireCube(transform.position, _spawnAreaRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + _spawnAreaStart, 1f);
    }
}
