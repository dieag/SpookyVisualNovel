using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour {

	public static DialogueSystem instance;
	public ELEMENTS elements;
	public GameObject root;

	void Awake()
	{
		instance = this;
	}
	
	public void Say(string speech, string speaker = "", bool additive = false)
	{
		root.SetActive(true);
		StopSpeaking();
		if (additive)
			speechText.text = targetSpeech;
		speaking = StartCoroutine(Speaking(speech, additive, speaker));
	}


	public void StopSpeaking()
	{
		if (isSpeaking)
		{
			StopCoroutine(speaking);
		}
		if(textArchitect != null && textArchitect.isConstructing)
		{
			textArchitect.Stop();
		}
		speaking = null;
	}

	public bool isSpeaking {get{return speaking != null;}}
	public string targetSpeech = "";
	Coroutine speaking = null;
	public TextArchitect textArchitect = null;
	[HideInInspector] public bool isWaitingForUserInput = false;
	IEnumerator Speaking(string speech, bool additive, string speaker = "")
	{
		speechPanel.SetActive(true);
		string additiveSpeech = additive ? speechText.text : "";
		targetSpeech = additiveSpeech + speech;

		if(textArchitect == null)
			textArchitect = new TextArchitect(speechText, speech, additiveSpeech);
		else
			textArchitect.Renew(speech, additiveSpeech);

		speakerNameText.text = DetermineSpeaker(speaker);
		speakerNamePanel.SetActive(speakerNameText.text != "");
		isWaitingForUserInput = false;

		while(textArchitect.isConstructing)
		{
			if (Input.GetKey(KeyCode.Space))
				textArchitect.skip = true;
			yield return new WaitForEndOfFrame();
		}

		isWaitingForUserInput = true;
		while(isWaitingForUserInput)
		{
			yield return new WaitForEndOfFrame();
		}
		StopSpeaking();
	}

	string DetermineSpeaker(string s)
	{
		string retVal = speakerNameText.text;
		if( s != speakerNameText.text && s != "")
		{
			retVal = (s.ToLower().Contains("narrator")) ? "" : s;
		}
		return retVal;
	}

	public void Close()
	{
		StopSpeaking();
		speechPanel.SetActive(false);
	}
	// Update is called once per frame
	void Update () {
		
	}

	[System.Serializable]
	public class ELEMENTS
	{
		public GameObject speechPanel;
		public GameObject speakerNamePanel;
		public TextMeshProUGUI speakerNameText;
		public TextMeshProUGUI speechText;	
	}
	public GameObject speechPanel {get{return elements.speechPanel;}}
	public TextMeshProUGUI speakerNameText {get{return elements.speakerNameText;}}
	public TextMeshProUGUI speechText {get{return elements.speechText;}}	
	public GameObject speakerNamePanel {get{return elements.speakerNamePanel;}}
}
