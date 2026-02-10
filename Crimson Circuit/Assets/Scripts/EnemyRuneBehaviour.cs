using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRuneBehaviour : MonoBehaviour
{
    public GameObject player;
    public float fireRate = 0.2f;
    private float fireCooldown;
    public LineRenderer lineRenderer;
    public Health health;

    public float moveInterval = 5f;
    public float moveSpeed = 1f;
    public float minDistanceFromPlayer = 1f;
    public float maxDistanceFromPlayer = 3f;
    private Vector3 targetPosition;
    private Vector3 offsetFromPlayer;
    public GameObject BossPrefab;

    // Start is called before the first frame update
    void Start()
    {
        BossPrefab = GameObject.Find("ThirdBoss(Clone)");
        player = GameObject.FindGameObjectWithTag("Player");
        health = player.GetComponent<Health>();
        StartCoroutine(MoveRandomlyAroundPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0)
        {

            if (player != null)
            {
                ShootLaser(player);
                health.TakeDamage(3f);
                fireCooldown = 1f / fireRate;
            }
        }
        if (BossPrefab != null)
        {
            Vector3 targetPosition = BossPrefab.transform.position + offsetFromPlayer;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void ShootLaser(GameObject target)
    {
        transform.LookAt(target.transform);
        StartCoroutine(LaserEffect(target));
    }

    private IEnumerator LaserEffect(GameObject target)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target.transform.position);
        yield return new WaitForSeconds(0.5f);

        lineRenderer.enabled = false;
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
                randomDirection.y = 1f; // Keep movement on horizontal plane
                distance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
            }
            while ((randomDirection * distance).magnitude < minDistanceFromPlayer);

            offsetFromPlayer = randomDirection * distance;

            yield return new WaitForSeconds(moveInterval);
        }
    }
}
