using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Unity.VisualScripting;

public class SpatialAudioExample : MonoBehaviour
{
    //Creates a reference for the location of the audio file
    //Defined in the editor
    public string audioReference;

    //Creates an instance of the audio
    //Allows for the audio file to be manipulated such as: parameters, play/pause
    private EventInstance audioInstance;

    //Creates a variable able of storing information about a GameObject
    //In this case it will be used to fetch the transform of the player
    private GameObject player;

    //The value of distance must be between 0-1 and most distances will be greater than 1
    //Therefore we attach the multiplier to keep the value of distance between 0-1
    private int maxDist = 30;

    //Called when the game starts
    void Start()
    {
        //Stores the audio into the instance
        audioInstance = RuntimeManager.CreateInstance(audioReference);
        //Locates the player by tag and attaches it to the variable
        player = GameObject.FindGameObjectWithTag("Player");
    }

    //Function is called every frame
    void Update()
    {
        //Place your trigger logic within here
        //Examples of trigger logics
        //Time delay
        //Collision
        //Entered within range 
    }

    //Call this function using PlayAudio() to play the audio
    void PlayAudio()
    {
        //Calculates the distance between this object and the player
        float dist = Vector3.Distance(transform.position, player.transform.position);
        //Calculates the distance between 0-1
        dist /= maxDist;
        //Applies this value to the parameter stored in the FMOD audio
        audioInstance.setParameterByName("Distance", dist);
        //Plays the audio
        audioInstance.start();
    }

    //Call this function using UpdateDistance() to update the volume
    void UpdateDistance() 
    {
        //Calculates the distance between this object and the player
        float dist = Vector3.Distance(transform.position, player.transform.position);
        //Calculates the distance between 0-1
        dist /= maxDist;
        //Applies this value to the parameter stored in the FMOD audio
        audioInstance.setParameterByName("Distance", dist);
    }
}
