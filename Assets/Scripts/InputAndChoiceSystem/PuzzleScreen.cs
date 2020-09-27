using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PuzzleScreen : MonoBehaviour
{

	public static PuzzleScreen instance;
	public PuzzleButton puzzleButton;
	public Timer timer;
    public GameObject inputField;
    public GameObject inputHeader;
    private string keyImage;

    void Awake()
	{
		instance = this;
	}

    bool checkPuzzleAnswer(string puzzleAnswer, string puzzleName){
        switch(puzzleName) {
            case "puzzle0": return puzzleAnswer == "100201104070";
            default: return false;
        }
    }

    IEnumerator WaitForKeyDown(KeyCode keyCode)
    {
        while(!Input.GetKeyDown(keyCode)) {
            if(timer.isTimeOut && timer.isTimerOn) {
                yield break;
            }
            yield return null; 
        }
            
    }

	public bool isHandlingPuzzle {get{return handlingPuzzle != null;}}
    Coroutine handlingPuzzle = null;
    public void puzzleStart(string puzzleName, string _keyImage, string keyCode, string passChapter, string failChapter, float timerStart)
    {
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
        NovelController.instance.blockNext = true;
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
            Debug.Log(NovelController.instance.lastPlayedClipData);
            NovelController.instance.Command_PlayMusic(NovelController.instance.lastPlayedClipData);
            NovelController.instance.Command_PlayAmbientMusic(NovelController.instance.lastPlayedAmbientClipData);
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
    }

    bool showingKey = false;
    public void ShowKey(PuzzleButton button)
    {
    	showingKey ^= true;
        if (showingKey)
        {
            NovelController.instance.Command_SetLayerImage(keyImage, BCFC.instance.cypherframe);
            inputHeader.SetActive(false);
            inputField.SetActive(false);
        }
        else
        {
            NovelController.instance.Command_SetLayerImage("null", BCFC.instance.cypherframe);
            inputHeader.SetActive(true);
            inputField.SetActive(true);
        }

    }

}
