using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class VolumeController : MonoBehaviour
{
    Bus masterBus;
    Bus musicBus;
    Bus ambienceBus;
    Bus dialogueBus;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        dialogueBus = RuntimeManager.GetBus("bus:/Dialogue");
    }

    public void SetMasterVolume(float value)
    {
        masterBus.setVolume(value);
    }

    public void SetMusicVolume(float value)
    {
        musicBus.setVolume(value);
    }

    public void SetAmbienceVolume(float value)
    {
        ambienceBus.setVolume(value);
    }

    public void SetDialogueVolume(float value)
    {
        dialogueBus.setVolume(value);
    }

}
