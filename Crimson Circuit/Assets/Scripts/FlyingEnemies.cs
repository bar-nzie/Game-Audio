using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemies : MonoBehaviour
{
    private GameObject[] airPoints;

    public Transform player;

    public GameObject bullet;

    public Transform attackPoint;

    public Health health;

    public float moveSpeed = 3f;
    public float buff;

    public LayerMask whatIsGround, whatIsPlayer;

    private GameObject currentTarget;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    bool playerInAttackRange;

    public AudioSource audioSource;
    public AudioClip shootSound;

    //States
    public float attackRange;

    public void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
    }

    public void Start()
    {
        airPoints = new GameObject[8];
        airPoints[0] = GameObject.Find("AirPoint");
        airPoints[1] = GameObject.Find("AirPoint (1)");
        airPoints[2] = GameObject.Find("AirPoint (2)");
        airPoints[3] = GameObject.Find("AirPoint (4)");
        airPoints[4] = GameObject.Find("AirPoint (5)");
        airPoints[5] = GameObject.Find("AirPoint (6)");
        airPoints[6] = GameObject.Find("AirPoint (7)");
        airPoints[7] = GameObject.Find("AirPoint (8)");
        PickNewAirPoint();

    }

    private void Update()
    {
        if (player == null) return;

        //Check if in attackRange
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        ChasePlayer();

        if (playerInAttackRange)
        {
            AttackPlayer();
        }
    }

    private void ChasePlayer()
    {
        if (currentTarget == null) return;

        transform.position = Vector3.MoveTowards(transform.position, currentTarget.transform.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentTarget.transform.position) < 0.5f)
        {
            StartCoroutine(AirPoint());
        }
    }

    private void AttackPlayer()
    {
        if (attackPoint == null || bullet == null || player == null)
        {
            Debug.LogWarning("Missing attackPoint, bullet prefab, or player reference!");
            return;
        }

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            StartCoroutine(LazerFire()); ;

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private IEnumerator LazerFire()
    {
        // 1. Cache aim direction toward player at charge start
        Vector3 aimDirection = (player.position - attackPoint.position).normalized;

        // 2. Instantiate bullet at attackPoint with zero scale
        GameObject bulletInstance = Instantiate(bullet, attackPoint.position, Quaternion.LookRotation(aimDirection));
        bulletInstance.transform.localScale = Vector3.zero;

        float chargeTime = 0.3f;
        float elapsed = 0f;
        Vector3 finalScale = new Vector3(3f, 3f, 3f);

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
        audioSource.PlayOneShot(shootSound);
        // 5. Apply force in cached direction
        Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(aimDirection * 10f, ForceMode.Impulse);
        }
    }

    private void PickNewAirPoint()
    {
        if (airPoints.Length > 0)
        {
            currentTarget = airPoints[Random.Range(0, airPoints.Length)];
        }
    }

    private IEnumerator AirPoint()
    {
        yield return new WaitForSeconds(5f);
        PickNewAirPoint();
    }

    public void ApplyBuff(float difficultyFactor)
    {
        buff = difficultyFactor;
        health.SetHealth(buff);
    }
}
