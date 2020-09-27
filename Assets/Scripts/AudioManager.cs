using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour 
{
	public static AudioManager instance;
	public static SONG activeSong = null;
	public static SONG activeAmbientSong = null;
	public static List<SONG> allSongs = new List<SONG>();
	public static List<SONG> allAmbientSongs = new List<SONG>();

    public float songTransitionSpeed = 3f;
	public bool songSmoothTransitions = true;	

	void Awake()
	{
			instance = this;
			DontDestroyOnLoad(gameObject);
	
	}

	public void PlaySFX(AudioClip effect, float volume = 1f, float pitch = 1f)
	{
		AudioSource source = CreateNewSource(string.Format("SFX [{0}]", effect.name));
		source.clip = effect;
		source.volume = volume;
		source.pitch = pitch;
		source.Play();

		Destroy(source.gameObject, effect.length);
	}

	public void PlaySong(AudioClip song, float maxVolume =1f, float pitch = 1f, float startingVolume = 0f, bool playOnStart = true, bool loop = true)
	{
		if(song != null)
		{
			for(int i = 0; i < allSongs.Count; i++)
			{
				SONG s = allSongs[i];
				if (s.clip == song)
				{
					activeSong = s;
					break;
				}

			}
			if (activeSong == null || activeSong.clip != song)
				activeSong = new SONG(song, maxVolume, pitch, startingVolume, ref allSongs, playOnStart, loop);
		} else
			activeSong = null;

		StopCoroutine(VolumeLeveling());
		StartCoroutine(VolumeLeveling());
	}
	public void StopSong()
	{

		activeSong = null;
		StopCoroutine(VolumeLeveling());
		StartCoroutine(VolumeLeveling());
	}

	public void PlayAmbientSong(AudioClip song, float maxVolume = 1f, float pitch = 1f, float startingVolume = 0f, bool playOnStart = true, bool loop = true)
	{

		if (song != null)
		{
			for (int i = 0; i < allAmbientSongs.Count; i++)
			{
				SONG s = allAmbientSongs[i];
				if (s.clip == song)
				{
					activeAmbientSong = s;
					break;
				}

			}
			if (activeAmbientSong == null || activeAmbientSong.clip != song)
				activeAmbientSong = new SONG(song, maxVolume, pitch, startingVolume, ref allAmbientSongs, playOnStart, loop);
		}
		else
			activeAmbientSong = null;

		StopCoroutine(AmbientVolumeLeveling());
		StartCoroutine(AmbientVolumeLeveling());
	}
	public void StopAmbientSong()
	{
		activeAmbientSong = null;
		StopCoroutine(AmbientVolumeLeveling());
		StartCoroutine(AmbientVolumeLeveling());
	}

	IEnumerator VolumeLeveling()
	{
		while(TransitionSongs())
			yield return new WaitForEndOfFrame();
	}

	IEnumerator AmbientVolumeLeveling()
	{
		while (TransitionAmbientSongs())
			yield return new WaitForEndOfFrame();
	}

	bool TransitionSongs()
	{
		bool anyValueChanged = false;

		float speed = songTransitionSpeed * Time.deltaTime;
        for (int i = allSongs.Count - 1; i >= 0; i--)
		{
			SONG song = allSongs[i];

			if (song == activeSong) {
				if (!song.isPlaying())
					song.Play();
				if (song.volume < song.maxVolume)
				{
					song.volume = songSmoothTransitions ? Mathf.MoveTowards(song.volume, song.maxVolume, speed) : song.maxVolume;
					anyValueChanged = true;	
				}
			} else {
				if (song.volume > 0f)
				{
					song.volume = songSmoothTransitions ? Mathf.MoveTowards(song.volume, 0f, speed) : 0f;
					anyValueChanged = true;
				}
				else
				{
					allSongs.RemoveAt(i);
					song.Destroy(ref allSongs);
					continue;
				}
			}
		}

		return anyValueChanged;
	}

	bool TransitionAmbientSongs()
	{
		bool anyValueChanged = false;

		float speed = songTransitionSpeed * Time.deltaTime;
		for (int i = allAmbientSongs.Count - 1; i >= 0; i--)
		{
			SONG song = allAmbientSongs[i];

			if (song == activeAmbientSong)
			{
				if (!song.isPlaying())
					song.Play();
				if (song.volume < song.maxVolume)
				{
					song.volume = songSmoothTransitions ? Mathf.MoveTowards(song.volume, song.maxVolume, speed) : song.maxVolume;
					anyValueChanged = true;
				}
			}
			else
			{
				if (song.volume > 0f)
				{
					song.volume = songSmoothTransitions ? Mathf.MoveTowards(song.volume, 0f, speed) : 0f;
					anyValueChanged = true;
				}
				allAmbientSongs.RemoveAt(i);
				song.Destroy(ref allAmbientSongs);
				continue;
			}
		}

		return anyValueChanged;
	}

	public static AudioSource CreateNewSource(string _name)
	{
		AudioSource newSource = new GameObject(_name).AddComponent<AudioSource>();
		newSource.transform.SetParent(instance.transform);
		return newSource;
	}

	[System.Serializable]
	public class SONG
	{
		public AudioSource source;
		public AudioClip clip {get{return source.clip;} set{source.clip = value;}}
		public float maxVolume = 1f;

		public SONG(AudioClip clip, float _maxVolume, float pitch, float startingVolume, ref List<SONG> allSongsList, bool playOnStart = true, bool loop = true)
		{
			source = AudioManager.CreateNewSource(string.Format("SONG [{0}]", clip.name));
			source.clip = clip;
			source.volume = startingVolume;
			maxVolume = _maxVolume;
			source.pitch = pitch;
			source.loop = loop;

			allSongsList.Add(this);
			if(playOnStart)
				source.Play();
		}

		public float volume { get{return source.volume;} set{source.volume = value;}}
		public float pitch { get{return source.pitch;} set{source.pitch = value;}}

		public void Play()
		{
			source.Play();
		}

		public bool isPlaying()
		{
			return source.isPlaying;
		}
		public void Stop()
		{
			source.Stop();
		}

		public void Pause()
		{
			source.Pause();
		}

		public void UnPause()
		{
			source.UnPause();
		}

		public void Destroy(ref List<SONG> allSongsList)
		{
			allSongsList.Remove(this);
			DestroyImmediate(source.gameObject);
		}
	}
}
