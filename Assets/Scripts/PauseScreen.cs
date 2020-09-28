using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
    public static PauseScreen instance;
    public static bool savePreviousState = false;
    public Button saveButton;
    public Button homeButton;
    [HideInInspector]
    public bool isPaused = false;
    public GameObject root;
    void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseGame();
        }
    }

    public void pauseGame()
    {
        isPaused ^= true;
        if(root != null)
            root.SetActive(isPaused);
        NovelController.instance.blockNext = isPaused;
    }

    public void SaveState(Button button)
    {
      NovelController.instance.SaveGameFile(savePreviousState);
    }

    public void GoHome(Button button)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
