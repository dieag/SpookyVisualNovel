using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GAMEFILE
{
    public string chapterName;
    public int chapterProgress = 0;

    public string cachedLastSpeaker = "";
    public string currentTextSystemSpeakerDisplayText = "";
    public string currentTextSystemDisplayText = "";

    public List<CHARACTERDATA> charactersInScene = new List<CHARACTERDATA>();

    public Texture background = null;
    public Texture cinematic = null;
    public Texture foreground = null;

    public SONGDATA music = null;
    public SONGDATA ambientMusic = null;

    public GAMEFILE()
    {
        this.chapterName = "chapter0_start";
        this.chapterProgress = 0;
        this.cachedLastSpeaker = "";

        charactersInScene = new List<CHARACTERDATA>();
    }

    [System.Serializable]
    public class CHARACTERDATA
    {
        public string characterName;
        public bool enabled = true;
        public string facialExpression;
        public string bodyExpression;
        public bool facingLeft = true;
        public Vector2 position = Vector2.zero;
        public bool flag = false;

        public CHARACTERDATA(Character c)
        {
            this.characterName = c.char_name;
            this.enabled = c.isVisibleInScene;
            this.facialExpression = c.renderers.expressionRenderer.sprite.name;
            this.bodyExpression = c.renderers.bodyRenderer.sprite.name;
            this.facingLeft = c.isFacingLeft;
            this.position = c._targetPosition;
        }

    }

    [System.Serializable]
    public class SONGDATA
    {
        public AudioClip clip;
        public float maxVolume;
        public float pitch;
        public float startingVolume;
        public bool playOnStart = true;
        public bool loop = true;
        public string lastPlayedAmbientMusic;
        public string lastPlayedMusic;

        public SONGDATA(AudioManager.SONG s)
        {
            this.clip = s.clip;
            this.maxVolume = s.maxVolume;
            this.pitch = s.source.pitch;
            this.startingVolume = s.source.volume;
            this.playOnStart = s.source.isPlaying;
            this.loop = s.source.loop;
            this.lastPlayedAmbientMusic = NovelController.instance.lastPlayedAmbientClipData;
            this.lastPlayedMusic = NovelController.instance.lastPlayedClipData;
        }

    }
}
