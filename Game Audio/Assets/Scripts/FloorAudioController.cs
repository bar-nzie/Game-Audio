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
        // When the game is started - the whispers are found and started.
        whisperInstance = RuntimeManager.CreateInstance(whisperEvent);
        RuntimeManager.AttachInstanceToGameObject(whisperInstance, gameObject);

        whisperInstance.start(); 
        whisperInstance.setParameterByName("FloorLevel", 0f);

        //Debug.Log("Whisper event started and set to FloorLevel 0");
    }

    void Update()
    {
        // Drawing a line from the player going down to hit the ground that the player is standing on
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.down * rayDistance, Color.red);

        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        RaycastHit hit;

        // If that line hits any gameobject that has been assigned the floor layer in Unity then the whispers will start to play
        if (Physics.Raycast(ray, out hit, rayDistance, floorLayer))
        {
            //Debug.Log("Raycast hit: " + hit.collider.name);
            //Debug.Log("Hit Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));

            if (!isOnFloor)
            {
                // This changes the Parameter that have been set up in FMOD changes 1 then the player is on floor 1 which means the whispers play
                //Debug.Log("Standing on Floor layer - Setting FloorLevel to 1");
                whisperInstance.setParameterByName("FloorLevel", 1f);
                isOnFloor = true;
            }
        }
        else
        {
            //Debug.Log("Raycast hit NOTHING or not Floor layer");

            if (isOnFloor)
            {
                // When the player is no on floor 1, the parameter that have been set up in FMOD changes 2 meaning the player is on floor 2 which means the whispers stop playing
                //Debug.Log("Left Floor layer - Setting FloorLevel to 0");
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