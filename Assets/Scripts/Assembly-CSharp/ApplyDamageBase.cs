using UnityEngine;

public class ApplyDamageBase : EffectBaseListener
{
	public int Damage = 10;

	public DamageType Type;

	public bool IgnoreArmor;

	protected GameObject source;

	protected AttributeModifier damageDealt;

	private Perk perk = Perk.None;

	private IMetaDamageBonuses metaDamageBonuses;

	protected override void Start()
	{
		base.Start();
		if (source == null)
		{
			SetSource(base.transform.root.gameObject);
		}
		SetDamageDealtAttribute(source);
		if (FindPerk(source))
		{
			FindMetaDamageBonuses();
		}
	}

	protected void OnMounted(GameObject parent)
	{
		SetSource(parent);
	}

	protected float MetaDamageMultiplier(bool headshot)
	{
		return DamageEffectsHelper.MetaDamageMultiplier(metaDamageBonuses, perk, headshot);
	}

	protected virtual float GetDamageMultiplier()
	{
		return damageDealt.Multiplier;
	}

	protected virtual float GetDamageMultiplier(GameObject attacker, GameObject victim)
	{
		float num = 1f;
		if (attacker != null && victim != null && attacker == victim)
		{
			num *= ScalingByGameSettings.SelfDamageByDifficulty();
		}
		return num;
	}

	protected void TriggerDamageEvents(GameObject attacker, GameObject victim, int damageAmount)
	{
		DamageEffectsHelper.TriggerDamageEvents(effectSystem, attacker, victim, damageAmount);
	}

	protected void TriggerKillEvents(GameObject attacker, GameObject victim, bool headshot = false)
	{
		DamageEffectsHelper.TriggerKillEvents(effectSystem, attacker, victim, headshot);
	}

	private bool FindPerk(GameObject source)
	{
		Soldier component = source.GetComponent<Soldier>();
		if (component != null)
		{
			perk = component.Perk;
			return true;
		}
		return false;
	}

	private void FindMetaDamageBonuses()
	{
		metaDamageBonuses = GlobalHelpers.FindMetaDamageBonusesUpwards(base.gameObject);
	}

	private void SetSource(GameObject source)
	{
		this.source = StandaloneEntity.Source(source);
	}

	private void SetDamageDealtAttribute(GameObject source)
	{
		damageDealt = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DamageDealt, source);
	}
}
