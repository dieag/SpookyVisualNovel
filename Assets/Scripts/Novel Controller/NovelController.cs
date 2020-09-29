using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NovelController : MonoBehaviour
{

    public static NovelController instance;

	List<string> data = new List<string>();
    [HideInInspector]
    public char delimiter = '|';
    [HideInInspector]
    public bool blockNext = false;
    public static bool loadingGameSave = false;

    void Awake()
    {
        instance = this;
    }

    int activeGameFileNumber = 0;
    GAMEFILE activeGameFile = null;
    GAMEFILE previousGameFile = new GAMEFILE();
    string activeChapterFile = "";

    // Start is called before the first frame update
    void Start()
    {
        if (loadingGameSave)
            LoadGameFile(0);
        else
            LoadChapterFile("chapter0_start");
    }

    public void LoadGameFile(int gameFileNumber)
    {
        activeGameFileNumber = gameFileNumber;
        string filePath = FileManager.savPath + "Resources/gameFiles/" + gameFileNumber.ToString() + ".txt";

        if(!System.IO.File.Exists(filePath))
        {
            FileManager.SaveEncryptedJSON(filePath, new GAMEFILE(),keys);
        }

        activeGameFile = FileManager.LoadEncryptedJSON<GAMEFILE>(filePath,keys);
        //Load the File
        data = FileManager.LoadFile(FileManager.savPath + "Resources/Story/" + activeGameFile.chapterName);
        activeChapterFile = activeGameFile.chapterName;
        cachedLastSpeaker = activeGameFile.cachedLastSpeaker;

        DialogueSystem.instance.Open(activeGameFile.currentTextSystemSpeakerDisplayText, activeGameFile.currentTextSystemDisplayText);

        //Load all characters back into scene
        for(int i = 0; i < activeGameFile.charactersInScene.Count; i++)
        {
            GAMEFILE.CHARACTERDATA data = activeGameFile.charactersInScene[i];
            Character character = CharacterManager.instance.createNewCharacter(data.characterName, data.enabled);
            character.SetBody(data.bodyExpression);
            character.SetExpression(data.facialExpression);
            if (data.facingLeft)
                character.FaceLeft();
            else
                character.FaceRight();
            character.SetPosition(data.position);
        }

        //Load the layer images back into the scene
        if (activeGameFile.background != null)
            BCFC.instance.background.SetTexture(activeGameFile.background);
        if (activeGameFile.cinematic != null)
            BCFC.instance.cinematic.SetTexture(activeGameFile.cinematic);
        if (activeGameFile.foreground != null)
            BCFC.instance.foreground.SetTexture(activeGameFile.foreground);

        //start the music back up
          if (activeGameFile.music != null) {
            GAMEFILE.SONGDATA song = activeGameFile.music;
            AudioManager.instance.PlaySong(song.clip, song.maxVolume, song.pitch, song.startingVolume, song.playOnStart, song.loop);
        }
        //start the music back up
        if (activeGameFile.ambientMusic != null)
        {
            GAMEFILE.SONGDATA song = activeGameFile.ambientMusic;
            AudioManager.instance.PlayAmbientSong(song.clip, song.maxVolume, song.pitch, song.startingVolume, song.playOnStart, song.loop);
        }

        if (handlingChapterFile != null)
            StopCoroutine(handlingChapterFile);
        handlingChapterFile = StartCoroutine(HandlingChapterFile());
        chapterProgress = activeGameFile.chapterProgress;
    }


    void setGameFile(ref GAMEFILE gamefile)
    {
        gamefile.chapterName = activeChapterFile;
        gamefile.chapterProgress = chapterProgress;
        gamefile.cachedLastSpeaker = cachedLastSpeaker;
        gamefile.currentTextSystemDisplayText = DialogueSystem.instance.speechText.text;
        gamefile.currentTextSystemSpeakerDisplayText = DialogueSystem.instance.speakerNameText.text;

        gamefile.charactersInScene.Clear();
        for (int i = 0; i < CharacterManager.instance.characters.Count; i++)
        {
            Character character = CharacterManager.instance.characters[i];
            GAMEFILE.CHARACTERDATA data = new GAMEFILE.CHARACTERDATA(character);
            gamefile.charactersInScene.Add(data);
        }

        //save the layers to disk
        BCFC b = BCFC.instance;
        gamefile.background = b.background.activeImage != null ? b.background.activeImage.texture : null;
        gamefile.cinematic = b.cinematic.activeImage != null ? b.cinematic.activeImage.texture : null;
        gamefile.foreground = b.foreground.activeImage != null ? b.foreground.activeImage.texture : null;

        //save the music to disk
        if (AudioManager.activeSong != null)
        {
            GAMEFILE.SONGDATA songdata = new GAMEFILE.SONGDATA(AudioManager.activeSong);
            gamefile.music = songdata;
        }
        if (AudioManager.activeAmbientSong != null)
        {
            GAMEFILE.SONGDATA songdata = new GAMEFILE.SONGDATA(AudioManager.activeAmbientSong);
            gamefile.ambientMusic = songdata;
        }
    }
    public void SaveGameFile(bool usePreviousGameFile)
    {
        string filePath = FileManager.savPath + "Resources/gameFiles/" + activeGameFileNumber.ToString() + ".txt";

        if (!System.IO.File.Exists(filePath))
        {
            FileManager.SaveEncryptedJSON(filePath, new GAMEFILE(), keys);
        }
        activeGameFile = FileManager.LoadEncryptedJSON<GAMEFILE>(filePath, keys);

        if (usePreviousGameFile)
            activeGameFile = previousGameFile;
        else
            setGameFile(ref activeGameFile);
        
        FileManager.SaveEncryptedJSON(filePath, activeGameFile,keys);
    }

    byte[] keys = new byte[3] { 23, 70, 194 };

    // Update is called once per frame
    void Update()
    {
    	if(Input.GetKeyDown(KeyCode.RightArrow))  
    	{
            Next();
    	}
    }

    private bool loadedNewFile = false;
    public void LoadChapterFile(string fileName)
    {
        _next = false;
        activeChapterFile = fileName;
    	data = FileManager.LoadFile(FileManager.savPath + "Resources/Story/" + fileName);
    	cachedLastSpeaker = "";
        loadedNewFile = true;
        if(handlingChapterFile != null)
            StopCoroutine(handlingChapterFile);
        handlingChapterFile = StartCoroutine(HandlingChapterFile());

        Next();
    }

    bool _next = false;
    public void Next()
    {
        if (!blockNext)
        {
            setGameFile(ref previousGameFile);
            _next = true;
        }
    }

    public bool isHandlingChapterFile {get{return handlingChapterFile != null;}}
    Coroutine handlingChapterFile = null;

    [HideInInspector] public int chapterProgress = 0;

    IEnumerator HandlingChapterFile()
    {
        chapterProgress = 0;
        while(chapterProgress < data.Count)
        {
            if(_next)
            {
                string line = data[chapterProgress];
                if (line.StartsWith("choice"))
                {
                    yield return HandlingChoiceLine(line);
                    chapterProgress++;
                }
                else
                {
                    chapterProgress++;
                    HandleLine(line);
                    while(isHandlingLine)
                    {
                        yield return new WaitForEndOfFrame();
                    }   
                }

            }
            yield return new WaitForEndOfFrame();
        }

        handlingChapterFile = null;
    }

    IEnumerator HandlingChoiceLine(string line)
    {
        string title = line.Split('"')[1];
        List<string> choices = new List<string>();
        List<string> actions = new List<string>();

        bool gatheringChoices = true;
        while(gatheringChoices)
        {
            chapterProgress++;
            line = data[chapterProgress];

            if(line == "{")
                continue;

            line = line.Replace("    ","");

            if(line != "}")
            {
                choices.Add(line.Split('"')[1]);
                actions.Add(data[chapterProgress+1].Replace("    ",""));
                chapterProgress++;
            }
            else
            {
                gatheringChoices = false;
            }

        }

        if (choices.Count > 0)
        {
            ChoiceScreen.Show(title, choices.ToArray()); yield return new WaitForEndOfFrame();
            while(ChoiceScreen.isWaitingForChoiceToBeMade)
                yield return new WaitForEndOfFrame();
            string action = actions[ChoiceScreen.lastChoiceMade.index];
            HandleLine(action);

            while(isHandlingLine)
                yield return new WaitForEndOfFrame();
        }
        else
        {
            Debug.LogError("Invalid choice operation. No choices were found.");
        }
    }


    void HandleLine(string rawLine)
    {
        CLM.LINE line = CLM.Interpret(rawLine);
        StopHandlingLine();
        handlingLine = StartCoroutine(HandlingLine(line));

    }

    void StopHandlingLine()
    {
        if(isHandlingLine)
            StopCoroutine(handlingLine);
        handlingLine = null;
    }

    [HideInInspector]
    public string cachedLastSpeaker = "";


    public bool isHandlingLine{get{return handlingLine != null;}}
    Coroutine handlingLine = null;

    IEnumerator HandlingLine(CLM.LINE line)
    {
        _next = false;
        int lineProgress = 0;

        while(lineProgress < line.segments.Count)
        {
            _next = false;
            CLM.LINE.SEGMENT segment = line.segments[lineProgress];
            
            if(lineProgress > 0)
            {
                if(segment.trigger == CLM.LINE.SEGMENT.TRIGGER.autoDelay)
                {
                    for (float timer = segment.autoDelay; timer >= 0; timer -= Time.deltaTime)
                    {
                        yield return new WaitForEndOfFrame();
                        if(_next)
                            break;
                    }
                } else {
                    while(!_next)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
            _next = false;

            segment.Run();

            while(segment.isRunning)
            {
                yield return new WaitForEndOfFrame();

                if(_next)
                {
                    if (!segment.architect.skip)
                        segment.architect.skip = true;
                    else
                        segment.ForceFinish();
                    _next = false;
                }
            }

            lineProgress++;
            yield return new WaitForEndOfFrame();
        }

        for(int i = 0; i < line.actions.Count; i++)
        {
            HandleAction(line.actions[i]);
        }
        handlingLine = null;

    }

    public void HandleAction(string action)
    {
		string[] data = action.Split('(',')');

		switch(data[0]){
			case "setBackground":
				Command_SetLayerImage(data[1], BCFC.instance.background);
				return;
			case "setForeground":
				Command_SetLayerImage(data[1], BCFC.instance.foreground);
				return;
			case "setCinematic":
				Command_SetLayerImage(data[1], BCFC.instance.cinematic);
				return;
			case "playSound":
				Command_PlaySound(data[1]);
				return;
			case "playMusic":
				Command_PlayMusic(data[1]);
				return;
            case "playAmbientMusic":
                Command_PlayAmbientMusic(data[1]);
                return;
            case "moveCharacter":
				Command_MoveCharacter(data[1]);
				return;
			case "setPosition":
				Command_SetPosition(data[1]);
				return;
			case "changeExpression":
				Command_ChangeExpression(data[1]);
				return;
            case "flip":
                Command_Flip(data[1]);
                return;
            case "faceLeft":
                Command_FaceLeft(data[1]);
                return;
            case "faceRight":
                Command_FaceRight(data[1]);
                return;
            case "enter":
                Command_Enter(data[1]);
                return;
            case "exit":
                Command_Exit(data[1]);
                return;
            case "transBackground":
                Command_TransLayer(BCFC.instance.background, data[1]); 
                return;
            case "transForeground":
                Command_TransLayer(BCFC.instance.foreground, data[1]);
                return;
            case "showScene":
                Command_ShowScene(data[1]);
                return;
            case "Load":
                Command_Load(data[1]);
                return;
            case "endGame":
                Command_EndGame(data[1]);
                return;
            case "loadPuzzle":
                Command_LoadPuzzle(data[1]);
                return;
            case "stopAmbientMusic":
                Command_StopAmbientMusic();
                return;
            case "stopMusic":
                Command_StopMusic();
                return;
            case "checkFlagLoad":
                Command_CheckFlagLoad(data[1]);
                return;
            case "setFlag":
                Command_SetFlag(data[1]);
                return;
            case "showDialogueBox":
                Command_ShowDialogueBox(data[1]);
                return;
        }
    }

    public void Command_ShowDialogueBox(string data)
    {
        bool flag;
        if (bool.TryParse(data, out flag))
            DialogueSystem.instance.root.SetActive(flag);
    }

    public void Command_Load(string chapterName)
    {
        NovelController.instance.LoadChapterFile(chapterName);
    }
    public void Command_CheckFlagLoad(string data)
    {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        string chapterNamePass = parameters[1];
        string chapterNameFail = parameters[2];

        Character c = CharacterManager.instance.getCharacter(character, true, false);
        if (c.flag)
        {
            NovelController.instance.LoadChapterFile(chapterNamePass);
        } else
        {
            NovelController.instance.LoadChapterFile(chapterNameFail);
        }

    }
    public void Command_SetFlag(string data)
    {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        bool flag;
        Character c = CharacterManager.instance.getCharacter(character, true, false);
        if (bool.TryParse(parameters[1], out flag))
        { 
            c.flag = flag; 
        }

    }

    public void Command_SetLayerImage(string data, BCFC.LAYER layer)
    {
    	string texName = data.Contains(",") ? data.Split(',')[0] : data;
    	Texture2D tex = texName == null ? null : Resources.Load("images/UI/backdrops/" + texName) as Texture2D;
    	float speed = 2f;
    	bool smooth = false;

    	if(data.Contains(","))
    	{
    		string[] parameters = data.Split(',');
    		foreach(string p in parameters)
    		{
    			float fVal = 0;
    			bool bVal = false;
    			if (float.TryParse(p, out fVal)) {
    				speed = fVal;
    				continue;
    			}
    			if(bool.TryParse(p, out bVal)) 
    			{
    				smooth = bVal;
    				continue;
    			}
    		}
    	}
    	layer.TransitionToTexture(tex, speed, smooth);
    }
    public void Command_PlaySound(string data)
    {
    	AudioClip clip = Resources.Load("Audio/SFX/" + data) as AudioClip;
    	if (clip != null)
    		AudioManager.instance.PlaySFX(clip);
    	else
    		Debug.LogError("Clip " + data + " does not exist");
    }
    [HideInInspector]
    public string lastPlayedClipData = "";
    public void Command_PlayMusic(string data, bool cacheLastPlayedClip = true)
    {
        if(cacheLastPlayedClip)
            lastPlayedClipData = data;
        string[] parameters = data.Split(',');
        AudioClip clip = Resources.Load("Audio/Music/" + parameters[0]) as AudioClip;
        float maxVolume = parameters.Length >= 2 ? float.Parse(parameters[1]) : 1f;
        float startingVolume = parameters.Length >= 3 ? float.Parse(parameters[2]): 1f;
    	if (clip != null) {
    		AudioManager.instance.PlaySong(clip, maxVolume, 1f, startingVolume);
        }
    	else
    		Debug.LogError("Clip " + data + " does not exist");
    }

    public void Command_StopMusic()
    {
        AudioManager.instance.StopSong();
    }
    [HideInInspector]
    public string lastPlayedAmbientClipData = "";
    public void Command_PlayAmbientMusic(string data, bool cacheLastPlayedClip = true)
    {
        if (cacheLastPlayedClip)
            lastPlayedAmbientClipData = data;
        string[] parameters = data.Split(',');
        AudioClip clip = Resources.Load("Audio/Music/" + parameters[0]) as AudioClip;
        float startingVolume = parameters.Length >= 2 ? float.Parse(parameters[1]) : 1f;
        float maxVolume = parameters.Length == 3 ? float.Parse(parameters[2]) : 1f;
        if (clip != null)
        {
            AudioManager.instance.PlayAmbientSong(clip, maxVolume, 1f, startingVolume);
        }
        else
            Debug.LogError("Clip " + data + " does not exist");
    }

    public void Command_StopAmbientMusic()
    {
        AudioManager.instance.StopAmbientSong();
    }

    void Command_MoveCharacter(string data)
    {
    	string[] parameters = data.Split(',');
    	string character = parameters[0];
    	float locationX = float.Parse(parameters[1]);
    	float locationY = float.Parse(parameters[2]);
    	float speed = parameters.Length >= 4 ? float.Parse(parameters[3]) : 3f;
    	bool smooth = parameters.Length == 5 ? bool.Parse(parameters[4]) : true;


    	Character c = CharacterManager.instance.getCharacter(character);
    	c.MoveTo(new Vector2(locationX, locationY), speed, smooth);
    }

    void Command_SetPosition(string data)
    {
    	string[] parameters = data.Split(',');
    	string character = parameters[0];
    	float locationX = float.Parse(parameters[1]);
    	float locationY = float.Parse(parameters[2]);
    	Character c = CharacterManager.instance.getCharacter(character,true,false);
    	c.SetPosition(new Vector2(locationX, locationY));
    }

    void Command_ChangeExpression(string data)
    {
	    string[] parameters = data.Split(',');
    	string character = parameters[0];
    	string region = parameters[1];
    	string expression = parameters[2];
    	float speed = parameters.Length == 4 ? float.Parse(parameters[3]) : 0;
    	Character c = CharacterManager.instance.getCharacter(character);
    	Sprite sprite = c.GetSprite(expression);
    	if (region.ToLower() == "body") {
    		if(speed == 0)
    			c.SetBody(sprite);
    		else
    			c.TransitionBody(sprite,speed,false);
    	}
    	else if (region.ToLower() == "face") {
    		if(speed == 0)
    			c.SetExpression(sprite);
    		else
    			c.TransitionExpression(sprite,speed,false);
    	}
    }

    void Command_Flip(string data)
    {
        Character c = CharacterManager.instance.getCharacter(data);
        c.Flip();
    }

    void Command_FaceLeft(string data)
    {
        Character c = CharacterManager.instance.getCharacter(data);
        c.FaceLeft();
    }

    void Command_FaceRight(string data)
    {
        Character c = CharacterManager.instance.getCharacter(data);
        c.FaceRight();
    }

    void Command_Exit(string data)
    {
       string[] parameters = data.Split(',');
       string[] characters = parameters[0].Split(';');
       float speed = 3;
       bool smooth = false;
       for(int i = 1; i < parameters.Length; i++)
       {
            float fVal = 0; bool bVal = false;
            if(float.TryParse(parameters[i], out fVal))
            {speed = fVal; continue;}
            if(bool.TryParse(parameters[i], out bVal))
            {smooth = bVal; continue;}
       } 

       foreach(string s in characters)
       {
         Character c = CharacterManager.instance.getCharacter(s);
         c.FadeOut(speed,smooth);
       }
    }
    void Command_Enter(string data)
    {
       string[] parameters = data.Split(',');
       string[] characters = parameters[0].Split(';');
       float speed = 3;
       bool smooth = false;
       for(int i = 1; i < parameters.Length; i++)
       {
            float fVal = 0; bool bVal = false;
            if(float.TryParse(parameters[i], out fVal))
            {speed = fVal; continue;}
            if(bool.TryParse(parameters[i], out bVal))
            {smooth = bVal; continue;}
       } 

       foreach(string s in characters)
       {
         Character c = CharacterManager.instance.getCharacter(s, true, false);
         if (!c.enabled)
         {
            c.renderers.bodyRenderer.color = new Color(1,1,1,0);
            c.renderers.expressionRenderer.color = new Color(1,1,1,0);
            c.enabled = true;
            c.TransitionBody(c.renderers.bodyRenderer.sprite,speed,smooth);
            c.TransitionExpression(c.renderers.expressionRenderer.sprite,speed,smooth);
         }
         else
            c.FadeIn(speed,smooth);
       }
    }

    void Command_TransLayer(BCFC.LAYER layer, string data)
    {
        string[] parameters = data.Split(',');

        string texName = parameters[0];
        string transTexName = parameters[1];
        Texture2D tex = texName == "null" ? null : Resources.Load("images/UI/backdrops/" + texName) as Texture2D;
        Texture2D transTex = Resources.Load("images/TransitionEffects/" + transTexName) as Texture2D;

        float spd = 2f;
        bool smooth = false;

        for(int i = 2; i < parameters.Length; i++)
        {
            string p = parameters[i];
            float fVal = 0;
            bool bVal = false;
            if(float.TryParse(p, out fVal))
                {spd = fVal; continue;}
            if(bool.TryParse(p, out bVal))
                {smooth = bVal; continue;}
        }

        TransitionMaster.TransitionLayer(layer, tex, transTex, spd, smooth);
    }

    void Command_ShowScene(string data)
    {
        string[] parameters = data.Split(',');

        bool show = bool.Parse(parameters[0]);
        string texName = parameters[1];
        Texture2D transTex = Resources.Load("images/TransitionEffects/" + texName) as Texture2D;
        float spd = 2f;
        bool smooth = false;

        for(int i = 2; i < parameters.Length; i++)
        {
            string p = parameters[i];
            float fVal = 0;
            bool bVal = false;
            if(float.TryParse(p, out fVal))
                {spd = fVal; continue;}
            if(bool.TryParse(p, out bVal))
                {smooth = bVal; continue;}
        }

        TransitionMaster.ShowScene(show, spd, smooth, transTex);
    }

    void Command_EndGame(string endGameImage)
    {
        EndGameScreen.instance.setEndGame(endGameImage);
    }

    void Command_LoadPuzzle(string data)
    {
        string[] parameters = data.Split(',');
        string puzzleName = parameters[0];
        string keyImage = parameters[1];
        string keycode = parameters[2];
        string passChapter = parameters.Length > 3 ? parameters[3] : "";
        string failChapter = parameters.Length > 3 ? parameters[4] : "";
        float time = 0f;
        float fVal = 0;
        if (parameters.Length > 3)
        {
            if (float.TryParse(parameters[5], out fVal)) { time = fVal; }
        }
        PuzzleScreen.instance.puzzleStart(puzzleName,keyImage, keycode, passChapter,failChapter,time);
    }
}
