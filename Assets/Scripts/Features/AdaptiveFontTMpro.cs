using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AdaptiveFontTMpro : MonoBehaviour {

	TextMeshProUGUI txt;
	public bool continualUpdate = true;

	public int fontSizeAtDefaultResolution = 40;
	public static float defaultResolution = 2225f;
	// Use this for initialization
	void Start () {
		txt = GetComponent<TextMeshProUGUI> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(continualUpdate)
		{
			InvokeRepeating("Adjust", 0f, Random.Range(0.3f,1f));
		}
		else
		{
			Adjust();
			enabled = false;
		}
	}

	void Adjust()
	{
		if(!enabled || !gameObject.activeInHierarchy)
			return;
		float totalCurrentRes = Screen.height + Screen.width;
		float perc = totalCurrentRes / defaultResolution;
		int fontsize = Mathf.RoundToInt((float)fontSizeAtDefaultResolution * perc);

		txt.fontSize = fontsize;
	}
}
