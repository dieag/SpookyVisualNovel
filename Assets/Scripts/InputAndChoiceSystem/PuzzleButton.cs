using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PuzzleButton : MonoBehaviour
{
  	public TextMeshProUGUI tmpro;
  	public string text {get{return tmpro.text;} set {tmpro.text = value;}}

  	[HideInInspector]
  	public bool showPage = false;
}
