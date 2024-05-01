using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Tooltip("Prefab must have component 'Spawnable'!")][SerializeField] private Spawnable _itemPrefab;
    [SerializeField] private float _itemSpawnWidth;
    public float ItemSpawnWidth => _itemSpawnWidth;
    [SerializeField] private int _itemInStorageCount = 100;
    [SerializeField] private List<Spawnable> _spawnedItems = new List<Spawnable>();

    [SerializeField] private Vector3 _spawnAreaCenter;
    [SerializeField] private Color _gizmoColor;

    private float _currentActiveItems;
    public float CurrentActiveItems => _currentActiveItems;

    private void Start()
    {
        fillObjectPool();
    }

    public void ActivateItem()
    {
        Vector3 randomPos = new Vector3(Random.Range(-_itemSpawnWidth / 2, _itemSpawnWidth / 2), _spawnAreaCenter.y, _spawnAreaCenter.z);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = _gizmoColor;
        Gizmos.DrawWireCube(transform.position + _spawnAreaCenter, new Vector3(ItemSpawnWidth, 2f, 1f));
    }
}
