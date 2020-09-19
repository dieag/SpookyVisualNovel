using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScreenTesting : MonoBehaviour
{
	public string displayTitle = "";

    // Update is called once per frame
    void Update()
    {
    	if(Input.GetKeyDown(KeyCode.Alpha1))
    	{
    		InputScreen.Show(displayTitle);
    	}
    	if(Input.GetKeyDown(KeyCode.Return) && InputScreen.isWaitingForUserInput)
    	{
    		InputScreen.instance.Accept();
    		print("Your entered the value of " + InputScreen.currentInput);
    	}
    }
}
