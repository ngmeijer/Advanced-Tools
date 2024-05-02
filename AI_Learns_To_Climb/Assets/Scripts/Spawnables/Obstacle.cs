using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : Spawnable
{
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
