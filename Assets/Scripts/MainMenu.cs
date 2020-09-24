using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public Animator startGamePanel;

    private IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(2f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("VisualNovel");
    }

    public void ClickStartGame()
    {
        startGamePanel.SetTrigger("activate");
        StartCoroutine(WaitForAnimation());
    }
}
