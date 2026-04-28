using UnityEngine;
using System.Collections;

public class RocksSpawner : MonoBehaviour
{
    public GameObject rocks;

    void Start()
    {
        InvokeRepeating(nameof(thirdParty), 1, 7);
    }

    void thirdParty()
    {
        StartCoroutine(spawnDroplet());
    }

    private IEnumerator spawnDroplet()
    {
        //Debug.Log("hmm");
        yield return new WaitForSeconds(Random.Range(0, 10));
        //Debug.Log("Running");
        Vector3 spawnPos = new Vector3(Random.Range(-10, 10), transform.position.y, Random.Range(0, 10));
        Instantiate(rocks, spawnPos, Quaternion.identity);
        yield return null;
    }
}