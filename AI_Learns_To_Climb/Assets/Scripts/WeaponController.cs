using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponController : MonoBehaviour
{
    public UnityEvent WeaponOnHit = new UnityEvent();

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Agent"))
        {
            gameObject.SetActive(false);
        }
    }
}
