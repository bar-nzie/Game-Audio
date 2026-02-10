using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbCollision : MonoBehaviour
{
    public GameObject impactEffect;
    public float damageRadius = 5f;
    public float damageAmount = 10f;

    void OnCollisionEnter(Collision other)
    {
        // Deal AOE damage
        Collider[] hits = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player hit by meteor!");
                // hit.GetComponent<PlayerHealth>()?.TakeDamage(damageAmount);
            }
        }

        //Instantiate(impactEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
