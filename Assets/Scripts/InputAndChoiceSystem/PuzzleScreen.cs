using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzleScreen : MonoBehaviour
{

	public static PuzzleScreen instance;
	public PuzzleButton cypherButton;
    public PuzzleButton enterButton;
	public Timer timer;
    public GameObject inputField;
    public GameObject inputHeader;
    private string keyImage;
    private bool enterButtonHit = false;

    void Awake()
	{
		instance = this;
	}

    IEnumerator WaitForKeyDown(KeyCode keyCode)
    {
        while(!Input.GetKeyDown(keyCode) && !enterButtonHit) {
            if(timer.isTimeOut && timer.isTimerOn) {
                yield break;
            }
            yield return null; 
        }
            
    }

    Coroutine handlingPuzzle = null;
    public void puzzleStart(string puzzleName, string _keyImage, string keyCode, string passChapter, string failChapter, float timerStart)
    {
        PauseScreen.savePreviousState = true;
        NovelController.instance.blockNext = true;
        NovelController.instance.Command_SetLayerImage(puzzleName + ",1f,true", BCFC.instance.foreground);
        if (timerStart > 0f) {
           NovelController.instance.Command_PlayMusic("music_puzzle,0.5,0.5", false);
           NovelController.instance.Command_StopAmbientMusic();
        }
        InputScreen.Show("Enter the Key");
        handlingPuzzle = StartCoroutine(HandlePuzzle(puzzleName, keyCode, passChapter,failChapter,timerStart));
        keyImage = _keyImage;
	}

    IEnumerator HandlePuzzle(string puzzleName, string keyCode, string passChapter, string failChapter, float timerStart)
    {
        bool gameDone = false;
        AudioClip wrongAnswerClip = Resources.Load("Audio/SFX/effect_puzzle_fail") as AudioClip;
        AudioClip passedClip = Resources.Load("Audio/SFX/effect_puzzle_pass") as AudioClip;
        AudioClip failClip = Resources.Load("Audio/SFX/effect_puzzle_timeup") as AudioClip;
        bool passedTimedGame = true;
        string puzzleInput = InputScreen.currentInput;

        timer.StartTimer(timerStart);
        if (!timer.isTimerOn)
        {
            timer.gameObject.SetActive(false);
        }   
        else
        {
            timer.gameObject.SetActive(true);
        }
        while(gameDone == false) {
            yield return StartCoroutine(WaitForKeyDown(KeyCode.Return));
            if(timer.isTimeOut && timer.isTimerOn) {
                //Out of Time so failed
                passedTimedGame = false;
                break;
            }
            puzzleInput = InputScreen.currentInput;
            gameDone = (puzzleInput == keyCode);
            if (gameDone == false) {
                enterButtonHit = false;
                AudioManager.instance.PlaySFX(wrongAnswerClip);
                InputScreen.Show("<color=red>Wrong Answer</color>", true);
            }
        }
        timer.StopTimer();
        InputScreen.instance.Accept();
        NovelController.instance.Command_SetLayerImage("null", BCFC.instance.foreground);
        NovelController.instance.Command_SetLayerImage("null", BCFC.instance.cypherframe);
        InputScreen.Hide();
        if (timerStart > 0f)
        {
            NovelController.instance.playLastClipData();
            NovelController.instance.playLastAmbientMusic();
        }
        if(passedTimedGame)
            AudioManager.instance.PlaySFX(passedClip);
        else
            AudioManager.instance.PlaySFX(failClip);
        if (passChapter != "" && failChapter != "")
        {
            if(passedTimedGame)
                NovelController.instance.Command_Load(passChapter);
            else
                NovelController.instance.Command_Load(failChapter);
        }
        NovelController.instance.blockNext = false;
        NovelController.instance.Next();
        PauseScreen.savePreviousState = false;
    }

    bool showingKey = false;
    public void ShowKey(PuzzleButton button)
    {
        if (InputScreen.isRevealing) return;
        AudioClip key_look_sound = Resources.Load("Audio/SFX/effect_key_look") as AudioClip;
        AudioManager.instance.PlaySFX(key_look_sound);
        showingKey ^= true;
        if (showingKey)
        {
            NovelController.instance.Command_SetLayerImage(keyImage, BCFC.instance.cypherframe);
            inputHeader.SetActive(false);
            inputField.SetActive(false);
            enterButton.gameObject.SetActive(false);
        }
        else
        {
            NovelController.instance.Command_SetLayerImage("null", BCFC.instance.cypherframe);
            inputHeader.SetActive(true);
            inputField.SetActive(true);
            enterButton.gameObject.SetActive(true);
        }

    }

    public void EnterAnswer(PuzzleButton button)
    {
        if(!showingKey && !InputScreen.isRevealing)
            enterButtonHit = true;
    }

}
