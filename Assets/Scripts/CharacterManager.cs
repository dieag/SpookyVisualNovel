using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterManager : MonoBehaviour {

	public static CharacterManager instance;


	/*All characterse must be attached to this character panel*/
	public RectTransform characterPanel;

	/*A list of all the characters in the scene*/
	public List<Character> characters = new List<Character>();

	/// <summary>
	/// Easy lookup of characters
	/// </summary>
	public Dictionary<string, int> characterDictionary = new Dictionary<string, int>();

	void Awake()
	{
		instance = this;
	}
	
	public Character getCharacter(string characterName, bool createCharacter = true, bool enableCharacterOnStart = true)
	{
		int index = -1;
		if (characterDictionary.TryGetValue(characterName, out index))
		{
			return characters [index];
		}
		else if (createCharacter)
		{
			return createNewCharacter (characterName, enableCharacterOnStart);
		}
		return null;
	}
	//<summary>
	//Creats a new character
	//</summary>
	public Character createNewCharacter(string characterName, bool enableOnStart = true)
	{
		Character newCharacter = new Character (characterName, enableOnStart);

		characterDictionary.Add(characterName, characters.Count);
		characters.Add(newCharacter);
		return newCharacter;
	}

	public class CHARACTERPOSITIONS
	{
		public Vector2 bottemLeft = new Vector2(0, 0);
		public Vector2 topRight = new Vector2(1f, 1f);
		public Vector2 center = new Vector2(0.5f, 0.5f);
		public Vector2 bottomRight = new Vector2(1f, 0);
		public Vector2 topLeft = new Vector2(0, 1f);
	}
	public static CHARACTERPOSITIONS characterPositions = new CHARACTERPOSITIONS();

	public class CHARACTEREXPRESSIONS
	{
		public int normal = 0;
		public int shy = 1;
	}
	public static CHARACTEREXPRESSIONS characterExpressions = new CHARACTEREXPRESSIONS();

}
