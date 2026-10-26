using EffectEvents;
using EventHelpers;
using UnityEngine;

public class ApplyAoeDamageEffect : ApplyDamageBase
{
	protected override void Execute(TriggerBase e)
	{
		if (e.Id == TriggerId.HitAOE)
		{
			if (!NoCustomEvents() && !MatchCustomEvent(e.CustomName))
			{
				return;
			}
			HitAOE hitAOE = e as HitAOE;
			Health component = hitAOE.HitTransform.GetComponent<Health>();
			if (component != null && component.CurrentHealth > 0)
			{
				GameObject gameObject = source;
				GameObject victim = component.transform.root.gameObject;
				float damageDealtMultiplier = GetDamageMultiplier(hitAOE) * GetDamageMultiplier(gameObject, victim);
				int num = component.TakeDamage(Damage, Type, damageDealtMultiplier, IgnoreArmor);
				if (num > 0)
				{
					TriggerDamageEvents(gameObject, victim, num);
				}
				if (component.CurrentHealth == 0)
				{
					TriggerKillEvents(gameObject, victim);
					component.Killer = gameObject;
				}
			}
		}
		else
		{
			Debug.LogError(string.Format("ApplyAoeDamageEffect:Execute() -> {0} not supported!", Trigger.ToString()));
		}
	}

	protected virtual float GetDamageMultiplier(HitAOE hitAoe)
	{
		return GetDamageMultiplier() * MetaDamageMultiplier(false) * AOEDamageMultiplier(hitAoe.AOERadius, hitAoe.DistanceFromImpact);
	}

	private float AOEDamageMultiplier(float aoeRadius, float distanceFromImpact)
	{
		float result = 1f;
		float num = distanceFromImpact / aoeRadius;
		if (num >= 0.25f && num < 0.6f)
		{
			result = 0.67f;
		}
		else if (num >= 0.6f)
		{
			result = 0.33f;
		}
		return result;
	}
}
