using EffectEvents;
using EventHelpers;
using UnityEngine;

public class MeleeDashEffect : EffectBaseListener
{
	public bool Active = true;

	public float MaxDashDistance = 1f;

	public float WeaponRange = 2f;

	public float SearchAngle = 45f;

	public float TargetDistance = 1.5f;

	public float SpeedMultiplier = 2f;

	private PlayerActionController actionController;

	private new string name = string.Empty;

	private void OnMounted(GameObject parent)
	{
		actionController = parent.GetComponent<PlayerActionController>();
	}

	protected override void Execute(TriggerBase e)
	{
		bool flag = MatchCustomEvent(e.CustomName);
		if (!NoCustomEvents() && !flag)
		{
			return;
		}
		if (Active)
		{
			if (actionController != null)
			{
				name = e.CustomName;
				actionController.AddAction(new MeleeDashAction(AttackCallback, MaxDashDistance, WeaponRange, SearchAngle, TargetDistance, SpeedMultiplier));
			}
		}
		else
		{
			AttackCallback();
		}
	}

	private void AttackCallback()
	{
		effectSystem.TriggerInternal(new Cast(name));
	}
}
