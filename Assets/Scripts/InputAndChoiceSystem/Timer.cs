using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Timer : MonoBehaviour
{
	public TextMeshProUGUI timerText;
	private float startTime;
	private bool startTimer = false;
	private float timeLeft {get{return startTime - Time.time;}}
	public bool isTimeOut{get{return timeLeft <= 0f;}}
	public bool isTimerOn{get{return startTimer == true;}}

    // Start is called before the first frame update

    public void StartTimer(float _startTime)
    {
    	if(_startTime <= 0f) return;
        startTime = Time.time + _startTime;
        startTimer = true;
    }

    public void StopTimer()
    {
    	startTimer = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (PauseScreen.instance.isPaused)
        {
            startTime += Time.deltaTime;
            return;
        }
    	if(!startTimer) return;
        float t = timeLeft;
        if(isTimeOut) return;

        float fseconds = (t % 60);
        int fminutes = ((int) t / 60);
        string minutes = fminutes.ToString();
        string seconds = fseconds.ToString("f2");

        if(fminutes == 0f && fseconds < 30f)
        	timerText.text = "<color=red>" + minutes + ":" + seconds + "</color>";
        else
        	timerText.text = minutes + ":" + seconds;

    }
}
