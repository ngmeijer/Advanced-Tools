using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnArea))]
public class ObjectPool : MonoBehaviour
{
    [Tooltip("Prefab must have component 'Spawnable'!")][SerializeField] private Spawnable _itemPrefab;
    [SerializeField] private int _itemInStorageCount = 100;
    public int ItemInStorageCount => _itemInStorageCount;

    [SerializeField] private List<Spawnable> _spawnedItems = new List<Spawnable>();

    private float _currentActiveItems;
    public float CurrentActiveItems => _currentActiveItems;

    [SerializeField] private SpawnArea _spawnArea;

    private void Start()
    {
        fillObjectPool();
    }

    public void ActivateItem()
    {
        Vector3 randomPos = _spawnArea.GetRandomPosition();
        GameObject newItem = getItemFromPool();
        newItem.transform.localPosition = randomPos;
        newItem.SetActive(true);
        _currentActiveItems++;
    }

    private GameObject getItemFromPool()
    {
        for (int i = 0; i < _spawnedItems.Count; i++)
        {
            if (_spawnedItems[i] == null)
            {
                Debug.Log("Item instance is null.");
                continue;
            }

            if (_spawnedItems[i].gameObject.activeInHierarchy)
                continue;

            return _spawnedItems[i].gameObject;
        }

        GameObject item = Instantiate(_itemPrefab.gameObject, transform.localPosition, Quaternion.identity, this.transform);
        Spawnable itemComponent = item.GetComponent<Spawnable>();
        itemComponent.OnHitCollider.AddListener(removeItem);
        _spawnedItems.Add(itemComponent);
        return item;
    }

    private void fillObjectPool()
    {
        for (int i = 0; i < _itemInStorageCount; i++)
        {
            GameObject obstacle = Instantiate(_itemPrefab.gameObject, transform.localPosition, Quaternion.identity, this.transform);
            obstacle.SetActive(false);
            Spawnable itemComponent = obstacle.GetComponent<Spawnable>();
            itemComponent.OnHitCollider.AddListener(removeItem);
            _spawnedItems.Add(itemComponent);
        }
    }

    private void removeItem(Spawnable pitemToRemove)
    {
        int index = _spawnedItems.IndexOf(pitemToRemove);
        _spawnedItems[index].gameObject.SetActive(false);
        _spawnedItems[index].gameObject.transform.rotation = Quaternion.identity;
        _currentActiveItems--;
    }

    public void DeactivateAlltems()
    {
        for(int i = 0; i < _spawnedItems.Count; i++)
        {
            if (_spawnedItems[i].transform.gameObject.activeInHierarchy == false)
                continue;
            _spawnedItems[i].transform.gameObject.SetActive(false);
        }

        _currentActiveItems = 0;
    }
}
