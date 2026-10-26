using EffectEvents;
using EventHelpers;
using UnityEngine;

public class TriggerRandomAudioEffect : EffectBaseListener
{
	public AudioSource Audio;

	public AudioClip[] Clips;

	public float Volume = 1f;

	public bool LetTimeAffectPitch = true;

	public bool OverridePlayingAudio = true;

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

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (!NoCustomEvents() && !flag)
		{
			return;
		}
		if (Audio != null)
		{
			if (OverridePlayingAudio || (!OverridePlayingAudio && !Audio.isPlaying))
			{
				AudioSource availibleAudioSource = GetAvailibleAudioSource();
				if (Clips != null)
				{
					availibleAudioSource.clip = Clips[Random.Range(0, Clips.Length)];
				}
				if (LetTimeAffectPitch)
				{
					availibleAudioSource.pitch = Time.timeScale;
				}
				availibleAudioSource.volume = Volume * AudioManager.VolumeSetting;
				availibleAudioSource.Play();
			}
		}
		else if (Clips != null && Clips.Length > 0)
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
			AudioClip audioClip = Clips[Random.Range(0, Clips.Length)];
			if (audioClip != null)
			{
				AudioManager.PlayClipAtPoint(audioClip, position, Volume, (!LetTimeAffectPitch) ? 1f : Time.timeScale);
			}
		}
	}
}
