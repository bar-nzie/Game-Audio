using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class BGMusic : MonoBehaviour
{

    public EventReference BGEvent;

    private EventInstance BGInstance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BGInstance = RuntimeManager.CreateInstance(BGEvent);
        
         BGInstance.start(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
