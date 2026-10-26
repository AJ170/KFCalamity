using EffectHelpers;
using UnityEngine;

public class ZedTimeSoldierEffect : EffectBaseTarget
{
	private AttributeModifier damageResistance;

	private GameObject hurryParticle;

	private float damageTakenMultiplier = 0.5f;

	private float duration = 6f;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = duration;
		maxRepeats = 1;
		damageResistance = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DamageReceived, gameObject);
	}

	protected override void OnActivated()
	{
		damageResistance.Alter(this, damageTakenMultiplier);
	}

	public override void Shutdown()
	{
		base.Shutdown();
		damageResistance.Restore(this);
	}
}
