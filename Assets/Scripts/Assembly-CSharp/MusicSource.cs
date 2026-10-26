using UnityEngine;

public class MusicSource
{
	private enum InternalState
	{
		Inactive = 0,
		FadingIn = 1,
		Active = 2,
		ActiveVolumeFade = 3,
		FadingOut = 4
	}

	private AudioSource Source;

	private AudioClip activeClip;

	private bool looping;

	private float fadeTime = 1f;

	private float targetVolume = 1f;

	private float currentVolume;

	private float globalVolume = 1f;

	private Lerp01 fadeLerp;

	private InternalState state;

	public bool Active
	{
		get
		{
			return state != InternalState.Inactive;
		}
	}

	public MusicSource(AudioSource source, float globalVolume, float fadeTime)
	{
		Source = source;
		this.globalVolume = globalVolume;
		this.fadeTime = fadeTime;
		fadeLerp = new Lerp01();
		UpdateSourceVolume();
	}

	public void Update()
	{
		if (state == InternalState.FadingIn || state == InternalState.FadingOut || state == InternalState.ActiveVolumeFade)
		{
			UpdateFade();
		}
	}

	public void Play(AudioClip clip)
	{
		StartFade(targetVolume);
		if (Source.isPlaying)
		{
			Source.Stop();
		}
		SetLooping(false);
		Source.clip = clip;
		Source.enabled = true;
		Source.Play();
		activeClip = clip;
		state = InternalState.FadingIn;
	}

	public void FadeOut()
	{
		StartFade(0f);
		state = InternalState.FadingOut;
	}

	public void Stop()
	{
		Source.Stop();
		Source.enabled = false;
		Source.clip = null;
		activeClip = null;
		SetVolume(0f);
		state = InternalState.Inactive;
	}

	public void SetLooping(bool looping)
	{
		this.looping = looping;
		Source.loop = looping;
	}

	public bool IsCurrentTrackEnding()
	{
		if ((state == InternalState.Active || state == InternalState.ActiveVolumeFade || state == InternalState.FadingIn) && !looping && (!Source.isPlaying || activeClip.length - Source.time < fadeTime))
		{
			return true;
		}
		return false;
	}

	private void StartFade(float targetVolume)
	{
		StartFade(targetVolume, fadeTime);
	}

	private void StartFade(float targetVolume, float fadeTime)
	{
		fadeLerp.Lerp(currentVolume, targetVolume, fadeTime, OnFadeComplete, true);
	}

	private void UpdateFade()
	{
		fadeLerp.Update();
		SetVolume(fadeLerp.Current);
	}

	private void OnFadeComplete()
	{
		if (state == InternalState.FadingIn || state == InternalState.ActiveVolumeFade)
		{
			state = InternalState.Active;
		}
		else if (state == InternalState.FadingOut)
		{
			Stop();
		}
	}

	public void SetTargetVolume(float targetVolume, float overrideFadeTime)
	{
		this.targetVolume = targetVolume;
		if (Active && state != InternalState.FadingOut)
		{
			if (overrideFadeTime != -1f)
			{
				StartFade(targetVolume, overrideFadeTime);
			}
			else
			{
				StartFade(targetVolume);
			}
			if (state == InternalState.Active)
			{
				state = InternalState.ActiveVolumeFade;
			}
		}
	}

	public void SetGlobalVolume(float globalVolume)
	{
		this.globalVolume = globalVolume;
		UpdateSourceVolume();
	}

	private void SetVolume(float volume)
	{
		currentVolume = volume;
		UpdateSourceVolume();
	}

	private void UpdateSourceVolume()
	{
		Source.volume = currentVolume * globalVolume;
	}
}
