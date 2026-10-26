using System;
using System.Collections.Generic;
using UnityEngine;

public class ZedAnimationController : AnimationController
{
	private static Dictionary<ZedAnimationTrigger, int> triggerHashes;

	protected override void Awake()
	{
		base.Awake();
		SetupHashKeys();
	}

	public void SetBool(ZedAnimationTrigger trigger, bool value)
	{
		if (triggerHashes.ContainsKey(trigger))
		{
			SetBool(triggerHashes[trigger], value);
		}
	}

	public void SetFloat(ZedAnimationTrigger trigger, float value)
	{
		if (triggerHashes.ContainsKey(trigger))
		{
			SetFloat(triggerHashes[trigger], value);
		}
	}

	public bool GetBool(ZedAnimationTrigger trigger)
	{
		bool result = false;
		if (triggerHashes.ContainsKey(trigger))
		{
			result = GetBool(triggerHashes[trigger]);
		}
		return result;
	}

	public float GetFloat(ZedAnimationTrigger trigger)
	{
		float result = 0f;
		if (triggerHashes.ContainsKey(trigger))
		{
			result = GetFloat(triggerHashes[trigger]);
		}
		return result;
	}

	public void TriggerAnimation(ZedAnimationTrigger trigger)
	{
		TriggerAnimation(trigger, true);
	}

	public void TriggerAnimation(ZedAnimationTrigger trigger, bool value)
	{
		if (triggerHashes.ContainsKey(trigger))
		{
			TriggerAnimation(triggerHashes[trigger], value);
		}
	}

	public void TriggerTimedAnimation(ZedAnimationTrigger trigger, float duration)
	{
		TriggerTimedAnimation(trigger, true, duration);
	}

	public void TriggerTimedAnimation(ZedAnimationTrigger trigger, bool value, float duration)
	{
		if (triggerHashes.ContainsKey(trigger))
		{
			TriggerTimedAnimation(triggerHashes[trigger], value, duration);
		}
	}

	private void SetupHashKeys()
	{
		if (triggerHashes != null)
		{
			return;
		}
		triggerHashes = new Dictionary<ZedAnimationTrigger, int>();
		foreach (int value in Enum.GetValues(typeof(ZedAnimationTrigger)))
		{
			triggerHashes.Add((ZedAnimationTrigger)value, Animator.StringToHash(((ZedAnimationTrigger)value).ToString()));
		}
	}

	public override int GetHash(ZedAnimationTrigger trigger)
	{
		return triggerHashes[trigger];
	}
}
