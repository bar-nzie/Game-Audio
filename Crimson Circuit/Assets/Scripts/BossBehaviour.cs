using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Cinemachine;

public class BossBehaviour : MonoBehaviour
{
    public float shockwaveRadius = 10f;
    public float shockwaveForce = 500f;
    public float shockwaveDuration = 1f;
    public GameObject shockwaveVisualPrefab;
    public CameraShake cameraShake;
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.2f;
    public GameObject sword;
    private Vector3 StartPosition;
    public float slamSpeed = 10f;
    public float raiseSpeed = 10f;
    public GameObject Spawner;
    public GameObject spawnerPrefab;
    public int spawnerCount = 6;
    public float radius = 5f;
    public float rotationSpeed = 45f;

    public AudioClip shoot1;
    public AudioClip shoot2;
    public AudioSource AudioSource;

    private List<Transform> spawners = new List<Transform>();
    private float meleeRadius = 20f;
    public Transform player;
    public float attackCooldown = 10f;
    private float lastAttackTime;
    private bool isAttacking;

    public CinemachineImpulseSource impulseSource;
    public Animator animator;



    public float idleHeight = 4f;
    public float slamHeight = 0.5f;
    public float heightLerpSpeed = 10f;


    void Start()
    {
        StartPosition = sword.transform.position;

        player = GameObject.Find("Player").transform;

        animator.SetBool("IsIdle", true);

    }

    private void Update()
    {
        player = GameObject.Find("Player").transform;
        //if (Input.GetKeyDown(KeyCode.Q)) // Trigger shockwave on space key for testing
        //{
        //    StartCoroutine(ShockwaveAttack());
        //}
        //if (Input.GetKeyDown(KeyCode.O)) // Trigger shockwave on space key for testing
        //{
        //    StartCoroutine(SlashAttack());
        //}
        //if (Input.GetKeyDown(KeyCode.I)) // Trigger shockwave on space key for testing
        //{
        //    StartCoroutine(OrbAttack());
        //}
        //if (Input.GetKeyDown(KeyCode.U)) // Trigger shockwave on space key for testing
        //{
        //    ActivateOrbBarrage();
        //}
        foreach (Transform spawner in spawners)
        {
            if (spawner == null) continue;

            spawner.RotateAround(transform.position, Vector3.up, rotationSpeed * Time.deltaTime);
        }

        if (!isAttacking && player != null)
        {
            Vector3 lookDirection = player.position - transform.position;
            lookDirection.y = 0f; // Ignore vertical difference
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // Smooth turn
            }
        }

