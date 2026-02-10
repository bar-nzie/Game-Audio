using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberBehaviour : MonoBehaviour
{
    private bool hasExploded;
    public GameObject shockwavePrefab;
    private float explosionRadius = 10f;
    private int explosionDamage = 50;
    private float shockwaveDuration = 0.5f;
    public Transform player;
    private float moveSpeed = 5f;
    public Forcefield forcefield;
    private bool forcefieldActivated;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerObj").transform;
        forcefield = FindAnyObjectByType<Forcefield>();
    }

    // Update is called once per frame
    void Update()
    {
        forcefieldActivated = forcefield.IsForcefieldActive();
        ChasePlayer();
    }

    private void ChasePlayer()
    {
        if (player == null) return;
        transform.LookAt(player);
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
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
