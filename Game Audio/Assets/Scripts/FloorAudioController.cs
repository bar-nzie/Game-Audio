using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FloorAudioController : MonoBehaviour
{
    public EventReference whisperEvent;
    public LayerMask floorLayer;
    public float rayDistance = 2f;

    private EventInstance whisperInstance;
    private bool isOnFloor = false;

    void Start()
    {
        whisperInstance = RuntimeManager.CreateInstance(whisperEvent);
        RuntimeManager.AttachInstanceToGameObject(whisperInstance, gameObject);

        whisperInstance.start(); 
        whisperInstance.setParameterByName("FloorLevel", 0f);
    }

    void Update()
    {
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.down * rayDistance, Color.red);

        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, floorLayer))
        {
            if (!isOnFloor)
            {
                whisperInstance.setParameterByName("FloorLevel", 1f);
                isOnFloor = true;
            }
        }
        else
        {
            if (isOnFloor)
            {
                whisperInstance.setParameterByName("FloorLevel", 0f);
                isOnFloor = false;
            }
        }
    }

    void OnDestroy()
    {
        whisperInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        whisperInstance.release();
    }
}