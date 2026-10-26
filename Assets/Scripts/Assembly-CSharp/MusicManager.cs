using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	[Serializable]
	public class MusicSet
	{
		public string Name;

		public AudioClip[] Music;
	}

	[Serializable]
	public class LevelMusicSettings
	{
		public string MusicSet;

		public string[] Levels;
	}

	public MusicSet[] MusicSets;

	public LevelMusicSettings[] LevelSettings;

	public bool ForceLevelMusicOnLevelLoaded = true;

	private MusicSet activeMusicSet;

	private MusicSet activeLevelMusicSet;

	public AudioSource AudioSourceOne;

	public AudioSource AudioSourceTwo;

	public float Volume = 1f;

	private static float globalVolume = 1f;

	private float temporaryVolume = 1f;

	public float FadeTime = 1f;

	private MusicSource[] MusicSources;

	private int activeTrackID = -1;

	private static MusicManager instance;

	private float TotalVolume
	{
		get
		{
			return Volume * globalVolume;
		}
	}

	public static MusicManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		if (instance != null)
		{
			UnityEngine.Object.Destroy(instance.gameObject);
		}
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		SetupMusicSources();
		ValidateMusicSets();
		UpdateLevelMusic();
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	private void OnLevelWasLoaded()
	{
		UpdateLevelMusic();
	}

	private void Update()
	{
		UpdateMusicSources();
		UpdateActiveTrack();
	}

	private void PlayNextTrack()
	{
		if (activeMusicSet == null)
		{
			return;
		}
		FadeOutPlayingMusicSources();
		MusicSource availableMusicSource = GetAvailableMusicSource();
		if (availableMusicSource == null)
		{
			return;
		}
		AudioClip nextMusicClip = GetNextMusicClip();
		if (nextMusicClip != null)
		{
			availableMusicSource.Play(nextMusicClip);
			if (activeMusicSet.Music.Length == 1)
			{
				availableMusicSource.SetLooping(true);
			}
		}
	}

	private AudioClip GetNextMusicClip()
	{
		if (activeMusicSet != null)
		{
			AudioClip result = null;
			int num = activeMusicSet.Music.Length;
			int num2 = -1;
			if (num == 0)
			{
				return null;
			}
			if (num == 1)
			{
				num2 = 0;
			}
			else
			{
				do
				{
					num2 = UnityEngine.Random.Range(0, num);
				}
				while (num2 == activeTrackID);
			}
			if (num2 != -1)
			{
				result = activeMusicSet.Music[num2];
				activeTrackID = num2;
			}
			return result;
		}
		return null;
	}

	private void StopAll()
	{
		FadeOutPlayingMusicSources();
		activeMusicSet = null;
		activeLevelMusicSet = null;
	}

	private void UpdateActiveTrack()
	{
		MusicSource[] musicSources = MusicSources;
		foreach (MusicSource musicSource in musicSources)
		{
			if (musicSource.IsCurrentTrackEnding())
			{
				PlayNextTrack();
			}
		}
	}

	public void SetActiveLevelMusicSet(string musicSetName)
	{
		MusicSet musicSet = MusicSetByName(musicSetName);
		if (musicSet != null)
		{
			SetActiveLevelMusicSet(musicSet);
		}
	}

	public void SetActiveLevelMusicSet(MusicSet musicSet)
	{
		if (musicSet != activeLevelMusicSet)
		{
			bool flag = activeMusicSet == null || activeMusicSet == activeLevelMusicSet;
			activeLevelMusicSet = musicSet;
			if (flag)
			{
				StartMusicSet(musicSet);
			}
		}
	}

	private void UpdateLevelMusic()
	{
		string loadedLevelName = Application.loadedLevelName;
		MusicSet musicSet = FindMusicSetByLevel(loadedLevelName);
		if (musicSet != null && (musicSet != activeLevelMusicSet || (ForceLevelMusicOnLevelLoaded && musicSet != activeMusicSet)))
		{
			if (activeMusicSet == null || activeMusicSet == activeLevelMusicSet || ForceLevelMusicOnLevelLoaded)
			{
				StartMusicSet(musicSet);
			}
			activeLevelMusicSet = musicSet;
		}
	}

	public void PlayLevelMusicSet()
	{
		if (activeLevelMusicSet != null && activeLevelMusicSet != activeMusicSet)
		{
			StartMusicSet(activeLevelMusicSet);
		}
	}

	public void StartMusicSet(string musicSetName)
	{
		MusicSet musicSet = MusicSetByName(musicSetName);
		if (musicSet != null)
		{
			StartMusicSet(musicSet);
		}
	}

	private void StartMusicSet(MusicSet musicSet)
	{
		if (musicSet != activeMusicSet)
		{
			activeMusicSet = musicSet;
			activeTrackID = -1;
			PlayNextTrack();
		}
	}

	public void StopMusicSet(string musicSetName)
	{
		if (activeMusicSet != null && activeMusicSet.Name == musicSetName)
		{
			if (activeLevelMusicSet != activeMusicSet)
			{
				StartMusicSet(activeLevelMusicSet);
			}
			else
			{
				StopAll();
			}
		}
	}

	private MusicSet FindMusicSetByLevel(string levelName)
	{
		string text = FindMusicSetNameByLevel(levelName);
		if (text != null)
		{
			return MusicSetByName(text);
		}
		return null;
	}

	private string FindMusicSetNameByLevel(string levelName)
	{
		LevelMusicSettings[] levelSettings = LevelSettings;
		foreach (LevelMusicSettings levelMusicSettings in levelSettings)
		{
			string[] levels = levelMusicSettings.Levels;
			foreach (string text in levels)
			{
				if (text == levelName)
				{
					return levelMusicSettings.MusicSet;
				}
			}
		}
		return null;
	}

	private MusicSet MusicSetByName(string musicSetName)
	{
		MusicSet[] musicSets = MusicSets;
		foreach (MusicSet musicSet in musicSets)
		{
			if (musicSet != null && musicSet.Name == musicSetName)
			{
				return musicSet;
			}
		}
		Debug.LogWarning("[MusicManager] Failed to locate music set by name " + musicSetName);
		return null;
	}

	private void ValidateMusicSets()
	{
		for (int i = 0; i < MusicSets.Length; i++)
		{
			if (MusicSets[i].Music.Length == 0)
			{
				Debug.Log("[MusicManager] Found invalid music set " + MusicSets[i].Name);
				MusicSets[i] = null;
			}
		}
	}

	private void UpdateMusicSources()
	{
		MusicSource[] musicSources = MusicSources;
		foreach (MusicSource musicSource in musicSources)
		{
			musicSource.Update();
		}
	}

	private void SetupMusicSources()
	{
		MusicSources = new MusicSource[2];
		MusicSources[0] = new MusicSource(AudioSourceOne, TotalVolume, FadeTime);
		MusicSources[1] = new MusicSource(AudioSourceTwo, TotalVolume, FadeTime);
	}

	private void FadeOutPlayingMusicSources()
	{
		MusicSource[] musicSources = MusicSources;
		foreach (MusicSource musicSource in musicSources)
		{
			if (musicSource.Active)
			{
				musicSource.FadeOut();
			}
		}
	}

	private MusicSource GetAvailableMusicSource()
	{
		MusicSource musicSource = null;
		MusicSource[] musicSources = MusicSources;
		foreach (MusicSource musicSource2 in musicSources)
		{
			if (!musicSource2.Active)
			{
				musicSource = musicSource2;
			}
		}
		if (musicSource == null)
		{
			musicSource = GetFirstMusicSource();
		}
		return musicSource;
	}

	private MusicSource GetFirstMusicSource()
	{
		if (MusicSources.Length > 0)
		{
			return MusicSources[0];
		}
		return null;
	}

	public static void SetGlobalVolume(float globalVolume)
	{
		MusicManager.globalVolume = globalVolume;
		if (Instance != null)
		{
			MusicSource[] musicSources = Instance.MusicSources;
			foreach (MusicSource musicSource in musicSources)
			{
				musicSource.SetGlobalVolume(Instance.TotalVolume);
			}
		}
	}

	public void SetTemporaryVolume(float temporaryVolume, float overrideFadeTime = -1f)
	{
		this.temporaryVolume = temporaryVolume;
		MusicSource[] musicSources = MusicSources;
		foreach (MusicSource musicSource in musicSources)
		{
			musicSource.SetTargetVolume(this.temporaryVolume, overrideFadeTime);
		}
	}
}
