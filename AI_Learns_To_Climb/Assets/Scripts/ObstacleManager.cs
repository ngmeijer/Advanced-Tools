using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] private int _currentObstaclesInTrainingArea;
    [SerializeField] private int _maxObstaclesInTrainingArea = 10;
    [SerializeField] private GameObject _obstaclePrefab;

    [Tooltip("How often should a new obstacle spawn in seconds?")][SerializeField] private float _obstacleSpawnrate;
    private float _durationSinceObstacleSpawned;

    [SerializeField] private float obstacleSpawnWidth;

    [SerializeField] private int _obstaclesInStorageCount = 100;
    [SerializeField] private List<Obstacle> _spawnedObstacles = new List<Obstacle>();

    private void Start()
    {
        fillObjectPool();
    }

    private void Update()
    {
        handleObstacleSpawning();
    }

    private void handleObstacleSpawning()
    {
        if (_currentObstaclesInTrainingArea >= _maxObstaclesInTrainingArea)
            return;

        _durationSinceObstacleSpawned += Time.deltaTime;
        if (_durationSinceObstacleSpawned >= _obstacleSpawnrate)
        {
            _currentObstaclesInTrainingArea++;
            _durationSinceObstacleSpawned = 0;

            activateObstacle();
        }
    }

    private void activateObstacle()
    {
        Vector3 randomPos = new Vector3(Random.Range(-obstacleSpawnWidth / 2, obstacleSpawnWidth / 2), 15, 0);
        GameObject newObstacle = getObstacleFromPool();
        newObstacle.transform.localPosition = randomPos;
        newObstacle.SetActive(true);
    }

    private GameObject getObstacleFromPool()
    {
        for (int i = 0; i < _spawnedObstacles.Count; i++)
        {
            if (_spawnedObstacles[i] == null)
            {
                Debug.Log("Obstacle instance is null.");
                continue;
            }

            if (_spawnedObstacles[i].gameObject.activeInHierarchy)
                continue;

            return _spawnedObstacles[i].gameObject;
        }

        GameObject obstacle = Instantiate(_obstaclePrefab, transform.localPosition, Quaternion.identity, this.transform);
        Obstacle obstacleComponent = obstacle.GetComponent<Obstacle>();
        _spawnedObstacles.Add(obstacleComponent);
        return obstacle;
    }

    private void fillObjectPool()
    {
        for (int i = 0; i < _obstaclesInStorageCount; i++)
        {
            GameObject obstacle = Instantiate(_obstaclePrefab, transform.localPosition, Quaternion.identity, this.transform);
            obstacle.SetActive(false);
            Obstacle obstacleComponent = obstacle.GetComponent<Obstacle>();
            obstacleComponent.OnHitCollider.AddListener(removeObstacle);
            _spawnedObstacles.Add(obstacleComponent);
        }
    }

    private void removeObstacle(Obstacle pObstacleToRemove)
    {
        int index = _spawnedObstacles.IndexOf(pObstacleToRemove);
        _spawnedObstacles[index].gameObject.SetActive(false);
        _spawnedObstacles[index].gameObject.transform.rotation = Quaternion.identity;
        _currentObstaclesInTrainingArea--;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, 10, transform.position.z), new Vector3(obstacleSpawnWidth, 5f, 1f));
    }
}
