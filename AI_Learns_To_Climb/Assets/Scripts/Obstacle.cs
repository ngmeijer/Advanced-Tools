using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Event_OnHitCollider : UnityEvent<Obstacle> { }

public class Obstacle : MonoBehaviour
{
    public Event_OnHitCollider OnHitCollider = new Event_OnHitCollider();

    [SerializeField] [Range(1,100)] private int _damage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Obstacle"))
            return;

        OnHitCollider?.Invoke(this);
    }

    private void OnDestroy()
    {
        OnHitCollider.RemoveAllListeners();
    }
}
