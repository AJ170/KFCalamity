using System.Collections.Generic;
using UnityEngine;

public class OldPlayerAnimationController : AnimationController
{
	private struct ActiveTrigger
	{
		public int Hash;

		public bool Value;

		public ActiveTrigger(int hash, bool value)
		{
			Hash = hash;
			Value = value;
		}
	}

	private static Dictionary<PlayerAnimationTrigger, int> triggerHashes;

	private List<ActiveTrigger> activeTriggers = new List<ActiveTrigger>();

	private Vector3 lastPosition = Vector3.zero;

	private Vector3 velocity = Vector3.zero;

	private Vector3 localVelocity = Vector3.zero;

	private float speed;

	private bool idle = true;

	private int playerAnimationLayers = 2;

	private new void Awake()
	{
		if (triggerHashes == null)
		{
			SetupKeys();
		}
	}

	private void SetupKeys()
	{
		triggerHashes = new Dictionary<PlayerAnimationTrigger, int>();
		triggerHashes.Add(PlayerAnimationTrigger.Running, Animator.StringToHash("Running"));
		triggerHashes.Add(PlayerAnimationTrigger.Sprinting, Animator.StringToHash("Sprinting"));
		triggerHashes.Add(PlayerAnimationTrigger.Attack, Animator.StringToHash("Attack"));
		triggerHashes.Add(PlayerAnimationTrigger.AttackOffhand, Animator.StringToHash("AttackOffhand"));
		triggerHashes.Add(PlayerAnimationTrigger.Ironsight, Animator.StringToHash("Ironsight"));
		triggerHashes.Add(PlayerAnimationTrigger.Reload, Animator.StringToHash("Reload"));
		triggerHashes.Add(PlayerAnimationTrigger.Throw, Animator.StringToHash("Throw"));
		triggerHashes.Add(PlayerAnimationTrigger.ResetAttack, Animator.StringToHash("ResetAttack"));
		triggerHashes.Add(PlayerAnimationTrigger.Dodge, Animator.StringToHash("Dodge"));
		triggerHashes.Add(PlayerAnimationTrigger.Speed, Animator.StringToHash("Speed"));
		triggerHashes.Add(PlayerAnimationTrigger.Direction, Animator.StringToHash("Direction"));
	}

	protected void Update()
	{
		UpdateActiveTriggers();
	}

	private void FixedUpdate()
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
				SetBool("Running", true);
				idle = false;
			}
			localVelocity.Normalize();
			SetFloat("Speed", localVelocity.z);
			SetFloat("Direction", localVelocity.x);
		}
		else if (!idle && speed < 0.1f)
		{
			SetBool("Running", false);
			idle = true;
		}
		lastPosition = position;
	}

	private void UpdateActiveTriggers()
	{
		if (!(animator != null) || activeTriggers.Count <= 0)
		{
			return;
		}
		AnimatorStateInfo[] array = new AnimatorStateInfo[playerAnimationLayers];
		for (int i = 0; i < playerAnimationLayers; i++)
		{
			array[i] = animator.GetCurrentAnimatorStateInfo(i);
		}
		for (int num = activeTriggers.Count - 1; num >= 0; num--)
		{
			ActiveTrigger trigger = activeTriggers[num];
			AnimatorStateInfo[] array2 = array;
			foreach (AnimatorStateInfo animatorStateInfo in array2)
			{
				if (animatorStateInfo.tagHash == trigger.Hash)
				{
					ResetTrigger(trigger);
					activeTriggers.RemoveAt(num);
					break;
				}
			}
		}
	}

	public void TriggerAnimation(PlayerAnimationTrigger trigger)
	{
		TriggerAnimation(trigger, true);
	}

	public void TriggerAnimation(PlayerAnimationTrigger trigger, bool value)
	{
		if (animator != null && (trigger != PlayerAnimationTrigger.Attack || !AttackAnimationActive()))
		{
			if (triggerHashes.ContainsKey(trigger))
			{
				int num = triggerHashes[trigger];
				animator.SetBool(num, value);
				activeTriggers.Add(new ActiveTrigger(num, value));
			}
			else
			{
				Debug.LogWarning(string.Format("[PlayerAnimationController:TriggerAnimation] No hash defined for trigger {0}.", trigger.ToString()));
			}
		}
	}

	private void ResetTrigger(ActiveTrigger trigger)
	{
		if (animator != null)
		{
			animator.SetBool(trigger.Hash, !trigger.Value);
		}
	}

	private bool AttackAnimationActive()
	{
		return animator.GetCurrentAnimatorStateInfo(1).tagHash == triggerHashes[PlayerAnimationTrigger.Attack];
	}
}
