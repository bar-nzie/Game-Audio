using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class UIController : MonoBehaviour
{
    public GameObject bg;

    public static bool isPaused;

    void Start()
    {
        bg.SetActive(false);
        isPaused = false;
    }

    // Update is called once per frame
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
        Debug.Log("Game Paused!");
        FMODUnity.RuntimeManager.PauseAllEvents(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        bg.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("Game Unpaused");
        FMODUnity.RuntimeManager.PauseAllEvents(false);
    }
}
