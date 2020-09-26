using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class EndGameScreen : MonoBehaviour
{
    public static EndGameScreen instance;
    public Animator endGamePanel;
    public GameObject root;
    private bool endGameClicked = false;


    void Awake()
    {
        instance = this;
    }


    private IEnumerator WaitForAnimation()
    {
        endGameClicked = true;
        yield return new WaitForSeconds(2f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator WaitForFadeOut(string endGameImage)
    {
        yield return new WaitForSeconds(2f);
        NovelController.instance.Command_SetLayerImage(endGameImage + ",100f,false", BCFC.instance.foreground);
        root.SetActive(true);
        endGamePanel.SetTrigger("showEndGame");
    }

    public void ClickRetry()
    {
        if (endGameClicked == false)
        {
            endGamePanel.SetTrigger("end");
            StartCoroutine(WaitForAnimation());
        }
    }

    public void setEndGame(string endGameImage)
    {
        endGamePanel.SetTrigger("activate");
        StartCoroutine(WaitForFadeOut(endGameImage));
    }
}
