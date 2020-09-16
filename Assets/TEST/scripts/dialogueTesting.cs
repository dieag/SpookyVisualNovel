using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dialogueTesting : MonoBehaviour {

	DialogueSystem dialogue;

	// Use this for initialization
	void Start () {
		dialogue = DialogueSystem.instance;
	}
	
	public string[] s = new string[]
	{
		"There's an old saying in Tennessee:Bush",
		" I know it's in Texas, probably in Tennessee that says...",
		" Fool me once, shame on ...",
		" Shame on you. Fool me...",
		" You can't get fooled again!"
	};
	// Update is called once per frame
	int index = 0;
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (!dialogue.isSpeaking || dialogue.isWaitingForUserInput)
			{
				if (index >= s.Length)
				{
					return;
				}
				SayString(s[index]);
				index++;
			}
		}
	}

	void SayString(string s)
	{
		//Debug.Log("string: " + s);
		string[] parts = s.Split(':');
		string speech = parts[0];
		string speaker = (parts.Length >= 2) ? parts[1] : "";
		dialogue.Say(speech, speaker,true);
	}
}
