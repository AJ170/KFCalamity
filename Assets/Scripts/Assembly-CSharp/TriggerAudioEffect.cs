using System;
using EffectEvents;
using EventHelpers;
using UnityEngine;

public class TriggerAudioEffect : EffectBaseListener
{
	public AudioSource Audio;

	public AudioClip Clip;

	public float Volume = 1f;

	public bool LetTimeAffectPitch = true;

	public bool Loop;

	private int source;

	private AudioSource[] additionalAudioSources;

	protected override void Start()
	{
		base.Start();
		additionalAudioSources = GetComponentsInChildren<AudioSource>();
	}

	private AudioSource GetAvailibleAudioSource()
	{
		if (additionalAudioSources.Length == 0)
		{
			return Audio;
		}
		source = ++source % (additionalAudioSources.Length + 1);
		if (source == 0)
		{
			return Audio;
		}
		return additionalAudioSources[source - 1];
	}

	private void StopLoopingClip()
	{
		if (Audio.isPlaying && Audio.loop)
		{
			Audio.Stop();
		}
		else if (additionalAudioSources.Length > 0)
		{
			AudioSource audioSource = Array.Find(additionalAudioSources, (AudioSource a) => a.isPlaying && a.loop);
			if (audioSource != null)
			{
				audioSource.Stop();
			}
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (!NoCustomEvents() && !flag)
		{
			return;
		}
		if (Audio != null)
		{
			AudioSource availibleAudioSource = GetAvailibleAudioSource();
			if (Clip != null)
			{
				availibleAudioSource.clip = Clip;
			}
			if (LetTimeAffectPitch)
			{
				availibleAudioSource.pitch = Time.timeScale;
			}
			if (Loop)
			{
				StopLoopingClip();
			}
			availibleAudioSource.volume = Volume * AudioManager.VolumeSetting;
			availibleAudioSource.loop = Loop;
			availibleAudioSource.Play();
		}
		else if (Clip != null)
		{
			Vector3 position;
			if (e.Id == TriggerId.Impact)
			{
				Impact impact = (Impact)e;
				position = impact.Position;
			}
			else
			{
				position = base.transform.position;
			}
			AudioManager.PlayClipAtPoint(Clip, position, Volume, (!LetTimeAffectPitch) ? 1f : Time.timeScale);
		}
	}
}
