using EventHelpers;
using UnityEngine;

public class PlayerAnimationTriggerEffect : EffectBaseListener
{
	public PlayerAnimationTrigger AnimationTrigger = PlayerAnimationTrigger.Attack;

	public bool Value = true;

	private PlayerAnimationController animationController;

	private void OnMounted(GameObject parent)
	{
		animationController = parent.GetComponent<PlayerAnimationController>();
		if (animationController == null)
		{
			Debug.LogError(string.Format("TriggerAnimation failed to locate Animator in parent {0}.", parent.name));
		}
		Debug.LogWarning("[PlayerAnimationTriggerEffect] Using depricated class in object " + parent.name);
	}

	protected override void Execute(TriggerBase e)
	{
		if (animationController != null)
		{
			bool flag = MatchCustomEvent(e.CustomName);
			if (customEventListeners.Count == 0 || flag)
			{
				animationController.TriggerAnimation(AnimationTrigger, Value);
			}
		}
	}
}
