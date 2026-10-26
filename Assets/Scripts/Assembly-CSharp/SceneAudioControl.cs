using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SceneAudioControl : MonoBehaviour
{
	private float volume = 1f;

	private AudioSource audioSource;

	private void Awake()
	{
		audioSource = base.GetComponent<AudioSource>();
		if (base.GetComponent<AudioSource>() != null)
		{
			volume = base.GetComponent<AudioSource>().volume;
			audioSource.volume = volume * AudioManager.VolumeSetting;
			Invoke("EnableAudio", 1f);
		}
	}

	private void EnableAudio()
	{
		audioSource.enabled = true;
		audioSource.Play();
	}
}
