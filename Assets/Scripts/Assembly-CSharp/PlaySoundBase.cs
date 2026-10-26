using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundBase : MonoBehaviour
{
	public AudioClip[] Audio;

	private AudioSource source;

	private float volume = 1f;

	private void Awake()
	{
		source = base.gameObject.GetComponent<AudioSource>();
		volume = source.volume;
	}

	protected void PlayRandomClip(bool loop = false)
	{
		if (!(source != null) || Audio == null || Audio.Length <= 0)
		{
			return;
		}
		int num = Random.Range(0, Audio.Length);
		AudioClip audioClip = Audio[num];
		if (audioClip != null)
		{
			if (source.isPlaying)
			{
				source.Stop();
			}
			source.loop = loop;
			source.volume = volume * AudioManager.VolumeSetting;
			source.clip = audioClip;
			source.Play();
		}
	}
}
