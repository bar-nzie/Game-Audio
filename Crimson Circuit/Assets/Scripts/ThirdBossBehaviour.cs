using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdBossBehaviour : MonoBehaviour
{
    public GameObject[] sweepingLasers;
    public GameObject[] gridLasers;
    public GameObject enemyRunePrefab;
    public GameObject enemy1;
    public GameObject enemy2;
    private float lastAttackTime;
    private bool isAttacking;
    private float attackCooldown = 7f;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, 5f, transform.position.z);
        StartCoroutine(SpawnEnemies());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //GridLasers();
            //StartCoroutine(SweepingLasers());
        }
        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(PerformRandomAttack());
        }
    }
    private IEnumerator SweepingLasers()
    {
        foreach (GameObject laser in sweepingLasers)
        {
            laser.GetComponent<LazerBehaviour>().StartLaser();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void GridLasers()
    {
        int index = 0;
        for (int i = -6; i <= 5; i++)
        {
            Vector3 start = new Vector3(transform.position.x + i * 7f, 1, transform.position.z -50f);
            Vector3 end = new Vector3(transform.position.x + i * 7f, 1, transform.position.z + 50f);
            gridLasers[index++].GetComponent<LazerBehaviour>().Activate(start, end);
        }
        for (int j = -4; j <= 3; j++)
        {
            Vector3 start = new Vector3(transform.position.x - 50f, 1, transform.position.z + j * 8f);
            Vector3 end = new Vector3(transform.position.x + 50f, 1, transform.position.z + j * 8f);
            gridLasers[index++].GetComponent<LazerBehaviour>().Activate(start, end);
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (enemy1 == null)
            {
                enemy1 = Instantiate(enemyRunePrefab, transform.position, Quaternion.identity);
            }
            if (enemy2 == null)
            {
                enemy2 = Instantiate(enemyRunePrefab, transform.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(10f);
        }
    }

    private IEnumerator PerformRandomAttack()
    {
        isAttacking = true;

        int attackChoice = Random.Range(0, 2); // 0, 1, or 2

        switch (attackChoice)
        {
            case 0:
                yield return StartCoroutine(SweepingLasers());
                break;
            case 1:
                GridLasers();
                break;
        }

        lastAttackTime = Time.time;
        isAttacking = false;
    }
}
