using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class architectTextTesting : MonoBehaviour {

	public Text text;
	public TextMeshProUGUI tmprotext;
	TextArchitect architect;

	[TextArea(5,10)]
	public string say;

	// Use this for initialization
	void Start () {
		architect = new TextArchitect(tmprotext, say);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			architect = new TextArchitect(tmprotext, say);
		}
	}
}
