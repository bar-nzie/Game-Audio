using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Respawn : MonoBehaviour
{
    public string sceneToLoad = "Level1";

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void StartGameScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void MenuScene()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
