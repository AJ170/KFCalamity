using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
	private class Trigger
	{
		public int Hash;

		public bool Value;

		public int Frame;

		public Trigger(int hash, bool value, int frame = 0)
		{
			Hash = hash;
			Value = value;
			Frame = frame;
		}
	}

	private class TimedTrigger : Trigger
	{
		public float Counter;

		public TimedTrigger(int hash, bool value, float duration)
			: base(hash, value)
		{
			Counter = duration;
		}
	}

	private static Dictionary<string, int> hashedKeys = new Dictionary<string, int>();

	private List<Trigger> activeTriggers = new List<Trigger>();

	private List<TimedTrigger> activeTimedTriggers = new List<TimedTrigger>();

	public Animator animator;

	private int layerCount;

	protected virtual void Awake()
	{
		if (animator == null)
		{
			Debug.LogError(string.Format("[AnimationController] No animator set on unit {0}.", base.gameObject.name));
			base.enabled = false;
		}
	}

	private void OnDestroy()
	{
		animator = null;
	}

	private void Start()
	{
		SetupLayerWeights();
	}

	private void Update()
	{
		UpdateTriggers();
		UpdateTimedTriggers();
	}

	public void SetBool(string name, bool value)
	{
		if (animator != null)
		{
			animator.SetBool(name, value);
		}
	}

	public void SetBool(int hash, bool value)
	{
		if (animator != null)
		{
			animator.SetBool(hash, value);
		}
	}

	public void SetFloat(string name, float value)
	{
		if (animator != null)
		{
			animator.SetFloat(name, value);
		}
	}

	public void SetFloat(int hash, float value)
	{
		if (animator != null)
		{
			animator.SetFloat(hash, value);
		}
	}

	public bool GetBool(string name)
	{
		bool result = false;
		if (animator != null)
		{
			result = animator.GetBool(name);
		}
		return result;
	}

	public bool GetBool(int hash)
	{
		bool result = false;
		if (animator != null)
		{
			result = animator.GetBool(hash);
		}
		return result;
	}

	public float GetFloat(string name)
	{
		float result = 0f;
		if (animator != null)
		{
			result = animator.GetFloat(name);
		}
		return result;
	}

	public float GetFloat(int hash)
	{
		float result = 0f;
		if (animator != null)
		{
			result = animator.GetFloat(hash);
		}
		return result;
	}

	private void UpdateTriggers()
	{
		if (!(animator != null) || activeTriggers.Count <= 0)
		{
			return;
		}
		AnimatorStateInfo[] array = new AnimatorStateInfo[layerCount];
		for (int i = 0; i < layerCount; i++)
		{
			array[i] = animator.GetCurrentAnimatorStateInfo(i);
		}
		for (int num = activeTriggers.Count - 1; num >= 0; num--)
		{
			Trigger trigger = activeTriggers[num];
			AnimatorStateInfo[] array2 = array;
			foreach (AnimatorStateInfo animatorStateInfo in array2)
			{
				if (animatorStateInfo.tagHash == trigger.Hash && Time.frameCount != trigger.Frame)
				{
					ResetTrigger(trigger);
					activeTriggers.RemoveAt(num);
					break;
				}
			}
		}
	}

	public virtual void TriggerAnimation(string name)
	{
		TriggerAnimation(GetHash(name), true);
	}

	public virtual void TriggerAnimation(string name, bool value)
	{
		TriggerAnimation(GetHash(name), value);
	}

	public void TriggerAnimation(int hash)
	{
		TriggerAnimation(hash, true);
	}

	public void TriggerAnimation(int hash, bool value)
	{
		if (animator != null)
		{
			SetBool(hash, value);
			AddTrigger(hash, value, Time.frameCount);
		}
	}

	private void AddTrigger(int hash, bool value, int frame = 0)
	{
		activeTriggers.Add(new Trigger(hash, value, frame));
	}

	private void ResetTrigger(Trigger trigger)
	{
		if (animator != null)
		{
			animator.SetBool(trigger.Hash, !trigger.Value);
		}
	}

	protected void ClearTrigger(int hash)
	{
		for (int i = 0; i < activeTriggers.Count; i++)
		{
			if (activeTriggers[i].Hash == hash)
			{
				activeTriggers.RemoveAt(i);
			}
		}
	}

	private void UpdateTimedTriggers()
	{
		if (!(animator != null) || activeTimedTriggers.Count <= 0)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		for (int num = activeTimedTriggers.Count - 1; num >= 0; num--)
		{
			TimedTrigger timedTrigger = activeTimedTriggers[num];
			timedTrigger.Counter -= deltaTime;
			if (timedTrigger.Counter <= 0f)
			{
				ResetTrigger(timedTrigger);
				activeTimedTriggers.RemoveAt(num);
			}
		}
	}

	public virtual void TriggerTimedAnimation(string name, float duration)
	{
		TriggerTimedAnimation(GetHash(name), true, duration);
	}

	public virtual void TriggerTimedAnimation(string name, bool value, float duration)
	{
		TriggerTimedAnimation(GetHash(name), value, duration);
	}

	public void TriggerTimedAnimation(int hash, float duration)
	{
		TriggerTimedAnimation(hash, true, duration);
	}

	public void TriggerTimedAnimation(int hash, bool value, float duration)
	{
		if (animator != null && !RemoveDuplicateTimers(hash, value, duration))
		{
			SetBool(hash, value);
			AddTimedTrigger(hash, value, duration);
		}
	}

	private bool RemoveDuplicateTimers(int hash, bool value, float duration)
	{
		bool result = false;
		for (int num = activeTimedTriggers.Count - 1; num >= 0; num--)
		{
			TimedTrigger timedTrigger = activeTimedTriggers[num];
			if (timedTrigger.Hash == hash)
			{
				if (value != timedTrigger.Value || timedTrigger.Counter < duration)
				{
					activeTimedTriggers.RemoveAt(num);
				}
				else
				{
					result = true;
				}
				break;
			}
		}
		return result;
	}

	private void AddTimedTrigger(int hash, bool value, float duration)
	{
		activeTimedTriggers.Add(new TimedTrigger(hash, value, duration));
	}

	private void SetupLayerWeights()
	{
		layerCount = animator.layerCount;
		for (int i = 0; i < layerCount; i++)
		{
			animator.SetLayerWeight(i, 1f);
		}
	}

	public int GetHash(string name)
	{
		if (!hashedKeys.ContainsKey(name))
		{
			hashedKeys.Add(name, Animator.StringToHash(name));
		}
		return hashedKeys[name];
	}

	public virtual int GetHash(PlayerAnimationTrigger trigger)
	{
		return -1;
	}

	public virtual int GetHash(ZedAnimationTrigger trigger)
	{
		return -1;
	}

	public bool IsAnimationWithTagActive(int hash, int layer)
	{
		return animator.GetCurrentAnimatorStateInfo(layer).tagHash == hash;
	}
}
