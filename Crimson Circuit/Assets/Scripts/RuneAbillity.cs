using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RuneAbillity : MonoBehaviour
{
    public float attackRange = 10f;
    public float fireRate = 0.2f;
    private float fireCooldown;

    public LineRenderer lineRenderer;
    public Health health;
    public Transform player;

    public float moveInterval = 5f;
    public float moveSpeed = 1f;
    public float minDistanceFromPlayer = 1f;
    public float maxDistanceFromPlayer = 3f;
    private Vector3 targetPosition;
    private Vector3 offsetFromPlayer;

    public bool isUnlocked = false;
    public GameObject Rune;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.enabled = false;
        StartCoroutine(MoveRandomlyAroundPlayer());
    }

    public void unlock()
    {
        isUnlocked = true;
    }
    public void unlockCheck(bool check)
    {
        isUnlocked = check;
    }

    public bool forcefieldUnlocked() { return isUnlocked; }

    // Update is called once per frame
    void Update()
    {
        fireCooldown -= Time.deltaTime;
        if (isUnlocked )
        {
            Rune.SetActive(true);
            if (fireCooldown <= 0)
            {
                GameObject target = FindNearestEnemy();

                if (target != null)
                {
                    health = target.GetComponent<Health>();
                    ShootLaser(target);
                    health.TakeDamage(10f);
                    fireCooldown = 1f / fireRate;
                }
            }
            Vector3 targetPosition = player.position + offsetFromPlayer;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
        else
        {
            Rune.SetActive(false);
        }
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] Bosses = GameObject.FindGameObjectsWithTag("Boss");
        GameObject nearest = null;
        float shortestDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < shortestDist && dist <= attackRange)
            {
                shortestDist = dist;
                nearest = enemy;
            }
        }
        foreach (GameObject boss in Bosses)
        {
            float dist = Vector3.Distance(transform.position, boss.transform.position);
            if (dist < shortestDist && dist <= attackRange)
            {
                shortestDist = dist;
                nearest = boss;
            }
        }
        return nearest;
    }
    void ShootLaser(GameObject target)
    {
        transform.LookAt(target.transform);
        StartCoroutine(LaserEffect(target));
    }

    IEnumerator MoveRandomlyAroundPlayer()
    {
       while (true)
        {
            // Pick a new random offset that's not too close
            Vector3 randomDirection;
            float distance;

            do
            {
                randomDirection = Random.insideUnitSphere;
                randomDirection.y = 0f; // Keep movement on horizontal plane
                distance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
            }
            while ((randomDirection * distance).magnitude < minDistanceFromPlayer);

            offsetFromPlayer = randomDirection * distance;

            yield return new WaitForSeconds(moveInterval);
        }
    }

    private IEnumerator LaserEffect(GameObject target)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target.transform.position);
        yield return new WaitForSeconds(0.5f);

        lineRenderer.enabled = false;
    }
}
