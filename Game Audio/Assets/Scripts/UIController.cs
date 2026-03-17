using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class UIController : MonoBehaviour
{
    public GameObject bg;
    public GameObject options;

    public static bool isPaused;

    void Start()
    {
        bg.SetActive(false);
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            
            TogglePause();
        }
    }

    public void TogglePause()
    {
        bg.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
        FMODUnity.RuntimeManager.PauseAllEvents(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        bg.SetActive(false);
        options.SetActive(false);
        Time.timeScale = 1f;
        FMODUnity.RuntimeManager.PauseAllEvents(false);
    }

    public void ToggleOptions(){

        options.SetActive(true);
    }
}
