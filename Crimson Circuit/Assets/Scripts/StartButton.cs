using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public string sceneToLoad = "Level1";
    private string quitscene = "StartMenu";
    public Movement player;
    public GameInfoManager gameInfoManager;
    public bool StartMenu;

    private void Start()
    {
        if (StartMenu)
        {
            Time.timeScale = 1.0f;
        }
    }

    public void StartGameScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void Resume()
    {
        Debug.Log("CLICKED!");

        player.OnPause();
    }

    public void QuitToMenu()
    {
        gameInfoManager.Saving();
        SceneManager.LoadScene(quitscene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
