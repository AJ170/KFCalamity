using System.Collections;
using Calamity.Camera;
using EventHelpers;
using UnityEngine;

public class CameraShakeEffect : EffectBaseListener
{
	public float Intensity = 0.6f;

	public CameraShakeDecayType DecayType;

	public float Decay = 0.002f;

	public bool UseDuration;

	public float Duration = 1f;

	public bool StopWhenDestroyed;

	public bool StopOnKill;

	public bool UseDelay;

	public float Delay;

	private CameraController cameraController;

	protected override void Start()
	{
		base.Start();
		Camera main = Camera.main;
		if (main != null)
		{
			cameraController = main.GetComponent<CameraController>();
		}
		if (StopOnKill)
		{
			effectSystem.EventSystem.Subscribe(TriggerId.Kill, Execute, this);
		}
	}

	private void OnDestroy()
	{
		if (StopWhenDestroyed)
		{
			StopEffect();
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (NoCustomEvents() || flag)
		{
			if (StopOnKill && e.Id == TriggerId.Kill)
			{
				StopEffect();
			}
			else if (UseDelay && Delay > 0f)
			{
				StartCoroutine(WaitForDelay());
			}
			else
			{
				CreateEffect();
			}
		}
	}

	private IEnumerator WaitForDelay()
	{
		yield return new WaitForSeconds(Delay);
		CreateEffect();
	}

	private void CreateEffect()
	{
		if (cameraController != null)
		{
			cameraController.AddEffect(new ShakeCameraEffect(Intensity, DecayType, Decay, UseDuration, Duration, base.gameObject));
		}
	}

	private void StopEffect()
	{
		if (cameraController != null)
		{
			cameraController.RemoveEffectsBySource(base.gameObject);
		}
	}
}
