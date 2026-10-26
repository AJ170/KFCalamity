using EffectHelpers;
using UnityEngine;

public class DamageOverTimeEffect : EffectBaseTarget
{
	protected int damagePerTick = 3;

	protected DamageType damageType;

	protected bool ignoreArmor;

	private Transform root;

	private Health healthComponent;

	private AttributeModifier damageDealt;

	private float damageDealtMultiplier = 1f;

	private Perk attackerPerk = Perk.None;

	private IMetaDamageBonuses attackerMetaDamageBonuses;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Debuff;
		buffClass = BuffClass.None;
		repeatInterval = 1f;
		maxRepeats = 3;
		root = gameObject.transform.root;
		healthComponent = root.GetComponent<Health>();
		if (healthComponent == null)
		{
			Debug.LogError(string.Format("DamageOverTimeEffect:Awake() Missing Health component in {0}", root.name));
		}
		if (FindAttackerPerk())
		{
			FindAttackerMetaDamageBonuses();
		}
		CreateDamageModifier();
		UpdateDamageMultiplier();
	}

	protected override void OnRefreshed(EffectBaseTarget newEffect)
	{
		if (FindAttackerPerk())
		{
			FindAttackerMetaDamageBonuses();
		}
		CreateDamageModifier();
		UpdateDamageMultiplier();
	}

	protected override void OnUpdate()
	{
		if (healthComponent != null && healthComponent.CurrentHealth > 0)
		{
			GameObject gameObject = base.Source;
			GameObject victim = root.gameObject;
			UpdateDamageMultiplier(gameObject, victim);
			int num = healthComponent.TakeDamage(damagePerTick, damageType, damageDealtMultiplier, ignoreArmor);
			if (num > 0)
			{
				TriggerDamageEvents(gameObject, victim, num);
			}
			if (healthComponent.CurrentHealth == 0)
			{
				OnKill(gameObject, victim);
				healthComponent.Killer = gameObject;
			}
		}
	}

	private void TriggerDamageEvents(GameObject attacker, GameObject victim, int damageAmount)
	{
		DamageEffectsHelper.TriggerDelegateDamageEvents(base.Owner, attacker, victim, damageAmount);
	}

	private void TriggerKillEvents(GameObject attacker, GameObject victim)
	{
		DamageEffectsHelper.TriggerDelegateKillEvents(base.Owner, attacker, victim);
	}

	protected virtual void OnKill(GameObject attacker, GameObject victim)
	{
		TriggerKillEvents(attacker, victim);
	}

	private bool FindAttackerPerk()
	{
		Soldier component = base.Source.GetComponent<Soldier>();
		if (component != null)
		{
			attackerPerk = component.Perk;
			return true;
		}
		return false;
	}

	private void CreateDamageModifier()
	{
		damageDealt = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DamageDealt, base.Source);
	}

	private void FindAttackerMetaDamageBonuses()
	{
		if (base.Creator != null)
		{
			attackerMetaDamageBonuses = GlobalHelpers.FindMetaDamageBonusesUpwards(base.Creator.gameObject);
		}
	}

	private void UpdateDamageMultiplier(GameObject attacker, GameObject victim)
	{
		if (attacker != null && victim != null)
		{
			damageDealtMultiplier = damageDealt.Multiplier * SelfDamageMultiplier(attacker, victim) * DamageEffectsHelper.MetaDamageMultiplier(attackerMetaDamageBonuses, attackerPerk, false);
		}
	}

	private void UpdateDamageMultiplier()
	{
		UpdateDamageMultiplier(base.Source, root.gameObject);
	}

	private float SelfDamageMultiplier(GameObject attacker, GameObject victim)
	{
		float num = 1f;
		if (attacker == victim)
		{
			num *= ScalingByGameSettings.SelfDamageByDifficulty();
		}
		return num;
	}
}
