using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterTesting : MonoBehaviour {

	public Character Raelin;

	// Use this for initialization
	void Start () {
		Raelin = CharacterManager.instance.getCharacter("Raelin",enableCharacterOnStart:false);
		Raelin.GetSprite(2);
	}
		
	public float amt = 5f;
	public Vector2 moveTarget;
	public float moveSpeed;
	public bool smooth;
	public float speed = 5f;
	public bool smoothTransitions = false;

	public int bodyIndex, expressionIndex = 0;
	int i = 0;
	public string[] s = new string[]
	{
		"There's an old <color=red>saying in Tennessee</color>",
		" I know it's in Texas, probably in Tennessee that says...",
		" Fool me once, shame on ...",
		" Shame on you. Fool me...",
		" You can't get fooled again!"
	};
	// Update is called once per frame
	void Update () {
				if (Input.GetKeyDown(KeyCode.Space))
				{
					if (i < s.Length)
						Raelin.Say(s[i]);
					else 
						DialogueSystem.instance.Close();
					i++;
				}
				if(Input.GetKey(KeyCode.M))
				{
					Raelin.MoveTo(moveTarget, moveSpeed, smooth);
				}
				if(Input.GetKey(KeyCode.S))
				{
					Raelin.StopMoving(true);
				}
				if(Input.GetKey(KeyCode.B))
				{
					if(Input.GetKey(KeyCode.T))
						Raelin.TransitionBody(Raelin.GetSprite(bodyIndex), speed);
					else
						Raelin.SetBody(bodyIndex);
				}
				if(Input.GetKey(KeyCode.E))
				{
					if(Input.GetKey(KeyCode.T))
						Raelin.TransitionExpression(Raelin.GetSprite(expressionIndex), speed);
					else
						Raelin.SetExpression(expressionIndex);
				}
	}
}
