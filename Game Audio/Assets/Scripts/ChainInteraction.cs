using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class ChainInteraction : MonoBehaviour
{
    public EventReference chainSound;

    private EventInstance chainInstance;

    private bool playerInRange = false;

    void Start()
    {
        chainInstance = RuntimeManager.CreateInstance(chainSound);
        RuntimeManager.AttachInstanceToGameObject(chainInstance, gameObject);
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed - Starting chain scatterer");

            chainInstance.start();
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
            chainInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    void OnDestroy()
    {
        chainInstance.release();
    }
}