using EventHelpers;
using UnityEngine;

public class TimeToggledAudioEffect : EffectBaseListener
{
	public AudioSource Audio;

	public AudioClip Clip;

	public float Volume = 1f;

	public bool LetTimeAffectPitch = true;

	public float StopDelay;

	private float delayTimer;

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (!NoCustomEvents() && !flag)
		{
			return;
		}
		if (Audio != null && !Audio.isPlaying)
		{
			if (Clip != null)
			{
				Audio.clip = Clip;
			}
			if (LetTimeAffectPitch)
			{
				Audio.pitch = Time.timeScale;
			}
			Audio.volume = Volume * AudioManager.VolumeSetting;
			Audio.Play();
		}
		else if (Audio == null)
		{
			Debug.LogError("TimeToggledAudioEffect requires a AudioSource!");
		}
		delayTimer = StopDelay;
	}

	private void Update()
	{
		if (Audio.isPlaying)
		{
			if (delayTimer == 0f)
			{
				Audio.Stop();
			}
			else
			{
				delayTimer = Mathf.Max(0f, delayTimer - Time.deltaTime);
			}
		}
	}
}
