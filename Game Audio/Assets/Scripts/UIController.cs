using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class UIController : MonoBehaviour
{
    public GameObject bg;
    public GameObject options;
    public Bus ambBus;
    public Bus musBus;

    public string audioReference;
    private EventInstance audioInstance;

    public EventReference BGEvent;

    private EventInstance BGInstance;

    public static bool isPaused;

    void Start()
    {
        bg.SetActive(false);
        isPaused = false;

        audioInstance = RuntimeManager.CreateInstance(audioReference);
        BGInstance = RuntimeManager.CreateInstance(BGEvent);

        BGInstance.start();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            
            TogglePause();
            AudioHandler();
        }
    }

    public void TogglePause()
    {
        bg.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
        //FMODUnity.RuntimeManager.PauseAllEvents(true);
        FMODUnity.RuntimeManager.GetBus("bus:/Ambience").setPaused(true);
        audioInstance.start();
        BGInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void ResumeGame()
    {
        isPaused = false;
        bg.SetActive(false);
        options.SetActive(false);
        Time.timeScale = 1f;
        FMODUnity.RuntimeManager.PauseAllEvents(false);
        FMODUnity.RuntimeManager.GetBus("bus:/Ambience").setPaused(false);
        audioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        BGInstance.start();
    }

    public void ToggleOptions(){

        options.SetActive(true);
    }

    public void AudioHandler(){

        audioInstance.start();
        Debug.Log("Pause Played");
    }
}
