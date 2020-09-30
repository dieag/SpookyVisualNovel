using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseScreen : MonoBehaviour
{
    public static PauseScreen instance;
    public static bool savePreviousState = false;
    public Button saveButton;
    public Button homeButton;
    public Slider volumeSlider;
    [HideInInspector]
    public bool isPaused = false;
    public GameObject root;
    public TextMeshProUGUI pauseText;
    public MainMenuButton reloadGameButton;
    public MainMenuButton saveGameButton;
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
      pauseText.text = "<color=red>Game Saved</color>";
    }

    public void GoHome(Button button)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

       public void ChangeVol() {
        AudioListener.volume = volumeSlider.value;
    }

    public void ButtonHover(MainMenuButton button)
    {
        pauseText.text = button.menuText;
    }
    public void ButtonUnHover()
    {
        pauseText.text = "";
    }
}
