using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossBehaviour : MonoBehaviour
{
    public GameObject[] Shooters;
    public GameObject enemyPrefab;
    private float firerate = 0.5f;
    private bool isSpinning;
    public GameObject explosivePrefab;
    public GameObject player;
    public GameObject attackPoint;
    private float moveSpeed;
    private float fastSpinSpeed = 45f;

    private float attackCooldown = 10f;
    private float lastAttackTime = -999f; // Start far in past to allow first attack immediately
    private bool isAttacking = false;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        //if (Input.GetKeyUp(KeyCode.K))
        //{
        //   StartCoroutine(SpawnEnemies());
        //}
        if (!isSpinning && player != null)
        {
            Vector3 lookDirection = player.transform.position - transform.position;
            lookDirection.y = 0f; // Ignore vertical difference

            if (lookDirection.sqrMagnitude > 0.001f) // Avoid NaNs
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 0.25f); // Slow turn
            }
        }
        if (isSpinning)
        {
            transform.Rotate(Vector3.up, fastSpinSpeed * Time.deltaTime);
        }
        if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(PerformRandomAttack());
        }
    }

    private IEnumerator FastFire()
    {
        firerate = 0.1f;
        isSpinning = true;

        yield return new WaitForSeconds(5f);

        firerate = 0.5f;
        isSpinning = false;
    }

    private IEnumerator ChargeAttack()
    {
        // 1. Cache aim direction toward player at charge start
        Vector3 aimDirection = (player.transform.position - attackPoint.transform.position).normalized;

        // 2. Instantiate bullet at attackPoint with zero scale
        GameObject bulletInstance = Instantiate(explosivePrefab, attackPoint.transform.position, Quaternion.LookRotation(aimDirection));
        bulletInstance.transform.localScale = Vector3.zero;

        float chargeTime = 5f;
        float elapsed = 0f;
        Vector3 finalScale = new Vector3(10f, 10f, 10f);

        // 3. Scale up the bullet over chargeTime
        while (elapsed < chargeTime)
        {
            if (bulletInstance == null) yield break;
            float t = elapsed / chargeTime;
            bulletInstance.transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 4. Make sure it's fully scaled
        bulletInstance.transform.localScale = finalScale;
    }

    private IEnumerator SpawnEnemies()
    {
        int enemiesToSpawn = 3;
        

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // Random position within ±40 units of boss (on XZ plane)
            Vector3 randomOffset = new Vector3(Random.Range(-40f, 40f), 0f, Random.Range(-40f, 40f));
            Vector3 spawnPosition = transform.position + randomOffset;

            // Set fixed ground height (adjust if needed)
            spawnPosition.y = 1f;

            // Spawn enemy
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.Euler(0, Random.Range(0f, 360f), 0));

            yield return null;
        }
    }

    private IEnumerator PerformRandomAttack()
    {
        isAttacking = true;

        int attackChoice = Random.Range(0, 3); // 0, 1, or 2

        switch (attackChoice)
        {
            case 0:
                yield return StartCoroutine(FastFire());
                break;
            case 1:
                yield return StartCoroutine(ChargeAttack());
                break;
            case 2:
                yield return StartCoroutine(SpawnEnemies());
                break;
        }

        lastAttackTime = Time.time;
        isAttacking = false;
    }


    public float GetFireRate() { return firerate; }
}
