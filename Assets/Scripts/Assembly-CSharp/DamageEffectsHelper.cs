using EffectEvents;
using UnityEngine;

public class DamageEffectsHelper
{
	public static void TriggerDamageEvents(EffectSystem effectSystem, GameObject attacker, GameObject victim, int damageAmount)
	{
		if (effectSystem != null)
		{
			effectSystem.TriggerInternal(new DamageDealt(victim, damageAmount));
			if (effectSystem.SystemType == EffectSystemType.Weapon)
			{
				effectSystem.TriggerGlobal(new WeaponDamageDealt(victim));
			}
			effectSystem.TriggerRemote(victim, new DamageReceived(attacker, damageAmount));
		}
	}

	public static void TriggerKillEvents(EffectSystem effectSystem, GameObject attacker, GameObject victim, bool headshot = false)
	{
		if (effectSystem != null)
		{
			effectSystem.TriggerInternal(new Kill(victim, headshot));
			if (effectSystem.SystemType == EffectSystemType.Weapon)
			{
				effectSystem.TriggerGlobal(new WeaponKill(victim));
			}
			effectSystem.TriggerRemote(victim, new BeingKilled(attacker, headshot, effectSystem));
			effectSystem.TriggerRemote(victim, new Death(headshot));
		}
	}

	public static void TriggerDelegateDamageEvents(EffectSystem effectSystem, GameObject attacker, GameObject victim, int damageAmount)
	{
		if (attacker != null)
		{
			EffectEventDelegator component = attacker.GetComponent<EffectEventDelegator>();
			if (component != null)
			{
				component.ListenTrigger(new DamageDealt(victim, damageAmount));
			}
		}
		effectSystem.TriggerRemote(victim, new DamageReceived(attacker, damageAmount));
	}

	public static void TriggerDelegateKillEvents(EffectSystem effectSystem, GameObject attacker, GameObject victim)
	{
		if (attacker != null)
		{
			EffectEventDelegator component = attacker.GetComponent<EffectEventDelegator>();
			if (component != null)
			{
				component.ListenTrigger(new Kill(victim));
			}
		}
		effectSystem.TriggerRemote(victim, new BeingKilled(attacker));
		effectSystem.TriggerRemote(victim, new Death());
	}

	public static float MetaDamageMultiplier(IMetaDamageBonuses metaDamageBonuses, Perk perk, bool headshot)
	{
		float num = 1f;
		if (metaDamageBonuses != null)
		{
			if (perk != Perk.None)
			{
				num *= metaDamageBonuses.GetDamageMultiplier(perk);
				if (headshot)
				{
					num *= metaDamageBonuses.GetHeadshotMultiplier(perk);
				}
			}
			num *= metaDamageBonuses.GetDamageMultiplier();
		}
		return num;
	}
}
