using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceScreenTesting : MonoBehaviour
{
	public string title = "I like ....";
	public string[] choices;

    // Update is called once per frame
    void Start()
    {

    	StartCoroutine(DynamicStoryExample());
    }

    IEnumerator DynamicStoryExample()
    {
    	NovelController.instance.LoadChapterFile("chapter_1_test"); yield return new WaitForEndOfFrame();
    	while(NovelController.instance.isHandlingChapterFile)
    		yield return new WaitForEndOfFrame();

    	ChoiceScreen.Show("Whats youre name?", "Fuck you", "Name");
    	while(ChoiceScreen.isWaitingForChoiceToBeMade)
    		yield return new WaitForEndOfFrame();
    	if(ChoiceScreen.lastChoiceMade.index == 0)
    		NovelController.instance.LoadChapterFile("chapter_1a_test");
    	else
    		NovelController.instance.LoadChapterFile("chapter_1b_test");

    	yield return new WaitForEndOfFrame();
    	NovelController.instance.Next();

    	while(NovelController.instance.isHandlingChapterFile)
    		yield return new WaitForEndOfFrame();
    }
}
