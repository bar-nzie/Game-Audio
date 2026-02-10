using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionDamage = 100f;
    public GameObject shockwavePrefab;
    public float shockwaveDuration = 2f;
    private bool hasExploded = false;

    private void OnTriggerEnter(Collider other)
    {
        Explode();
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
            if (hit.CompareTag("Enemy") || hit.CompareTag("Boss"))
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
