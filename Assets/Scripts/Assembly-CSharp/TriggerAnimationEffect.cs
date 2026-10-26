using EventHelpers;
using UnityEngine;

public class TriggerAnimationEffect : EffectBaseListener
{
	public enum AnimationCategory
	{
		Player = 0,
		Zed = 1,
		Custom = 2
	}

	public enum TriggerType
	{
		Trigger = 0,
		Timed = 1,
		Persistent = 2
	}

	public AnimationCategory TriggerCategory = AnimationCategory.Custom;

	public PlayerAnimationTrigger PlayerTrigger = PlayerAnimationTrigger.Attack;

	public ZedAnimationTrigger ZedTrigger = ZedAnimationTrigger.Attack;

	public string CustomTrigger;

	public TriggerType Type;

	public float Duration;

	public bool Value = true;

	private int hash = -1;

	protected AnimationController animationController;

	protected virtual void OnMounted(GameObject parent)
	{
		animationController = parent.GetComponent<AnimationController>();
		if (animationController == null)
		{
			Debug.LogError(string.Format("TriggerAnimation failed to locate Animator in parent {0}.", parent.name));
		}
		else
		{
			GetHash();
		}
	}

	protected override void Execute(TriggerBase e)
	{
		if (!(animationController != null))
		{
			return;
		}
		bool flag = MatchCustomEvent(e.CustomName);
		if ((customEventListeners.Count == 0 || flag) && hash != -1)
		{
			switch (Type)
			{
			case TriggerType.Trigger:
				animationController.TriggerAnimation(hash, Value);
				break;
			case TriggerType.Timed:
				animationController.TriggerTimedAnimation(hash, Value, Duration);
				break;
			case TriggerType.Persistent:
				animationController.SetBool(hash, Value);
				break;
			}
		}
	}

	protected void GetHash()
	{
		if (TriggerCategory == AnimationCategory.Player)
		{
			hash = animationController.GetHash(PlayerTrigger);
		}
		else if (TriggerCategory == AnimationCategory.Zed)
		{
			hash = animationController.GetHash(ZedTrigger);
		}
		else if (!string.IsNullOrEmpty(CustomTrigger))
		{
			hash = animationController.GetHash(CustomTrigger);
		}
		else
		{
			Debug.Log("Invalid custom animation trigger in " + base.transform.root.name + " " + base.transform.parent.name);
		}
	}
}
