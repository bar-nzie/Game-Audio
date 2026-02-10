using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SixShooterBehaviour : MonoBehaviour
{
    public int spawnCount = 6;
    public float radius = 3f;
    private bool isAttacking;
    public GameObject spawnerPrefab;
    private List<Transform> spawners = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            isAttacking = true;

            float angle = i * Mathf.PI * 2f / spawnCount;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z) + offset;

            GameObject spawner = Instantiate(spawnerPrefab, spawnPos, Quaternion.LookRotation(offset.normalized));
            spawner.transform.SetParent(transform); // Parent to boss to allow orbiting
            spawners.Add(spawner.transform);
        }

        yield break;
    }
}
