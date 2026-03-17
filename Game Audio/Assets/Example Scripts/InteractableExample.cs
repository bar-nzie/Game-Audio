using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Unity.VisualScripting;

public class InteractableExample : MonoBehaviour
{
    //Creates a reference for the location of the audio file
    //Defined in the editor
    public string audioReference;

    //Creates an instance of the audio
    //Allows for the audio file to be manipulated such as: parameters, play/pause
    private EventInstance audioInstance;

    //This bool determines whether the player can interact or not
    private bool canInteract;

    //Called when the game starts
    void Start()
    {
        //Stores the audio into the instance
        audioInstance = RuntimeManager.CreateInstance(audioReference);
    }

    //Function is called every frame
    void Update()
    {
        //Checks if player is in range
        if(canInteract)
        {
            //Check if player has pressed the interact button before calling the interact function
            if(Input.GetKeyDown(KeyCode.E)) Interaction();
        }
    }

    //Function containing logic for audio playing
    void Interaction()
    {
        //Starts playing the audio
        audioInstance.start();
    }

    //This is called when a GameObject enters the collider
    private void OnTriggerEnter(Collider other)
    {
        //Checks if the GameObject colliding is the player
        if(other.CompareTag("Player"))
        {
            canInteract = true;
        }
    }

    //This is called when a GameObject exits the collider
    private void OnTriggerExit(Collider other)
    {
        canInteract = false;
    }
}
