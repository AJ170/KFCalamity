using System;
using UnityEngine;

public class WeaponSecondaryReloadAudio : MonoBehaviour
{
	[Serializable]
	public class AudioInfo
	{
		public AudioClip Clip;

		public float ClipDelay;

		public bool Enabled = true;
	}

	public AudioSource Audio;

	public AudioInfo[] Clips;

	public float Volume = 1f;

	public bool LetTimeAffectPitch = true;

	public bool RandomClip;

	private bool IsActive;

	private int audioCounter;

	private float counter;

	public void Execute()
	{
		if (Audio == null)
		{
			Audio = base.GetComponent<AudioSource>();
		}
		if (Audio != null)
		{
			IsActive = true;
			return;
		}
		Debug.LogError("[WeaponReloadAudio] Failed to locate AudioSource");
		base.enabled = false;
	}

	private void Update()
	{
		if (!IsActive || !(Audio != null))
		{
			return;
		}
		if (RandomClip)
		{
			int num = UnityEngine.Random.Range(0, Clips.Length);
			if (counter >= Clips[num].ClipDelay)
			{
				Audio.clip = Clips[UnityEngine.Random.Range(0, Clips.Length)].Clip;
				ApplyAudioSettings();
				Audio.Play();
				Reset();
			}
		}
		else if (counter >= Clips[audioCounter].ClipDelay)
		{
			if (Clips[audioCounter].Enabled)
			{
				Audio.clip = Clips[audioCounter].Clip;
				ApplyAudioSettings();
				Audio.Play();
			}
			audioCounter++;
		}
		counter += Time.deltaTime;
		if (audioCounter >= Clips.Length)
		{
			Reset();
		}
	}

	private void ApplyAudioSettings()
	{
		if (Audio != null)
		{
			Audio.volume = Volume * AudioManager.VolumeSetting;
			Audio.pitch = ((!LetTimeAffectPitch) ? 1f : Time.timeScale);
		}
	}

	public void Reset()
	{
		IsActive = false;
		audioCounter = 0;
		counter = 0f;
	}

	public void Abort()
	{
		if (Audio != null)
		{
			Audio.Stop();
		}
		Reset();
	}
}
