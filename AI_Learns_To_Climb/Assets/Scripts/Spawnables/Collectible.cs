using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : Spawnable
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Agent"))
            return;

        OnHitCollider?.Invoke(this);
    }

    private void OnDestroy()
    {
        OnHitCollider.RemoveAllListeners();
    }
}
