using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossShooters : MonoBehaviour
{
    public GameObject orbPrefab;
    public float fireRate = 0.5f;
    public float orbSpeed = 10f;
    public SecondBossBehaviour SecondBossBehaviour;

    private void Start()
    {
        StartCoroutine(FireLoop());
    }

    private void Update()
    {
        fireRate = SecondBossBehaviour.GetFireRate();
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
