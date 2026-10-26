using EffectHelpers;

public class PatriarchLastStandBoostEffect : EffectBaseTarget
{
	private AttributeModifier speedAttribute;

	private float speedMultiplier = 2.1f;

	protected override void Awake()
	{
		base.Awake();
		effectType = EffectType.Continuous;
		buffType = BuffType.Buff;
		buffClass = BuffClass.None;
		repeatInterval = 6f;
		maxRepeats = 1;
		speedAttribute = AttributeController.CreateAttribute<AttributeModifier>(AttributeName.Speed, gameObject);
	}

	protected override void OnActivated()
	{
		speedAttribute.Alter(this, speedMultiplier);
	}

	public override void Shutdown()
	{
		base.Shutdown();
		speedAttribute.Restore(this);
	}
}
