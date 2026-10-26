using UnityEngine;

public class UnstealthAudio : MonoBehaviour
{
	public AudioSource Audio;

	public AudioClip[] Clips;

	public float PlayChance;

	public float Volume = 1f;

	public bool LetTimeAffectPitch = true;

	private void Awake()
	{
		if (Audio == null)
		{
			Debug.LogError("UnstealthAudio:Awake() -> Missing audio source");
		}
		if (Clips.Length == 0)
		{
			Debug.LogError("UnstealthAudio:Awake() -> Missing audio clip(s)");
		}
	}

	public void Play()
	{
		if (Audio != null && Clips.Length > 0 && PlayChance > 0f && Random.value <= PlayChance)
		{
			Audio.volume = Volume * AudioManager.VolumeSetting;
			Audio.pitch = ((!LetTimeAffectPitch) ? 1f : Time.timeScale);
			Audio.clip = Clips[Random.Range(0, Clips.Length)];
			Audio.Play();
		}
	}
}
