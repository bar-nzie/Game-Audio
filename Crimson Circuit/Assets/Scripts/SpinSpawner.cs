using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinSpawner : MonoBehaviour
{
    public GameObject orbPrefab;
    public float fireRate = 0.1f;
    public float orbSpeed = 10f;
    public float lifeSpan = 5f;

    private void Start()
    {
        StartCoroutine(FireLoop());
        Destroy(gameObject, lifeSpan);
    }

    IEnumerator FireLoop()
    {
        while (true)
        {
            GameObject orb = Instantiate(orbPrefab, transform.position, transform.rotation);
            Rigidbody rb = orb.GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = transform.forward * orbSpeed;

            yield return new WaitForSeconds(fireRate);
        }
    }
}
