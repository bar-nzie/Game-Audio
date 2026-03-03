using System.Collections;
using UnityEngine;

public class WaterSpawner : MonoBehaviour
{
    public GameObject droplet;

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
        Debug.Log("hmm");
        yield return new WaitForSeconds(Random.Range(0,3));
        Debug.Log("Running");
        Vector3 spawnPos = new Vector3(Random.Range(-10,10), transform.position.y, Random.Range(-3, 3));
        Instantiate(droplet, spawnPos, Quaternion.identity);
        yield return null;
    }
}