        PerformAttack();
    }

    IEnumerator ShockwaveAttack()
    {
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsShockwave", true);
        // Wait until animation starts
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Shockwave"));
        GameObject visual = Instantiate(shockwaveVisualPrefab, transform.position, Quaternion.identity);
        if (cameraShake != null)
        {
            StartCoroutine(cameraShake.Shake(shakeDuration, shakeMagnitude));
        }
        impulseSource.GenerateImpulse();
        float elapsed = 0f;

        HashSet<Collider> hitPlayers = new HashSet<Collider>();

        while (elapsed < shockwaveDuration)
        {
            float currentRadius = Mathf.Lerp(0, shockwaveRadius, elapsed / shockwaveDuration);
            visual.transform.localScale = new Vector3(currentRadius, currentRadius, currentRadius);
            ApplyShockwaveDamage(currentRadius, hitPlayers);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(visual);
        // Then wait for animation to finish
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("Shockwave")
        );

        animator.SetBool("IsShockwave", false);
        animator.SetBool("IsIdle", true);
    }

    void ApplyShockwaveDamage(float currentRadius, HashSet<Collider> hitPlayers)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, currentRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player") && !hitPlayers.Contains(hit))
            {
                hitPlayers.Add(hit); // mark as hit

                Debug.Log("Hit");

                // Optional: deal damage or trigger player reaction
                // hit.GetComponent<PlayerHealth>()?.TakeDamage(10);
            }
        }
    }

    IEnumerator SlashAttack()
    {
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsSlam", true);
        isAttacking = true;
        yield return StartCoroutine(MoveToHeight(slamHeight));
        // Wait until the "Slam" animation starts
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("SlamAttack"));
        lastAttackTime = Time.time;

        Coroutine impulseCoroutine = StartCoroutine(TriggerImpulseEverySecond());
        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("SlamAttack")
        );

        StopCoroutine(impulseCoroutine);

        animator.SetBool("IsSlam", false);
        animator.SetBool("IsIdle", true);
        yield return StartCoroutine(MoveToHeight(idleHeight));
        isAttacking = false;
    }

    //private IEnumerator SlamRoutine()
    //{
    //    Vector3 startPos = sword.transform.position;
    //    Vector3 downPos = new Vector3(startPos.x, 1f, startPos.z);
    //    Vector3 upPos = new Vector3(startPos.x, 10f, startPos.z);

    //    // Move sword down
    //    while (Vector3.Distance(sword.transform.position, downPos) > 0.01f)
    //    {
    //        sword.transform.position = Vector3.MoveTowards(sword.transform.position, downPos, slamSpeed * Time.deltaTime);
    //        yield return null;
    //    }
    //    impulseSource.GenerateImpulse();
    //    sword.transform.position = downPos;

    //    // Pause at bottom
    //    yield return new WaitForSeconds(0.3f);

    //    // Move sword back up
    //    while (Vector3.Distance(sword.transform.position, upPos) > 0.01f)
    //    {
    //        sword.transform.position = Vector3.MoveTowards(sword.transform.position, upPos, raiseSpeed * Time.deltaTime);
    //        yield return null;
    //    }

    //    // Make sure sword ends exactly at upPos
    //    sword.transform.position = upPos;
    //}

    //private IEnumerator RotateOverTime(float angle, float duration)
    //{
    //    Quaternion startRotation = transform.rotation;
    //    Quaternion endRotation = Quaternion.Euler(0f, transform.eulerAngles.y + angle, 0f);

    //    float elapsed = 0f;
    //    while (elapsed < duration)
    //    {
    //        transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
    //        elapsed += Time.deltaTime;
    //        yield return null;
    //    }

    //    // Ensure it ends exactly at target
    //    transform.rotation = endRotation;
    //}

    private IEnumerator OrbAttack()
    {
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsAriel", true);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("ArielAttack"));

        AudioSource.PlayOneShot(shoot1);

        for (int i = 0; i < 100; i++)
        {
            Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-40f, 40f), 10f, transform.position.z + Random.Range(-40f, 40f));

            Instantiate(Spawner, randomPosition, Quaternion.identity);
        }

        yield return new WaitUntil(() =>
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("ArielAttack")
        );

        animator.SetBool("IsAriel", false);
        animator.SetBool("IsIdle", true);
        yield return null;
    }

    public void ActivateOrbBarrage()
    {
        isAttacking = true;
        StartCoroutine(OrbBarrageWithDelayedCooldown());
    }

    private IEnumerator OrbBarrageWithDelayedCooldown()
    {
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsSpin", true);

        AudioSource.PlayOneShot(shoot2);

        for (int i = 0; i < spawnerCount; i++)
        {
            isAttacking = true;

            float angle = i * Mathf.PI * 2f / spawnerCount;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Vector3 spawnPos = new Vector3(transform.position.x, 2f, transform.position.z) + offset;

            GameObject spawner = Instantiate(spawnerPrefab, spawnPos, Quaternion.LookRotation(offset.normalized));
            spawner.transform.SetParent(transform); // Parent to boss to allow orbiting
            spawners.Add(spawner.transform);
        }

        // Wait 10 seconds before setting cooldown
        yield return new WaitForSeconds(10f);

        animator.SetBool("IsSpin", false);
        animator.SetBool("IsIdle", true);

        lastAttackTime = Time.time;
        isAttacking = false;
    }

    public void PerformAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown || isAttacking == true)
            return; // Still on cooldown, do nothing

        if (player == null)
        {
            Debug.LogWarning("Player not assigned.");
            return;
        }

        float distanceToPlayer = Vector3.Distance(gameObject.transform.position, player.position);
        Debug.Log(distanceToPlayer);

        if (distanceToPlayer <= meleeRadius)
        {
            // Close range
            int closeAttackIndex = Random.Range(0, 2); // 0 or 1
            if (closeAttackIndex == 0)
            {
                StartCoroutine(ShockwaveAttack());
            }
            else
            {
                StartCoroutine(SlashAttack());
            }   
        }
        else
        {
            // Long range
            int longRangeAttackIndex = Random.Range(0, 2); // 0 or 1
            if (longRangeAttackIndex == 0)
            {
                StartCoroutine(OrbAttack());
            }
            else
            {
                ActivateOrbBarrage();
            }
        }

        lastAttackTime = Time.time; // Reset cooldown timer
    }

   

    IEnumerator MoveToHeight(float targetY)
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(currentPos.x, targetY, currentPos.z);

        while (Mathf.Abs(transform.position.y - targetY) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * heightLerpSpeed);
            yield return null;
        }

        transform.position = targetPos;
    }

    IEnumerator TriggerImpulseEverySecond()
    {
        while (true)
        {
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse();
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
