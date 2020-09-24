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
	public void puzzleStart(string puzzleName, string keyCode, string passChapter, string failChapter, float timerStart)
	{
		NovelController.instance.Command_SetLayerImage(puzzleName+",1f,true",BCFC.instance.foreground);
        NovelController.instance.Command_PlayMusic("music_puzzle,0.5,0.5",false);
        InputScreen.Show("Enter the Key");
        handlingPuzzle = StartCoroutine(HandlePuzzle(puzzleName,keyCode,passChapter,failChapter,timerStart));
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
        if(!timer.isTimerOn)
            timer.gameObject.SetActive(false);
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
        NovelController.instance.Command_PlayMusic(NovelController.instance.lastPlayedClipData);
        NovelController.instance.Command_PlayAmbientMusic(NovelController.instance.lastPlayedAmbientClipData);
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
        NovelController.instance.Next();
    }

    bool showingKey = false;
    public void ShowKey(PuzzleButton button)
    {
    	showingKey ^= true;
    	if(showingKey)
    		NovelController.instance.Command_SetLayerImage("cypher_key", BCFC.instance.cypherframe);
    	else
    		NovelController.instance.Command_SetLayerImage("null", BCFC.instance.cypherframe);

    }

}
