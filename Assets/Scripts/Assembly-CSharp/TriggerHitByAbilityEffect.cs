using EffectEvents;
using EventHelpers;
using UnityEngine;

public class TriggerHitByAbilityEffect : EffectBaseListener
{
	public string AbilityName;

	public bool TriggerRemote = true;

	protected override void Execute(TriggerBase e)
	{
		if ((e.Id != TriggerId.Hit && e.Id != TriggerId.HitAOE) || string.IsNullOrEmpty(AbilityName))
		{
			return;
		}
		HitBase hitBase = e as HitBase;
		if (!NoCustomEvents() && !MatchCustomEvent(e.CustomName))
		{
			return;
		}
		HitByAbility e2 = new HitByAbility(AbilityName);
		if (TriggerRemote)
		{
			GameObject gameObject = hitBase.HitTransform.gameObject;
			PhysicalProjectile component = gameObject.GetComponent<PhysicalProjectile>();
			if (component != null)
			{
				component.TriggerInternal(e2);
			}
			else
			{
				effectSystem.TriggerRemote(gameObject, e2);
			}
		}
		else
		{
			effectSystem.TriggerInternal(e2);
		}
	}
}
