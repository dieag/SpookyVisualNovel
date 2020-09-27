using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Character {

	public string char_name;
	[HideInInspector]public RectTransform root;
	[HideInInspector]public bool flag = false;

	public bool enabled {get {return root.gameObject.activeInHierarchy;} set{root.gameObject.SetActive(value); visibleInScene = value; }}

	public Vector2 anchorPadding {get{return root.anchorMax - root.anchorMin;}}

//Begin Transitioning
	public Sprite GetSprite(int index = 0)
	{
		Sprite[] sprites = Resources.LoadAll<Sprite>("images/Characters/"+ char_name);

		return sprites[index];
	}

	public Sprite GetSprite(string spriteName = "")
	{
		Sprite[] sprites = Resources.LoadAll<Sprite>("images/Characters/"+ char_name);
		for(int i = 0; i < sprites.Length; i++)
		{
			if(sprites[i].name == spriteName)
				return sprites[i];
		}
		return sprites.Length > 0 ? sprites[0] : null;
	}


	public void SetBody(int index)
	{
		renderers.bodyRenderer.sprite = GetSprite(index);
		lastBodySprite = renderers.bodyRenderer.sprite;

	}
	public void SetBody(Sprite sprite)
	{
		renderers.bodyRenderer.sprite = sprite;
		lastBodySprite = renderers.bodyRenderer.sprite;

	}
	public void SetBody(string spriteName)
	{
		if (spriteName == "AlphaOnly")
		{
			SetBody(Resources.Load<Sprite>("images/AlphaOnly"));
		}
		else
		{
			renderers.bodyRenderer.sprite = GetSprite(spriteName);
			lastBodySprite = renderers.bodyRenderer.sprite;
		}

	}

	public void SetExpression(int index)
	{
		renderers.expressionRenderer.sprite = GetSprite(index);
		lastFacialSprite = renderers.expressionRenderer.sprite;

	}
	public void SetExpression(Sprite sprite)
	{
   		renderers.expressionRenderer.sprite = sprite;
		lastFacialSprite = renderers.expressionRenderer.sprite;

	}
	public void SetExpression(string spriteName)
	{
		if (spriteName == "AlphaOnly")
		{
			SetExpression(Resources.Load<Sprite>("images/AlphaOnly"));
		}
		else
		{
			renderers.expressionRenderer.sprite = GetSprite(spriteName);
			lastFacialSprite = renderers.expressionRenderer.sprite;
		}
	}

	//Body Transitioning
	bool isTransitioningBody {get{return transitioningBody != null;}}
	Coroutine transitioningBody = null;

	public void TransitionBody(Sprite sprite, float speed, bool smooth = false)
	{
		StopTransitioningBody();
		transitioningBody = CharacterManager.instance.StartCoroutine(TransitioningBody(sprite,speed,smooth));
	}

	void StopTransitioningBody()
	{
		if (isTransitioningBody)
			CharacterManager.instance.StopCoroutine(transitioningBody);
		transitioningBody = null;
	}

	public IEnumerator TransitioningBody(Sprite sprite, float speed, bool smooth)
	{
		for(int i = 0; i < renderers.allBodyRenderers.Count; i++)
		{
			Image image = renderers.allBodyRenderers [i];
			if (image.sprite == sprite)
			{
				renderers.bodyRenderer = image;
				break;
			}
		}

		if (renderers.bodyRenderer.sprite != sprite)
		{
			Image image = GameObject.Instantiate(renderers.bodyRenderer.gameObject, renderers.bodyRenderer.transform.parent).GetComponent<Image>();
			renderers.allBodyRenderers.Add(image);
			renderers.bodyRenderer = image;
			image.color = GlobalFunctions.SetAlpha(image.color,0f);
			image.sprite = sprite;
		}

		while(GlobalFunctions.TransitionImages(ref renderers.bodyRenderer, ref renderers.allBodyRenderers, speed, smooth, true))
			yield return new WaitForEndOfFrame();
		StopTransitioningBody();
	}

	//Expression Transitioning
	bool isTransitioningExpression {get{return transitioningExpression != null;}}
	Coroutine transitioningExpression = null;

	public void TransitionExpression(Sprite sprite, float speed, bool smooth = false)
	{
		StopTransitioningExpression();
		transitioningExpression = CharacterManager.instance.StartCoroutine(TransitioningExpression(sprite,speed,smooth));
	}

	void StopTransitioningExpression()
	{
		if (isTransitioningExpression)
    			CharacterManager.instance.StopCoroutine(transitioningExpression);
		transitioningExpression = null;
	}

	public IEnumerator TransitioningExpression(Sprite sprite, float speed, bool smooth)
	{
		for(int i = 0; i < renderers.allExpressionRenderers.Count; i++)
		{
			Image image = renderers.allExpressionRenderers [i];
			if (image.sprite == sprite)
			{
				renderers.expressionRenderer = image;
				break;
			}
		}

		if (renderers.expressionRenderer.sprite != sprite)
		{
			Image image = GameObject.Instantiate(renderers.expressionRenderer.gameObject, renderers.expressionRenderer.transform.parent).GetComponent<Image>();
			renderers.allExpressionRenderers.Add(image);
			renderers.expressionRenderer = image;
			image.color = GlobalFunctions.SetAlpha(image.color,0f);
			image.sprite = sprite;
		}

		while(GlobalFunctions.TransitionImages(ref renderers.expressionRenderer, ref renderers.allExpressionRenderers, speed, smooth,true))
			yield return new WaitForEndOfFrame();
		StopTransitioningExpression();
	}
//End Transitioning

	public void Flip()
	{
		root.localScale = new Vector3(root.localScale.x * -1, 1, 1);
	}

	public bool isFacingLeft { get { return root.localScale.x == 1; } }
	public void FaceLeft()
	{
		root.localScale = Vector3.one;
	}

	public bool isFacingRight { get { return root.localScale.y == -1; } }
	public void FaceRight()
	{
		root.localScale = new Vector3(-1, 1, 1);
	}

	public bool isVisibleInScene {
        get { return visibleInScene; }
	}
	bool visibleInScene = true;

	public void FadeOut(float speed = 3, bool smooth = false)
	{
		Sprite alphaSprite = Resources.Load<Sprite>("images/AlphaOnly");
		lastBodySprite = renderers.bodyRenderer.sprite;
		lastFacialSprite = renderers.expressionRenderer.sprite;

		TransitionBody(alphaSprite, speed, smooth);
		TransitionExpression(alphaSprite, speed, smooth);
		visibleInScene = false;
	}

	public void SetFlag(bool value)
    {
		flag = value;
    }

	public bool GetFlag(bool value)
	{
		return flag;
	}

	Sprite lastBodySprite, lastFacialSprite = null;
	public void FadeIn(float speed = 3, bool smooth = false)
	{
		if(lastBodySprite != null && lastFacialSprite != null)
		{
			TransitionBody(lastBodySprite, speed, smooth);
			TransitionExpression(lastFacialSprite, speed, smooth);
			visibleInScene = true;
		}
	}

	public Character (string _name, bool enabledOnStart = true)
	{
		//Debug.Log("Name: " + _name);
		CharacterManager cm = CharacterManager.instance;
		GameObject prefab = Resources.Load("Characters/Character["+_name+"]") as GameObject;
		GameObject ob = GameObject.Instantiate(prefab, cm.characterPanel.GetComponent<RectTransform>(), false) as GameObject;
		
		root = ob.GetComponent<RectTransform> ();
		char_name = _name;

		//ob.transform.SetParent(cm.transform,false);
		renderers.bodyRenderer = ob.transform.Find("BodyLayer").GetComponentInChildren<Image> ();
		renderers.expressionRenderer = ob.transform.Find("ExpressionLayer").GetComponentInChildren<Image> ();
		renderers.allBodyRenderers.Add(renderers.bodyRenderer);
		renderers.allExpressionRenderers.Add(renderers.expressionRenderer);

		dialogue = DialogueSystem.instance;

		enabled = enabledOnStart;
		visibleInScene = enabled;
	}

	[System.Serializable]
	public class Renderers
	{
		//<summary>
		// The body renderer for a multi layer character
		// </summary>
		public Image bodyRenderer;
		//<summary>
		// The expressions renderer for a multi layer character
		// </summary>
		public Image expressionRenderer;

		public List<Image> allBodyRenderers = new List<Image>();
		public List<Image> allExpressionRenderers = new List<Image>();
	}

	public Renderers renderers = new Renderers();
	
	DialogueSystem dialogue;
	public void Say(string speech, bool speech_addon = false)
	{
			if (!enabled)
				enabled = true;
			
			dialogue.Say(speech, char_name, speech_addon);
	}

	public Vector2 _targetPosition {
        get { return targetPosition; }
	}


	Vector2 targetPosition;
	Coroutine moving;
	bool isMoving{get{return moving != null;}}
	public void MoveTo(Vector2 target, float speed, bool smooth = true)
	{
		StopMoving();
		moving = CharacterManager.instance.StartCoroutine(Moving(target, speed, smooth));
	}

	public void StopMoving(bool arriveAtTargetPositionImmediately = false)
	{
		if(isMoving)
		{
			CharacterManager.instance.StopCoroutine(moving);
			if(arriveAtTargetPositionImmediately)
				SetPosition(targetPosition);

			moving = null;
		}
	}

	public void SetPosition(Vector2 target)
	{
		targetPosition = target;

		Vector2 padding = anchorPadding;
		float maxX = 1f - padding.x;
		float maxY = 1f - padding.y;

		Vector2 minAnchorTarget = new Vector2(maxX * targetPosition.x, maxY * targetPosition.y);
		root.anchorMin = minAnchorTarget;
		root.anchorMax = root.anchorMin + padding;
	}

	IEnumerator Moving(Vector2 target, float speed, bool smooth)
	{
		targetPosition = target;

		Vector2 padding = anchorPadding;
		float maxX = 1f - padding.x;
		float maxY = 1f - padding.y;

		Vector2 minAnchorTarget = new Vector2(maxX * targetPosition.x, maxY * targetPosition.y);
		speed *= Time.deltaTime;
		while(root.anchorMin != root.anchorMax)
		{
			root.anchorMin = (!smooth) ? Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed) : Vector2.Lerp(root.anchorMin, minAnchorTarget,speed);
			root.anchorMax = root.anchorMin + padding;
			yield return new WaitForEndOfFrame ();
		}
		StopMoving();

	}
}
