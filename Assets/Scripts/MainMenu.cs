using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private bool waitForAnimation = false;
    public Animator startGamePanel;

    private void Start()
    {
        AudioClip clip = Resources.Load("Audio/Music/music_title") as AudioClip;
        AudioManager.instance.PlaySong(clip);
    }

    private IEnumerator WaitForFadeIn()
    {
        yield return new WaitForSeconds(2f);
        waitForAnimation = true;
    }

    private IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("VisualNovel");
    }

    public void ClickStartGame()
    {
        if (!waitForAnimation)
        {
            startGamePanel.SetTrigger("activate");
            StartCoroutine(WaitForAnimation());
        }
    }
}
