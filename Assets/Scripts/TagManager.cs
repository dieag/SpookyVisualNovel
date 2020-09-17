using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour 
{

	public static void Inject(ref string s)
	{
		if (!s.Contains("["))
			return;
		//s.Replace("[mainCharName]", gameFile.mainCharacterName);
		s = s.Replace("[mainCharName]", "Raelin");

		s = s.Replace("[curyHolyrelic]", "Diving Arc");
		//s.Replace("[curyHolyrelic]", gameFile.Relic);
	}

	public static string[] SplitByTags(string targetText)
	{
		return targetText.Split(new char[2]{'<','>'});
	}
}