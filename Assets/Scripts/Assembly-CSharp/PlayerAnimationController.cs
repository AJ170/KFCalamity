using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : AnimationController
{
	private static Dictionary<PlayerAnimationTrigger, int> triggerHashes;

	private Vector3 lastPosition = Vector3.zero;

	private Vector3 velocity = Vector3.zero;

	private Vector3 localVelocity = Vector3.zero;

	private float speed;

	private bool idle = true;

	protected override void Awake()
	{
		base.Awake();
		SetupHashKeys();
	}

	private void FixedUpdate()
	{
		UpdateAnimations();
	}

	private void UpdateAnimations()
	{
		Vector3 position = base.transform.position;
		velocity = (position - lastPosition) / Time.deltaTime;
		localVelocity = base.transform.InverseTransformDirection(velocity);
		localVelocity.y = 0f;
		speed = localVelocity.magnitude;
		if (speed > 0.1f)
		{
			if (idle)
			{
				SetBool(PlayerAnimationTrigger.Running, true);
				idle = false;
			}
			localVelocity.Normalize();
			SetFloat(PlayerAnimationTrigger.Speed, localVelocity.z);
			SetFloat(PlayerAnimationTrigger.Direction, localVelocity.x);
		}
		else if (!idle && speed < 0.1f)
		{
			SetBool(PlayerAnimationTrigger.Running, false);
			idle = true;
		}
		lastPosition = position;
	}

	public void SetBool(PlayerAnimationTrigger trigger, bool value)
	{
		if (triggerHashes.ContainsKey(trigger))
		{
			SetBool(triggerHashes[trigger], value);
		}
	}

	public void SetFloat(PlayerAnimationTrigger trigger, float value)
	{
		if (triggerHashes.ContainsKey(trigger))
		{
			SetFloat(triggerHashes[trigger], value);
		}
	}

	public bool GetBool(PlayerAnimationTrigger trigger)
	{
		bool result = false;
		if (triggerHashes.ContainsKey(trigger))
		{
			result = GetBool(triggerHashes[trigger]);
		}
		return result;
	}

	public float GetFloat(PlayerAnimationTrigger trigger)
	{
		float result = 0f;
		if (triggerHashes.ContainsKey(trigger))
		{
			result = GetFloat(triggerHashes[trigger]);
		}
		return result;
	}

	public void TriggerAnimation(PlayerAnimationTrigger trigger)
	{
		TriggerAnimation(trigger, true);
	}

	public void TriggerAnimation(PlayerAnimationTrigger trigger, bool value)
	{
		if (triggerHashes.ContainsKey(trigger))
		{
			TriggerAnimation(triggerHashes[trigger], value);
		}
	}

	public void TriggerTimedAnimation(PlayerAnimationTrigger trigger, float duration)
	{
		TriggerTimedAnimation(trigger, true, duration);
	}

	public void TriggerTimedAnimation(PlayerAnimationTrigger trigger, bool value, float duration)
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
		triggerHashes = new Dictionary<PlayerAnimationTrigger, int>();
		foreach (int value in Enum.GetValues(typeof(PlayerAnimationTrigger)))
		{
			triggerHashes.Add((PlayerAnimationTrigger)value, Animator.StringToHash(((PlayerAnimationTrigger)value).ToString()));
		}
	}

	public override int GetHash(PlayerAnimationTrigger trigger)
	{
		return triggerHashes[trigger];
	}

	public void ClearAttackTrigger()
	{
		int hash = triggerHashes[PlayerAnimationTrigger.Attack];
		ClearTrigger(hash);
		SetBool(hash, false);
		hash = triggerHashes[PlayerAnimationTrigger.AttackOffhand];
		ClearTrigger(hash);
		SetBool(hash, false);
	}
}
