using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightningBehaviour : MonoBehaviour
{
    public Transform player;

    public GameObject bullet;

    public GameObject attackTarget;

    public Animator animator;

    public Health health;
    
    public LineRenderer lineRenderer;

    public float moveSpeed = 3f;
    public float buff;
    public float yaxis = 4f;
    private Vector3 positioning;

    public LayerMask whatIsGround, whatIsPlayer;

    //Attacking
    private float timeBetweenAttacks;
    bool alreadyAttacked;
    bool playerInAttackRange;

    //States
    public float attackRange;
    public float range = 20.0f;

    public void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        attackTarget.SetActive(true);
    }

    private void Start()
    {
        timeBetweenAttacks = Random.Range(2f, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;
        AttackPlayer();
    }

    private void AttackPlayer()
    {
        if (player == null) return;

        if (!alreadyAttacked)
        {
            animator.SetBool("IsAttacking", true);

            StartCoroutine(Strike());

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;

        animator.SetBool("IsAttacking", false);
    }

    private IEnumerator Strike()
    {
        // 1. Cache player position for strike (in case player moves)
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y + yaxis, player.position.z);
        attackTarget.transform.position = targetPosition;

        // 2. Instantiate bullet at that position with zero scale
        GameObject bulletInstance = Instantiate(bullet, attackTarget.transform.position, Quaternion.identity);
        bulletInstance.transform.localScale = Vector3.zero;

        float chargeTime = 0.3f;
        float elapsed = 0f;
        Vector3 finalScale = new Vector3(1f, 2.5f, 1f);

        // 3. Animate scale up (charge-up effect)
        while (elapsed < chargeTime)
        {
            if (bulletInstance == null)  // Check if bullet was destroyed
                yield break;
            attackTarget.transform.position = targetPosition;
            float t = elapsed / chargeTime;
            bulletInstance.transform.localScale = Vector3.Lerp(Vector3.zero, finalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }


        bulletInstance.transform.localScale = finalScale;

        // 4. Apply downward force
        Rigidbody rb = bulletInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.down * 70f, ForceMode.Impulse);
        }
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, attackTarget.transform.position);
        Vector3 position = new Vector3(attackTarget.transform.position.x + Random.Range(-1f, 1f), attackTarget.transform.position.y - 2f, attackTarget.transform.position.z + Random.Range(-1f, 1f));
        lineRenderer.SetPosition(1, position);
        position = new Vector3(attackTarget.transform.position.x + Random.Range(-1f, 1f), attackTarget.transform.position.y - 4f, attackTarget.transform.position.z + Random.Range(-1f, 1f));
        lineRenderer.SetPosition(2, position);
        position = new Vector3(attackTarget.transform.position.x, attackTarget.transform.position.y - 6f, attackTarget.transform.position.z);
        lineRenderer.SetPosition(3, position);

    }

    public void ApplyBuff(float difficultyFactor)
    {
        buff = difficultyFactor;
        health.SetHealth(buff);
    }
}
