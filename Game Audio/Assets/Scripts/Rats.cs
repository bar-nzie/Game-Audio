using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class Rats : MonoBehaviour
{
    public EventReference ratEvent;
    public LayerMask floorLayer;
    public float rayDistance = 2f;

    private EventInstance ratInstance;
    private bool isOnFloor = false;

    void Start()
    {
        ratInstance = RuntimeManager.CreateInstance(ratEvent);
        RuntimeManager.AttachInstanceToGameObject(ratInstance, gameObject);

        ratInstance.start(); 
        ratInstance.setParameterByName("FloorLevel", 0f);
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
                ratInstance.setParameterByName("FloorLevel", 1f);
                isOnFloor = true;
            }
        }
        else
        {
            if (isOnFloor)
            {
                ratInstance.setParameterByName("FloorLevel", 0f);
                isOnFloor = false;
            }
        }
    }

    void OnDestroy()
    {
        ratInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        ratInstance.release();
    }
}