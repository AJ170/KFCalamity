using EffectHelpers;

public class PatriarchHealDamageReductionEffect : EffectBaseTarget
{
	private AttributeModifier speedAttribute;

	private AttributeModifier damageResistance;

	private float dmgReductionMultiplier = 0.35f;

	private float speedMultiplier = 1.5f;

	private float duration = 15f;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = duration;
		maxRepeats = 1;
		damageResistance = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.DamageReceived, gameObject);
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, gameObject);
	}

	protected override void OnActivated()
	{
		speedAttribute.Alter(this, speedMultiplier);
		damageResistance.Alter(this, dmgReductionMultiplier);
	}

	public override void Shutdown()
	{
		base.Shutdown();
		speedAttribute.Restore(this);
		damageResistance.Restore(this);
	}
}
