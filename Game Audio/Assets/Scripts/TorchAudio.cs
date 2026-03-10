using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class TorchAudio : MonoBehaviour
{
    public EventReference torchEvent;
    public GameObject Player;

    private EventInstance torchInstance;
    public bool isNextToTorch = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        torchInstance = RuntimeManager.CreateInstance(torchEvent);
        RuntimeManager.AttachInstanceToGameObject(torchInstance, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(){

        isNextToTorch = true;

        if (isNextToTorch == true) {

            torchInstance.start();
            Debug.Log("Enter range of torch");
        }
    }

    public void OnTriggerExit(){
        
        isNextToTorch = false;
        Debug.Log("Exit range of torch");
    }

}
