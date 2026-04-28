using UnityEngine;
using FMODUnity;

public class FallingRocks : MonoBehaviour
{
    public EventReference rocksEvent;

    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        float distanceParam = dist / 30f;
        RuntimeManager.PlayOneShot(rocksEvent, transform.position);
        Debug.Log("Rock sound played");
        Destroy(gameObject, 0.2f);
    }
}