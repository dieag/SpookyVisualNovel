using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLM : MonoBehaviour
{
	public static LINE Interpret(string rawLine)
	{
		return new LINE(rawLine);
	}

	public class LINE{

		public string speaker = "";

		public List<SEGMENT> segments = new List<SEGMENT>();
		public List<string> actions = new List<string>();

		public LINE(string rawLine) 
		{
			string[] dialogueAndActions = rawLine.Split('"');
			char actionSplitter = NovelController.instance.delimiter;
			string[] actionArr = dialogueAndActions.Length == 3 ? dialogueAndActions[2].Split(actionSplitter) : dialogueAndActions[0].Split(actionSplitter);

			if (dialogueAndActions.Length == 3)
			{
				speaker = dialogueAndActions[0] == "" ? NovelController.instance.cachedLastSpeaker : dialogueAndActions[0];
				if(speaker[speaker.Length-1] == NovelController.instance.delimiter)
				{
					speaker = speaker.Remove(speaker.Length - 1);
				}
				NovelController.instance.cachedLastSpeaker = speaker;

				SegmentDialogue(dialogueAndActions[1]);
			}
			for(int i = 0; i < actionArr.Length; i++)
			{
				actions.Add(actionArr[i]);
			}
		}

		void SegmentDialogue(string dialogue)
		{
			segments.Clear();
			string[] parts = dialogue.Split('{','}');

			for (int i = 0; i < parts.Length; i++)
			{
				SEGMENT segment = new SEGMENT();
				bool isOdd = (i % 2 != 0);

				if(isOdd)
				{
					string[] commandData = parts[i].Split(' ');
					switch(commandData[0])
					{
						case "c": //wait for input and clear
							segment.trigger = SEGMENT.TRIGGER.waitClick;
							break;
						case "a": //wati for input and append
							segment.trigger = SEGMENT.TRIGGER.waitClick;
							segment.pretext = segments.Count > 0 ? segments[segments.Count - 1].dialogue : "";
							break;
						case "w": //wait for set time and clear
							segment.trigger = SEGMENT.TRIGGER.autoDelay;
							segment.autoDelay = float.Parse(commandData[1]);
							break;
						case "wa": //wait for set time and append
							segment.trigger = SEGMENT.TRIGGER.autoDelay;
							segment.autoDelay = float.Parse(commandData[1]);
							segment.pretext = segments.Count > 0 ? segments[segments.Count - 1].dialogue : "";
							break;
					}
					i++;
				}

				segment.dialogue = parts[i];
				segment.line = this;

				segments.Add(segment);
			}
		}

		public class SEGMENT
		{
			public LINE line;
			public string dialogue = "";
			public string pretext = "";
			public enum TRIGGER{waitClick,autoDelay};
			public TRIGGER trigger = TRIGGER.waitClick;

			public float autoDelay = 0;

			public void Run()
			{
				if(running != null)
					NovelController.instance.StopCoroutine(running);
				running = NovelController.instance.StartCoroutine(Running());
			}

			public bool isRunning {get{return running != null;}}
			Coroutine running = null;
			public TextArchitect architect = null;
			IEnumerator Running()
			{
				print(line.speaker);
				if(line.speaker != "narrator")
				{
					Character character = CharacterManager.instance.getCharacter(line.speaker);
					character.Say(dialogue, pretext != "");
				}
				else
				{
					DialogueSystem.instance.Say(dialogue, line.speaker, pretext != "");
				}

				architect = DialogueSystem.instance.textArchitect;
				while(architect.isConstructing)
					yield return new WaitForEndOfFrame();
				running = null;
			}

			public void ForceFinish()
			{
				if(running != null)
					NovelController.instance.StopCoroutine(running);
				running = null;
				if(architect != null)
					architect.ForceFinish();
			}
		}

	}

}
