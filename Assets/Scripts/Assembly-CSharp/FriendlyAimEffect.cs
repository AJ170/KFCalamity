using EffectEvents;
using EventHelpers;
using UnityEngine;

public class FriendlyAimEffect : EffectBaseListener
{
	public float SearchAngle = 30f;

	public float SearchRange = 15f;

	public float FireAngle = 3f;

	public FriendlyAimType AimType;

	public bool RequireTarget;

	public FriendlyAimTargetEvaluator TargetEvaluator;

	private PlayerActionController actionController;

	private void OnMounted(GameObject parent)
	{
		actionController = parent.GetComponent<PlayerActionController>();
		if (actionController == null)
		{
			Debug.LogWarning("[FriendlyAimEffect:OnMounted] Failed to locate actionController in parent");
		}
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if ((NoCustomEvents() || flag) && actionController != null)
		{
			FriendlyAimAction action = new FriendlyAimAction(SearchAngle, SearchRange, FireAngle, RequireTarget, AimType, TargetEvaluator, TargetCallback);
			actionController.AddAction(action);
		}
	}

	private void TargetCallback(GameObject target)
	{
		if (target != null)
		{
			effectSystem.TriggerInternal(new FriendlyAim(target));
		}
		else if (!RequireTarget)
		{
			effectSystem.TriggerInternal(new FriendlyAimFailed());
		}
	}
}
