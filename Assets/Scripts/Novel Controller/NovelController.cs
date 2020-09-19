using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovelController : MonoBehaviour
{

    public static NovelController instance; 

	List<string> data = new List<string>();
    [HideInInspector]
	public char delimiter = '|';

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadChapterFile("TestingLineSegmentation");
        //LoadChapterFile("chapter0_start");
    }

    // Update is called once per frame
    void Update()
    {
    	if(Input.GetKeyDown(KeyCode.RightArrow))  
    	{
            
            Next();
    	}
    }

    public void LoadChapterFile(string fileName)
    {
    	data = FileManager.LoadFile(FileManager.savPath + "Resources/Story/" + fileName);
    	cachedLastSpeaker = "";

        if(handlingChapterFile != null)
            StopCoroutine(handlingChapterFile);
        handlingChapterFile = StartCoroutine(HandlingChapterFile());
    }

    bool _next = false;
    public void Next()
    {
        _next = true;
    }

    public bool isHandlingChapterFile {get{return handlingChapterFile != null;}}
    Coroutine handlingChapterFile = null;
    IEnumerator HandlingChapterFile()
    {
        int progress = 0;
        while(progress < data.Count)
        {
            if(_next)
            {
                HandleLine(data[progress]);
                progress ++;
                while(isHandlingLine)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            yield return new WaitForEndOfFrame();
        }

        handlingChapterFile = null;
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
            case "inputScreen":
                //Command_InputScreen(data[1]);
                //Input text, function to check for correct input
                return;
            //case ("transCinematic"):

		}
    }

    void Command_SetLayerImage(string data, BCFC.LAYER layer)
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
    void Command_PlaySound(string data)
    {
    	AudioClip clip = Resources.Load("Audio/SFX/" + data) as AudioClip;
    	if (clip != null)
    		AudioManager.instance.PlaySFX(clip);
    	else
    		Debug.LogError("Clip " + data + " does not exist");
    }
    void Command_PlayMusic(string data)
    {
    	AudioClip clip = Resources.Load("Audio/Music/" + data) as AudioClip;
    	if (clip != null)
    		AudioManager.instance.PlaySong(clip);
    	else
    		Debug.LogError("Clip " + data + " does not exist");
    }

    void Command_MoveCharacter(string data)
    {
    	string[] parameters = data.Split(',');
    	string character = parameters[0];
    	float locationX = float.Parse(parameters[1]);
    	float locationY = float.Parse(parameters[2]);
    	float speed = parameters.Length >= 4 ? float.Parse(parameters[3]) : 1f;
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
    	Character c = CharacterManager.instance.getCharacter(character);
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

}
