using UnityEngine;
using FMODUnity;

public class ChainInteraction : MonoBehaviour
{
    public string chainRef;

    public EventReference chainSound;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed - Playing chain sound");
            RuntimeManager.PlayOneShot(chainSound, transform.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player entered chain trigger");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player left chain trigger");
        }
    }
}