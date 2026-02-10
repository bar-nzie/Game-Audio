using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public GameObject bullet;

    public Transform attackPoint;

    public Health health;

    public float moveSpeed = 3f;
    public float buff;

    public LayerMask whatIsGround, whatIsPlayer;

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
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (player == null) return;

        if (Vector3.Distance(player.position, transform.position) < 12f)
        {
            agent.SetDestination(transform.position);
        }
        else
        {
            ChasePlayer();
        }
        //Check if in attackRange
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInAttackRange )
        {
            AttackPlayer();
        }
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(player.position, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            // If path is invalid or partial, fallback to direct movement
            agent.isStopped = true;
            ManualChasePlayer();
        }
    }

    private void ManualChasePlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Prevent floating
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    private void AttackPlayer()
    {
        if (attackPoint == null || bullet == null || player == null)
        {
            Debug.LogWarning("Missing references for attackPoint, bullet, or player.");
            return;
        }

        transform.LookAt(player);

        if(!alreadyAttacked )
        {
            StartCoroutine(BurstFire()); ;

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private IEnumerator BurstFire()
    {
        int burstCount = 4;
        float burstDelay = 0.2f;
        float chargeTime = 0.3f;  // charge duration before firing
        Vector3 finalScale = Vector3.one;

        for (int i = 0; i < burstCount; i++)
        {
            Vector3 targetPosition = new Vector3(player.position.x, player.position.y + 1f, player.position.z);
            Vector3 direction = (targetPosition - attackPoint.position).normalized;

            GameObject bulletInstance = Instantiate(bullet, attackPoint.position, Quaternion.LookRotation(direction));
            bulletInstance.transform.localScale = Vector3.zero;

            float elapsed = 0f;

            // Charge-up scaling animation
            while (elapsed < chargeTime)
            {
                if (bulletInstance == null)  // Check if bullet was destroyed
                    yield break;             // Exit coroutine safely

                float t = elapsed / chargeTime;
                bulletInstance.transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (bulletInstance == null) // Check again before applying force
                yield break;

            bulletInstance.transform.localScale = finalScale;
            audioSource.PlayOneShot(shootSound);
            Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(direction * 25f, ForceMode.Impulse);
            }

            yield return new WaitForSeconds(burstDelay);
        }
    }

    public void ApplyBuff(float difficultyFactor)
    {
        buff = difficultyFactor;
        health.SetHealth(buff);
    }
}
