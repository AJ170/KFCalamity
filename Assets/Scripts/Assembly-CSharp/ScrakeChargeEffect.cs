using EffectHelpers;

public class ScrakeChargeEffect : EffectBaseTarget
{
	private AttributeModifier speedAttribute;

	private EffectResistance constraintResistance;

	private float speedMultiplier = 2f;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Infinite;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, gameObject);
		constraintResistance = EffectResistanceController.CreateResistance(BuffClass.Constraint, gameObject);
	}

	protected override void OnActivated()
	{
		speedAttribute.Alter(this, speedMultiplier);
	}

	public override void Shutdown()
	{
		base.Shutdown();
		speedAttribute.Restore(this);
		constraintResistance.Restore(this);
	}
}
