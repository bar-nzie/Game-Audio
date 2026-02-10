using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExplosion : MonoBehaviour
{
    public Transform player;       // Assign in inspector or find dynamically
    public float moveSpeed = 5f;   // Movement speed
    private float chaseDuration = 7f;
    private float waitBeforeChase = 5f;
    private bool hasExploded;
    public GameObject shockwavePrefab;
    private float explosionRadius = 30f;
    private int explosionDamage = 50;
    private float shockwaveDuration = 0.5f;
    public Forcefield forcefield;
    private bool forcefieldActivated;

    private bool isChasing = false;
    private float chaseTimer = 0f;

    void Start()
    {
        forcefield = FindAnyObjectByType<Forcefield>();
        // Find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        StartCoroutine(ChaseRoutine());
    }

    private void Update()
    {
        forcefieldActivated = forcefield.IsForcefieldActive();
    }

    IEnumerator ChaseRoutine()
    {
        // Wait before starting chase
        yield return new WaitForSeconds(waitBeforeChase);

        isChasing = true;
        chaseTimer = 0f;

        while (chaseTimer < chaseDuration)
        {
            if (player == null)
                break;

            // Move towards player
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;

            chaseTimer += Time.deltaTime;
            yield return null;
        }

        // Destroy self after chase or if player lost
        Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded) { return; }
        Vector3 position = transform.position;

        hasExploded = true;

        // 1. Create shockwave visual (optional)
        if (shockwavePrefab != null)
        {
            GameObject shockwave = Instantiate(shockwavePrefab, position, Quaternion.identity);
            StartCoroutine(ExpandShockwave(shockwave));
        }

        // 2. Damage all enemies in radius
        Collider[] hits = Physics.OverlapSphere(position, explosionRadius);
        foreach (Collider hit in hits)
        {
            if (forcefieldActivated)
            {
                return;
            }
            if (hit.CompareTag("Player"))
            {
                Health health = hit.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(explosionDamage);
                }
            }
        }

        // 3. Destroy grenade
    }

    private IEnumerator ExpandShockwave(GameObject visual)
    {
        transform.localScale = Vector3.zero;
        float elapsed = 0f;
        visual.transform.localScale = Vector3.zero;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * explosionRadius * 2f; // Diameter visual

        while (elapsed < shockwaveDuration)
        {
            float t = elapsed / shockwaveDuration;
            visual.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(visual);
        Destroy(gameObject);
    }
}
