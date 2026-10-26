using System;
using System.Collections.Generic;
using UnityEngine;

public class OldAnimationController : MonoBehaviour
{
	[Serializable]
	public class LayerWeight
	{
		public int LayerID;

		public float Weight;
	}

	private interface Trigger
	{
		string Name { get; }
	}

	private class TriggeredAnimation : Trigger
	{
		public int FrameCounter = 2;

		private string name;

		public bool Value = true;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public TriggeredAnimation(string name)
			: this(name, true)
		{
		}

		public TriggeredAnimation(string name, bool value)
		{
			this.name = name;
			Value = value;
		}
	}

	private class TriggeredTimedAnimation : Trigger
	{
		private string name;

		public float Duration;

		public bool Value;

		public float Counter;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public TriggeredTimedAnimation(string name, float duration)
			: this(name, duration, true)
		{
		}

		public TriggeredTimedAnimation(string name, float duration, bool value)
		{
			this.name = name;
			Duration = duration;
			Value = value;
			Counter = duration;
		}
	}

	public Animator animator;

	public List<LayerWeight> DefaultLayerWeight = new List<LayerWeight>();

	private List<TriggeredAnimation> triggeredAnimations = new List<TriggeredAnimation>(2);

	private List<TriggeredTimedAnimation> timedTriggers = new List<TriggeredTimedAnimation>(2);

	public float AnimationSpeed
	{
		get
		{
			float result = 1f;
			if (animator != null)
			{
				result = animator.speed;
			}
			return result;
		}
	}

	private void Start()
	{
		if (animator != null)
		{
			foreach (LayerWeight item in DefaultLayerWeight)
			{
				animator.SetLayerWeight(item.LayerID, item.Weight);
			}
			return;
		}
		Debug.LogError(string.Format("[AnimationController] No animator set on unit {0}.", base.gameObject.name));
	}

	protected virtual void Update()
	{
		UpdateTriggeredAnimations();
		UpdateTimedTriggers();
	}

	private void UpdateTriggeredAnimations()
	{
		for (int num = triggeredAnimations.Count - 1; num >= 0; num--)
		{
			TriggeredAnimation triggeredAnimation = triggeredAnimations[num];
			triggeredAnimation.FrameCounter--;
			if (triggeredAnimation.FrameCounter <= 0)
			{
				OnTriggeredAnimationExpiration(triggeredAnimation);
				triggeredAnimations.RemoveAt(num);
			}
		}
	}

	private void UpdateTimedTriggers()
	{
		for (int num = timedTriggers.Count - 1; num >= 0; num--)
		{
			TriggeredTimedAnimation triggeredTimedAnimation = timedTriggers[num];
			triggeredTimedAnimation.Counter -= Time.deltaTime;
			if (triggeredTimedAnimation.Counter <= 0f)
			{
				OnTimedTriggerExpiration(triggeredTimedAnimation);
				timedTriggers.RemoveAt(num);
			}
		}
	}

	private void OnTriggeredAnimationExpiration(TriggeredAnimation triggeredAnimation)
	{
		if (animator != null)
		{
			animator.SetBool(triggeredAnimation.Name, !triggeredAnimation.Value);
		}
	}

	private void OnTimedTriggerExpiration(TriggeredTimedAnimation trigger)
	{
		if (animator != null && animator.GetBool(trigger.Name) == trigger.Value)
		{
			animator.SetBool(trigger.Name, !trigger.Value);
		}
	}

	public void TriggerAnimation(string triggerName)
	{
		TriggerAnimation(triggerName, true);
	}

	public void TriggerAnimation(string triggerName, bool value)
	{
		if (animator != null)
		{
			animator.SetBool(triggerName, value);
			RemoveDuplicates(triggeredAnimations, triggerName);
			triggeredAnimations.Add(new TriggeredAnimation(triggerName, value));
		}
	}

	public void TimedTrigger(string triggerName, float duration)
	{
		TimedTrigger(triggerName, duration, true);
	}

	public void TimedTrigger(string triggerName, float duration, bool value)
	{
		if (animator != null)
		{
			animator.SetBool(triggerName, value);
			RemoveDuplicates(timedTriggers, triggerName);
			timedTriggers.Add(new TriggeredTimedAnimation(triggerName, duration, value));
		}
	}

	private void RemoveDuplicates<T>(List<T> triggerList, string name) where T : Trigger
	{
		for (int num = triggerList.Count - 1; num >= 0; num--)
		{
			if (triggerList[num].Name == name)
			{
				triggerList.RemoveAt(num);
				break;
			}
		}
	}

	public void SetBool(string triggerName, bool value)
	{
		if (animator != null)
		{
			animator.SetBool(triggerName, value);
		}
	}

	public void SetFloat(string triggerName, float value)
	{
		if (animator != null)
		{
			animator.SetFloat(triggerName, value);
		}
	}

	public void SetAnimationSpeed(float value)
	{
		if (animator != null)
		{
			animator.speed = value;
		}
	}

	public bool GetBool(string triggerName)
	{
		bool result = false;
		if (animator != null)
		{
			result = animator.GetBool(triggerName);
		}
		return result;
	}
}
