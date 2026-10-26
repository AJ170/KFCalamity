using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundRandomizer : MonoBehaviour
{
	public float MinDelay;

	public float MaxDelay;

	public AudioClip[] SoundBank;

	private AudioSource source;

	private float volume = 1f;

	private void Start()
	{
		source = base.GetComponent<AudioSource>();
		volume = source.volume;
		StartCoroutine(Play());
	}

	private void PlaySound()
	{
		source.volume = volume * AudioManager.VolumeSetting;
		source.clip = SoundBank[Random.Range(0, SoundBank.Length)];
		source.Play();
		StartCoroutine(Play());
	}

	private IEnumerator Play()
	{
		float waitTime2 = 0f;
		if (source.clip != null)
		{
			waitTime2 = source.clip.length;
		}
		waitTime2 += Random.Range(MinDelay, MaxDelay);
		yield return new WaitForSeconds(waitTime2);
		PlaySound();
	}
}
