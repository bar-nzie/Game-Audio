using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class DragonFlyover : MonoBehaviour
{
    public EventReference dragonFlyoverRef;
    public Transform startPoint;
    public Transform endPoint;
    public float flyDuration = 5f;

    private EventInstance dragonFlyover;
    private bool dragonActive = false;
    private float timer = 0f;

    void Start()
    {
        dragonFlyover = RuntimeManager.CreateInstance(dragonFlyoverRef);
        RuntimeManager.AttachInstanceToGameObject(dragonFlyover, gameObject);

        Debug.Log("Dragon FMOD instance created");
    }

    void Update()
    {
        if (!dragonActive) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / flyDuration);

        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, t);

        RuntimeManager.AttachInstanceToGameObject(dragonFlyover, gameObject);

        Debug.DrawLine(startPoint.position, endPoint.position, Color.cyan);
        Debug.Log("Dragon position: " + transform.position + " | Progress: " + t);

        if (t >= 1f)
        {
            dragonFlyover.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            dragonActive = false;
            Debug.Log("Dragon flyover finished");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !dragonActive)
        {
            timer = 0f;
            transform.position = startPoint.position;

            dragonFlyover.start();
            dragonActive = true;

            Debug.Log("Dragon flyover started");
            Debug.Log("Start point: " + startPoint.position);
            Debug.Log("End point: " + endPoint.position);
        }
    }

    void OnDestroy()
    {
        dragonFlyover.release();
    }
}