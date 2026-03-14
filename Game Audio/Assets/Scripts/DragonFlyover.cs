using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class DragonFlyover : MonoBehaviour
{
    public EventReference dragonFlyoverRef;

    private EventInstance dragonFlyover;

    private float flyoverValue = 0f;
    private bool dragonActive = false;

    void Start()
    {
        Debug.Log("DragonFlyover script started");

        dragonFlyover = RuntimeManager.CreateInstance(dragonFlyoverRef);

        RuntimeManager.AttachInstanceToGameObject(dragonFlyover, gameObject);
    }

    void Update()
    {
        if (dragonActive)
        {
            flyoverValue += Time.deltaTime * 0.3f;

            dragonFlyover.setParameterByName("Flyover", flyoverValue);

            Debug.Log("Flyover value: " + flyoverValue);

            if (flyoverValue >= 1f)
            {
                Debug.Log("Dragon flyover finished");

                dragonFlyover.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

                dragonActive = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered trigger: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered dragon trigger!");

            flyoverValue = 0f;

            dragonFlyover.start();

            dragonActive = true;
        }
    }
}