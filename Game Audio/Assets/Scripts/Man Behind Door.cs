using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class ManBehindDoor : MonoBehaviour
{   
    public string manbehindboorRef;

    public EventReference manbehindboorSound;

    private bool playerInRange = false;

    private EventInstance manbehindboor;
    public GameObject player;

    bool played = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Update()
    {
        if(played) return;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed - Playing chain sound");
            RuntimeManager.PlayOneShot(manbehindboorSound, transform.position);
            played = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Player entered door trigger");
        }

        float dist = Vector3.Distance(transform.position, player.transform.position);
        dist /= 30;
        manbehindboor.setParameterByName("TorchDistance", dist);
        manbehindboor.start();
    }
}
