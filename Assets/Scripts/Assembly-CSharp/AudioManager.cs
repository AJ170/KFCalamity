using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioSourcePool AudioSourcePool;

	public float Volume = 1f;

	public AudioClip[] ClipDatabase;

	private static AudioManager instance;

	private static float globalVolume = 1f;

	private float totalVolume = 1f;

	public static float VolumeSetting
	{
		get
		{
			if (instance != null)
			{
				return instance.totalVolume;
			}
			return globalVolume;
		}
	}

	private void Awake()
	{
		Volume = Mathf.Clamp01(Volume);
		ApplyVolumeSettings();
		InitializePool();
		instance = this;
	}

	private void InitializePool()
	{
		GameObject gameObject = new GameObject("Audio Sources");
		gameObject.transform.parent = base.transform;
		AudioSourcePool.Initialize(gameObject.transform);
	}

	private void Update()
	{
		AudioSourcePool.Update();
	}

	public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume, float pitch)
	{
		if (instance != null)
		{
			if (clip != null)
			{
				GameObject poolObject = instance.AudioSourcePool.GetPoolObject();
				poolObject.SetActive(true);
				poolObject.transform.position = position;
				AudioSource audioSource = poolObject.GetComponent<AudioSource>();
				audioSource.clip = clip;
				audioSource.volume = volume * instance.totalVolume;
				audioSource.pitch = pitch;
				audioSource.Play();
			}
			else
			{
				Debug.LogWarning("AudioManager:PlayAtLocation() Missing AudioClip");
			}
		}
	}

	public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume)
	{
		PlayClipAtPoint(clip, position, volume, 1f);
	}

	public static void PlayClipAtPoint(AudioClip clip, Vector3 position)
	{
		PlayClipAtPoint(clip, position, 1f, 1f);
	}

	public static void PlayClipAtPoint(string clipNameInDatabase, Vector3 position, float volume, float pitch)
	{
		if (instance != null)
		{
			AudioClip clipInDatabase = instance.GetClipInDatabase(clipNameInDatabase);
			PlayClipAtPoint(clipInDatabase, position, volume, pitch);
		}
	}

	public static void PlayClipAtPoint(string clipNameInDatabase, Vector3 position)
	{
		PlayClipAtPoint(clipNameInDatabase, position, 1f, 1f);
	}

	public static void StopClip(AudioClip clip)
	{
		if (instance != null && clip != null)
		{
			instance.AudioSourcePool.StopBusyWithClip(clip);
		}
	}

	public static void SetGlobalVolume(float volume)
	{
		globalVolume = Mathf.Clamp01(volume);
		if (instance != null)
		{
			instance.ApplyVolumeSettings();
		}
	}

	public static float GlobalVolume()
	{
		return globalVolume;
	}

	private void ApplyVolumeSettings()
	{
		totalVolume = Volume * globalVolume;
	}

	private AudioClip GetClipInDatabase(string clipName)
	{
		AudioClip[] clipDatabase = ClipDatabase;
		foreach (AudioClip audioClip in clipDatabase)
		{
			if (clipName == audioClip.name)
			{
				return audioClip;
			}
		}
		return null;
	}
}
