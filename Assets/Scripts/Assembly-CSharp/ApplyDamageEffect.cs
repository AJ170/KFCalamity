using EffectEvents;
using EventHelpers;
using UnityEngine;

public class ApplyDamageEffect : ApplyDamageBase
{
	public bool UseHeadshotDamage;

	public int HeadshotDamage = 10;

	protected override void Execute(TriggerBase e)
	{
		if (!NoCustomEvents() && !MatchCustomEvent(e.CustomName))
		{
			return;
		}
		if (e.Id == TriggerId.Hit || e.Id == TriggerId.HitAOE)
		{
			HitBase hitBase = e as HitBase;
			Health component = hitBase.HitTransform.GetComponent<Health>();
			if (component != null && component.CurrentHealth > 0)
			{
				GameObject gameObject = source;
				GameObject victim = component.transform.root.gameObject;
				int num = 0;
				bool flag = false;
				if (UseHeadshotDamage && hitBase.Headshot)
				{
					num = HeadshotDamage;
					flag = true;
				}
				else
				{
					num = Damage;
				}
				float damageDealtMultiplier = GetDamageMultiplier(flag) * GetDamageMultiplier(gameObject, victim);
				int num2 = component.TakeDamage(num, Type, damageDealtMultiplier, IgnoreArmor);
				if (num2 > 0)
				{
					TriggerDamageEvents(gameObject, victim, num2);
				}
				if (component.CurrentHealth == 0)
				{
					TriggerKillEvents(gameObject, victim, flag);
					component.Killer = gameObject;
					component.KilledByHeadshot = flag;
				}
			}
		}
		else
		{
			Debug.LogError(string.Format("ApplyDamageEffect:Execute() -> {0} not supported!", Trigger.ToString()));
		}
	}

	protected virtual float GetDamageMultiplier(bool headshot)
	{
		return GetDamageMultiplier() * MetaDamageMultiplier(headshot);
	}
}
